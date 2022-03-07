using System;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Collections.Generic;

/* created by: SWT-P_WS_21/22 */


/// <summary>Class implements the Network Manager for the Start Lobby.</summary>
public class NetworkManagerLobby : NetworkManager
{
    /// <summary>Specifies the minimum number of clients required.</summary>
    [SerializeField]
    private int minPlayers = 2;

    /// <summary>Specifies the name of the menue scene.</summary>
    [Scene]
    [SerializeField]
    private string menuScene = "";

    [Header("Room")]
    /// <summary>Specifies which prefab is to be used for the clients.</summary>
    [SerializeField]
    private NetworkRoomPlayer roomPlayerPrefab = null;

    [Header("Game")]
    /// <summary>Specifies which prefab is to be used for the clients in the game.</summary>
    [SerializeField]
    private NetworkGamePlayer gamePlayerPrefab = null;

    [SerializeField]
    private GameObject playerSpawnSystem = null;

    /// <summary>Holds the object of all clients prefabs, as a list.</summary>
    [SerializeField]
    public List<NetworkRoomPlayer> roomPlayers = new List<NetworkRoomPlayer>();

    /// <summary>Holds the object of all clients ingame prefabs, as a list.</summary>
    [SerializeField]
    public List<NetworkGamePlayer> gamePlayers = new List<NetworkGamePlayer>();

    /// <summary>Holds methods defined by the class "JoinLobbyMenu".</summary>
    public static event Action OnClientConnected;

    /// <summary>Holds methods defined by the class "JoinLobbyMenu".</summary>
    public static event Action OnClientDisconnected;

    public static event Action<NetworkConnection> OnServerReadied;

    /// <summary>
    /// Loads all available prefabs when the host server is started.
    /// </summary>
    public override void OnStartServer()
    {
        spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();
    }

    /// <summary>
    /// Loads all available prefabs when the client is started and registers them with the client-networkmanager.
    /// </summary>
    public override void OnStartClient()
    {
        var spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");

        foreach (var prefab in spawnablePrefabs)
        {
            NetworkClient.RegisterPrefab(prefab);
        }
    }

    /// <summary>
    /// Is called on the client when it is connected to a server and calls the function that is stored in "OnClientConnected".
    /// </summary>
    /// <param name="conn">Network connection from client to server and server to client.</param>
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        OnClientConnected?.Invoke();
    }

    /// <summary>
    /// Is called on the client when it is disconnected to a server and calls the function that is stored in "OnClientDisconnected".
    /// </summary>
    /// <param name="conn">Network connection from client to server and server to client.</param>
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        OnClientDisconnected?.Invoke();
    }

    /// <summary>
    /// When a client connecting to the server, checks whether the maximum number of clients has already been 
    /// reached and whether the client connecting is in the menu scene.
    /// </summary>
    /// Maximale Anzahl ist erreicht => Client wird disconnected
    /// Client nicht in der Menü Szene => Client wird disconnected
    /// <param name="conn">Network connection from client to server and server to client.</param>
    public override void OnServerConnect(NetworkConnection conn)
    {
        if (numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        }

        if (SceneManager.GetActiveScene().path != this.menuScene)
        {
            conn.Disconnect();
            return;
        }
    }

    /// <summary>
    /// When a client wants to connect to the server, it is checked whether it is in the correct scene and 
    /// whether it is the leader. Then a prefab for the client is instantiated and the method 
    /// "NetworkServer.AddPlayerForConnection()" is called.
    /// </summary>
    /// <param name="conn">Network connection from client to server and server to client.</param>
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        if (SceneManager.GetActiveScene().path == menuScene)
        {
            bool isLeader = roomPlayers.Count == 0;

            NetworkRoomPlayer roomPlayerInstance = Instantiate(roomPlayerPrefab);

            roomPlayerInstance.IsLeader = isLeader;

            NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
        }
    }

    /// <summary>
    /// Called on the server when a client disconnects.
    /// Removes the client from the "roomPlayers" list.
    /// The method "notifyPlayersOfReadyState()" is then called.
    /// </summary>
    /// <param name="conn">Network connection from client to server and server to client.</param>
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if (conn.identity != null)
        {
            var player = conn.identity.GetComponent<NetworkRoomPlayer>();
            roomPlayers.Remove(player);
            notifyPlayersOfReadyState();
        }

        base.OnServerDisconnect(conn);
    }

    /// <summary>
    /// This is called when a server is stopped - including when a host is stopped.
    /// Empties the "roomPlayers" list.
    /// </summary>
    public override void OnStopServer()
    {
        roomPlayers.Clear();
    }

    /// <summary>
    /// Calls the method "handleReadyToStart()" for all clients in the "roomPlayers" list.
    /// </summary>
    public void notifyPlayersOfReadyState()
    {
        foreach (var player in roomPlayers)
        {
            player.handleReadyToStart(isReadyToStart());
        }
    }

    /// <summary>
    /// Checks that all the requirements for starting the game have been met.
    /// </summary>
    /// <returns>
    /// false = minimum number of players not reached
    /// false = all players are not yet ready
    /// otherwise true
    /// </returns>
    public bool isReadyToStart()
    {
        if (numPlayers < minPlayers)
            return false;

        foreach (var player in roomPlayers)
            if (!player.isReady)
                return false;

        return true;
    }

    /// <summary>
    /// Starts the game from the host.
    /// Starts only when all players are ready.
    /// </summary>
    public void startGame()
    {
        if (SceneManager.GetActiveScene().path == menuScene)
        {
            if (!isReadyToStart())
                return;

            ServerChangeScene("1. Final");
        }
    }

    /// <summary>
    /// Changes the scene from the server and all clients to the game scene.
    /// Changes the prefabs from the players to the prefabs for the game.
    /// </summary>
    /// <param name="newSceneName">Requires the name of the scene to be loaded.</param>
    public override void ServerChangeScene(string newSceneName)
    {
        if (SceneManager.GetActiveScene().path == menuScene && newSceneName.StartsWith("1. Final"))
        {
            for (int i = roomPlayers.Count - 1; i >= 0; i--)
            {
                var conn = roomPlayers[i].connectionToClient;
                var gamePlayerInstance = Instantiate(gamePlayerPrefab);
                gamePlayerInstance.SetDisplayName(roomPlayers[i].displayName);

                NetworkServer.Destroy(conn.identity.gameObject);

                NetworkServer.ReplacePlayerForConnection(conn, gamePlayerInstance.gameObject, true);
            }
        }
        base.ServerChangeScene(newSceneName);
    }

    /// <summary>
    /// Spawns the spawn manager for each player, at scene change.
    /// </summary>
    /// <param name="newSceneName">Name of the scene to switch to.</param>
    public override void OnServerSceneChanged(string newSceneName)
    {
        if (newSceneName.StartsWith("1. Final"))
        {
            GameObject playerSpawnSystemInstance = Instantiate(playerSpawnSystem);
            NetworkServer.Spawn(playerSpawnSystemInstance);
        }
        base.OnServerChangeScene(newSceneName);
    }

    /// <summary>
    /// Calls the spawn method from the spawn manager, 
    /// for each client, when the client has loaded the new scene.
    /// </summary>
    /// <param name="conn">Network connection to the client.</param>
    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);

        OnServerReadied?.Invoke(conn);
    }
}
