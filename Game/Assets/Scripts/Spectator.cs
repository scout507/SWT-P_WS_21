using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Spectator : NetworkBehaviour
{
    public float movementSpeed = 1;

    public float speedM = 2;
    public float speedV = 2;

    private float yaw = 0;
    private float pitch = 0;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (!isLocalPlayer)
            Destroy(gameObject);
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        if (!FindObjectOfType<IngameMenu>().menuCanvas.enabled)
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            yaw += speedM * Input.GetAxis("Mouse X");
            pitch -= speedV * Input.GetAxis("Mouse Y");

            transform.eulerAngles = new Vector3(pitch, yaw, 0);
            transform.position +=
                transform.TransformDirection(Vector3.forward) * v
                + transform.TransformDirection(Vector3.right) * h;
        }
    }
}
