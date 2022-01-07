using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] CharacterController controller;

    /// <summary>
    /// Prefab of the player's model
    /// </summary>
    [SerializeField] GameObject playerModel;
    
    /// <summary>
    /// Move vector relative to the player
    /// </summary>
    Vector3 moveRelative;

    /// <summary>
    /// Walking speed multiplier
    /// </summary>
    [SerializeField] float speed = 9f;

    /// <summary>
    /// Sprint speed multiplier
    /// </summary>
    [SerializeField] float sprintSpeedMultiplier = 1.5f;

    /// <summary>
    /// Crouch speed multiplier
    /// </summary>
    [SerializeField] float crouchSpeedMultiplier = .33f;

    /// <summary>
    /// Gravity force to apply to the player
    /// </summary>
    [SerializeField] float gravity = 19.62f;

    /// <summary>
    /// 
    /// </summary>
    [SerializeField] float jumpHeight = 2f;

    /// <summary>
    /// Change of height when crouching
    /// </summary>
    [SerializeField] float crouchHeightChange = .25f;

    /// <summary>
    /// Change of height when going prone
    /// </summary>
    [SerializeField] float proneHeightChange = .66f;

    /// <summary>
    /// Player's current stamina
    /// </summary>
    [SerializeField] float stamina = 10f;

    /// <summary>
    /// Maximum stamina
    /// </summary>
    [SerializeField] float staminaMax = 10f;

    /// <summary>
    /// Stamina to drain
    /// </summary>
    [SerializeField] float staminaDrain = .33f;

    /// <summary>
    /// Stamina to regain
    /// </summary>
    [SerializeField] float staminaRegen = .01f;

    /// <summary>
    /// Is the player's stamina on cooldown / is the player currently exhausted?
    /// </summary>
    bool staminaOnCooldown = false;

    [SerializeField] Transform groundCheck;
    [SerializeField] float groundDistance = 0.4f;
    [SerializeField] LayerMask groundMask;

    Vector3 velocity;

    /// <summary>
    /// Is the player currently on ground? 
    /// </summary>
    bool isGrounded = false;

    bool isInAir = false;

    /// <summary>
    /// Is the player currently sprinting?
    /// </summary>
    bool isSprinting = false;

    /// <summary>
    /// Is the player currently crouching?
    /// </summary>
    bool isCrouching = false;
    bool duringCrouchAnimation = false;

    /// <summary>
    /// Is the player currently prone?
    /// </summary>
    bool isProne = false;

    [SerializeField] float mouseSensitivity = 100f;
    [SerializeField] GameObject cameraMountPoint;

    /// <summary>
    /// Initial pitch of the player's view
    /// </summary>
    float xRotation = 0f;

    public int currentTaunt = 0;

    /// <summary>
    /// Returns the player's view pitch
    /// </summary>
    public float GetPitch() 
    {
        return xRotation;
    }

    /// <summary>
    /// Returns move vector relative to player, used for animations
    /// </summary>
    public Vector3 GetMoveRelative() 
    {
        return moveRelative;
    }

    /// <summary>
    /// Returns if the player is crouching
    /// </summary>
    public bool GetIsCrouching() 
    {
        return isCrouching;
    }

    public bool GetIsProne() 
    {
        return isProne;
    }

    public bool GetIsAirborne() 
    {
        return isInAir;
    }

    public override void OnStartLocalPlayer()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Transform cameraTransform = Camera.main.gameObject.transform;  // Find main camera which is part of the scene instead of the prefab
        cameraTransform.parent = cameraMountPoint.transform;  // Make the camera a child of the mount point
        cameraTransform.position = cameraMountPoint.transform.position;  // Set position/rotation same as the mount point
        cameraTransform.rotation = cameraMountPoint.transform.rotation;
    }

    /// <summary>
    /// Checks if the player is currently on ground
    /// </summary>
    /// <returns>Returns true or false</returns>
    public bool CheckGrounded()
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
            return (isGrounded && Input.GetAxis("Vertical") > 0.1f && forward.magnitude > 0.1f && Input.GetButton("Sprint") && stamina > 0);
        }
    }

    void Crouch()
    {
        controller.center = new Vector3(0, -0.1f, 0);
        controller.radius = 0.4f;
        controller.height = 1.35f;
        // move ybot y to -0.824f
        cameraMountPoint.transform.localPosition = new Vector3(0.085f, 0.26f, 0.06f);
        isCrouching = true;
    }

    void Uncrouch()
    {
        controller.center = new Vector3(0, 0, 0);
        controller.radius = 0.35f;
        controller.height = 1.65f;
        // move ybot y to -0.92f
        cameraMountPoint.transform.localPosition = new Vector3(0f, 0.756f, 0.05f);
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
            isInAir = false;
        }

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraMountPoint.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        Vector3 right = transform.right * Input.GetAxis("Horizontal") * speed;
        Vector3 forward = transform.forward * Input.GetAxis("Vertical") * speed;

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

        if ((!isSprinting && move.magnitude > speed) || (isSprinting && move.magnitude > speed * sprintSpeedMultiplier))
        {
            move = move.normalized;
            move *= isSprinting ? speed * sprintSpeedMultiplier : speed;

        }

        controller.Move(move * Time.deltaTime);
        moveRelative = transform.InverseTransformDirection(move);

        if (Input.GetButtonDown("Jump") && isGrounded) 
        {
            Uncrouch();
            isInAir = true;
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
            if (!isProne)
            {
                isProne = true;
            }
            else 
            {
                isProne = false;
            }
        }*/

        velocity.y -= gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (move.magnitude > 0.1f) currentTaunt = 0;
        if (Input.GetKey("t")) 
        {
            if (Input.GetKeyDown("1")) currentTaunt = 1;
            if (Input.GetKeyDown("2")) currentTaunt = 2;
            if (Input.GetKeyDown("3")) currentTaunt = 3;
        }

        Debug.Log("Pitch: " + GetPitch());
    }
}