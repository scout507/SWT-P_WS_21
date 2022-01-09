using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Compass : MonoBehaviour
{

    public RawImage compassImage;
    public Transform player;

    // The image of teh compass filling(the letters NESW), will adjust their y-axis based on the direction the player is looking
    private void Update()
    {
        compassImage.uvRect = new Rect (player.localEulerAngles.y / 360f, 0f, 1f, 1f);
    }
}
