using System;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Collections.Generic;

/// <summary>Class implements the Network Manager for the Start Lobby.</summary>
public class NetworkManagerLobby : NetworkManager
{
    /// <summary>Specifies the minimum number of clients required.</summary>
    [SerializeField] private int minPlayers = 2;

    /// <summary>Specifies the name of the menue scene.</summary>
    [Scene] [SerializeField] private string menuScene = "";

    [Header("Room")]
    /// <summary>Specifies which prefab is to be used for the clients.</summary>
    [SerializeField] private NetworkRoomPlayer roomPlayerPrefab = null;

    /// <summary>Holds the object of all clients' prefabs, as a list.</summary>
    [SerializeField] public List<NetworkRoomPlayer> roomPlayers = new List<NetworkRoomPlayer>();

    /// <summary>Holds methods defined by the class "JoinLobbyMenu".</summary>
    public static event Action OnClientConnected;

    /// <summary>Holds methods defined by the class "JoinLobbyMenu".</summary>
    public static event Action OnClientDisconnected;


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
        if (numPlayers < minPlayers) return false;

        foreach (var player in roomPlayers) if (!player.isReady) return false;

        return true;
    }
}