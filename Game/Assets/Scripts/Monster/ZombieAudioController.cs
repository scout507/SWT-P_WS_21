using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ZombieAudioController : NetworkBehaviour
{

    public AudioSource audioSource; // The Audio Source that is attached to the Monster
    float stepCoolDown; // The Time until the next Footstep Sound is Played
    float idleCooldown; // The Time until the next Idle Sound is Played

    [SerializeField] AudioClip[] monsterFootsteps; // The Audio Clips, Zombie Footstep Sounds
    [SerializeField] AudioClip[] monsterIdleSound; // The Audio Clips, Zombie Footstep Sounds
    [SerializeField] AudioClip[] monsterDmgTakenSound; // The Audio Clips, Zombie Injured and Death Sounds
    [SerializeField] AudioClip[] monsterAttackSound; // The Audio Clips, Zombie Attack Sounds
    int monsterFootstep; // The selected Zombie Footstep Sound
    int monsterIdle; // The selected Zombie Idle Sound 
    bool isAlive = true;

    private void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
        monsterFootstep = Random.Range(0, 4);
        monsterIdle = Random.Range(0, 7);
    }

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
    /// The ClientRpC for the Zombie Footstep Sound
    /// </summary>
    [ClientRpc]
    void RPCPlayFootStepSound(float randomf)
    {
        if (!audioSource) audioSource = this.GetComponent<AudioSource>();
        audioSource.pitch = randomf;
        audioSource.PlayOneShot(monsterFootsteps[monsterFootstep], 0.4f);
    }

    /// <summary>
    /// The ClientRpC for the Zombie Idle Sound
    /// </summary>
    [ClientRpc]
    void RPCPlayZombieIdleSound()
    {
        if (!audioSource) audioSource = this.GetComponent<AudioSource>();
        audioSource.PlayOneShot(monsterIdleSound[monsterIdle], 0.8f);
    }

    /// <summary>
    /// The ClientRpC for the Zombie Sound, when the Zombie took Damage
    /// </summary>
    [ClientRpc]
    public void RPCPlayZombieDmgSound(int selected)
    {
        if (!audioSource) audioSource = this.GetComponent<AudioSource>();
        audioSource.PlayOneShot(monsterDmgTakenSound[selected], 0.6f);
    }

    /// <summary>
    /// The ClientRpC for the Zombie Sound, when the Zombie Died
    /// </summary>
    [ClientRpc]
    public void RPCPlayZombieDeathSound(int selected)
    {
        isAlive = false;
        if (!audioSource) audioSource = this.GetComponent<AudioSource>();
        audioSource.PlayOneShot(monsterDmgTakenSound[selected], 0.8f);
    }

    /// <summary>
    /// The ClientRpC for the Zombie Sound, when the Zombie Attacked
    /// </summary>
    [ClientRpc]
    public void RPCPlayZombieAttackSound(int selected)
    {
        if (!audioSource) audioSource = this.GetComponent<AudioSource>();
        audioSource.PlayOneShot(monsterAttackSound[selected], 0.8f);
    }
}
