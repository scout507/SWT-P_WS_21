using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/// <summary>
/// Prototype script for destructable objects. 
/// </summary>
public class DestructableObject : NetworkBehaviour
{
    [SerializeField] float maxHealth;
    [SyncVar] public float health;
    [SyncVar] public bool active; //true when hp are above 0

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
