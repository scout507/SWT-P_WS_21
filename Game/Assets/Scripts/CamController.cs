using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/* created by: SWT-P_WS_21/22 */

/// <summary>
/// This Script is responsible for the right position of a thrown camera, for managing the owner of the camera 
/// and for destroying the gameobject in case of its death.
/// </summary>
public class CamController : Device
{
    /// <summary>
    /// This is the point, where the camera is mounted when an player enters it.
    /// </summary>
    public Transform cameraMount;
    /// <summary>
    /// This is the owner of the camera.
    /// </summary>
    GameObject owner;
    /// <summary>
    /// This is the index of the camera in the list of set up cameras of the owner.
    /// </summary>
    int numberOfCam;
    /// <summary>
    /// This flag indicates if a player uses the camera at the moment.
    /// </summary>
    bool isActiv = false;

    /// <summary>
    /// This function sets the owner of the camera.
    /// </summary>
    /// <param name="newOwner">The game object of the new owner.</param>
    public void SetOwner(GameObject newOwner)
    {
        owner = newOwner;
    }

    /// <summary>
    /// This command calls the function for setting the right index of the camera.
    /// </summary>
    /// <param name="number">Index in List of set up cameras.</param>
    [Command]
    public void CmdSetNumber(int number)
    {
        SetNumber(number);
    }

    /// <summary>
    /// This sets the right index of the camera on the server.
    /// </summary>
    /// <param name="number">Index in List of set up cameras.</param>
    public void SetNumber(int number)
    {
        numberOfCam = number;
    }

    /// <summary>
    /// On collision the camera rotates in the correct position.
    /// The rotation depends on the orientation of the wall.
    /// </summary>
    /// <param name="other"></param>
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.transform.gameObject.layer == 0 || other.collider.transform.gameObject.layer == 7 || other.collider.transform.gameObject.layer == 8 || other.collider.transform.gameObject.layer == 9)
        {
            return;
        }
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        transform.rotation = Quaternion.FromToRotation(Vector3.up, other.contacts[0].normal);
        if (this.transform.rotation == Quaternion.Euler(90f, 0f, 0f))
        {
            this.transform.rotation = Quaternion.Euler(0f, 90f, 90f);
        }
        if (this.transform.rotation == Quaternion.Euler(0f, 0f, -90f))
        {
            this.transform.rotation = Quaternion.Euler(180f, 0f, -90f);
        }
        if (this.transform.rotation == Quaternion.Euler(-90f, 0f, 0f))
        {
            this.transform.rotation = Quaternion.Euler(0f, -90f, 90f);
        }
    }

    /// <summary>
    /// This function destroys the camera and returns the players perspective to the character if the camera was in use.
    /// </summary>
    public override void TargetDeath()
    {
        if (!isServer)
        {
            return;
        }
        if (isActiv)
        {
            TargetReturnToPlayer(owner.GetComponent<NetworkIdentity>().connectionToClient);
        }
        TargetRemoveDevice(owner.GetComponent<NetworkIdentity>().connectionToClient, numberOfCam);
        RpcDestroyDevice(gameObject);
    }

    /// <summary>
    /// If the camera is destroyed it needs to be removed from the list of set up cams.
    /// </summary>
    /// <param name="deviceOwner">Owner of destroyed camera.</param>
    /// <param name="number">Index of destroyed camera.</param>
    [TargetRpc]
    void TargetRemoveDevice(NetworkConnection deviceOwner, int number)
    {
        deviceOwner.identity.GetComponent<IQCam>().RemoveDevice(number);
    }

    /// <summary>
    /// Calls function in player character which pulls back the view to the player character.
    /// </summary>
    /// <param name="deviceOwner"></param>
    [TargetRpc]
    void TargetReturnToPlayer(NetworkConnection deviceOwner)
    {
        deviceOwner.identity.GetComponent<IQCam>().ReturnToPlayer();
    }

    /// <summary>
    /// Sets the isActiv flag depending on whether it is used at the moment or not.
    /// </summary>
    [Command]
    public void CmdSetActive()
    {
        if (isActiv) isActiv = false;
        else isActiv = true;
    }
}
