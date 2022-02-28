using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

abstract public class Device : NetworkBehaviour
{   
    [SyncVar]
    [SerializeField]
    int health = 30;

    /// <summary>
    /// The method TakeDamage is responsible for suffering damage.
    /// </summary>
    /// <param name="amount">The parameter amount indicates how much damage was sustained.</param>
    public void TakeDamage(int amount)
    {
        if (!isServer)
        {
            return;
        }
        else{
            health -= amount;
        }

        if (health <= 0)
        {
            TargetDeath();
        }
    }

    /// <summary>
    /// This is called when a melee weapon hits the Device.
    /// </summary>
    /// <param name="other">The collider of the gameobject which hit this gameobject.</param>
    private void OnTriggerEnter(Collider other)
    {
        other.transform.root.GetComponent<Melee>().meleeHit(gameObject);
    }

    abstract public void TargetDeath();


    [ClientRpc]
    public void RpcDestroyDevice(GameObject device)
    {
        Destroy(device);
    }
}