using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

/* created by: SWT-P_WS_21/22 */


/// <summary>
/// The ShootGun class is an abstract class from which every weapon must inherit. It implements the network behavior when enemies are hit.
/// </summary>
public abstract class ShootGun : NetworkBehaviour
{
    /// <summary>Damage output for gun.</summary>
    public int gunDamage;

    /// <summary>Firerate for gun.</summary>
    public float fireRate;

    /// <summary>Range for gun.</summary>
    public float weaponRange;

    /// <summary>Gun end for animations.</summary>
    public Transform gunEnd;

    /// <summary>Prefab of gun.</summary>
    public GameObject gun;

    /// <summary>Point where gun is loaded.</summary>
    public Transform gunMount;

    /// <summary>Ammunition of gun.</summary>
    public int gunAmmo;

    /// <summary>Maximum Ammo in one magazin..</summary>
    public int magSize;

    /// <summary>Time of the next shot you can take.</summary>
    public float nextFire;

    /// <summary>Time of the next reload you can take.</summary>
    public float nextReload;

    /// <summary>Delay time, so the reload does not happen instantly.</summary>
    public float reloadDelay;

    /// <summary>Set ammount of Recoil per Shot.</summary>
    public float recoil;

    /// <summary>Icon of the Weapon for the UI.</summary>
    public Sprite icon;

    /// <summary>Weapon-Inventory of the Player.</summary>
    public Inventory inventory;

    /// <summary>True when player is attacking.</summary>
    [SyncVar] public bool inAttack;

    /// <summary>Audio Script that controlls Gun Sound.</summary>
    public AudioController audioController;

    /// <summary>The range at witch shots can trigger zombies.</summary>
    public float triggerRange = 15f;

    /// <summary>true if the player can do inputs.</summary>
    public bool canInteract = true;

    /// <summary>Flag if player is reloading.</summary>
    public bool isReloading = false;

    /// <summary>
    /// Checks for nearby zombies and triggers them.
    /// </summary>
    public void TriggerAggro()
    {
        Collider[] nearbyZombies = Physics.OverlapSphere(
            this.gameObject.transform.position,
            triggerRange
        );
        foreach (Collider nearbyZombie in nearbyZombies)
        {
            if (nearbyZombie.GetComponent<MonsterController>())
            {
                CmdTriggerMonster(nearbyZombie.gameObject, this.gameObject);
            }
        }
    }

    /// <summary>
    /// Destroys gameobject of gun when a new gun is equipped.
    /// </summary>
    private void OnDisable()
    {
        Destroy(gunMount.GetChild(0).gameObject);
    }

    /// <summary>
    /// Loads prefab of gun when this gun is equipped.
    /// </summary>
    private void OnEnable()
    {
        Instantiate(gun, gunMount);
        if (isLocalPlayer)
        {
            inventory = GetComponentInChildren<Inventory>();
            inventory.UpdateInfo(this.icon, this.gunAmmo, this.magSize);
        }

    }

    /// <summary>
    /// Implements a single shot, different for every weapon.
    /// </summary>
    public abstract void Shoot();

    /// <summary>
    /// Implements reloading, different for every weapon.
    /// </summary>
    public abstract void Reload();

    /// <summary>
    /// Very simple recoil for better representation of gun.
    /// </summary>
    public void Recoil()
    {
        float xRotation = GetComponent<PlayerMovement>().GetXRotation();
        xRotation -= recoil;
        GetComponent<PlayerMovement>().SetXRotation(xRotation);
    }

    /// <summary>
    /// Gets called when player is hit.
    /// </summary>
    /// <param name="player">Gameobject of player who is hit.</param>
    /// <param name="damageAmount">Amount of damage.</param>
    [Command]
    public void CmdShootPlayer(GameObject player, int damageAmount)
    {
        player.GetComponent<Health>().TakeDamage(damageAmount);
    }

    /// <summary>
    /// Gets called when player is hit.
    /// </summary>
    /// <param name="player">Gameobject of player who is hit.</param>
    /// <param name="damageAmount">Amount of damage.</param>
    [Command]
    public void CmdShootDevice(GameObject device, int damageAmount)
    {
        device.GetComponent<Device>().TakeDamage(damageAmount);
    }

    /// <summary>
    /// Gets called when monster is hit.
    /// </summary>
    /// <param name="monster">Gameobject of monster which is hit.</param>
    /// <param name="damageAmount">Amount of damage.</param>
    [Command]
    public void CmdShootMonster(GameObject monster, int damageAmount)
    {
        monster.GetComponent<MonsterController>().TakeDamage(damageAmount);
    }

    /// <summary>
    /// Calls the AggroMob method to trigger nearby monsters.
    /// </summary>
    /// <param name="monster">The monster to trigger</param>
    /// <param name="player">The player triggering it.</param>
    [Command]
    public void CmdTriggerMonster(GameObject monster, GameObject player)
    {
        if(monster && monster.GetComponent<MonsterController>()) monster.GetComponent<MonsterController>().AggroMob(player);
    }

    /// <summary>
    /// Function for hit on wall, can start Animation or something on point of hit.
    /// </summary>
    /// <param name="hit">Position of point of impact.</param>
    [ClientRpc]
    public void RpcHitWall(Vector3 hit)
    {
        return;
    }

    /// <summary>
    /// Method for server, so it can deploy the point of impact of hit on a wall to all clients.
    /// </summary>
    /// <param name="hit">Position of point of impact.</param>
    [Command]
    public void CmdShootWall(Vector3 hit)
    {
        RpcHitWall(hit);
    }
}
