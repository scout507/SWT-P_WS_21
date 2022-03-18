using System.Collections.Generic;
using UnityEngine;
using Mirror;

/* created by: SWT-P_WS_21/22 */

/// <summary>
/// This class implements the Defence/Timer task. Inherits from Task.
/// </summary>
public class TaskTimer : Task
{
    /// <summary>The spawnable zombies</summary>
    [SerializeField]
    GameObject zombie;

    /// <summary>Places for the Zombies to spawn</summary>
    [SerializeField]
    List<GameObject> zombieSpawns;

    /// <summary>Time for the next zombies to spawn</summary>
    float nextZombies = 2f;

    /// <summary>True when the task has started</summary>
    bool started;

    /// <summary>The amount of times the hp has come down to 0</summary>
    float fails;

    /// <summary>
    /// Handles Client interaction. Also handles the timer on the server.
    /// </summary>
    void Update()
    {
        if (isClient) //vvv Player interaction with the task vvv
        {
            if (
                players.Contains(
                    NetworkClient.localPlayer.gameObject.GetComponent<NetworkIdentity>().netId
                )
                && active
                && !started
            ) //Checks if the player is in range
            {
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

                //Check if the task is failed or finished
                if (dObjScript.health <= 1)
                {
                    fails += 1 * Time.deltaTime;
                }
                if (fails >= 3)
                {
                    started = false;
                    taskDescription = "Task failed. Go back to the task to try again.";
                    fails = 0;
                    UndoTask();
                    dObjScript.attackAble = false;
                }
                else if (dObjScript.health < 100)
                {
                    dObjScript.health += 1 * Time.deltaTime;
                    UpdateText();
                }
                else
                    FinishTask();

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
        nextZombies = Random.Range(3, 10);
        int zombieAmount = Random.Range(1, 4);

        for (int i = 0; i < zombieAmount; i++)
        {
            GameObject zombieInstance = Instantiate(
                zombie,
                zombieSpawns[Random.Range(0, zombieSpawns.Count)].transform.position,
                Quaternion.identity
            );
            NetworkServer.Spawn(zombieInstance);
        }
    }

    /// <summary>
    /// Updates the text for the UI to show progress of the task.
    /// </summary>
    void UpdateText()
    {
        int progress = (int)dObjScript.health;
        taskDescription = "Defend the object: " + progress.ToString() + "% done!";
    }

    /// <summary>
    /// Adds players to the players List once they are in the interactive radius.
    /// </summary>
    /// <param name="other">Collider of the entering GameObject</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && isServer)
        {
            TargetRpcSendMessage(other.gameObject.GetComponent<NetworkIdentity>().connectionToClient, "Press 'E' to start the Defence-task");
            players.Add(other.GetComponent<NetworkIdentity>().netId);
        }
    }

    /// <summary>
    /// Used to start the Countdown by the players.
    /// </summary>
    [Command(requiresAuthority = false)]
    void CmdStartCountdown()
    {
        dObjScript.attackAble = true;
        started = true;
    }

    /// <summary>
    /// Sends a message to the choosen player.
    /// </summary>
    /// <param name="target">The players NetworkConnecton</param>
    /// <param name="message">The message for the player to recieve</param>
    [TargetRpc]
    void TargetRpcSendMessage(NetworkConnection target, string message)
    {
        if (NetworkClient.localPlayer.gameObject.GetComponent<Chatbox>()) NetworkClient.localPlayer.gameObject.GetComponent<Chatbox>().AddMessage(message);
    }
}
