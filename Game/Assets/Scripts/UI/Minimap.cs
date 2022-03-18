/* created by: SWT-P_WS_21/22 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script controlls the Transform-Information of the Minimap Camera.
/// </summary>
public class Minimap : MonoBehaviour
{
    /// <summary>
    /// movement information of the player object
    /// </summary>
    public Transform player;

    /// <summary>
    /// update at the end of a frame
    /// </summary>
    void LateUpdate()
    {
        Vector3 newPosition = player.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;
    }
}
