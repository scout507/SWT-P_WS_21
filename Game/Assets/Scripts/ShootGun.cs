using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public abstract class ShootGun : NetworkBehaviour
{
    public int gunDamage; // Damage output for gun, maybe attached to Gun-Object later
    public float fireRate; // Firerate for gun, maybe attached to Gun-Object later
    public float weaoponRange; // Range for gun, maybe attached to Gun-Object later
    public Transform gunEnd; // Gun end for Animations, maybe attached to Gun-Object later 
    public GameObject gun; // Gun-Object, can change
    public Transform gunMount;
    public int gunAmmo;
    public float nextFire; // Time of the next shot you can take 
    public float recoil; // Set ammount of Recoil per Shot

    // Gets called if Player is hit
    [Command]
    public void CmdShootPlayer(GameObject player, int damageAmount)
    {
       Debug.Log("Hit Player!");
       player.GetComponent<Health>().TakeDamage(damageAmount);
    }

    // Called when monster is hit
    [Command]
    public void CmdShootMonster(GameObject monster, int damageAmount)
    {
        monster.GetComponent<MonsterController>().TakeDamage(damageAmount);
    }

    // Function for hit on wall, can start Animation or something on point of hit
    [ClientRpc]
    public void RpcHitWall(Vector3 hit)
    {
        Debug.Log("Hit Wall!");
    }

    // Gets called if Wall is hit
    [Command]
    public void CmdShootWall(Vector3 hit)
    {
        RpcHitWall(hit);
    }

    private void OnDisable() 
    {
        Destroy(gunMount.GetChild(0).gameObject);
    }

    private void OnEnable() {
        Instantiate(gun, gunMount);
    }
    /**
    Function for shooting, casts Ray and calls functions based on object that is hit
    */
    public abstract void Shoot();
    public void Recoil()
    {
        GetComponent<PlayerMovement>().xRotation -= recoil;
    }
}
