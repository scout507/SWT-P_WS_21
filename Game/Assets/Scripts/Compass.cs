using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Compass : MonoBehaviour
{
    // the image of the compass
    public RawImage compassImage;
    // the player controller
    public Transform player;

    private void Update() 
    {
        // changes the image of the compass based on the direction the playerController is looking.
        compassImage.uvRect = new Rect(player.localEulerAngles.y / 360f, 0f, 1f , 1f);
        //Debug.Log(player.localEulerAngles.y);
    }
}
