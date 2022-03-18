using UnityEngine;
using Mirror;

/* created by: SWT-P_WS_21/22 */


/// <summary>
/// The Script to handle the Sounds on the Player
/// </summary>
public class AudioController : NetworkBehaviour
{

    /// <summary>
    /// The Audio Source that is attached to the Player
    /// </summary>
    public AudioSource audioSource;

    /// <summary>
    /// The Audio Clips, Gun Sounds and Footstep Sounds
    /// </summary>
    [SerializeField] AudioClip[] gunSounds;

    /// <summary>
    /// The Audio Clips, Gun Sounds and Footstep Sounds
    /// </summary>
    [SerializeField] AudioClip[] playerSounds;

    /// <summary>
    /// The Time until the next Footstep Sound is Played
    /// </summary>
    private float stepCoolDown;


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
                stepCoolDown = playerSounds[10].length;
            }
        }
    }


    /// <summary>
    /// The Command for the Footstep Sound
    /// </summary>
    [Command]
    void CmdPlayFootStepSound()
    {
        float randomf = 1f + Random.Range(-0.2f, 0.2f);
        RPCPlayFootStepSound(randomf);
    }

    /// <summary>
    /// The ClientRpC for the Footstep Sound, to play the sound on all Clients
    /// </summary>
    [ClientRpc]
    void RPCPlayFootStepSound(float randomf)
    {
        if (!audioSource) audioSource = this.GetComponent<AudioSource>();
        audioSource.pitch = randomf;
        audioSource.PlayOneShot(playerSounds[10], 0.4f);
    }


    /// <summary>
    /// Method is called on Weapon Usage, once per shot
    /// WeaponNumbe is the Number of the Used weapon, Number is linked to a SoundClip
    /// 0 = Pistol; 1 = MP ; 2 = Shotgun ; 3 = Melee ; 4 = Rifle
    /// 5 = Pistol reload; 6 = MP reload; 7 = Shotgun reload; 8 = Rifle reload
    /// </summary>
    [Command]
    public void CmdPlayGunSound(int weaponNumber)
    {
        float randomf = 1f + Random.Range(-0.1f, 0.1f);
        RPCPlayGunSound(weaponNumber, randomf);
    }


    /// <summary>
    /// The ClientRpC for the Gun Sound, to play the sound on all Clients
    /// with a fixed Volume level
    /// and a changing pitch value
    /// </summary>
    [ClientRpc]
    void RPCPlayGunSound(int weaponNumber, float randomf)
    {
        if (!audioSource) audioSource = this.GetComponent<AudioSource>();
        audioSource.pitch = randomf;
        audioSource.PlayOneShot(gunSounds[weaponNumber], 0.5f);
    }

    /// <summary>
    /// Method is called when the Player takes Damage
    /// with a changeg pitch value
    /// and different Damage Sound every time its called
    /// </summary>
    [Command]
    public void CmdPlayDmgTakenSound(int min, int max)
    {
        int random = Random.Range(min, max);
        float randomf = 1f + Random.Range(-0.2f, 0.2f);
        RPCPlayDmgTakenSound(random, randomf);
    }


    /// <summary>
    /// The ClientRpC for the Player Damage taken Sound
    /// with a fixed Volume level
    /// and a changing pitch value
    /// </summary>
    [ClientRpc]
    void RPCPlayDmgTakenSound(int random, float randomf)
    {
        if (!audioSource) audioSource = this.GetComponent<AudioSource>();
        audioSource.pitch = randomf;
        audioSource.PlayOneShot(playerSounds[random], 0.6f);
    }
}
