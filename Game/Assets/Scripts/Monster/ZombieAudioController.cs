using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ZombieAudioController : NetworkBehaviour
{

    public AudioSource audioSource; // The Audio Source that is attached to the Monster
    float stepCoolDown; // The Time until the next Footstep Sound is Played
    float idleCooldown;

    [SerializeField] AudioClip[] monsterFootsteps; // The Audio Clips, Gun Sounds and Footstep Sounds
    [SerializeField] AudioClip[] monsterIdleSound;
    [SerializeField] AudioClip[] monsterDmgTakenSound;
    [SerializeField] AudioClip[] monsterAttackSound;
    int monsterFootstep;
    int monsterIdle;

    private void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
        monsterFootstep = Random.Range(0, 4);
        monsterIdle = Random.Range(0, 7);
    }

    // Update is called once per frame
    void Update()
    {
        stepCoolDown -= Time.deltaTime;
        if (stepCoolDown < 0f)
        {
            float randomf = 1f + Random.Range(-0.2f, 0.2f);
            RPCPlayFootStepSound(randomf);
            stepCoolDown = monsterFootsteps[monsterFootstep].length;
        }

        idleCooldown -= Time.deltaTime;
        if (idleCooldown < 0f)
        {
            RPCPlayZombieIdleSound();
            idleCooldown = monsterIdleSound[monsterIdle].length * 10;
        }
    }

    /// <summary>
    /// The ClientRpC for the Footstep Sound, to play the sound on all Clients
    /// </summary>
    [ClientRpc]
    void RPCPlayFootStepSound(float randomf)
    {
        audioSource.pitch = randomf;
        audioSource.PlayOneShot(monsterFootsteps[monsterFootstep], 0.4f);
    }

    [ClientRpc]
    void RPCPlayZombieIdleSound()
    {
        audioSource.PlayOneShot(monsterIdleSound[monsterIdle], 0.8f);
    }

    [Command]
    public void CmdPlayZombieDmgSound()
    {
        int selected = Random.Range(0, 3);
        RPCPlayZombieDmgSound(selected);
    }

    [ClientRpc]
    public void RPCPlayZombieDmgSound(int selected)
    {
        Debug.Log("Zombie dmg taken");
        audioSource.PlayOneShot(monsterDmgTakenSound[selected], 0.8f);
    }

    [Command]
    public void CmdPlayZombieDeathSound()
    {
        int selected = Random.Range(3, 5);
        RPCPlayZombieDeathSound(selected);
    }

    [ClientRpc]
    void RPCPlayZombieDeathSound(int selected)
    {
        Debug.Log("Zombie death");
        audioSource.PlayOneShot(monsterDmgTakenSound[selected], 0.8f);
    }

    [Command]
    public void CmdPlayZombieAttackSound()
    {
        int selected = Random.Range(0, 5);
        RPCPlayZombieAttackSound(selected);
    }

    [ClientRpc]
    public void RPCPlayZombieAttackSound(int selected)
    {
        audioSource.PlayOneShot(monsterAttackSound[selected], 0.8f);
    }
}
