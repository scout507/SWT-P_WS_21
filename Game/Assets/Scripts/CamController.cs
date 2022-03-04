using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class CamController : Device
{
    public Transform cameraMount;
    GameObject owner;
    int numberOfCam;
    bool isActiv = false;
    // Update is called once per frame
    public void SetOwner(GameObject newOwner)
    {
        owner = newOwner;
    }

    [Command]
    public void CmdSetNumber(int number)
    {
        SetNumber(number);
    }
    public void SetNumber(int number)
    {
        numberOfCam = number;
    }

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

    public override void TargetDeath()
    {
        if (!isServer)
        {
            return;
        }
        if(isActiv)
        {
            TargetReturnToPlayer(owner.GetComponent<NetworkIdentity>().connectionToClient);
        }
        TargetRemoveDevice(owner.GetComponent<NetworkIdentity>().connectionToClient, numberOfCam);
        RpcDestroyDevice(gameObject);
    }

    [TargetRpc]
    void TargetRemoveDevice(NetworkConnection deviceOwner, int number)
    {
        deviceOwner.identity.GetComponent<IQCam>().RemoveDevice(number);
    }

    [TargetRpc]
    void TargetReturnToPlayer(NetworkConnection deviceOwner)
    {
        deviceOwner.identity.GetComponent<IQCam>().ReturnToPlayer();
    }

    [Command]
    public void CmdSetActive()
    {
        if (isActiv) isActiv = false;
        else isActiv = true;
    }
}
