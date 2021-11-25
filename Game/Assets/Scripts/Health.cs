using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class Health : NetworkBehaviour 
{
    [SyncVar]
    public int health = 100; // Variable for Health, synced on all Clients

    /**
    Function for taking Demage, runs on Server
    */
    public void TakeDamage(int amount)
    {
        if(!isServer) return;

        health -= amount;
        TargetDamage(amount);
        if(health <= 0)
        {
            TargetDeath();
        }
    }

    /**
    Function for taking Damage, runs on Client for Effects on Screen and so on
    */
    [TargetRpc]
    public void TargetDamage(int amount)
    {
        Debug.Log("Took damage:" + amount);
    }

    /**
    Death function for Client
    Dummy at the moment
    */
    [TargetRpc]
    void TargetDeath()
    {
        //Debug dummy, swap with real death sequence later
        gameObject.GetComponent<PlayerMovement>().enabled = false;
        gameObject.GetComponent<ShootGun>().enabled = false;
        Debug.Log("You are Dead");
    }
}
