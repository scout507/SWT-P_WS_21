using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/* created by: SWT-P_WS_21/22 */


public class TaskTimer : Task
{
    /// <summary>The spawnable zombies</summary>
    [SerializeField] GameObject zombie;
    /// <summary>Places for the Zombies to spawn</summary>
    [SerializeField] List<GameObject> zombieSpawns;
    /// <summary>The progress toward completion each second. Finished on reaching 100</summary>
    [SerializeField] float progressRate = 1f;
    /// <summary>Time for the next zombies to spawn</summary>
    float nextZombies = 2f;
    /// <summary>True when the task has started</summary>
    bool started;

    /// <summary>
    /// Handles Client interaction. Also handles the timer on the server.
    /// </summary>
    void Update()
    {
        if (isClient) //vvv Player interaction with the task vvv 
        {
            if (players.Contains(NetworkClient.localPlayer.gameObject.GetComponent<NetworkIdentity>().netId) && active && !started) //Checks if the player is in range
            {
                Debug.Log("Press E");

                if (Input.GetKeyDown(KeyCode.E))
                {
                    CmdStartCountdown();
                }
            }
        }

        if (isServer)
        {

            if (started && active)
            {
                nextZombies -= Time.deltaTime;

                if(dObjScript.health <100) dObjScript.health += 1*Time.deltaTime;
                else FinishTask();

                UpdateText();

                if (nextZombies <= 0)
                {
                    SpawnZombies();
                }
            }
        }
    }

    /// <summary>
    /// Spawns the Zombies during the task.
    /// </summary>
    void SpawnZombies()
    {
        nextZombies = Random.Range(3,10);
        int zombieAmount = Random.Range(1,4);

        for(int i = 0; i<zombieAmount; i++)
        {
            GameObject zombieInstance = Instantiate(zombie, zombieSpawns[Random.Range(0,zombieSpawns.Count)].transform.position ,Quaternion.identity);
            NetworkServer.Spawn(zombieInstance); 
        }
    }

    /// <summary>
    /// Updates the text for the UI to show progress of the task.
    /// </summary>
    void UpdateText()
    {
        int progress = (int) dObjScript.health;
        taskDescription = "Defend the object: " + progress.ToString() + "% done!";
    }

    /// <summary>
    /// Used to start the Countdown by the players.
    /// </summary>
    [Command(requiresAuthority = false)]
    void CmdStartCountdown()
    {
        started = true;
    }
}
