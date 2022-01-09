using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TaskManager : NetworkBehaviour
{
    /// <summary>Amount of simple tasks per game</summary>
    [SerializeField] int amountSimple;
    /// <summary>Amount of medium tasks per game</summary>
    [SerializeField] int amountMedium;
    /// <summary>Amount of hard tasks per game</summary>
    [SerializeField] int amountHard;

    /// <summary>List of simple Tasks</summary>
    [SerializeField] GameObject[] simpleTasks;
    /// <summary>List of medium Tasks</summary>
    [SerializeField] GameObject[] mediumTasks;
    /// <summary>List of hard Tasks</summary>
    [SerializeField] GameObject[] hardTasks;

    /// <summary>All tasks active this game</summary>
    List<GameObject> tasks = new List<GameObject>();


    

    void Start()
    {
        if(!isServer) return;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isServer) return;
    }

    /// <summary>
    /// Randomly chooses tasks from their lists and adds them to the overall tasks List
    /// </summary>
    void SpawnTasks(){

        //Create list for all difficulties
        List<GameObject> simples = new List<GameObject>();
        List<GameObject> mediums = new List<GameObject>();
        List<GameObject> hards = new List<GameObject>();

        //Add all tasks to their list
        for(int i = 0; i<simpleTasks.Length; i++)
        {
            simples.Add(simpleTasks[i]);
        }
        for(int i = 0; i<mediumTasks.Length; i++)
        {
            mediums.Add(mediumTasks[i]);
        }
        for(int i = 0; i<hardTasks.Length; i++)
        {
            hards.Add(hardTasks[i]);
        }

        //Randomly choose Tasks and add them to tasks list
        for(int i = 0; i<amountSimple; i++)
        {
            int r = Random.Range(0,simples.Count);
            tasks.Add(simples[r]);
            simples.RemoveAt(r);
        }

        for(int i = 0; i<amountMedium; i++)
        {
            int r = Random.Range(0,mediums.Count);
            tasks.Add(mediums[r]);
            mediums.RemoveAt(r);
        }

        for(int i = 0; i<amountHard; i++)
        {
            int r = Random.Range(0,hards.Count);
            tasks.Add(hards[r]);
            hards.RemoveAt(r);
        }
    }
}
