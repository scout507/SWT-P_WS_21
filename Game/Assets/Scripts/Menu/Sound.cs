using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* created by: SWT-P_WS_21/22 */

/// <summary>
/// Implement the class for one sound.
/// </summary>
[System.Serializable]
public class Sound
{
    /// <summary>Name of the sound.</summary>
    public string name;

    /// <summary>Holds the Soundfile.</summary>
    public AudioClip clip;

    /// <summary>Volume of the sound</summary>
    [Range(0f, 1f)]
    public float volume;

    /// <summary>Pitch of the sound</summary>
    [Range(.1f, 3f)]
    public float pitch;

    /// <summary>true: when the sound is to be looped</summary>
    public bool loop;

    [HideInInspector]
    public AudioSource source;
}
