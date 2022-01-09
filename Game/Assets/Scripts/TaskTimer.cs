using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TaskTimer : Task
{
    [SerializeField] float timer;

    bool started;

    void Update()
    {
        if(isLocalPlayer)
        {
            if(players.Contains(this.gameObject) && active)
            {
                Debug.Log("Press E");

                if(Input.GetKeyDown(KeyCode.E))
                {
                    StartCountdown();
                }
            }
        }

        if(isServer)
        {
            if(started && active)
            {
                timer -= Time.deltaTime;

                if(timer <= 0)
                {
                    FinishTask();
                    Debug.Log("Finished!!");
                }
            }
        }
    }


    [Command]
    void StartCountdown()
    {
        started = true;
    }
}
