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



    /// <summary>
    /// Used for choosing and spawning tasks
    /// </summary>
    void Start()
    {
        if (!isServer) return;
        ChooseTasks();
        SpawnTasks();
    }

    /// <summary>
    /// Randomly chooses tasks from their lists and adds them to the overall tasks List
    /// </summary>
    void ChooseTasks()
    {
        int id = 0;
        //Create list for all difficulties
        List<GameObject> simples = new List<GameObject>();
        List<GameObject> mediums = new List<GameObject>();
        List<GameObject> hards = new List<GameObject>();

        //Add all tasks to their list
        for (int i = 0; i < simpleTasks.Length; i++)
        {
            simples.Add(simpleTasks[i]);
        }
        for (int i = 0; i < mediumTasks.Length; i++)
        {
            mediums.Add(mediumTasks[i]);
        }
        for (int i = 0; i < hardTasks.Length; i++)
        {
            hards.Add(hardTasks[i]);
        }

        //Randomly choose Tasks and add them to tasks list
        for (int i = 0; i < amountSimple; i++)
        {
            int r = Random.Range(0, simples.Count);
            simples[r].GetComponent<Task>().id = id;
            tasks.Add(simples[r]);
            simples.RemoveAt(r);
            id++;
        }

        for (int i = 0; i < amountMedium; i++)
        {
            int r = Random.Range(0, mediums.Count);
            mediums[r].GetComponent<Task>().id = id;
            tasks.Add(mediums[r]);
            mediums.RemoveAt(r);
            id++;
        }

        for (int i = 0; i < amountHard; i++)
        {
            int r = Random.Range(0, hards.Count);
            hards[r].GetComponent<Task>().id = id;
            tasks.Add(hards[r]);
            hards.RemoveAt(r);
            id++;
        }
    }

    /// <summary>
    /// Spawns tasks contained in the Tasks-List at their spawn location.
    /// </summary>
    void SpawnTasks()
    {
        foreach (GameObject task in tasks)
        {

            GameObject taskInstance = Instantiate(task, task.GetComponent<Task>().spawn, Quaternion.identity);
            taskInstance.GetComponent<Task>().active = true;
            NetworkServer.Spawn(taskInstance);
        }

    }

    /// <summary>
    /// Used to get Information about the tasks.
    /// </summary>
    /// <returns>Returns a list of string arrays. [0] id, [1] name, [2] description, [3] bool value of done (true/false)</returns>
    public List<string[]> GetTaskInfo()
    {
        List<string[]> info = new List<string[]>();

        for (int i = 0; i < tasks.Count; i++)
        {
            string[] infoArr = new string[4];
            Task task = tasks[i].GetComponent<Task>();
            infoArr[0] = task.id.ToString();
            infoArr[1] = task.name;
            infoArr[2] = task.taskDescription;
            infoArr[3] = task.done.ToString();
            info.Add(infoArr);
        }

        return info;
    }

    public bool CheckAllFinished()
    {
        for(int i = 0; i<tasks.Count; i++){
            if(!tasks[i].GetComponent<Task>().done) return false;
        }

        return true;
    }
}
