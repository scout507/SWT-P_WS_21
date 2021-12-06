using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

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

    public void TakeDamage(float amount){
        health -= amount;
    }
}
