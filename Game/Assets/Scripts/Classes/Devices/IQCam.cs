using UnityEngine;
using Mirror;

/* created by: SWT-P_WS_21/22 */

/// <summary>
/// This Script controls the camera device.
/// It is responsible for setting up new cams and change view to the camera perspective.
/// </summary>
public class IQCam : NetworkBehaviour
{
    /// <summary>This variable represents the device in the hands of IQ, which is rendered if she has her gadget ready.</summary>
    [SerializeField] GameObject device;

    /// <summary>This variable is the gameobject of the cam, which is spawned when she sets one up</summary>
    [SerializeField] GameObject cam;

    /// <summary>Point where device is loaded.</summary>
    [SerializeField] Transform deviceMount;

    /// <summary>Point where camera is spawned</summary>
    [SerializeField] Transform throwPoint;

    /// <summary>Weapon inventory of player</summary>
    [SerializeField]
    Inventory inventory;

    /// <summary>Sprite for the UI.</summary>
    [SerializeField] Sprite icon;

    /// <summary>Count of remaining cams</summary>
    int remainingCams = 3;

    /// <summary>Time when next camera can be thrown</summary>
    float nextThrow;

    /// <summary>Rate in which cameras can be set up</summary>
    float throwRate = 0.25f;

    /// <summary>Flag if player is in cams</summary>
    bool isInCams = false;

    /// <summary>Array of cameras which are set up</summary>
    private GameObject[] cameras = new GameObject[3];

    /// <summary>Tracks how many cameras are set up</summary>
    private int setCamerasCount = 0;

    /// <summary>Tracks which camera was last activ</summary>
    private int lastActiveCam = 0;

    /// <summary>
    /// The correct rotation and the destruction of cameras is controlled in CamController. 
    /// This script needs a way to set the camera activ and to get the right camera, it is saved in this variable.
    /// </summary>
    CamController activeCam;

