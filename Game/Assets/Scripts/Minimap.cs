using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    /// <summary>
    /// movement information of the player object
    /// </summary>
    public Transform player;

    /// <summary>
    /// update at the end of a frame
    /// </summary>
    void LateUpdate ()
    {
        Vector3 newPosition = player.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;
    }
}
