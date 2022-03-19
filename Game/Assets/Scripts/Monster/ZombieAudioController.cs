using UnityEngine;
using Mirror;


/* created by: SWT-P_WS_21/22 */


/// <summary>
/// The Script to handle the Sounds on the Monster
/// </summary>
public class ZombieAudioController : NetworkBehaviour
{
    /// <summary>
    /// The Audio Source that is attached to the Monster
    /// </summary>
    public AudioSource audioSource;

    /// <summary>
    /// The Audio Clips, Zombie Footstep Sounds
    /// </summary>
    [SerializeField] AudioClip[] monsterFootsteps;

    /// <summary>
    /// The Audio Clips, Zombie Footstep Sounds
    /// </summary>
    [SerializeField] AudioClip[] monsterIdleSound;

    /// <summary>
    /// The Audio Clips, Zombie Injured and Death Sounds
    /// </summary>
    [SerializeField] AudioClip[] monsterDmgTakenSound;

    /// <summary>
    /// The Audio Clips, Zombie Attack Sounds
    /// </summary>
    [SerializeField] AudioClip[] monsterAttackSound;

    /// <summary>
    /// The Time until the next Footstep Sound is Played
    /// </summary>
    private float stepCoolDown;

    /// <summary>
    /// The Time until the next Idle Sound is Played
    /// </summary>
    private float idleCooldown;

    /// <summary>
    /// The selected Zombie Footstep Sound
    /// </summary>
    private int monsterFootstep;

    /// <summary>
    /// The selected Zombie Idle Sound 
    /// </summary>
    private int monsterIdle;

    /// <summary>
    /// Checks if the zombie is alive, stops Footsteps and Idle Sounds when dead
    /// </summary>
    private bool isAlive = true;


    private void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
        monsterFootstep = Random.Range(0, 4);
        monsterIdle = Random.Range(0, 7);
    }

    /// <summary>
    /// The Footstep Sounds gets called every time the last Sound stopped
    /// The Idle Sounds gets called every 10*Idle Sound length
    /// </summary>
    void Update()
    {
        if (!isServer) return;

        stepCoolDown -= Time.deltaTime;
        if (stepCoolDown < 0f && isAlive)
        {
            float randomf = 1f + Random.Range(-0.2f, 0.2f);
            RPCPlayFootStepSound(randomf);
            stepCoolDown = monsterFootsteps[monsterFootstep].length;
        }

        idleCooldown -= Time.deltaTime;
        if (idleCooldown < 0f && isAlive)
        {
            RPCPlayZombieIdleSound();
            idleCooldown = monsterIdleSound[monsterIdle].length * 10;
        }
    }

    /// <summary>
    /// Checks if there's a wall or an object between the player and the zombie.
    /// Used to adjust volume for zombies behind walls, and under ground.
    /// </summary>
    /// <returns>Returns true if the zombie can raycast the player.</returns>
    bool CheckWalls()
    {
        RaycastHit hit;
        if (Physics.Raycast(NetworkClient.localPlayer.gameObject.transform.position, this.gameObject.transform.position, out hit, 30f))
        {
            if (hit.collider.gameObject.layer == 8) return false;
            else return true;
        }

        return true;
    }

    /// <summary>
    /// The ClientRpC for the Zombie Footstep Sound
    /// </summary>
    [ClientRpc]
    void RPCPlayFootStepSound(float randomf)
    {
        float vol = 0.4f;
        if (CheckWalls()) vol *= 0.2f;
        if (!audioSource) audioSource = this.GetComponent<AudioSource>();
        audioSource.pitch = randomf;
        audioSource.PlayOneShot(monsterFootsteps[monsterFootstep], vol);
    }

    /// <summary>
    /// The ClientRpC for the Zombie Idle Sound
    /// </summary>
    [ClientRpc]
    void RPCPlayZombieIdleSound()
    {
        float vol = 0.8f;
        if (CheckWalls()) vol *= 0.2f;
        if (!audioSource) audioSource = this.GetComponent<AudioSource>();
        audioSource.PlayOneShot(monsterIdleSound[monsterIdle], vol);
    }

    /// <summary>
    /// The ClientRpC for the Zombie Sound, when the Zombie took Damage
    /// </summary>
    [ClientRpc]
    public void RPCPlayZombieDmgSound(int selected)
    {
        float vol = 0.6f;
        if (CheckWalls()) vol *= 0.2f;
        if (!audioSource) audioSource = this.GetComponent<AudioSource>();
        audioSource.PlayOneShot(monsterDmgTakenSound[selected], vol);
    }

    /// <summary>
    /// The ClientRpC for the Zombie Sound, when the Zombie Died
    /// </summary>
    [ClientRpc]
    public void RPCPlayZombieDeathSound(int selected)
    {
        float vol = 0.8f;
        if (CheckWalls()) vol *= 0.2f;
        isAlive = false;
        if (!audioSource) audioSource = this.GetComponent<AudioSource>();
        audioSource.PlayOneShot(monsterDmgTakenSound[selected], vol);
    }

    /// <summary>
    /// The ClientRpC for the Zombie Sound, when the Zombie Attacked
    /// </summary>
    [ClientRpc]
    public void RPCPlayZombieAttackSound(int selected)
    {
        float vol = 0.8f;
        if (CheckWalls()) vol *= 0.2f;
        if (!audioSource) audioSource = this.GetComponent<AudioSource>();
        audioSource.PlayOneShot(monsterAttackSound[selected], vol);
    }
}
