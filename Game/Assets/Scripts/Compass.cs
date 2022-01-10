using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Compass : MonoBehaviour
{
<<<<<<< HEAD
    // the image of the compass
    public RawImage compassImage;
    // the player controller
    public Transform player;

    private void Update() 
    {
        // changes the image of the compass based on the direction the playerController is looking.
        compassImage.uvRect = new Rect(player.localEulerAngles.y / 360f, 0f, 1f , 1f);
=======
    /// <summary>
    /// sprite of teh compass directions
    /// </summary>
    public RawImage compassImage;

    /// <summary>
    /// the players transformation info 
    /// </summary>
    public Transform player;

    /// <summary>
    /// changes the image of the compass based on the direction the playerController is looking.
    /// </summary>
    private void Update()
    {
        compassImage.uvRect = new Rect(player.localEulerAngles.y / 360f, 0f, 1f, 1f);
>>>>>>> origin/main
    }
}
