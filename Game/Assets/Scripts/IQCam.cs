using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class IQCam : NetworkBehaviour
{

    public GameObject device;
    public GameObject cam;
    public int remainingCams = 3;
    float nextFire;
    float fireRate;
    bool isInCams = false;
    public Transform deviceMount; // Point where device is loaded
    public Transform throwPoint;
    private GameObject[] cameras = new GameObject[3];

    [SerializeField]
    private int setCamerasCount = 0;

    [SerializeField]
    private int lastActiveCam = 0;
    public Transform cameraMountPoint;
    CamController activeCam;

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        if (Input.GetButtonDown("Fire1") && Time.time > nextFire && !isInCams)
        {
            nextFire = Time.time + fireRate;
            if (remainingCams > 0)
            {
                CmdSetNewCam(Camera.main.transform.forward);
                --remainingCams;
            }
            else
            {
                Debug.Log("Out of Cams!");
            }
        }
        if (Input.GetKeyDown(KeyCode.X))
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

    public void GetInCam()
    {
        activeCam = cameras[lastActiveCam].gameObject.GetComponent<CamController>();
        activeCam.CmdSetActive();
        Transform cameraTransform = Camera.main.gameObject.transform; // Find main camera which is part of the scene instead of the prefab
        cameraMountPoint = activeCam.transform;
        cameraTransform.parent = activeCam.cameraMount.transform; // Make the camera a child of the mount point
        cameraTransform.position = activeCam.cameraMount.transform.position; // Set position/rotation same as the mount point
        cameraTransform.rotation = activeCam.cameraMount.transform.rotation;
    }

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

    /// <summary>
    /// Destroys gameobject of gun when a new gun is equipped.
    /// </summary>
    private void OnDisable()
    {
        Destroy(deviceMount.GetChild(0).gameObject);
    }

    /// <summary>
    /// Loads prefab of gun when this gun is equipped.
    /// </summary>
    private void OnEnable()
    {
        Instantiate(device, deviceMount);
    }
}
