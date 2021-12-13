using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

/// <summary>
/// The ShootGun class is an abstract class from which every weapon must inherit. It implements the network behavior when enemies are hit.
/// </summary>
public abstract class ShootGun : NetworkBehaviour
{
    public int gunDamage; // Damage output for gun
    public float fireRate; // Firerate for gun
    public float weaoponRange; // Range for gun
    public Transform gunEnd; // Gun end for animations
    public GameObject gun; // Prefab of gun
    public Transform gunMount; // Point where gun is loaded
    public int gunAmmo; // Ammunition of gun
    public float nextFire; // Time of the next shot you can take 
    public float recoil; // Set ammount of Recoil per Shot

    /// <summary>
    /// Gets called when player is hit. 
    /// </summary>
    /// <param name="player">Gameobject of player who is hit.</param>
    /// <param name="damageAmount">Amount of damage.</param>
    [Command]
    public void CmdShootPlayer(GameObject player, int damageAmount)
    {
       Debug.Log("Hit Player!");
       player.GetComponent<Health>().TakeDamage(damageAmount);
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
    /// Function for hit on wall, can start Animation or something on point of hit.
    /// </summary>
    /// <param name="hit">Position of point of impact.</param>
    [ClientRpc]
    public void RpcHitWall(Vector3 hit)
    {
        Debug.Log("Hit Wall!");
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
    }

    /// <summary>
    /// Implements a single shot, different for every weapon.
    /// </summary>
    public abstract void Shoot();

    /// <summary>
    /// Very simple recoil for better representation of gun.
    /// </summary>
    public void Recoil()
    {
        GetComponent<PlayerMovement>().xRotation -= recoil;
    }
}
