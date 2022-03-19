using UnityEngine;
using Mirror;

/* created by: SWT-P_WS_21/22 */


/// <summary>
/// The Health class manages the player's health points. It is also responsible for capturing damage and also the death of the player.
/// </summary>
public class Health : NetworkBehaviour
{
    /// <summary>Variable for Health, synced on all Clients.</summary>
    [SyncVar]
    public int health = 100;

    /// <summary>Reference to the UI element HealthBar.</summary>
    HealthBar healthBar;
    /// <summary>Holds the prefab for a dead player.</summary>
    public GameObject deadPlayerPrefab = null;
    /// <summary>Holds the prefab for a spectator.</summary>
    public GameObject spectatorPlayerPrefab = null;
    /// <summary>Is true when the player is dead.</summary>
    public bool isDead = false;
    /// <summary>Audio Script that controlls Gun Sound</summary>
    public AudioController audioController;
    /// <summary>UI element responsible for flashing red on beeing hit.</summary>
    public GameObject hitflash;

    /// <summary>
    /// In the start function the needed variables are set.
    /// </summary>
    void Start()
    {
        if (!isLocalPlayer) return;

        healthBar = GetComponentInChildren<HealthBar>();
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
        else TargetRpcGotHealed();
        if (health <= 0 && !isDead)
        {
            TargetRpcDamageSounds(connectionToClient, 10, 11);
            isDead = true;
            TargetDeath();
        }
    }

    /// <summary>
    /// Disables the hitflash UI element.
    /// </summary>
    void DisableHitFlash()
    {
        hitflash.SetActive(false);
    }

    /// <summary>
    /// The method TargetDamage is called when a player is hit. It can then trigger an animation or something similar.
    /// </summary>
    [TargetRpc]
    public void TargetDamage()
    {
        hitflash.SetActive(true);
        Invoke("DisableHitFlash", 0.2f);
    }

    /// <summary>
    /// The method GotHealed is called when a player is healed. It can then trigger an animation or something similar.
    /// </summary>
    [TargetRpc]
    public void TargetRpcGotHealed()
    {
        //TODO: Add animation/sound for healing. (Missing healing animation for current player-model)
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
        CmdDestroyPlayer();
    }

    /// <summary>
    /// Spawns the deadPlayer prefab and the spectator prefab.
    /// Sets the game object of the spectator as a new player prefab.
    /// </summary>
    [Command]
    void CmdDestroyPlayer()
    {
        gameObject.tag = "Untagged";
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
