using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/* created by: SWT-P_WS_21/22 */

/// <summary>
/// Meeting Button used to call/alarm meetings
/// </summary>
public class MeetingButton : NetworkBehaviour, IInteractable
{
    /// <summary>
    /// Is the meeting button ready to be interacted with?
    /// </summary>
    private bool isReady;

    /// <summary>
    /// Cooldown timer used to determine if the meeting button can be used again
    /// </summary>
    private float cooldownTimer;
    
    /// <summary>
    /// Cooldown time to wait before the meeting button is usable again
    /// </summary>
    [SerializeField] private float cooldown;

    /// <summary>
    /// AudioSource of the meeting button to play sounds
    /// </summary>
    [SerializeField] private AudioSource audioSource;

    void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Interactable");
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
    }

    /// <summary>
    /// Handler for player interaction
    /// </summary>
    void IInteractable.OnInteract() 
    {
        if (!isReady) return;
        audioSource.PlayOneShot(audioSource.clip);
        isReady = false;
        cooldownTimer = cooldown;
    }
}
