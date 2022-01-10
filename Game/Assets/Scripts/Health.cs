using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Health : NetworkBehaviour
{
    /// <summary>
    /// Variable for Health, synced on all Clients
    /// </summary>
    [SyncVar]
    public int health = 100;

<<<<<<< HEAD
    public HealthBar healthBar;

    void Start()
    {
        healthBar.SetMaxHealth(health);
    }

    /**
    Function for taking Damage, runs on Server
    */
=======
    /// <summary>
    /// Function for taking Damage, runs on Server
    /// </summary>
    /// <param name="amount"></param>
>>>>>>> origin/main
    public void TakeDamage(int amount)
    {
        if (!isServer)
            return;

        health -= amount;
<<<<<<< HEAD
        healthBar.SetHealth(health);
=======

>>>>>>> origin/main
        TargetDamage(amount);
        if (health <= 0)
        {
            TargetDeath();
        }
    }

    /// <summary>
    /// Function for taking Damage, runs on Client for Effects on Screen and so on
    /// </summary>
    /// <param name="amount"></param>
    [TargetRpc]
    public void TargetDamage(int amount)
    {
        Debug.Log("Took damage:" + amount);
    }

    /// <summary>
    /// Death function for Client
    /// Dummy at the moment
    /// </summary>
    [TargetRpc]
    void TargetDeath()
    {
        //Debug dummy, swap with real death sequence later
        gameObject.GetComponent<PlayerMovement>().enabled = false;
        gameObject.GetComponent<ShootGun>().enabled = false;
        Debug.Log("You are Dead");
    }
}
