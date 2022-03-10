using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/* created by: SWT-P_WS_21/22 */


/// <summary>
/// Script to interact with the Turret
/// </summary>
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
    /// checks which weapon script is active
    /// 0 = Pistol; 1 = MP ; 2 = Shotgun ; 3 = Melee ; 4 = Rifle
    /// </summary>
    private int[] weaponScriptActive = new int[5];

    /// <summary>
    /// Checks if the Player is in the Turret
    /// </summary>
    private bool inVehicle = false;

    /// <summary>
    /// Reference to the Player Network ID
    /// </summary>
    public uint playerinUseID;

    /// <summary>
    /// Firerate of the Turret
    /// </summary>
    [SerializeField] float fireRate = 1f;

    /// <summary>
    /// Fire rate of the Turret
    /// </summary>
    float nextFire = 0;

    /// <summary>
    /// The Player can use:
    /// E to enter the Vehicle
    /// Escpae to Exit the Vehicle
    /// Use Fire1 Button to shot with the Vehicle
    /// Rotate the Turret when the Player is inside the Vehicle
    /// The Order of the Weapon Scripts: 0 = Pistol; 1 = MP ; 2 = Shotgun ; 3 = Melee ; 4 = Rifle
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

                if (gameObject.GetComponent<Pistol>()?.enabled == true)
                {
                    weaponScriptActive[0] = 1;
                    gameObject.GetComponent<Pistol>().enabled = false;
                }
                if (gameObject.GetComponent<MP>()?.enabled == true)
                {
                    weaponScriptActive[1] = 1;
                    gameObject.GetComponent<MP>().enabled = false;
                }
                if (gameObject.GetComponent<Shotgun>()?.enabled == true)
                {
                    weaponScriptActive[2] = 1;
                    gameObject.GetComponent<Shotgun>().enabled = false;
                }
                if (gameObject.GetComponent<Melee>()?.enabled == true)
                {
                    weaponScriptActive[3] = 1;
                    gameObject.GetComponent<Melee>().enabled = false;
                }
                if (gameObject.GetComponent<Rifle>()?.enabled == true)
                {
                    weaponScriptActive[4] = 1;
                    gameObject.GetComponent<Rifle>().enabled = false;
                }
            }

            if (inVehicle)
            {
                float rotateDirection = Input.GetAxis("Horizontal");
                float upDownDirection = Input.GetAxis("Vertical");
                turret.cmdrotateTurret(rotateDirection, upDownDirection);
                transform.position = turretCameraMount.transform.position;
                transform.rotation = turretCameraMount.transform.rotation;
            }


            if (Input.GetKeyDown(KeyCode.Escape))
            {
                playerCamera.transform.parent = playerCameraMount.transform;
                playerCamera.transform.position = playerCameraMount.transform.position;
                playerCameraMount.transform.parent.rotation = Quaternion.Euler(0, turretCameraMount.transform.eulerAngles.y, 0);


                inVehicle = false;
                gameObject.GetComponent<PlayerMovement>().enabled = true;


                if (weaponScriptActive[0] == 1)
                {
                    gameObject.GetComponent<Pistol>().enabled = true;
                }
                if (weaponScriptActive[1] == 1)
                {
                    gameObject.GetComponent<MP>().enabled = true;
                }
                if (weaponScriptActive[2] == 1)
                {
                    gameObject.GetComponent<Shotgun>().enabled = true;
                }
                if (weaponScriptActive[3] == 1)
                {
                    gameObject.GetComponent<Melee>().enabled = true;
                }
                if (weaponScriptActive[4] == 1)
                {
                    gameObject.GetComponent<Rifle>().enabled = true;
                }

                turret.exitVehicle();
            }

            if (Input.GetButtonDown("Fire1") && Time.time > nextFire)
            {
                nextFire = Time.time + fireRate;
                turret.Shoot();
            }
        }
    }
}
