using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Turret : NetworkBehaviour
{


    private PlayerMovement player;

    GameObject playerCamera;

    /// <summary>
    /// checks if the Player is in the Turret
    /// </summary>
    public bool inVehicle = false;

    /// <summary>
    /// checks if the Player is close enough to the Turret to interact
    /// </summary>
    public bool interactRange = false;


    /// <summary>
    /// Turret Camera Mount
    /// </summary>
    [SerializeField] GameObject cameraMountPoint;

    /// <summary>
    /// Player Camera Mount
    /// </summary>
    private GameObject playerCameraMountPoint;

    /// <summary>
    /// Rotation speed of the turret
    /// </summary>
    private float rotateSpeed = 100f;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame

    /// <summary>
    /// The player can use the Vehicle with E and Exit it with Escp
    /// Inside the Turret it can be moved with A and D to rotate it around its y-Axis
    /// </summary>
    void Update()
    {
        {
            if (Input.GetKeyDown(KeyCode.E) && interactRange == true && inVehicle == false)
            {
                inVehicle = true;
                player.canMove = false;
                playerCamera.transform.parent = cameraMountPoint.transform;
                playerCamera.transform.position = cameraMountPoint.transform.position;
                playerCamera.transform.rotation = cameraMountPoint.transform.rotation;
            }
            if (Input.GetKeyDown(KeyCode.Escape) && interactRange == true && inVehicle == true)
            {
                playerCamera.transform.parent = playerCameraMountPoint.transform;
                playerCamera.transform.position = playerCameraMountPoint.transform.position;
                playerCamera.transform.rotation = playerCameraMountPoint.transform.rotation;
                player.canMove = true;
                inVehicle = false;
            }

            if (inVehicle)
            {

                float rotateDirection = Input.GetAxis("Horizontal");
                rotateTurret(rotateDirection);

            }
        }

    }


    /// <summary>
    /// Client calls the Server to move the Object
    /// </summary>
    /// <param name="rotateDirection">
    /// The Directon and the Value of the Movement
    /// </param>
    [Command(requiresAuthority = false)]
    void rotateTurret(float rotateDirection)
    {
        transform.transform.Rotate(new Vector3(0, rotateDirection, 0) * rotateSpeed * Time.deltaTime);
    }



    /// <summary>
    /// When the player is close enough to the Turret, he can interact with it
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        interactRange = true;
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera");
        player = other.gameObject.GetComponent<PlayerMovement>();
        playerCameraMountPoint = other.gameObject.transform.GetChild(0).gameObject;
    }


    /// <summary>
    /// When the player leaves the Turret Collider range, he can no longer interact with it
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        interactRange = false;
    }

}
