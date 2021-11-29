using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{

    [SerializeField] CharacterController controller;
    [SerializeField] float speed = 9f;
    [SerializeField] float gravity = 19.62f;
    [SerializeField] float jumpHeight = 2f;

    [SerializeField] Transform groundCheck;
    [SerializeField] float groundDistance = 0.4f;
    [SerializeField] LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;

    [SerializeField] float mouseSensitivity = 100f;
    [SerializeField] GameObject cameraMountPoint;

    float xRotation = 0f;


    public override void OnStartLocalPlayer()
    {
        
        Cursor.lockState = CursorLockMode.Locked;
        Transform cameraTransform = Camera.main.gameObject.transform;  //Find main camera which is part of the scene instead of the prefab
        cameraTransform.parent = cameraMountPoint.transform;  //Make the camera a child of the mount point
        cameraTransform.position = cameraMountPoint.transform.position;  //Set position/rotation same as the mount point
        cameraTransform.rotation = cameraMountPoint.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isLocalPlayer){
            return;
        }

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if(isGrounded && velocity.y < 0) {
            velocity.y = -2f;
        }

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraMountPoint.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        controller.Move(move * speed * Time.deltaTime);

        if(Input.GetButtonDown("Jump") && isGrounded){
            velocity.y = Mathf.Sqrt(jumpHeight * 2f * gravity);
        }

        velocity.y -= gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
