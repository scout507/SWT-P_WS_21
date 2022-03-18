using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/* created by: SWT-P_WS_21/22 */


/// <summary>
/// Script to interact with the Turret on the Player Object
/// </summary>
public class ControllTurret : NetworkBehaviour
{

    /// <summary>
    /// Reference to the Turret Gameobject
    /// </summary>
    public Turret turret;

    /// <summary>
    /// Reference to the Player Network ID
    /// </summary>
    public uint playerinUseID;

    /// <summary>
    /// Reference to the Player Camera Mount
    /// </summary>
    [SerializeField] GameObject cameraMount;


    /// <summary>
    /// checks which weapon script is active
    /// 0 = Pistol; 1 = MP ; 2 = Shotgun ; 3 = Melee ; 4 = Rifle
    /// </summary>
    private int[] weaponScriptActive = new int[12];

    /// <summary>
    /// Checks if the Player is in the Turret
    /// </summary>
    private bool inVehicle = false;

    /// <summary>
    /// Firerate of the Turret
    /// </summary>
    [SerializeField] float fireRate = 1f;

    /// <summary>
    /// Fire rate of the Turret
    /// </summary>
    private float nextFire = 0;

    /// <summary>
    /// Time until Player can enter the Tower again
    /// </summary>
    private float exitTime;

    /// <summary>
    /// Time until Player can leave the Tower 
    /// </summary>
    private float enterTime;

    /// <summary>
    /// Gameobject of the Camera Mount on the Turret Object
    /// </summary>
    private GameObject turretCameraMount;

    /// <summary>
    /// Gameobject of the Player Camera Mount on the Player Object
    /// </summary>
    private GameObject playerCameraMount;

    /// <summary>
    /// Gameobject of the Camera
    /// </summary>
    private GameObject playerCamera;

    /// <summary>
    /// Gameobject of the Player Mount on the Turret Object
    /// </summary>
    private GameObject turretPlayerMount;


