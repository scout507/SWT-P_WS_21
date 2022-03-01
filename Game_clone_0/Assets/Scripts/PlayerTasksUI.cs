using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class PlayerTasksUI : NetworkBehaviour
{
    TaskManager taskManager;
    List<string[]> taskList = new List<string[]>();
    [SerializeField] GameObject taskUI;
    List<GameObject> taskListUI = new List<GameObject>();
    bool created;
    float updateInterval = 1f;
    float updateTimer;


    void Start()
    {
        taskManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<TaskManager>();
    }

    void Update()
    {
        if(!isLocalPlayer) return;

        updateTimer -= Time.deltaTime;
        if(taskManager.tasksCreated && !created)
        {
            createUIList();
            created = true;
        }

        if(created && updateTimer <= 0)
        {
            UpdateTaskUI();
            updateTimer = updateInterval;
        } 
    }


    void createUIList()
    {
        if(!isServer) CmdSyncList();
        taskList = taskManager.GetTaskInfo();
        
        
        for(int i = 0; i < taskList.Count; i++)
        {

            GameObject panel = Instantiate(new GameObject(), transform.position, Quaternion.identity);
            TextMeshProUGUI textElement = panel.AddComponent<TextMeshProUGUI>();
            panel.transform.SetParent(taskUI.transform);
            //taskUIObjects[i].SetActive(true);
            //TextMeshProUGUI textElement = taskUIObjects[i].GetComponent<TextMeshProUGUI>();
            string textSting = (i+1).ToString() + ". " + taskList[i][1] + " - " + taskList[i][2];
            textElement.text = textSting;
            textElement.enableAutoSizing = true;
            textElement.color = Color.black;
            
            taskListUI.Add(panel);
        }
    }

    void UpdateTaskUI()
    {
        taskList = taskManager.GetTaskInfo();
        for(int i = 0; i < taskList.Count; i++)
        {
            if(taskList[i][3] == "True")
            {
                taskListUI[i].GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Strikethrough;
            }
            else
            {
                taskListUI[i].GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
            } 
        }    
    }

    [Command]
    void CmdSyncList()
    {
        taskManager.SyncList();
    }
}
