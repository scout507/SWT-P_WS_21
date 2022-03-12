using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/* created by: SWT-P_WS_21/22 */

/// <summary>Required for the movement of the spectator.</summary>
public class Spectator : NetworkBehaviour
{
    /// <summary>How quickly the viewer can move forward.</summary>
    public float movementSpeed = 1;

    /// <summary>Horizontal movement speed.</summary>
    public float speedH = 2;

    /// <summary>Vertical movement speed.</summary>
    public float speedV = 2;

    /// <summary>Rotation on the y axis.</summary>
    private float yaw = 0;

    /// <summary>Rotation on the z axis.</summary>
    private float pitch = 0;

    /// <summary>Is true if the script is to be active.</summary>
    public bool active = true;

    /// <summary>
    /// If the player is the locale player, 
    /// the camera is dragged to the game object at the beginning.
    /// </summary>
    public override void OnStartLocalPlayer()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Transform cameraTransform = Camera.main.gameObject.transform;
        cameraTransform.parent = transform;
        cameraTransform.position = transform.position;
        cameraTransform.rotation = transform.rotation;
    }

    /// <summary>
    /// Is executed only on the local player.
    /// Processes the input of the player and moves the game object in this direction.
    /// </summary>
    void Update()
    {
        if (!isLocalPlayer)
            return;

        if (active)
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            yaw += speedH * Input.GetAxis("Mouse X");
            pitch -= speedV * Input.GetAxis("Mouse Y");

            transform.eulerAngles = new Vector3(pitch, yaw, 0);
            transform.position +=
                (
                    transform.TransformDirection(Vector3.forward) * v
                    + transform.TransformDirection(Vector3.right) * h
                ) * movementSpeed;
        }
    }
}
