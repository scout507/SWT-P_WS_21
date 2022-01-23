using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ControllTurret : NetworkBehaviour
{


    /// <summary>
    /// Reference to the Turret Gameobject
    /// </summary>
    public Turret turret;

    /// <summary>
    /// Reference to the Player Camera Mount
    /// </summary>
    [SerializeField] GameObject cameraMount;

    /// <summary>
    /// Checks if the Player is in the Turret
    /// </summary>
    private bool inVehicle = false;

    /// <summary>
    /// Reference to the Player Network ID
    /// </summary>
    public uint playerinUseID;


    // Update is called once per frame
    /// <summary>
    /// The Player can use:
    /// E to enter the Vehicle
    /// Escpae to Exit the Vehicle
    /// Use Fire1 Button to shot with the Vehicle
    /// Rotate the Turret when the Player is inside the Vehicle
    /// </summary>
    void Update()
    {

        if (!isLocalPlayer)
        {
            return;
        }
        if (gameObject.GetComponent<NetworkIdentity>().netId == playerinUseID)
        {
            GameObject turretCameraMount = turret.transform.GetChild(0).GetChild(0).gameObject;
            GameObject playerCameraMount = gameObject.transform.GetChild(0).gameObject;
            GameObject playerCamera = GameObject.FindGameObjectWithTag("MainCamera");

            if (Input.GetKeyDown(KeyCode.E))
            {
                playerCamera.transform.parent = turretCameraMount.transform;
                playerCamera.transform.position = turretCameraMount.transform.position;
                playerCamera.transform.rotation = turretCameraMount.transform.rotation;
                turret.enterVehicle();
                inVehicle = true;

                gameObject.GetComponent<PlayerMovement>().enabled = false;
                gameObject.GetComponent<ShootGun>().enabled = false;
            }

            if (inVehicle)
            {
                float rotateDirection = Input.GetAxis("Horizontal");
                turret.cmdrotateTurret(rotateDirection);
            }


            if (Input.GetKeyDown(KeyCode.Escape))
            {
                playerCamera.transform.parent = playerCameraMount.transform;
                playerCamera.transform.position = playerCameraMount.transform.position;
                playerCamera.transform.rotation = playerCameraMount.transform.rotation;

                inVehicle = false;
                gameObject.GetComponent<PlayerMovement>().enabled = true;
                gameObject.GetComponent<ShootGun>().enabled = true;
                turret.exitVehicle();
            }

            if (Input.GetButtonDown("Fire1"))
            {
                turret.Shoot();
            }
        }
    }
}
