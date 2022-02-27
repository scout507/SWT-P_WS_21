/* created by: SWT-P_SW_21/22 */
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

/* created by: SWT-P_WS_21/22 */


/// <summary>
/// The class PlayerMovement is responsible for the movement of the player and it manages which weapon is selected.
/// </summary>
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
    [SyncVar] Vector3 moveRelative;

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
    [SyncVar] bool isGrounded = false;

    /// <summary>
    /// Is the player currently in air? 
    /// </summary>
    [SyncVar] bool isAirborne = false;

    /// <summary>
    /// Is the player currently sprinting?
    /// </summary>
    [SyncVar] bool isSprinting = false;

    /// <summary>
    /// Is the player currently crouching?
    /// </summary>
    [SyncVar] bool isCrouching = false;

    /// <summary>
    /// Is the player currently prone?
    /// </summary>
    bool isProne = false;

    [SerializeField]
    float mouseSensitivity = 100f;

    [SerializeField]
    GameObject cameraMountPoint;

    /// <summary>
    /// Initial pitch of the player's view
    /// </summary>
    float xRotation = 0f;

    /// <summary>
    /// Player's current taunt (0 = none)
    /// </summary>
    [SyncVar] int currentTaunt = 0;

    /// <summary>
    /// Returns the player's view pitch
    /// </summary>
    public float GetPitch()
    {
        return xRotation;
    }

    /// <summary>
    /// Getter for the player's taunt
    /// </summary>
    public int GetCurrentTaunt()
    {
        return currentTaunt;
    }

    /// <summary>
    /// Returns move vector relative to player, used for animations
    /// </summary>
    public Vector3 GetMoveRelative()
    {
        return moveRelative;
    }

    /// <summary>
    /// Getter for isGrounded
    /// </summary>
    public bool GetIsGrounded()
    {
        return isGrounded;
    }

    /// <summary>
    /// Returns if the player is crouching
    /// </summary>
    public bool GetIsCrouching()
    {
        return isCrouching;
    }

    /// <summary>
    /// Getter for isProne
    /// </summary>
    public bool GetIsProne()
    {
        return isProne;
    }

    /// <summary>
    /// Getter for isAirborne
    /// </summary>
    public bool GetIsAirborne()
    {
        return isAirborne;
    }

    /// <summary>
    /// Sets the player's relative move vector
    /// </summary>
    [Command]
    void SetMoveRelative(Vector3 newMoveRelative)
    {
        moveRelative = newMoveRelative;
    }

    /// <summary>
    /// Sets the player's current taunt
    /// </summary>
    [Command]
    void SetCurrentTaunt(int taunt)
    {
        currentTaunt = taunt;
    }

    /// <summary>
    /// Sets the player's isGrounded
    /// </summary>
    [Command]
    void SetIsGrounded(bool newIsGrounded)
    {
        isGrounded = newIsGrounded;
    }

    /// <summary>
    /// Sets the player's isAirborne
    /// </summary>
    [Command]
    void SetIsAirborne(bool newIsAirborne)
    {
        isAirborne = newIsAirborne;
    }

    [SyncVar(hook = nameof(SwitchWeapon))]
    public int selectedWeapon = 0;

    /// <summary>
    /// When a player starts a client and enters a game the layer of the gameObject of the local player is set to default and
    /// set the main camera to first person view.
    /// </summary>
    public override void OnStartLocalPlayer()
    {
        gameObject.layer = 0;
        foreach (Transform child in gameObject.transform)
        {
            child.gameObject.layer = 0;
        }
        Cursor.lockState = CursorLockMode.Locked;
        Transform cameraTransform = Camera.main.gameObject.transform; // Find main camera which is part of the scene instead of the prefab
        cameraTransform.parent = cameraMountPoint.transform; // Make the camera a child of the mount point
        cameraTransform.position = cameraMountPoint.transform.position; // Set position/rotation same as the mount point
        cameraTransform.rotation = cameraMountPoint.transform.rotation;
    }

    /// <summary>
    /// When a player prefab is spawns, this selects the first weapon.
    /// </summary>
    private void Start()
    {
        selectedWeapon = 1;
        SwitchWeapon(selectedWeapon, selectedWeapon);
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

    /// <summary>
    /// Toggles player's position to crouched
    /// </summary>
    [Command]
    void Crouch()
    {
        controller.center = new Vector3(0, -0.1f, 0);
        controller.radius = 0.4f;
        controller.height = 1.35f;
        // move ybot y to -0.824f
        cameraMountPoint.transform.localPosition = new Vector3(0.085f, 0.26f, 0.06f);
        isCrouching = true;
    }


    /// <summary>
    /// Toggles player's position to uncrouched
    /// </summary>
    [Command]
    void Uncrouch()
    {
        controller.center = new Vector3(0, 0, 0);
        controller.radius = 0.35f;
        controller.height = 1.65f;
        // move ybot y to -0.92f
        cameraMountPoint.transform.localPosition = new Vector3(0f, 0.756f, 0.05f);
        isCrouching = false;
    }

    public float GetXRotation()
    {
        return xRotation;
    }

    public void SetXRotation(float newXRotation)
    {
        xRotation = newXRotation;
    }

    /// <summary>
    /// The Update methode is responsible for the movement and the changing of weapons.
    /// </summary>
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        SetIsGrounded(CheckGrounded());

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            SetIsAirborne(false);
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
        SetMoveRelative(transform.InverseTransformDirection(move));

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Uncrouch();
            SetIsAirborne(true);
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

        if (move.magnitude > 0.1f) SetCurrentTaunt(0);
        if (Input.GetKey("t"))
        {
            if (Input.GetKeyDown("1")) SetCurrentTaunt(1);
            if (Input.GetKeyDown("2")) SetCurrentTaunt(2);
            if (Input.GetKeyDown("3")) SetCurrentTaunt(3);
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0f && selectedWeapon < 5)
        {
            int newWeapon = selectedWeapon + 1;
            CmdSwitchWeapon(newWeapon);
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f && selectedWeapon > 1)
        {
            int newWeapon = selectedWeapon - 1;
            CmdSwitchWeapon(newWeapon);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CmdSwitchWeapon(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CmdSwitchWeapon(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            CmdSwitchWeapon(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            CmdSwitchWeapon(4);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            CmdSwitchWeapon(5);
        }
    }

    /// <summary>
    /// Switching weapons is handled by the server. This methode changes the index of the selected weapon to the new weapon.
    /// </summary>
    /// <param name="newWeapon">Index of new weapon which is now selected.</param>
    [Command]
    public void CmdSwitchWeapon(int newWeapon)
    {
        selectedWeapon = newWeapon;
    }

    /// <summary>
    /// Deactivates the script of the old weapon and activates the script of the new weapon.
    /// </summary>
    /// <param name="oldWeapon">Index of old weapon.</param>
    /// <param name="newWeapon">Index of new weapon.</param>
    private void SwitchWeapon(int oldWeapon, int newWeapon)
    {
        switch (oldWeapon)
        {
            case 1:
                GetComponent<MP>().enabled = false;
                break;
            case 2:
                GetComponent<Shotgun>().enabled = false;
                break;
            case 3:
                GetComponent<Rifle>().enabled = false;
                break;
            case 4:
                GetComponent<Pistol>().enabled = false;
                break;
            case 5:
                GetComponent<Melee>().enabled = false;
                break;
            default:
                break;
        }
        switch (newWeapon)
        {
            case 1:
                GetComponent<MP>().enabled = true;
                break;
            case 2:
                GetComponent<Shotgun>().enabled = true;
                break;
            case 3:
                GetComponent<Rifle>().enabled = true;
                break;
            case 4:
                GetComponent<Pistol>().enabled = true;
                break;
            case 5:
                GetComponent<Melee>().enabled = true;
                break;
            default:
                break;
        }
    }
}