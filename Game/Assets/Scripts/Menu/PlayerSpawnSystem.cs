using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

/// <summary>
/// Needed to spawn players when transitioning from the menu to the game.
/// </summary>
public class PlayerSpawnSystem : NetworkBehaviour
{

    /// <summary>
    /// Holds the prefab for the player that will be spawned.
    /// </summary>
    [SerializeField] private GameObject playerPrefab = null;

    /// <summary>
    /// Add the Function "SpawnPlayer()" to the Networkmanager.
    /// </summary>
    public override void OnStartServer()
    {
        NetworkManagerLobby.OnServerReadied += SpawnPlayer;
        base.OnStartServer();
    }

    [ServerCallback]
    private void OnDestroy() => NetworkManagerLobby.OnServerReadied -= SpawnPlayer;

    [Server]
    public void SpawnPlayer(NetworkConnection conn)
    {

        GameObject playerInstance = Instantiate(playerPrefab);
        //NetworkServer.Spawn(playerInstance, conn);
        NetworkServer.ReplacePlayerForConnection(conn, playerInstance.gameObject, true);
    }


}
