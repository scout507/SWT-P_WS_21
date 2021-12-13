using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

/// <summary>
/// The class PlayerMovement is responsible for the movement of the player and it manages which weapon is selected.
/// </summary>
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

    [SyncVar(hook = nameof(SwitchWeapon))]
    public int selectedWeapon = 0;
    public float xRotation = 0f;

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
        Transform cameraTransform = Camera.main.gameObject.transform;  //Find main camera which is part of the scene instead of the prefab
        cameraTransform.parent = cameraMountPoint.transform;  //Make the camera a child of the mount point
        cameraTransform.position = cameraMountPoint.transform.position;  //Set position/rotation same as the mount point
        cameraTransform.rotation = cameraMountPoint.transform.rotation;
    }

    /// <summary>
    /// When a player prefab is spawns, this selects the first weapon.
    /// </summary>
    private void Start() {
        selectedWeapon = 1;
        SwitchWeapon(selectedWeapon, selectedWeapon);
    }

    /// <summary>
    /// The Update methode is responsible for the movement and the changing of weapons.
    /// </summary>
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

        if(Input.GetAxis("Mouse ScrollWheel") > 0f && selectedWeapon < 5)
        {
            int newWeapon = selectedWeapon + 1;
            CmdSwitchWeapon(newWeapon);
        }
        if(Input.GetAxis("Mouse ScrollWheel") < 0f && selectedWeapon > 1)
        {
            int newWeapon = selectedWeapon - 1;
            CmdSwitchWeapon(newWeapon);
        }
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            CmdSwitchWeapon(1);
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            CmdSwitchWeapon(2);
        }
        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            CmdSwitchWeapon(3);
        }
        if(Input.GetKeyDown(KeyCode.Alpha4))
        {
            CmdSwitchWeapon(4);
        }
        if(Input.GetKeyDown(KeyCode.Alpha5))
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
        switch(oldWeapon)
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
        switch(newWeapon)
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
