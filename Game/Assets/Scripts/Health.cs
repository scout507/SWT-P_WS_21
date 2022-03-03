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

    public HealthBar healthBar;

    public GameObject deadPlayerPrefab = null;

    public GameObject spectatorPlayerPrefab = null;

    void Start()
    {

        health = 100;
        healthBar.SetMaxHealth(health);
        if (!isLocalPlayer) return;
        CmdRegisterPlayer();
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
        }
        if (amount > 0) TargetDamage();
        else GotHealed();
        if (health <= 0)
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
    /// The methode GotHealed is called when a player is healed. It can then trigger an animation or something similar.
    /// </summary>
    [TargetRpc]
    public void GotHealed()
    {
        Debug.Log("Got healed!");
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
        Debug.Log("You are Dead");
        CmdDestroyPlayer(gameObject);
    }

    [Command]
    void CmdDestroyPlayer(GameObject character)
    {
        GameObject deadPlayer = Instantiate(deadPlayerPrefab, transform.position, transform.rotation);
        GameObject spectator = Instantiate(spectatorPlayerPrefab, transform.position + Vector3.up * 5, transform.rotation);

        NetworkServer.Spawn(deadPlayer);
        RpcDestroyPlayer(character);
        Debug.Log(connectionToClient);
        NetworkServer.ReplacePlayerForConnection(connectionToClient, spectator.gameObject, true);
    }

    [ClientRpc]
    void RpcDestroyPlayer(GameObject character)
    {
        Destroy(character);
    }


    /// <summary>
    /// Calls the RoundManager to register the player upon joining the game.
    /// </summary>
    [Command]
    void CmdRegisterPlayer()
    {
        GameObject.FindGameObjectWithTag("Manager").GetComponent<RoundManager>().Register(this.gameObject);
    }
}
