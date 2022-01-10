using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TaskTimer : Task
{
    /// <summary>Timer in seconds</summary>
    [SerializeField] float timer;

    bool started;

    /// <summary>
    /// Handles Client interaction. Also handles the timer on the server.
    /// </summary>
    void Update()
    {
        if (isClient) //vvv Player interaction with the task vvv 
        {
            if (players.Contains(NetworkClient.localPlayer.gameObject.GetComponent<NetworkIdentity>().netId) && active) //Checks if the player is in range
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
                timer -= Time.deltaTime;

                if (timer <= 0)
                {
                    FinishTask();
                    Debug.Log("Finished!!");
                }
            }
        }
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
