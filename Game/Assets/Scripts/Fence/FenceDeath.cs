using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* created by: SWT-P_WS_21/22 */

public class FenceDeath : MonoBehaviour
{
    /// <summary>
    /// Holds the "DestructableObject" component of the Game object.
    /// </summary>
    DestructableObject board;

    /// <summary>
    /// Fetches the "DestructableObject" component from the Game object.
    /// </summary>
    void Start()
    {
        board = GetComponent<DestructableObject>();
    }

    /// <summary>
    /// Disables or enables the meshrender of the Game object,
    /// after the variable "active" from the "DestructableObject" component.
    /// </summary>
    void Update()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = board.active;
    }
}
