using Mirror;

/* created by: SWT-P_WS_21/22 */

/// <summary>This class is needed for the prefab of a client in the game.</summary>
public class NetworkGamePlayer : NetworkBehaviour
{
    /// <summary>A synchronised variable containing the name of the player or a placeholder.</summary>
    [SyncVar]
    public string displayName = "Loading...";

    /// <summary>
    /// Triggers when the client/host starts.
    /// Adds the client to the "gamePlayers" list of the network manager.
    /// </summary>
    public override void OnStartClient()
    {
        DontDestroyOnLoad(gameObject);

        (NetworkManager.singleton as NetworkManagerLobby).gamePlayers.Add(this);
    }

    /// <summary>
    /// Triggers when the client/host stops.
    /// Removes the client from the "gamePlayers" list of the network manager.
    /// </summary>
    public override void OnStopClient()
    {
        (NetworkManager.singleton as NetworkManagerLobby).gamePlayers.Remove(this);
    }

    /// <summary>
    /// Can only be called from the server.
    /// Change the name of the client.
    /// </summary>
    /// <param name="displayName">Name of the player.</param>
    [Server]
    public void SetDisplayName(string displayName)
    {
        this.displayName = displayName;
    }
}
