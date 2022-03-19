using UnityEngine;
using Mirror;

/* created by: SWT-P_WS_21/22 */

/// <summary>
/// Base class for the ingame tasks. Every task must inherit from this class.
/// Handles basic information about the task, which is used by the task manager.
/// </summary>
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
    [SyncVar] public string taskDescription;

    /// <summary>Progress in percent. 50% should be 0.5</summary>
    [SyncVar] public float progress;
    /// <summary>List of all Players within the interactable radius</summary>
    readonly public SyncList<uint> players = new SyncList<uint>();
    /// <summary>DestructableObject script to track the health of the task and make it attackable.</summary>
    public DestructableObject dObjScript;

    /// <summary>
    /// Gets all dependencies
    /// </summary>
    private void Start()
    {
        dObjScript = GetComponent<DestructableObject>();
    }

    /// <summary>
    /// Used to finish the task.
    /// </summary>
    public void FinishTask()
    {
        dObjScript.attackAble = false;
        active = false;
        done = true;
        progress = 1;
        UpdateHealth();
    }

    /// <summary>
    /// Used to undo the task and set it active again.
    /// </summary>
    public void UndoTask()
    {
        active = true;
        done = false;
        progress = 0;
        UpdateHealth();
    }

    /// <summary>
    /// Used to upgrade the health while building the task.
    /// </summary>
    private void UpdateHealth()
    {
        dObjScript.SetHealth(progress);
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
