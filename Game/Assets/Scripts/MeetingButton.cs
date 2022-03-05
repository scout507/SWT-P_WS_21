using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/* created by: SWT-P_WS_21/22 */

/// <summary>
/// Meeting Button used to call/alarm meetings
/// </summary>
public class MeetingButton : Interactable
{
    /// <summary>
    /// Is the meeting button ready to be interacted with?
    /// </summary>
    [SyncVar] private bool isReady;

    /// <summary>
    /// Cooldown timer used to determine if the meeting button can be used again
    /// </summary>
    [SyncVar] private float cooldownTimer;
    
    /// <summary>
    /// Cooldown time to wait before the meeting button is usable again
    /// </summary>
    [SerializeField] private float cooldown;

    /// <summary>
    /// AudioSource of the meeting button to play sounds
    /// </summary>
    [SerializeField] private AudioSource audioSource;


    /* <summary>
    /// Set the meeting button's ready status
    /// </summary>
    [Command]
    void CmdSetIsReady(bool isReady)
    {
        this.isReady = isReady;
    }
    
    /// <summary>
    /// Set the meeting button's cooldown timer
    /// </summary>
    void CmdSetCooldownTimer(float cooldownTimer)
    {
        this.cooldownTimer = cooldownTimer;
    }*/

    /// <summary>
    /// ClientRpc to play the meeting alarm for all clients
    /// </summary>
    [ClientRpc]
    void RPCPlayAlarm()
    {
        Debug.Log("RPCPlayAlarm()");
        audioSource.PlayOneShot(audioSource.clip);
    }

    public override void Start()
    {
        base.Start();
        isReady = true;
        cooldownTimer = 0;
    }

    void Update()
    {
        if (cooldownTimer > 0)
        {
           cooldownTimer -= Time.deltaTime;
        }
        else if (!isReady) {
            isReady = true;
        }

        Debug.Log("isReady: " + isReady);
        Debug.Log("cooldownTimer: " + cooldownTimer);
    }

    /// <summary>
    /// Handler for player interaction
    /// </summary>
    public override void OnInteract() 
    {
                Debug.Log("OnInteract()");

        if (!isReady) {
                    Debug.Log("OnInteract() not ready");
            return;

        }
                Debug.Log("OnInteract() ready");

        isReady = false;
        cooldownTimer = cooldown;
        RPCPlayAlarm();
        Debug.Log("OnInteract() finished");

    }
}
