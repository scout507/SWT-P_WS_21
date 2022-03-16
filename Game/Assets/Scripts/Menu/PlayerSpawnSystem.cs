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
    /// <summary>Spawn point of the player prefab</summary>///
    [SerializeField] Vector3 spawn = new Vector3(-22f,-27f,-0.74f);
    /// <summary>Offset for spawning the players</summary>///
    float offset = 0;

    /// <summary>
    /// Holds the prefabs for the player that can be spawned.
    /// Requires a minimum and maximum of 8 prefabs.
    /// </summary>
    [SerializeField]
    private List<GameObject> playerPrefabs = null;

    /// <summary>
    /// Contains the default prefab for the player that can be spawned 
    /// if there is no prefab left in the "playerPrefabs" list.
    /// </summary>
    [SerializeField]
    private GameObject playerPrefabDefault = null;

    /// <summary>
    /// Called when the class will been instantiated.
    /// Add the Function "SpawnPlayer()" to the Networkmanager.
    /// </summary>
    public override void OnStartServer()
    {
        NetworkManagerLobby.OnServerReadied += SpawnPlayer;
        base.OnStartServer();
    }

    /// <summary>
    /// Called when the class will been Destroyed.
    /// Removes the Function "SpawnPlayer()" to the Networkmanager.
    /// </summary>
    [ServerCallback]
    private void OnDestroy() => NetworkManagerLobby.OnServerReadied -= SpawnPlayer;

    /// <summary>
    /// Called only by the server.
    /// Randomly creates the player with one of the predefined presets/classes.
    /// If no more prefab is available, the default prefab is used.
    /// Also sets the display name on the player prefab.
    /// </summary>
    /// <param name="conn">Network connection from the client.</param>
    [Server]
    public void SpawnPlayer(NetworkConnection conn)
    {
        GameObject playerInstance;

        if (playerPrefabs.Count == 0)
            playerInstance = Instantiate(playerPrefabDefault, new Vector3(spawn.x + offset, spawn.y, spawn.z), Quaternion.identity);
        else
        {
            int index = new System.Random().Next(playerPrefabs.Count);
            playerInstance = Instantiate(playerPrefabs[index], new Vector3(spawn.x + offset, spawn.y, spawn.z), Quaternion.identity);
            playerPrefabs.RemoveAt(index);
            offset += 1.5f;
        }

        playerInstance.GetComponent<Player>().displayName = conn.identity.GetComponent<NetworkGamePlayer>().displayName;

        NetworkServer.ReplacePlayerForConnection(conn, playerInstance.gameObject, true);
    }
}
