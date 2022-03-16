using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/* created by: SWT-P_WS_21/22 */


/// <summary>
/// The Health class manages the player's health points. It is also responsible for capturing damage and also the death of the player.
/// </summary>
public class Health : NetworkBehaviour
{
    /// <summary>
    /// Variable for Health, synced on all Clients
    /// </summary>
    [SyncVar]
    public int health = 100;

    HealthBar healthBar;

    /// <summary>Holds the prefab for a dead player.</summary>
    public GameObject deadPlayerPrefab = null;
    /// <summary>Holds the prefab for a spectator.</summary>
    public GameObject spectatorPlayerPrefab = null;
    /// <summary>Is true when the player is dead.</summary>
    public bool isDead = false;
    /// <summary>Audio Script that controlls Gun Sound</summary>
    public AudioController audioController;

    void Start()
    {
        if (!isLocalPlayer) return;

        healthBar = GetComponentInChildren<HealthBar>();
        health = 100;
        audioController = this.GetComponent<AudioController>();
    }

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
        if (amount < 0 && health - amount > 100)
        {
            health = 100;
        }
        else
        {
            health -= amount;
            if (health > 0)
            {
                TargetRpcDamageSounds(connectionToClient, 1, 10);
            }
        }
        if (amount > 0) TargetDamage();
        else GotHealed();
        if (health <= 0 && !isDead)
        {
            TargetRpcDamageSounds(connectionToClient, 10, 11);
            isDead = true;
            TargetDeath();
        }
    }

    /// <summary>
    /// The method TargetDamage is called when a player is hit. It can then trigger an animation or something similar.
    /// </summary>
    [TargetRpc]
    public void TargetDamage()
    {
        Debug.Log("Took damage!");
    }

    /// <summary>
    /// The method GotHealed is called when a player is healed. It can then trigger an animation or something similar.
    /// </summary>
    [TargetRpc]
    public void GotHealed()
    {
        Debug.Log("Got healed!");
    }
    /// <summary>
    /// Triggers a damage sound on a player that got hit.
    /// </summary>
    /// <param name="target">Player that got hit</param>
    /// <param name="min">Soundindex</param>
    /// <param name="max">Soundindex</param>
    [TargetRpc]
    void TargetRpcDamageSounds(NetworkConnection target, int min, int max)
    {
        audioController.CmdPlayDmgTakenSound(min, max);
    }

    /// <summary>
    /// The method TargetDeath is called when a player dies. It destroys the gameobject of the player and resets the main camera.
    /// </summary>
    [TargetRpc]
    void TargetDeath()
    {
        Debug.Log("You are Dead");
        CmdDestroyPlayer();
    }

    /// <summary>
    /// Spawns the deadPlayer prefab and the spectator prefab.
    /// Sets the game object of the spectator as a new player prefab.
    /// </summary>
    [Command]
    void CmdDestroyPlayer()
    {
        GameObject deadPlayer = Instantiate(deadPlayerPrefab, transform.position, transform.rotation);
        GameObject spectator = Instantiate(spectatorPlayerPrefab, transform.position + Vector3.up * 5, transform.rotation);

        NetworkServer.Spawn(deadPlayer);
        NetworkServer.ReplacePlayerForConnection(connectionToClient, spectator.gameObject, true);
        RpcDestroyPlayer();
    }

    /// <summary>Deactivates the player object.</summary>
    [ClientRpc]
    void RpcDestroyPlayer()
    {
        gameObject.SetActive(false);
    }
}
