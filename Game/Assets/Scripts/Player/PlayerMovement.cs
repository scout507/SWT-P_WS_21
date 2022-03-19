using Mirror;
using UnityEngine;

/* created by: SWT-P_WS_21/22 */


/* edited by: SWT-P_WS_21/22*/
/// <summary>
/// The class PlayerMovement is responsible for the movement of the player and it manages which weapon is selected.
/// </summary>
public class PlayerMovement : NetworkBehaviour
{
    /// <summary>Player's character controller</summary>
    [SerializeField]
    CharacterController controller;

    /// <summary>Player camera</summary>
    public GameObject cam;

    /// <summary>Prefab of the player's model</summary>
    [SerializeField]
    GameObject playerModel;

    /// <summary>Move vector relative to the player</summary>
    [SyncVar]
    Vector3 moveRelative;

    /// <summary>Walking speed multiplier</summary>
    [SerializeField]
    float speed = 9f;

    /// <summary>Sprint speed multiplier</summary>
    [SerializeField]
    float sprintSpeedMultiplier = 1.5f;

    /// <summary>Crouch speed multiplier</summary>
    [SerializeField]
    float crouchSpeedMultiplier = .33f;

    /// <summary>Gravity force to apply to the player</summary>
    [SerializeField]
    float gravity = 19.62f;

    /// <summary>Player's jump height</summary>
    [SerializeField]
    float jumpHeight = 2f;

    /// <summary>Player's current stamina</summary>
    [SerializeField]
    float stamina = 10f;

    /// <summary>Maximum stamina</summary>
    [SerializeField]
    float staminaMax = 10f;

    /// <summary>Stamina to drain</summary>
    [SerializeField]
    float staminaDrain = .33f;

    /// <summary>Stamina to regain</summary>
    [SerializeField]
    float staminaRegen = .01f;

    /// <summary>Is the player's stamina on cooldown / is the player currently exhausted?</summary>
    bool staminaOnCooldown = false;

    /// <summary>Transform used by the ground check</summary>
    [SerializeField]
    Transform groundCheck;

    /// <summary>Ground-check's distance to the floor</summary>
    [SerializeField]
    float groundDistance = 0.4f;

    /// <summary>Ground layer mask used for on ground detection</summary>
    [SerializeField]
    LayerMask groundMask;

    /// <summary>Velocity vector used for gravity</summary>
    Vector3 velocity;

    /// <summary>Is the player currently on ground?</summary>
    [SyncVar]
    bool isGrounded = false;

    /// <summary>Is the player currently in air?</summary>
    [SyncVar]
    bool isAirborne = false;

    /// <summary>Is the player currently sprinting?</summary>
    [SyncVar]
    bool isSprinting = false;

    /// <summary>Is the player currently crouching?</summary>
    [SyncVar]
    bool isCrouching = false;

    /// <summary>Is the player currently prone?</summary>
    bool isProne = false;

    ///<summary>Player's mouse view sensitivity</summary>
    [SerializeField]
    float mouseSensitivity = 100f;

    /// <summary>Player's point where the camera is mounted</summary>
    [SerializeField]
    public GameObject cameraMountPoint;

    /// <summary>Initial pitch of the player's view</summary>
    float xRotation = 0f;

    /// <summary>Is true if the script is to be active.</summary>
    public bool active = true;

    /// <summary>Player's current taunt (0 = none)</summary>
    [SyncVar]
    int currentTaunt = 0;

    /// <summary>bool to check for a LAdderCollider</summary>
    bool insideLadder = false;

    /// <summary>The UP and DOWN speed for climbing ladders.</summary>
    public float speedUpDown = 10.0f;

    /// <summary>Transform-information of the player</summary>
    public Transform playerTransform;

    /// <summary>
    /// When a player starts a client and enters a game the layer of the gameObject of the local player is set to default and
    /// set the main camera to first person view.
    /// </summary>
    public override void OnStartLocalPlayer()
    {
        gameObject.layer = 0;
        foreach (Transform child in gameObject.transform)
        {
            setLayerDefault(child);
        }
        Cursor.lockState = CursorLockMode.Locked;
        cam.SetActive(true);
    }

    /// <summary>
    /// The Update methode is responsible for the movement and the changing of weapons.
    /// Only runs localy. Registers player input and switches between different forms of movement.
    /// </summary>
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (active)
        {
            CmdSetIsGrounded(CheckGrounded());

            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
                CmdSetIsAirborne(false);
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
                    CmdUncrouch();
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

            if (
                (!isSprinting && move.magnitude > speed)
                || (isSprinting && move.magnitude > speed * sprintSpeedMultiplier)
            )
            {
                move = move.normalized;
                move *= isSprinting ? speed * sprintSpeedMultiplier : speed;
            }

            controller.Move(move * Time.deltaTime);
            CmdSetMoveRelative(transform.InverseTransformDirection(move));

            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                CmdUncrouch();
                CmdSetIsAirborne(true);
                velocity.y = Mathf.Sqrt(jumpHeight * 2f * gravity);
            }

