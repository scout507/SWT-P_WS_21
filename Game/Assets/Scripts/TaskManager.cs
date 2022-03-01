using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/* created by: SWT-P_WS_21/22 */


public class TaskManager : NetworkBehaviour
{
    /// <summary>True when tasks are created and spawned</summary>
    [SyncVar] public bool tasksCreated;
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

    /// <summary>List of all tasks to spawn</summary>
    List<GameObject> tasks = new List<GameObject>();
    /// <summary>List of all task instances in the game</summary>
    List<GameObject> activeTasks = new List<GameObject>();

    List<GameObject> clientList = new List<GameObject>();

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
            activeTasks.Add(taskInstance);
        }

    }

    /// <summary>
    /// Used to get Information about the tasks.
    /// </summary>
    /// <returns>Returns a list of string arrays. [0] id, [1] name, [2] description, [3] bool value of done (true/false)</returns>
    public List<string[]> GetTaskInfo()
    {
        List<string[]> info = new List<string[]>();
        List<GameObject> listToCheck;

        if (!isServer) listToCheck = clientList;
        else listToCheck = activeTasks;

        for (int i = 0; i < listToCheck.Count; i++)
        {
            string[] infoArr = new string[4];
            Task task = listToCheck[i].GetComponent<Task>();
            infoArr[0] = task.id.ToString();
            infoArr[1] = task.name;
            infoArr[2] = task.taskDescription;
            infoArr[3] = task.done.ToString();
            info.Add(infoArr);
        }

        return info;
    }

    /// <summary>
    /// Can be called to check wether all tasks are finished yet.
    /// </summary>
    /// <returns>True if all tasks are finished</returns>
    public bool CheckAllFinished()
    {
        for (int i = 0; i < activeTasks.Count; i++)
        {
            if (!activeTasks[i].GetComponent<Task>().done) return false;
        }

        return true;
    }


    /// <summary>
    /// Used for choosing and spawning tasks
    /// </summary>
    public void InitTasks()
    {
        ChooseTasks();
        SpawnTasks();
        tasksCreated = true;
        RpcSyncList(activeTasks);
    }

    /// <summary>
    /// Called for synchronising the task list for clients.
    /// </summary>
    public void SyncList()
    {
        RpcSyncList(activeTasks);
    }

    /// <summary>
    /// Syncronises the task list on the client.
    /// </summary>
    /// <param name="list">Task List of the server</param>
    [ClientRpc]
    void RpcSyncList(List<GameObject> list)
    {
        clientList = new List<GameObject>();
        foreach (GameObject task in list)
        {
            clientList.Add(task);
        }
    }

}
