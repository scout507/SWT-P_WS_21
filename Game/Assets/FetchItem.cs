using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FetchItem : NetworkBehaviour
{

    public List<uint> players = new List<uint>();
    public List<uint> playersClient = new List<uint>();
    [SyncVar] uint usedBy;
    [SyncVar] bool used;
    public bool lightOn = true;
    //GameObject player;

    void Update()
    {
        
        if(isClient)
        {
            if(!used && playersClient.Contains(NetworkClient.localPlayer.gameObject.GetComponent<NetworkIdentity>().netId) && Input.GetKeyDown(KeyCode.E))
            {
                CmdPickUp(NetworkClient.localPlayer.gameObject);
                
            }
            else if(used && usedBy == NetworkClient.localPlayer.gameObject.GetComponent<NetworkIdentity>().netId && Input.GetKeyDown(KeyCode.E))
            {
                CmdLetGo();
            }
        }

        if(isServer && used)
        {
            //gameObject.transform.position = new Vector3(player.transform.position.x +0.5f, player.transform.position.y, player.transform.position.z);
        }
    }


    /// <summary>
    /// Adds players to the players List once they are in the interactive radius.
    /// </summary>
    /// <param name="other">Collider of the entering GameObject</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && isServer)
        {
            players.Add(other.GetComponent<NetworkIdentity>().netId);
            RpcSyncList(players);
        }
    }

    /// <summary>
    /// Removes players from the player list when they leave the interactive radius.
    /// </summary>
    /// <param name="other">Collider of the exiting GameObject</param>
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && isServer)
        {
            players.Remove(other.GetComponent<NetworkIdentity>().netId);
            RpcSyncList(players);
        } 
    }


    [Command(requiresAuthority = false)]
    void CmdPickUp(GameObject player)
    {
        gameObject.transform.rotation = Quaternion.Euler(0,0,0);
        gameObject.transform.SetParent(player.transform);
        usedBy = player.GetComponent<NetworkIdentity>().netId;
        used = true;
        GetComponent<Rigidbody>().isKinematic = true;
        RpcSetParent(player);
    }

    [Command(requiresAuthority = false)]
    void CmdLetGo()
    {
        gameObject.transform.parent = null;
        usedBy = 1000;
        used = false;
        RpcRemoveParent();
        GetComponent<Rigidbody>().isKinematic = false;
    }

    [ClientRpc]
    void RpcSyncList(List<uint>serverList)
    {
        playersClient = new List<uint>();
        foreach (uint player in serverList)
        {
            playersClient.Add(player);
        }
    }

    [ClientRpc]
    void RpcSetParent(GameObject player)
    {
        gameObject.transform.rotation = Quaternion.Euler(0,0,0);
        gameObject.transform.SetParent(player.transform);
        GetComponent<Rigidbody>().isKinematic = true;
    }

    [ClientRpc]
    void RpcRemoveParent()
    {
        gameObject.transform.parent = null;
        GetComponent<Rigidbody>().isKinematic = false;
    }

}
