using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Compass : NetworkBehaviour
{
    /// <summary>
    /// filling Image of the compass(the directions)
    /// </summary>
    public RawImage compassImage;
    /// <summary>
    /// the direction the player is looking
    /// </summary>
    [SerializeField] private Transform player;

    public override void OnStartLocalPlayer()
    {
        Transform player = Camera.main.gameObject.transform;  //Find main camera which is part of the scene instead of the prefab
    }

    /// <summary>
    /// The image of teh compass filling(the letters NESW), will adjust their y-axis based on the direction the player is looking
    /// </summary>
    private void Update()
    {
        compassImage.uvRect = new Rect (player.localEulerAngles.y / 360f, 0f, 1f, 1f);
    }
}