    /// <summary>
    /// The Player can use:
    /// E to enter the Vehicle
    /// Escpae to Exit the Vehicle
    /// Use Fire1 Button to shot with the Vehicle
    /// Rotate the Turret when the Player is inside the Vehicle
    /// The Order of the Weapon Scripts: 0 = Pistol; 1 = MP ; 2 = Shotgun ; 3 = Melee ; 4 = Rifle
    /// 5 = Bob; 6 = Doc; 7 = HealGun; 8 = Hunter; 9 = IQ; 10 = IQ Cam; 11 = Knut;
    /// </summary>
    void Update()
    {

        if (!isLocalPlayer)
        {
            return;
        }
        if (gameObject.GetComponent<NetworkIdentity>().netId == playerinUseID)
        {


            if (Input.GetKeyDown(KeyCode.E) && !inVehicle && Time.time > exitTime)
            {
                turretCameraMount = turret.transform.GetChild(0).GetChild(0).gameObject;
                playerCameraMount = gameObject.transform.GetChild(0).gameObject;
                turretPlayerMount = turret.transform.GetChild(0).GetChild(3).gameObject;
                playerCamera = GameObject.FindGameObjectWithTag("MainCamera");

                playerCamera.transform.parent = turretCameraMount.transform;
                playerCamera.transform.position = turretCameraMount.transform.position;
                playerCamera.transform.rotation = turretCameraMount.transform.rotation;


                turret.enterVehicle();
                inVehicle = true;

                gameObject.GetComponent<PlayerMovement>().enabled = false;

                if (gameObject.GetComponent<Pistol>()?.enabled == true)
                {
                    weaponScriptActive[0] = 1;
                    gameObject.GetComponent<Pistol>().enabled = false;
                }
                if (gameObject.GetComponent<MP>()?.enabled == true)
                {
                    weaponScriptActive[1] = 1;
                    gameObject.GetComponent<MP>().enabled = false;
                }
                if (gameObject.GetComponent<Shotgun>()?.enabled == true)
                {
                    weaponScriptActive[2] = 1;
                    gameObject.GetComponent<Shotgun>().enabled = false;
                }
                if (gameObject.GetComponent<Melee>()?.enabled == true)
                {
                    weaponScriptActive[3] = 1;
                    gameObject.GetComponent<Melee>().enabled = false;
                }
                if (gameObject.GetComponent<Rifle>()?.enabled == true)
                {
                    weaponScriptActive[4] = 1;
                    gameObject.GetComponent<Rifle>().enabled = false;
                }
                if (gameObject.GetComponent<Bob>()?.enabled == true)
                {
                    weaponScriptActive[5] = 1;
                    gameObject.GetComponent<Bob>().enabled = false;
                }
                if (gameObject.GetComponent<Doc>()?.enabled == true)
                {
                    weaponScriptActive[6] = 1;
                    gameObject.GetComponent<Doc>().enabled = false;
                }
                if (gameObject.GetComponent<HealGun>()?.enabled == true)
                {
                    weaponScriptActive[7] = 1;
                    gameObject.GetComponent<HealGun>().enabled = false;
                }
                if (gameObject.GetComponent<Hunter>()?.enabled == true)
                {
                    weaponScriptActive[8] = 1;
                    gameObject.GetComponent<Hunter>().enabled = false;
                }
                if (gameObject.GetComponent<IQ>()?.enabled == true)
                {
                    weaponScriptActive[9] = 1;
                    gameObject.GetComponent<IQ>().enabled = false;
                }
                if (gameObject.GetComponent<IQCam>()?.enabled == true)
                {
                    weaponScriptActive[10] = 1;
                    gameObject.GetComponent<IQCam>().enabled = false;
                }
                if (gameObject.GetComponent<Knut>()?.enabled == true)
                {
                    weaponScriptActive[11] = 1;
                    gameObject.GetComponent<Knut>().enabled = false;
                }
                enterTime = Time.time + 0.5f;
            }

            if (inVehicle)
            {
                float rotateDirection = Input.GetAxis("Horizontal");
                float upDownDirection = Input.GetAxis("Vertical");
                turret.cmdrotateTurret(rotateDirection, upDownDirection);
                transform.position = new Vector3(turretPlayerMount.transform.position.x, transform.position.y, turretPlayerMount.transform.position.z);
                transform.rotation = Quaternion.Euler(transform.rotation.x, turretPlayerMount.transform.eulerAngles.y, transform.rotation.z);
            }


            if (Input.GetKeyDown(KeyCode.E) && inVehicle && Time.time > enterTime)
            {

                playerCamera.transform.parent = playerCameraMount.transform;
                playerCamera.transform.position = cameraMount.transform.position;
                playerCamera.transform.localPosition += new Vector3(0.094f, 0.05f, 0.155f);
                playerCamera.transform.rotation = cameraMount.transform.rotation;


                inVehicle = false;
                gameObject.GetComponent<PlayerMovement>().enabled = true;


                if (weaponScriptActive[0] == 1)
                {
                    gameObject.GetComponent<Pistol>().enabled = true;
                }
                if (weaponScriptActive[1] == 1)
                {
                    gameObject.GetComponent<MP>().enabled = true;
                }
                if (weaponScriptActive[2] == 1)
                {
                    gameObject.GetComponent<Shotgun>().enabled = true;
                }
                if (weaponScriptActive[3] == 1)
                {
                    gameObject.GetComponent<Melee>().enabled = true;
                }
                if (weaponScriptActive[4] == 1)
                {
                    gameObject.GetComponent<Rifle>().enabled = true;
                }
                if (weaponScriptActive[5] == 1)
                {
                    gameObject.GetComponent<Bob>().enabled = true;
                }
                if (weaponScriptActive[6] == 1)
                {
                    gameObject.GetComponent<Doc>().enabled = true;
                }
                if (weaponScriptActive[7] == 1)
                {
                    gameObject.GetComponent<HealGun>().enabled = true;
                }
                if (weaponScriptActive[8] == 1)
                {
                    gameObject.GetComponent<Hunter>().enabled = true;
                }
                if (weaponScriptActive[9] == 1)
                {
                    gameObject.GetComponent<IQ>().enabled = true;
                }
                if (weaponScriptActive[10] == 1)
                {
                    gameObject.GetComponent<IQCam>().enabled = true;
                }
                if (weaponScriptActive[11] == 1)
                {
                    gameObject.GetComponent<Knut>().enabled = true;
                }

                turret.exitVehicle();

                exitTime = Time.time + 1f;
            }

            if (inVehicle && Input.GetButtonDown("Fire1") && Time.time > nextFire)
            {
                nextFire = Time.time + fireRate;
                turret.Shoot();
            }
        }
    }
}
