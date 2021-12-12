using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class Health : NetworkBehaviour 
{
    [SyncVar]
    public int maxHealth = 100; // Variable for Health, synced on all Clients
    public int health;

    public HealthBar healthBar;

    void Start()
    {
        health = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    /**
    Function for taking Damage, runs on Server
    */
    public void TakeDamage(int amount)
    {
        if(!isServer) return;

        health -= amount;
        healthBar.SetHealth(health);
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
