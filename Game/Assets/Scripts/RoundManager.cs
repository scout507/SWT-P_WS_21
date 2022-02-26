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
   /// <summary>A list containing all players </summary>//  
   List<GameObject> players = new List<GameObject>();
   GameObject impostor;
   [SerializeField]
   GameObject playerSpawn;
   float playerRefreshTime = 1f;
   float playerRefreshTimer;


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


   void Start()
   {
       if(!isServer) return;

       zombieSpawner = GetComponent<ZombieSpawner>();
       taskManager = GetComponent<TaskManager>();

       players = GetAllPlayers();
       Debug.Log(players.Count);
       ChooseImpostor();
   }


   void Update()
   {
       if(!isServer) return;

       //Timers
       gameTimer += Time.deltaTime;
       prepTimer -= Time.deltaTime;
       playerRefreshTimer -= Time.deltaTime;

       if(playerRefreshTimer <= 0)
       {
           players = GetAllPlayers();
           playerRefreshTimer = playerRefreshTime;
           Debug.Log(players.Count);
       }



       if(CheckGameOver()) ChooseWinner(); 
   }


   List<GameObject> GetAllPlayers()
   {
       List<GameObject> newPlayerList = new List<GameObject>();
       GameObject[] playersArray = GameObject.FindGameObjectsWithTag("Player");
       
       foreach (GameObject player in playersArray)
       {
           newPlayerList.Add(player);
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
       else if(players.Count == 0) return true;
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
            if(players.Count > 1)
            {
                //Team wins
            }
            else
            {
                if(players.Contains(impostor))
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




   


}
