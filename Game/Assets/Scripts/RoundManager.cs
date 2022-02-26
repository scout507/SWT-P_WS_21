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
   public int totalPlayers;
   /// <summary>A list containing all players </summary>//  
   List<uint> players = new List<uint>();
   List<uint> activePlayers = new List<uint>();
   uint impostor;
   [SerializeField]
   GameObject playerSpawn;
   float playerRefreshTime = 1f;
   float playerRefreshTimer;

   int joinedPlayers; 


    //NPC-related
   ZombieSpawner zombieSpawner; 
   

    // Task-related
   int finishedTasks;
   int totalTasks;
   TaskManager taskManager;

    //Game-related
   [SerializeField] 
   float timePerRound;
   [SerializeField]
   float prepTimer; 
   float gameTimer = 0;
   bool ready;
   bool started;

   void Start()
   {
       if(!isServer) return;

       zombieSpawner = GetComponent<ZombieSpawner>();
       taskManager = GetComponent<TaskManager>();
   }


   void Update()
   {
       if(!isServer) return;

       //Timers
       if(!ready)
       {
           if(joinedPlayers == totalPlayers) InitGame();
           else return;
       }

       if(!started)
       {
          prepTimer -= Time.deltaTime;
          if(prepTimer <= 0) StartGame();
          else return;
       }

       gameTimer += Time.deltaTime;
       playerRefreshTimer -= Time.deltaTime;

       if(playerRefreshTimer <= 0)
       {
           activePlayers = GetAllPlayers();
           playerRefreshTimer = playerRefreshTime;
       }



       if(CheckGameOver()) ChooseWinner(); 
   }


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


   void ChooseImpostor()
   {
       impostor = players[Random.Range(0, players.Count)];
       Debug.Log(impostor.ToString() + " is the impostor");
   }

   bool CheckGameOver()
   {
       if(gameTimer >= timePerRound) return true;
       else if(activePlayers.Count == 0) return true;
       else return taskManager.CheckAllFinished(); 
   }

   void ChooseWinner()
   {
        //TODO: Add endgame-screen or something.
        if(gameTimer >= timePerRound)
        {
            //Everyone loses the game
        }
        else if(taskManager.CheckAllFinished())
        {
            if(activePlayers.Count > 1)
            {
                //Team wins
            }
            else
            {
                if(activePlayers.Contains(impostor))
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

    void InitGame()
    {
        ChooseImpostor();
        activePlayers = GetAllPlayers();
        zombieSpawner.InitialSpawn();
        taskManager.InitTasks();
        ready = true;
        //TODO: start some Countdown on the HUD
    }

    void StartGame()
    {
        started = true;
        //TODO: Open doors and maybe some other stuff
        Debug.Log("The Game has started.");
        Debug.Log(taskManager.GetTaskInfo());
    }

    public void Register(GameObject player)
   {
       joinedPlayers++;
       players.Add(player.GetComponent<NetworkIdentity>().netId);
       RpcJoinMessage(player.GetComponent<NetworkIdentity>().netId.ToString());
       TargetRpcMoveToSpawn(player.GetComponent<NetworkIdentity>().connectionToClient, player); 
   }


    [ClientRpc]
    void RpcJoinMessage(string playerName)
    {   
        //TODO: Replace with actual UI message
        Debug.Log(playerName + " has joined the game!");
    }

    [TargetRpc]
    void TargetRpcMoveToSpawn(NetworkConnection target, GameObject player)
    {
        player.transform.position = playerSpawn.transform.position;
    }
}
