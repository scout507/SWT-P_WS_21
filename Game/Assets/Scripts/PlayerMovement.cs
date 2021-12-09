using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{

    [SerializeField] CharacterController controller;
    [SerializeField] float speed = 9f;
    [SerializeField] float sprintMultiplier = 1.5f;
    [SerializeField] float gravity = 19.62f;
    [SerializeField] float jumpHeight = 2f;
    [SerializeField] float crouchHeight = .66f;

    [SerializeField] float stamina = 10f;
    [SerializeField] float staminaMax = 10f;
    [SerializeField] float staminaDrain = .33f;
    [SerializeField] float staminaRegen = .01f;
    bool staminaOnCooldown = false;

    [SerializeField] Transform groundCheck;
    [SerializeField] float groundDistance = 0.4f;
    [SerializeField] LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded = false;
    bool isSprinting = false;
    bool isCrouching = false;

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

    bool checkGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    bool checkSprinting(Vector3 forward)
    {
        if (staminaOnCooldown)
        {
            return false;
        }
        else
        {
            return (isGrounded && !isCrouching && Input.GetAxis("Vertical") > 0 && forward.magnitude > 0.1f && Input.GetButton("Sprint") && stamina > 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        isGrounded = checkGrounded();

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraMountPoint.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        Vector3 right = (transform.right * Input.GetAxis("Horizontal")) * speed;
        Vector3 forward = (transform.forward * Input.GetAxis("Vertical")) * speed;

        isSprinting = checkSprinting(forward);
        if (isSprinting)
        {
            forward *= sprintMultiplier;
            stamina -= staminaDrain * Time.deltaTime;

            if (stamina < 0)
            {
                stamina = 0;
                staminaOnCooldown = true;
            }
        }
        else
        {
            //forward *= speed;
            stamina += staminaRegen * Time.deltaTime;

            if (stamina > staminaMax)
            {
                stamina = staminaMax;
            }

            if (stamina > staminaMax / 3)
            {
                staminaOnCooldown = false;
            }
        }

        Vector3 move = right + forward;

        if (isCrouching)
        {
            move *= 0.33f;
        }

        controller.Move(move * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded) 
        {
            velocity.y = Mathf.Sqrt(jumpHeight * 2f * gravity);
        }

        if (Input.GetButtonDown("Crouch"))
        {
            transform.localScale += new Vector3(0f, -0.25f, 0f);
            isCrouching = true;
        }

        if (Input.GetButtonUp("Crouch"))
        {
            transform.localScale += new Vector3(0f, 0.25f, 0f);
            isCrouching = false;
        }

        velocity.y -= gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

}
