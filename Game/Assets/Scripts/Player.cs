using UnityEngine;
using Mirror;

// created by: SWT-P_WS_21/22

/// <summary>This class holds variables which are needed on the player prefab.</summary>
public class Player : NetworkBehaviour
{
    /// <summary>Name of the player.</summary>
    [SyncVar]
    public string displayName = "";

    /// <summary>
    /// Calls the registration command on start.
    /// </summary>
    void Start()
    {
        if (!isLocalPlayer)
            return;
        CmdRegisterPlayer();
    }

    /// <summary>
    /// Calls the RoundManager to register the player upon joining the game.
    /// </summary>
    [Command]
    void CmdRegisterPlayer()
    {
        GameObject
            .FindGameObjectWithTag("Manager")
            .GetComponent<RoundManager>()
            .Register(this.gameObject);
    }
}
