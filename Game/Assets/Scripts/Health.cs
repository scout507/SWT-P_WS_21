using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/// <summary>
/// The Health class manages the player's health points. It is also responsible for capturing damage and also the death of the player.
/// </summary>
public class Health : NetworkBehaviour
{
    [SyncVar]
    public int health = 100;

    /// <summary>
    /// The method TakeDamage is responsible for suffering damage.
    /// </summary>
    /// <param name="amount">The parameter amount indicates how much damage was sustained.</param>
    public void TakeDamage(int amount)
    {
        if(!isServer) 
        return;
        health -= amount;
        TargetDamage();
        if(health <= 0)
        {
            TargetDeath();
        }
    }

    /// <summary>
    /// The methode TargetDamage is called when a player is hit. It can then trigger an animation or something similar.
    /// </summary>
    [TargetRpc]
    public void TargetDamage()
    {
        Debug.Log("Took damage!");
    }

    /// <summary>
    /// The methode TargetDeath is called when a player dies. It destroys the gameobject of the player and resets the main camera.
    /// </summary>
    [TargetRpc]
    void TargetDeath()
    {
        Camera.main.transform.parent = null;
        Camera.main.transform.position = new Vector3(-5.8f, 84.5f, -48.3f);
        Camera.main.transform.rotation = Quaternion.Euler(51f, 0f, 0f);
        Destroy(gameObject);
        Debug.Log("You are Dead");
    }
}
