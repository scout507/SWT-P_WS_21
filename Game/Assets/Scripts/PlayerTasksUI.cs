using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;


/// <summary>
/// This class handles the task-UI component.
/// Automaticly gets all tasks and displays them on screen.
/// </summary>
public class PlayerTasksUI : NetworkBehaviour
{
    /// <summary>Taskmanager for getting the information</summary>
    TaskManager taskManager;
    /// <summary>List of all tasks in this Game</summary>
    List<string[]> taskList = new List<string[]>();
    /// <summary>UI element holding the list</summary>
    [SerializeField] GameObject taskUI;
    /// <summary>List of all task UI elements</summary>
    List<GameObject> taskListUI = new List<GameObject>();
    /// <summary>True when the task UI has been created</summary>
    bool created;
    /// <summary>Interval in seconds for refreshing the UI info</summary>
    float updateInterval = 1f;
    /// <summary>Timer for refreshing the UI info</summary>
    float updateTimer;

    /// <summary>
    /// Gets dependencies.
    /// </summary>
    void Start()
    {
        taskManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<TaskManager>();
    }


    /// <summary>
    /// Handles intial creation and updates for the UI elements
    /// </summary>
    void Update()
    {
        if (!isLocalPlayer) return;

        updateTimer -= Time.deltaTime;
        if (taskManager.tasksCreated && !created)
        {
            createUIList();
            created = true;
        }

        if (created && updateTimer <= 0)
        {
            UpdateTaskUI();
            updateTimer = updateInterval;
        }
    }

    /// <summary>
    /// Fetches all task information from the taskmanager.
    /// Creates an UI element for each existing task.
    /// </summary>
    void createUIList()
    {
        if (!isServer) CmdSyncList();
        taskList = taskManager.GetTaskInfo();


        for (int i = 0; i < taskList.Count; i++)
        {
            GameObject panel = Instantiate(new GameObject(), transform.position, Quaternion.Euler(0, 0, 0));
            TextMeshProUGUI textElement = panel.AddComponent<TextMeshProUGUI>();
            panel.transform.SetParent(taskUI.transform);
            textElement.enableAutoSizing = true;
            textElement.fontSizeMin = 12;
            textElement.color = Color.black;
            taskListUI.Add(panel);
        }
    }


    /// <summary>
    /// Updates the task UI elements. Adds 'strikethrough' once they're finished.
    /// </summary>
    void UpdateTaskUI()

    {
        taskList = taskManager.GetTaskInfo();
        for (int i = 0; i < taskList.Count; i++)
        {
            TextMeshProUGUI taskUI = taskListUI[i].GetComponent<TextMeshProUGUI>();
            taskUI.rectTransform.rotation = Quaternion.Euler(0, 0, 0);
            taskUI.rectTransform.localScale = new Vector3(1, 1, 1);
            taskUI.text = (i + 1).ToString() + ". " + taskList[i][1] + " - " + taskList[i][2];

            if (taskList[i][3] == "True")
            {
                taskUI.fontStyle = FontStyles.Strikethrough;
            }
            else
            {
                taskUI.fontStyle = FontStyles.Bold;
            }
        }
    }

    /// <summary>
    /// Used for synchronisation of the tasklist.
    /// </summary>
    [Command]
    void CmdSyncList()
    {
        taskManager.SyncList();
    }
}