    /// <summary>Update() function is responsible for getting the user input.
    /// If Fire1 Button is pressed and the player is not activ in a camera, a new camera is thrown.
    /// On Key X the player can enter or leave the camera view.
    /// </summary>
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        inventory.UpdateInfo(icon, remainingCams, 3);
        if (Input.GetButtonDown("Fire1") && Time.time > nextThrow && !isInCams)
        {
            nextThrow = Time.time + throwRate;
            if (remainingCams > 0)
            {
                CmdSetNewCam(Camera.main.transform.forward);
                --remainingCams;
            }
        }
        if (Input.GetButtonDown("Fire2"))
        {
            if (!isInCams && setCamerasCount > 0)
            {
                GetComponent<PlayerMovement>().enabled = false;
                GetComponent<IQ>().enabled = false;
                isInCams = true;
                Camera.main.nearClipPlane = 0.01f;
                GetInCam();
            }
            else if (isInCams)
            {
                ReturnToPlayer();
            }
        }
        if (isInCams)
        {
            if (Input.GetButtonDown("Jump"))
            {
                if (setCamerasCount > 1 && lastActiveCam < setCamerasCount - 1)
                {
                    activeCam.CmdSetActive();
                    lastActiveCam += 1;
                    GetInCam();
                }
                else
                {
                    activeCam.CmdSetActive();
                    lastActiveCam = 0;
                    GetInCam();
                }
            }
        }
    }

    /// <summary>
    /// This function sets the camera view of the player to the view of the last activ camera. 
    /// </summary>
    public void GetInCam()
    {
        activeCam = cameras[lastActiveCam].gameObject.GetComponent<CamController>();
        activeCam.CmdSetActive();
        Transform cameraTransform = Camera.main.gameObject.transform; // Find main camera which is part of the scene instead of the prefab
        cameraTransform.parent = activeCam.cameraMount.transform; // Make the camera a child of the mount point
        cameraTransform.position = activeCam.cameraMount.transform.position; // Set position/rotation same as the mount point
        cameraTransform.rotation = activeCam.cameraMount.transform.rotation;
    }

    /// <summary>
    /// This function returns the camera view to the players character perspective.
    /// </summary>
    public void ReturnToPlayer()
    {
        GetComponent<PlayerMovement>().enabled = true;
        GetComponent<IQ>().enabled = true;
        isInCams = false;
        activeCam.CmdSetActive();
        Camera.main.nearClipPlane = 0.3f;
        PlayerMovement player = GetComponent<PlayerMovement>();
        Transform cameraTransform = Camera.main.gameObject.transform;
        cameraTransform.parent = player.cameraMountPoint.transform;
        cameraTransform.position = player.cameraMountPoint.transform.position; // Set position/rotation same as the mount point
        cameraTransform.rotation = player.cameraMountPoint.transform.rotation;
    }

    /// <summary>
    /// If a camera is destroyed it needs to be removed from the list of cameras.
    /// </summary>
    /// <param name="number">Index of camera, which should be removed.</param>
    public void RemoveDevice(int number)
    {
        if (number == 0)
        {
            if (setCamerasCount == 2)
            {
                cameras[0] = cameras[1];
                cameras[0].GetComponent<CamController>().CmdSetNumber(0);
            }
            else if (setCamerasCount == 3)
            {
                cameras[0] = cameras[1];
                cameras[1] = cameras[2];
                cameras[0].GetComponent<CamController>().CmdSetNumber(0);
                cameras[1].GetComponent<CamController>().CmdSetNumber(1);
            }
        }
        if (number == 1)
        {
            if (setCamerasCount > 2)
            {
                cameras[1] = cameras[2];
                cameras[1].GetComponent<CamController>().CmdSetNumber(1);
            }

        }
        setCamerasCount -= 1;
        switch (lastActiveCam)
        {
            case 1:
                lastActiveCam = 0;
                break;
            case 2:
                lastActiveCam = 1;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Destroys gameobject of device when a new gun is equipped.
    /// </summary>
    private void OnDisable()
    {
        Destroy(deviceMount.GetChild(0).gameObject);
    }

    /// <summary>
    /// Loads prefab of gun when this device is equipped.
    /// </summary>
    private void OnEnable()
    {
        Instantiate(device, deviceMount);
    }

    /// <summary>
    /// Sets the owner of a newly set up camera to the right player.
    /// Host and Guest need different methods.
    /// </summary>
    /// <param name="owner">Networkconnection of owner.</param>
    /// <param name="spawnedCamera">Newly set up camera.</param>
    [TargetRpc]
    void TargetSetNewCam(NetworkConnection owner, GameObject spawnedCamera)
    {
        if (!isServer)
        {
            spawnedCamera.GetComponent<CamController>().SetOwner(gameObject);
            cameras[setCamerasCount] = spawnedCamera;
            setCamerasCount++;
        }
        else
        {
            cameras[setCamerasCount - 1] = spawnedCamera;
        }
    }

    /// <summary>
    /// Spawns new camera and throws it in right direction, handled on Server.
    /// </summary>
    /// <param name="direction">Direction in which camera is thrown.</param>
    [Command]
    void CmdSetNewCam(Vector3 direction)
    {
        GameObject spawnedCamera = (GameObject)Instantiate(cam, throwPoint.position, Quaternion.identity);
        NetworkServer.Spawn(spawnedCamera);
        spawnedCamera.GetComponent<NetworkIdentity>().AssignClientAuthority(GetComponent<NetworkIdentity>().connectionToClient);
        spawnedCamera.GetComponent<Rigidbody>().AddForce(direction * 700);
        spawnedCamera.GetComponent<CamController>().SetNumber(setCamerasCount);
        spawnedCamera.GetComponent<CamController>().SetOwner(gameObject);
        TargetSetNewCam(GetComponent<NetworkIdentity>().connectionToClient, spawnedCamera);
        setCamerasCount++;
    }
}
