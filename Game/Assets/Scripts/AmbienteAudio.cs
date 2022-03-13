using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AmbienteAudio : NetworkBehaviour
{
    private AudioSource audioSource;
    [SerializeField] AudioClip[] ambienteSounds;
    float thundercd = 30;
    float windcd;
    float musiccd;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        thundercd -= Time.deltaTime;
        if (thundercd < 0f)
        {
            RPCPlayThunderSounds();
            thundercd = ambienteSounds[1].length + Random.Range(30, 200);
        }

        windcd -= Time.deltaTime;
        if (windcd < 0f)
        {
            RPCPlayWindSounds();
            windcd = ambienteSounds[2].length;
        }

        musiccd -= Time.deltaTime;
        if (musiccd < 0f)
        {
            RPCPlayMusicSounds();
            musiccd = ambienteSounds[0].length;
        }
    }

    [ClientRpc]
    void RPCPlayMusicSounds()
    {
        audioSource.PlayOneShot(ambienteSounds[0]);
    }

    [ClientRpc]
    void RPCPlayThunderSounds()
    {
        audioSource.PlayOneShot(ambienteSounds[1]);
    }

    [ClientRpc]
    void RPCPlayWindSounds()
    {
        audioSource.PlayOneShot(ambienteSounds[2]);
    }
}
