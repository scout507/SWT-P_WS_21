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
       zombieSpawner = GetComponent<ZombieSpawner>();
       taskManager = GetComponent<TaskManager>();

       players = GetAllPlayers();
       Debug.Log(players.Count);
       ChooseImpostor();
   }


   void Update()
   {
       //Timers
       gameTimer += Time.deltaTime;
       prepTimer -= Time.deltaTime; 

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
                    //impostor wins
                }
                else
                {
                    //team wins
                }
            }

        }
   }




   


}
