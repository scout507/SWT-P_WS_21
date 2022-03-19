using UnityEngine;
using Mirror;

/* created by: SWT-P_WS_21/22 */

/// <summary>
/// Meeting Button used to call/alarm meetings
/// </summary>
public class MeetingButton : Interactable
{
    /// <summary>Is the meeting button ready to be interacted with?</summary>
    [SyncVar] private bool isReady;

    /// <summary>Cooldown timer used to determine if the meeting button can be used again</summary>
    [SyncVar] private float cooldownTimer;

    /// <summary>Cooldown time to wait before the meeting button is usable again</summary>
    [SerializeField] private float cooldown;

    /// <summary>AudioSource of the meeting button to play sounds</summary>
    private AudioSource audioSource;

    /// <summary>
    /// Initializes the meeting button to be ready
    /// </summary>
    public override void Start()
    {
        base.Start();
        audioSource = GetComponent<AudioSource>();
        isReady = true;
        cooldownTimer = 0;
    }

    /// <summary>
    /// Update cooldown timer and ready status
    /// </summary>
    void Update()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
        else if (!isReady)
        {
            isReady = true;
        }
    }

    /// <summary>
    /// Handler for player interaction
    /// </summary>
    public override void OnInteract()
    {
        if (!isReady) return;
        isReady = false;
        cooldownTimer = cooldown;
        RPCPlayAlarm();
    }

    /// <summary>
    /// ClientRpc to play the meeting alarm for all clients
    /// </summary>
    [ClientRpc]
    void RPCPlayAlarm()
    {
        audioSource.PlayOneShot(audioSource.clip);
    }
}
