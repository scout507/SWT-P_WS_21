using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AudioController : NetworkBehaviour
{

    public AudioSource audioSource; // The Audio Source that is attached to the Player

    public AudioClip[] audioClips; // The Audio Clips, Gun Sounds and Footstep Sounds
    public float stepRate = 0.5f; // The Footstep Sound Rate
    public float stepCoolDown; // The Time until the next Footstep Sound is Played
    /// <summary>Volume for the sounds</summary>
    public float vol = 1f;

    private void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
    }


    /// <summary>
    /// The Footstep Sounds gets called on Player Movement with a delay between the next Footstep
    /// </summary>
    void Update()
    {
        if (isLocalPlayer)
        {
            stepCoolDown -= Time.deltaTime;
            if ((Input.GetAxis("Horizontal") != 0f || Input.GetAxis("Vertical") != 0f) && stepCoolDown < 0f)
            {
                CmdPlayFootStepSound();
                stepCoolDown = stepRate;
            }
        }
    }


    /// <summary>
    /// The Command for the Footstep Sound
    /// </summary>
    [Command]
    void CmdPlayFootStepSound()
    {
        RPCPlayFootStepSound();
    }

    /// <summary>
    /// The ClientRpC for the Footstep Sound, to play the sound on all Clients
    /// </summary>
    [ClientRpc]
    void RPCPlayFootStepSound()
    {
        audioSource.volume = 0.3f * vol;
        audioSource.pitch = 1f + Random.Range(-0.2f, 0.2f);
        audioSource.PlayOneShot(audioClips[5], 0.9f);
        stepCoolDown = stepRate;
    }

    /// <summary>
    /// Method is called on Weapon Usage, once per shot
    /// WeaponNumbe is the Number of the Used weapon, Number is linked to a SoundClip
    /// 0 = Pistol; 1 = MP ; 2 = Shotgun ; 3 = Melee ; 4 = Rifle
    /// </summary>
    public void PlayGunSound(int weaponNumber)
    {
        Debug.Log(weaponNumber);
        CmdSendGunSound(weaponNumber);
    }


    /// <summary>
    /// The Command for the Gun Sound
    /// </summary>
    [Command]
    void CmdSendGunSound(int weaponNumber)
    {
        RPCSendGunSound(weaponNumber);
    }


    /// <summary>
    /// The ClientRpC for the the Gun Sound, to play the sound on all Clients
    /// with a fixed Volume level
    /// and a changing pitch value
    /// </summary>
    [ClientRpc]
    void RPCSendGunSound(int weaponNumber)
    {
        audioSource.volume = 0.7f * vol;
        audioSource.pitch = 1f + Random.Range(-0.1f, 0.1f);
        audioSource.PlayOneShot(audioClips[weaponNumber]);
    }

}
