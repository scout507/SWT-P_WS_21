using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField]
    CharacterController controller;

    /// <summary>
    /// Prefab of the player's model
    /// </summary>
    [SerializeField]
    GameObject playerModel;

    /// <summary>
    /// Player's movement speed
    /// </summary>
    [SerializeField]
    float speed = 9f;

    /// <summary>
    /// Sprint speed multiplier
    /// </summary>
    [SerializeField]
    float sprintSpeedMultiplier = 1.5f;

    /// <summary>
    /// Crouch speed multiplier
    /// </summary>
    [SerializeField]
    float crouchSpeedMultiplier = .33f;

    /// <summary>
    /// Gravity force to apply to the player
    /// </summary>
    [SerializeField]
    float gravity = 19.62f;

    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    float jumpHeight = 2f;

    /// <summary>
    /// Change of height when crouching
    /// </summary>
    [SerializeField]
    float crouchHeightChange = .25f;

    /// <summary>
    /// Change of height when going prone
    /// </summary>
    [SerializeField]
    float proneHeightChange = .66f;

    /// <summary>
    /// Player's current stamina
    /// </summary>
    [SerializeField]
    float stamina = 10f;

    /// <summary>
    /// Maximum stamina
    /// </summary>
    [SerializeField]
    float staminaMax = 10f;

    /// <summary>
    /// Stamina to drain
    /// </summary>
    [SerializeField]
    float staminaDrain = .33f;

    /// <summary>
    /// Stamina to regain
    /// </summary>
    [SerializeField]
    float staminaRegen = .01f;

    /// <summary>
    /// Is the player's stamina on cooldown / is the player currently exhausted?
    /// </summary>
    bool staminaOnCooldown = false;

    [SerializeField]
    Transform groundCheck;

    [SerializeField]
    float groundDistance = 0.4f;

    [SerializeField]
    LayerMask groundMask;

    Vector3 velocity;

    /// <summary>
    /// Is the player currently on ground? 
    /// </summary>
    bool isGrounded = false;

    /// <summary>
    /// Is the player currently sprinting?
    /// </summary>
    bool isSprinting = false;

    /// <summary>
    /// Is the player currently crouching?
    /// </summary>
    bool isCrouching = false;

    /// <summary>
    /// Is the player currently prone?
    /// </summary>
    bool isProning = false;

    [SerializeField]
    float mouseSensitivity = 100f;

    [SerializeField]
    GameObject cameraMountPoint;

    /// <summary>
    /// Initial pitch of the player's view
    /// </summary>
    float xRotation = 0f;

    public override void OnStartLocalPlayer()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Transform cameraTransform = Camera.main.gameObject.transform; // Find main camera which is part of the scene instead of the prefab
        cameraTransform.parent = cameraMountPoint.transform; // Make the camera a child of the mount point
        cameraTransform.position = cameraMountPoint.transform.position; // Set position/rotation same as the mount point
        cameraTransform.rotation = cameraMountPoint.transform.rotation;
    }

    /// <summary>
    /// Checks if the player is currently on ground
    /// </summary>
    /// <returns>Returns true or false</returns>
    bool CheckGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    /// <summary>
    /// Checks if the player is currently on sprinting
    /// </summary>
    /// <returns>Returns true or false</returns>
    bool CheckSprinting(Vector3 forward)
    {
        if (staminaOnCooldown)
        {
            return false;
        }
        else
        {
            return (
                isGrounded
                && Input.GetAxis("Vertical") > 0
                && forward.magnitude > 0.1f
                && Input.GetButton("Sprint")
                && stamina > 0
            );
        }
    }

    void Crouch()
    {
        transform.localScale += new Vector3(0f, -crouchHeightChange, 0f);
        isCrouching = true;
    }

    void Uncrouch()
    {
        transform.localScale += new Vector3(0f, crouchHeightChange, 0f);
        isCrouching = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        isGrounded = CheckGrounded();

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

        isSprinting = CheckSprinting(forward);
        if (isSprinting)
        {
            if (isCrouching)
            {
                Uncrouch();
            }

            forward *= sprintSpeedMultiplier;
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
            move *= crouchSpeedMultiplier;
        }

        controller.Move(move * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * 2f * gravity);
        }

        if (Input.GetButtonDown("Crouch"))
        {
            if (!isCrouching)
            {
                Crouch();
            }
            else
            {
                Uncrouch();
            }
        }

        /*if (Input.GetButtonDown("Prone"))
        {
            if (!isProning)
            {
                transform.localScale += new Vector3(0f, -proneHeightChange, 0f);
                //groundDistance = 0.05f;
                isProning = true;
            }
            else 
            {
                transform.localScale += new Vector3(0f, proneHeightChange, 0f);
                //groundDistance = 0.4f;
                isProning = false;
            }
        }*/

        velocity.y -= gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
