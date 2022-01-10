using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/// <summary>
/// Prototype script for destructable objects. 
/// </summary>
public class DestructableObject : NetworkBehaviour
{
    [SerializeField] public float maxHealth;
    [SyncVar] public float health;
    
    /// <summary>True when hp are above 0</summary>
    [SyncVar] public bool active; 

    private void Start()
    {
        health = maxHealth;
    }

    private void Update()
    {
        if(health <= 0)
        {
            active = false;
        }
        else active = true;
    }

    /// <summary>
    /// Can be called to damage the object.
    /// </summary>
    /// <param name="amount">Amount of damage taken.</param>
    public void TakeDamage(float amount){
        health -= amount;
    }
}
