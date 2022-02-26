using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


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
   [SerializeField] 
   float timeBetweenWaves;
   float waveTimer;
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
       
       gameTimer += Time.deltaTime;
       prepTimer -= Time.deltaTime;
       playerRefreshTimer -= Time.deltaTime;

       if(playerRefreshTimer <= 0)
       {
           //players = GetAllPlayers();
           playerRefreshTimer = playerRefreshTime;
           Debug.Log(players.Count);
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
       int r = Random.Range(0, players.Count);
   }

   bool CheckGameOver()
   {
       if(gameTimer >= timePerRound) return true;
       else if(activePlayers.Count == 0) return true;
       else return taskManager.CheckAllFinished(); 
   }

   void ChooseWinner()
   {
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

   void Register(GameObject player)
   {
       player.transform.position = playerSpawn.transform.position;
       joinedPlayers++;
       players.Add(player.GetComponent<NetworkIdentity>().netId); 
   }

    void InitGame()
    {
        ChooseImpostor();
        activePlayers = GetAllPlayers();
        ready = true;
    }

   


}
