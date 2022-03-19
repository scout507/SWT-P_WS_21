using UnityEngine;
using Mirror;

/* created by: SWT-P_WS_21/22 */


/// <summary>
/// The Script to handle the Ambiente Sounds on the Map.
/// </summary>
public class AmbienteAudio : NetworkBehaviour
{

    /// <summary>The Ambiente Sounds that will be played</summary>
    [SerializeField] AudioClip[] ambienteSounds;

    /// <summary>The Audio Source that is attached to the Ambiente Box Prefab</summary>
    private AudioSource audioSource;

    /// <summary>The Timer until the Thunder Storm Sound is played again</summary>
    private float thundercd = 30;

    /// <summary>The Timer until the Wind Sound is played again</summary>
    private float windcd;

    /// <summary>The Timer until the Music is played again</summary>
    private float musiccd;

    /// <summary>
    /// Gets dependencies.
    /// </summary>
    void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
    }


    /// <summary>
    /// The Thunder Sounds plays 30s after the Start and then starts randomly every 30s-200s again
    /// The Wind Sound plays at the Start and then every 2 times its lenght again
    /// The Music plays constantly
    /// </summary>
    void Update()
    {
        if (!isServer) return;

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


    /// <summary>
    /// Calls the Music on all Clients
    /// </summary>
    [ClientRpc]
    void RPCPlayMusicSounds()
    {
        if (!audioSource) audioSource = this.GetComponent<AudioSource>();
        audioSource.PlayOneShot(ambienteSounds[0]);
    }

    /// <summary>
    /// Calls the Thunder on all Clients
    /// </summary>
    [ClientRpc]
    void RPCPlayThunderSounds()
    {
        if (!audioSource) audioSource = this.GetComponent<AudioSource>();
        audioSource.PlayOneShot(ambienteSounds[1]);
    }

    /// <summary>
    /// Calls the Wind on all Clients
    /// </summary>
    [ClientRpc]
    void RPCPlayWindSounds()
    {
        if (!audioSource) audioSource = this.GetComponent<AudioSource>();
        audioSource.PlayOneShot(ambienteSounds[2]);
    }
}
