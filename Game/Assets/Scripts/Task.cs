using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Task : NetworkBehaviour
{
    public bool active;
    public bool done;


    float progress; //progress in percent. 50% should be 0.5;
    List<GameObject> players = new List<GameObject>();
    DestructableObject dObjScript;
    
    private void Start()
    {
        dObjScript = GetComponent<DestructableObject>();
    }

    /// <summary>
    /// Used to upgrade the health while building the task.
    /// </summary>
    private void UpdateHealth()
    {
        dObjScript.SetHealth(progress);
    }

    /// <summary>
    /// Used to finish the task.
    /// </summary>
    private void FinishTask()
    {
        active = false;
        done = true;
    }

    /// <summary>
    /// Adds players to the players List once they are in the interactive radius.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player") players.Add(other.gameObject);
    }


    /// <summary>
    /// Removes players from the player list when they leave the interactive radius.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player") players.Remove(other.gameObject);
    }

    
}