            if (Input.GetButtonDown("Crouch"))
            {
                if (!isCrouching)
                {
                    CmdCrouch();
                }
                else
                {
                    CmdUncrouch();
                }
            }

            if (!insideLadder)
            {
                velocity.y -= gravity * Time.deltaTime;
                controller.Move(velocity * Time.deltaTime);
            }

            if (move.magnitude > 0.1f)
                CmdSetCurrentTaunt(0);
            if (Input.GetKey("t"))
            {
                if (Input.GetKeyDown("1"))
                    CmdSetCurrentTaunt(1);
                if (Input.GetKeyDown("2"))
                    CmdSetCurrentTaunt(2);
                if (Input.GetKeyDown("3"))
                    CmdSetCurrentTaunt(3);
            }

            if (Input.GetButtonDown("Interact"))
            {
                CmdInteract();
            }

            if (insideLadder == true && Input.GetKey("w"))
            {
                CmdSetIsGrounded(true);
                playerTransform.transform.position += Vector3.up * speedUpDown;
            }

            if (insideLadder == true && Input.GetKey("s"))
            {
                CmdSetIsGrounded(true);
                playerTransform.transform.position -= Vector3.up * speedUpDown;
            }
        }
    }


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
    /// Getter for currently selected weapon (id)
    /// </summary>
    public int GetSelectedWeapon()
    {
        return GetComponent<Classes>().GetSelectedWeapon();
    }

    /// <summary>Getter for the player's view pitch</summary>
    /// <returns>The player's view pitch in degrees</returns>
    public float GetXRotation()
    {
        return xRotation;
    }

    /// <summary>Setter for the player's view pitch</summary>
    public void SetXRotation(float newXRotation)
    {
        xRotation = newXRotation;
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
            return (
                isGrounded
                && Input.GetAxis("Vertical") > 0.1f
                && forward.magnitude > 0.1f
                && Input.GetButton("Sprint")
                && stamina > 0
            );
        }
    }


    /// <summary>Enables ladder movement</summary>
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Ladder")
        {
            insideLadder = true;
        }
    }

    /// <summary>Disables ladder movement</summary>
    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Ladder")
        {
            insideLadder = false;
        }
    }

    private void setLayerDefault(Transform gameObjectChild)
    {
        gameObjectChild.gameObject.layer = 0;
        foreach (Transform child in gameObjectChild)
        {
            setLayerDefault(child);
        }
    }

    /// <summary>Sends command to the server that the player tried to interact</summary>
    [Command]
    public void CmdInteract()
    {
        RaycastHit hit;
        bool hitInteractable = Physics.Raycast(
            cameraMountPoint.transform.position,
            cameraMountPoint.transform.forward,
            out hit,
            2,
            (1 << 3)
        );

        if (!hitInteractable)
            return;

        hit.collider.transform.parent.gameObject.GetComponent<Interactable>().OnInteract();
    }

    /// <summary>
    /// Toggles player's position to crouched
    /// </summary>
    [Command]
    void CmdCrouch()
    {
        controller.center = new Vector3(0, -0.1f, 0);
        controller.height = 1.35f;
        cameraMountPoint.transform.localPosition = new Vector3(0.085f, 0.26f, 0.06f);
        isCrouching = true;
    }

    /// <summary>
    /// Toggles player's position to uncrouched
    /// </summary>
    [Command]
    void CmdUncrouch()
    {
        controller.center = new Vector3(0, 0, 0);
        controller.height = 1.65f;
        cameraMountPoint.transform.localPosition = new Vector3(0f, 0.756f, 0.05f);
        isCrouching = false;
    }

    /// <summary>
    /// Sets the player's relative move vector
    /// </summary>
    [Command]
    void CmdSetMoveRelative(Vector3 newMoveRelative)
    {
        moveRelative = newMoveRelative;
    }

    /// <summary>
    /// Sets the player's current taunt
    /// </summary>
    [Command]
    void CmdSetCurrentTaunt(int taunt)
    {
        currentTaunt = taunt;
    }

    /// <summary>
    /// Sets the player's isGrounded
    /// </summary>
    [Command]
    void CmdSetIsGrounded(bool newIsGrounded)
    {
        isGrounded = newIsGrounded;
    }

    /// <summary>
    /// Sets the player's isAirborne
    /// </summary>
    [Command]
    void CmdSetIsAirborne(bool newIsAirborne)
    {
        isAirborne = newIsAirborne;
    }

}
