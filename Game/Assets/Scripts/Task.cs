using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/* created by: SWT-P_WS_21/22 */


public class Task : NetworkBehaviour
{
    /// <summary>True when the task is ready to be interacted with.</summary>
    [SyncVar] public bool active;
    /// <summary>True when the task is done.</summary>
    [SyncVar] public bool done;
    /// <summary>The place where the task should spawn.</summary>
    public Vector3 spawn;
    /// <summary>Id of the task, assinged by the TaskManager. Can be used to identify the Task.</summary>
    public int id;
    /// <summary>Name of the task for the UI.</summary>
    public string taskName;
    /// <summary>Short description for the UI.</summary>
    public string taskDescription;

    /// <summary>Progress in percent. 50% should be 0.5</summary>
    [SyncVar] public float progress;
    /// <summary>List of all Players within the interactable radius</summary>
    readonly public SyncList<uint> players = new SyncList<uint>();

    DestructableObject dObjScript;

    /// <summary>
    /// Gets all dependencies
    /// </summary>
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
    public void FinishTask()
    {
        active = false;
        done = true;
        progress = 1;
        UpdateHealth();
    }

    /// <summary>
    /// Adds players to the players List once they are in the interactive radius.
    /// </summary>
    /// <param name="other">Collider of the entering GameObject</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && isServer) players.Add(other.GetComponent<NetworkIdentity>().netId);
    }


    /// <summary>
    /// Removes players from the player list when they leave the interactive radius.
    /// </summary>
    /// <param name="other">Collider of the exiting GameObject</param>
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && isServer) players.Remove(other.GetComponent<NetworkIdentity>().netId);
    }


}
