using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/* created by: SWT-P_WS_21/22 */

/// <summary>
/// This class handles the background work for round management.
/// </summary>
public class RoundManager : NetworkBehaviour
{

    // Player-related
    /// <summary>Amount of players in this round</summary>//
    public int totalPlayers;
    /// <summary>Amount of players that have joined this round</summary>//
    int joinedPlayers;
    /// <summary>A list containing all net ID's of players </summary>//  
    List<uint> players = new List<uint>();
    /// <summary>A list containing all net ID's of players that are alive</summary>//
    List<uint> activePlayers = new List<uint>();
    /// <summary>Net-ID of the impostor</summary>//
    uint impostor;
    /// <summary>The player's spawn as an empty game-object</summary>//
    [SerializeField] GameObject playerSpawn;
    /// <summary>Refresh rate for searching for active players</summary>//
    float playerRefreshTime = 1f;
    /// <summary>Timer for searching for active players</summary>//
    float playerRefreshTimer;


    //NPC-related
    /// <summary>Zombiespawner-script</summary>//
    ZombieSpawner zombieSpawner;


    // Task-related
    /// <summary>TaskManager-script</summary>//
    TaskManager taskManager;

    //Game-related
    /// <summary>Total time a round can take before beeing game over</summary>//
    [SerializeField] float timePerRound;
    /// <summary>Length of the preperation phase</summary>//
    [SerializeField] float prepTimer;
    /// <summary>Timer for the round time</summary>//
    float gameTimer = 0;
    /// <summary>True when all players have joined the game</summary>//
    bool ready;
    /// <summary>True when the preperation phase is over</summary>//
    bool started;


    /// <summary>
    /// Gets all dependencies
    /// </summary>
    void Start()
    {
        if (!isServer) return;

        if(NetworkManager.singleton as NetworkManagerLobby != null) totalPlayers = (NetworkManager.singleton as NetworkManagerLobby).gamePlayers.Count;
        zombieSpawner = GetComponent<ZombieSpawner>();
        taskManager = GetComponent<TaskManager>();
    }


    /// <summary>
    /// Handles timers and checks for state changes
    /// </summary>
    void Update()
    {
        if (!isServer) return;

        //Timers
        if (!ready)
        {
            if (joinedPlayers == totalPlayers) InitGame();
            else return;
        }

        if (!started)
        {
            prepTimer -= Time.deltaTime;
            if (prepTimer <= 0) StartGame();
            else return;
        }

        gameTimer += Time.deltaTime;
        playerRefreshTimer -= Time.deltaTime;

        if (playerRefreshTimer <= 0)
        {
            activePlayers = GetAllPlayers();
            playerRefreshTimer = playerRefreshTime;
        }



        if (CheckGameOver()) ChooseWinner();
    }

    /// <summary>
    /// Used for getting a list of all active (alive) players
    /// </summary>
    /// <returns>A list of net-ID's of all active players.</returns>
    List<uint> GetAllPlayers()
    {
        List<uint> newPlayerList = new List<uint>();
        GameObject[] playersArray = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in playersArray)
        {
            newPlayerList.Add(player.GetComponent<NetworkIdentity>().netId);
        }

        return newPlayerList;
    }

    /// <summary>
    /// Initialises the game once all players have joined.
    /// </summary>
    void InitGame()
    {
        ChooseImpostor();
        activePlayers = GetAllPlayers();
        zombieSpawner.InitialSpawn();
        taskManager.InitTasks();
        ready = true;
        //TODO: start some Countdown on the HUD
    }

    /// <summary>
    /// Randomly chooses one of the players as the impostor. Calls a TargetRPC to inform the player that he has been chosen.
    /// </summary>
    void ChooseImpostor()
    {
        impostor = players[Random.Range(0, players.Count)];
        //TODO: Tell the impostor he's chosen as impostor

        GameObject[] playersArray = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in playersArray)
        {
            if (player.GetComponent<NetworkIdentity>().netId == impostor)
            {
                TargetRpcTellImpostor(player.GetComponent<NetworkIdentity>().connectionToClient);
            }
        }
    }

    /// <summary>
    /// Can bee use to determine wether the game is over.
    /// </summary>
    /// <returns>True when a game-over state is reached</returns>
    bool CheckGameOver()
    {
        if (gameTimer >= timePerRound) return true;
        else if (activePlayers.Count == 0) return true;
        else return taskManager.CheckAllFinished();
    }

    /// <summary>
    /// Chooses the winner of the entire round.
    /// </summary>
    void ChooseWinner()
    {
        //TODO: Add endgame-screen or something.
        if (gameTimer >= timePerRound)
        {
            //Everyone loses the game
        }
        else if (taskManager.CheckAllFinished())
        {
            if (activePlayers.Count > 1)
            {
                //Team wins
            }
            else
            {
                if (activePlayers.Contains(impostor))
                {
                    //Impostor wins
                }
                else
                {
                    //Team wins
                }
            }
        }
    }


    /// <summary>
    /// Starts the game once the preperation time is over.
    /// </summary>
    void StartGame()
    {
        started = true;
        //TODO: Open doors and maybe some other stuff
        Debug.Log("The Game has started.");
        Debug.Log(taskManager.GetTaskInfo());
    }

    /// <summary>
    /// This is used by the player to register once they joined the game. Adds the player to the players list.
    /// </summary>
    /// <param name="player">The gameobject of the player that calls this method.</param>
    public void Register(GameObject player)
    {
        joinedPlayers++;
        players.Add(player.GetComponent<NetworkIdentity>().netId);
        RpcJoinMessage(player.GetComponent<NetworkIdentity>().netId.ToString());
        TargetRpcMoveToSpawn(player.GetComponent<NetworkIdentity>().connectionToClient, player);
    }

    /// <summary>
    /// Sends a join-message to all players when a new player as joined the game.
    /// </summary>
    /// <param name="playerName">The name of the player that has joined. Currently the net-id is used here.</param>
    [ClientRpc]
    void RpcJoinMessage(string playerName)
    {
        //TODO: Replace with actual UI message
        Debug.Log(playerName + " has joined the game!");
    }

    /// <summary>
    /// Moves a player to the spawn position on the map.
    /// </summary>
    /// <param name="target">ConnectionToClient of the player</param>
    /// <param name="player">The gameobject of the player that needs to be moved</param>
    [TargetRpc]
    void TargetRpcMoveToSpawn(NetworkConnection target, GameObject player)
    {
        player.transform.position = playerSpawn.transform.position;
    }


    /// <summary>
    /// Sends a message to the chosen impostor.
    /// </summary>
    /// <param name="target">ConnectionToClient of the impostor</param>
    [TargetRpc]
    void TargetRpcTellImpostor(NetworkConnection target)
    {
        //TODO: Tell the impostor in the UI that he has been chosen
        Debug.Log("You are the impostor");
    }
}
