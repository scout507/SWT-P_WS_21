using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/* created by: SWT-P_WS_21/22 */

/// <summary>Manages the sound in the scene menu.</summary>
public class Soundmanager : MonoBehaviour
{
    /// <summary>Keeps all sounds in one list.</summary>
    public Sound[] sounds;

    /// <summary>Value for the volume in the menu.</summary>
    public float volume;

    /// <summary>Holds the instance of this class.</summary>
    public static Soundmanager instance;

    /// <summary>
    /// Sets the variables at the start of the script.
    /// </summary>
    void Awake()
    {
        if (instance == null) instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (Sound item in sounds)
        {
            item.source = gameObject.AddComponent<AudioSource>();
            item.source.clip = item.clip;
            item.source.volume = item.volume;
            item.source.pitch = item.pitch;
            item.source.loop = item.loop;
        }
    }

    /// <summary>
    /// Start the Theme Song at the beginning.
    /// </summary>
    void Start()
    {
        Play("Theme");
    }

    /// <summary>
    /// Changes the volume when it is changed in the UI.
    /// </summary>
    void Update()
    {
        AudioListener.volume = volume;
    }

    /// <summary>
    /// Plays a sound with the given name.
    /// </summary>
    /// <param name="name">Name of the sound</param>
    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Play();
    }
}

// Needed to play a sound in another class.
// FindObjectOfType<Soundmanager>().Play("name of the sound");