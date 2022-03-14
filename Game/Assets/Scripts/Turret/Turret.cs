using UnityEngine;
using Mirror;


/* created by: SWT-P_WS_21/22 */


/// <summary>
/// Script on the Turret 
/// </summary>
public class Turret : NetworkBehaviour
{

    /// <summary>
    /// checks if the Turret is in Use
    /// </summary>
    public bool inUse = false;


    /// <summary>
    /// Saves the Network Id from the player that is using the Turret
    /// </summary>
    NetworkIdentity playerNIDinUse;


    /// <summary>
    /// Reference to the controllTurret Script on the Player Object
    /// </summary>
    ControllTurret controlTurretPlayer;


    /// <summary>
    /// Rotation speed of the turret
    /// </summary>
    private float rotateSpeed = 0.5f;

    /// <summary>
    /// Damage amount of the Turret
    /// </summary>
    [SerializeField] int turretDamage = 20;

    /// <summary>
    /// Shooting range of the Turret
    /// </summary>
    [SerializeField] float weaoponRange = 30f;

    /// <summary>
    /// Particle system used when shooting
    /// </summary>
    [SerializeField] ParticleSystem shotflash;

    /// <summary>
    /// AudioSource used when shooting
    /// </summary>
    AudioSource audiosource;


    private void Start()
    {
        audiosource = this.GetComponent<AudioSource>();
    }


    /// <summary>
    /// Client calls the Server to rotate the Object
    /// </summary>
    /// <param name="rotateDirection">
    /// The Directon and the Value of the Movement
    /// </param>
    [Command]
    public void cmdrotateTurret(float rotateDirection, float updownDirection)
    {
        Transform MgTransform = transform.transform.GetChild(0).gameObject.transform;

        if (MgTransform.eulerAngles.x < 10 || MgTransform.eulerAngles.x > 330)
        {
            MgTransform.Rotate(updownDirection * rotateSpeed, 0f, 0f, Space.Self);
            MgTransform.Rotate(0f, rotateDirection * rotateSpeed, 0f, Space.World);
        }
        else if (MgTransform.eulerAngles.x >= 10 && MgTransform.eulerAngles.x <= 20 && updownDirection < 0)
        {
            MgTransform.Rotate(updownDirection * rotateSpeed, 0f, 0f, Space.Self);
        }
        else if (MgTransform.eulerAngles.x <= 330 && MgTransform.eulerAngles.x >= 320 && updownDirection > 0)
        {
            MgTransform.Rotate(updownDirection * rotateSpeed, 0f, 0f, Space.Self);
        }

    }


    /// <summary>
    /// Calls the Particle System animation on all Clients
    /// </summary>
    [ClientRpc]
    public void rpcturretFireAnimationn()
    {
        shotflash.Play();
    }

    /// <summary>
    /// Calls the Turret Shot Sound on all Clients
    /// </summary>
    [ClientRpc]
    public void RPCTurretShotSound()
    {
        audiosource.volume = 0.7f;
        audiosource.Play();
    }

    /// <summary>
    /// Shoots one shot defined by the attributes of the specific gun
    /// </summary>
    [Command]
    public void Shoot()
    {

        RaycastHit hit;
        Vector3 rayOrigin = gameObject.transform.GetChild(0).gameObject.transform.position;
        Vector3 direction = gameObject.transform.GetChild(0).forward;

        RPCTurretShotSound();
        rpcturretFireAnimationn();
        if (Physics.Raycast(rayOrigin, direction, out hit, weaoponRange, ~0))
        {
            Debug.DrawRay(rayOrigin, direction * weaoponRange, Color.green, 10);
            if (hit.collider.gameObject.tag == "Player")
            {
                RpcHitPlayer();
                hit.transform.gameObject.GetComponent<Health>().TakeDamage(turretDamage);

            }
            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Monster"))
            {
                RpcHitMonster();
                hit.transform.GetComponent<MonsterController>().TakeDamage(turretDamage);
            }
            else
            {
                RpcHitWall(hit.point);
            }
        }
        else
        {
            Debug.DrawRay(rayOrigin, direction * weaoponRange, Color.red, 10);
            RpcOutOfRange(hit.point);
        }


    }

    /// <summary>
    /// Method is called when a Player is hit
    /// </summary>
    [ClientRpc]
    void RpcHitPlayer()
    {
        Debug.Log("Turret: Hit Player!");
    }

    /// <summary>
    /// Method is called when a Monster is hit
    /// </summary>
    [ClientRpc]
    void RpcHitMonster()
    {
        Debug.Log("Turret: Hit Monster!");
    }

    /// <summary>
    /// Method is called when the Shoot range is to short
    /// </summary>
    /// <param name="hit">Position of point of impact</param>
    [ClientRpc]
    void RpcOutOfRange(Vector3 hit)
    {
        Debug.Log("Turret: Out of Range!");
    }

    /// <summary>
    /// Method is called when a wall is hit and notifies all Players about it
    /// </summary>
    /// <param name="hit">Position of point of impact</param>
    [ClientRpc]
    void RpcHitWall(Vector3 hit)
    {
        Debug.Log("Turret: Hit Wall!");
    }

    /// <summary>
    /// Method is called when a Player wants to enter the Vehicle
    /// </summary>
    [Command]
    public void enterVehicle()
    {
        inUse = true;

    }

    /// <summary>
    /// Method is called when a Player wants to exit the Vehicle
    /// </summary>
    [Command]
    public void exitVehicle()
    {
        inUse = false;
        controlTurretPlayer.enabled = false;
    }

    /// <summary>
    /// Method is called when the Player enters the Turret Collider
    /// Player gets Autority over the Object to interact with it
    /// </summary>
    [Command(requiresAuthority = false)]
    public void CMDGetAuthority(NetworkIdentity thisnetworkId, NetworkIdentity playerId)
    {
        if (thisnetworkId.connectionToClient != null)
        {
            thisnetworkId.RemoveClientAuthority();
        }
        thisnetworkId.AssignClientAuthority(playerId.connectionToClient);
    }


    /// <summary>
    /// Method is called when the Player leaves the Turret Collider
    /// Player gets Autority over the Object removed to no longer interact with it
    /// </summary>
    [Command]
    public void CMDRemoveAuthority(NetworkIdentity thisnetworkId)
    {
        thisnetworkId.RemoveClientAuthority();
    }


    /// <summary>
    /// When the player is close enough to the Turret, he gets Authority to interact with it
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && inUse == false)
        {
            CMDGetAuthority(GetComponent<NetworkIdentity>(), other.gameObject.GetComponent<NetworkIdentity>());
            inUse = true;
            playerNIDinUse = other.gameObject.GetComponent<NetworkIdentity>();
            controlTurretPlayer = other.gameObject.GetComponent<ControllTurret>();
            controlTurretPlayer.enabled = true;
            controlTurretPlayer.turret = gameObject.GetComponent<Turret>();
            controlTurretPlayer.playerinUseID = playerNIDinUse.netId;
        }
        else
        {
            Debug.Log("Turret in Use");
        }
    }


    /// <summary>
    /// When the player leaves the Turret Collider range, he can no longer interact with it
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player" && other.gameObject.GetComponent<NetworkIdentity>().netId == playerNIDinUse.netId)
        {
            inUse = false;
            controlTurretPlayer = other.gameObject.GetComponent<ControllTurret>();
            controlTurretPlayer.enabled = false;
        }
    }

}
