using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeetingButton : MonoBehaviour
{
    /// <summary>
    /// AudioSource of the meeting button to play sounds
    /// </summary>
    public AudioSource audioSource;

    private float countdown;

    void Start()
    {
        countdown = 5;
    }

    void Update()
    {
        countdown -= Time.deltaTime;
        if (countdown < 0) 
        {
            PlayAlarm();
            countdown = 25;
        }
    }

    void PlayAlarm() 
    {
        audioSource.PlayOneShot(audioSource.clip);
    }
}
