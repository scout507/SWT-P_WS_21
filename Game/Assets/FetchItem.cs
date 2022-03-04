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


    /// <summary>
    /// Used for player-object interaction.
    /// </summary>
    void Update()
    {

        if (isClient)
        {
            if (!used && playersClient.Contains(NetworkClient.localPlayer.gameObject.GetComponent<NetworkIdentity>().netId) && Input.GetKeyDown(KeyCode.E))
            {
                CmdPickUp(NetworkClient.localPlayer.gameObject);

            }
            else if (used && usedBy == NetworkClient.localPlayer.gameObject.GetComponent<NetworkIdentity>().netId && Input.GetKeyDown(KeyCode.E))
            {
                CmdLetGo();
            }
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

    /// <summary>
    /// Binds the Item to the player and prevents others from picking it up
    /// </summary>
    /// <param name="player">The player that picked up the item</param>
    [Command(requiresAuthority = false)]
    void CmdPickUp(GameObject player)
    {
        gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        gameObject.transform.SetParent(player.transform);
        usedBy = player.GetComponent<NetworkIdentity>().netId;
        used = true;
        GetComponent<Rigidbody>().isKinematic = true;
        RpcSetParent(player);
    }

    /// <summary>
    /// Used for unbinding the item from the player.
    /// </summary>
    [Command(requiresAuthority = false)]
    void CmdLetGo()
    {
        gameObject.transform.parent = null;
        usedBy = 1000;
        used = false;
        RpcRemoveParent();
        GetComponent<Rigidbody>().isKinematic = false;
    }

    /// <summary>
    /// Used for syncing the list of nearby players to the clients.
    /// </summary>
    /// <param name="serverList"></param>
    [ClientRpc]
    void RpcSyncList(List<uint> serverList)
    {
        playersClient = new List<uint>();
        foreach (uint player in serverList)
        {
            playersClient.Add(player);
        }
    }

    /// <summary>
    /// Binds the item to the player on the client-side.
    /// </summary>
    /// <param name="player">Player that picked up the item</param>
    [ClientRpc]
    void RpcSetParent(GameObject player)
    {
        gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        gameObject.transform.SetParent(player.transform);
        GetComponent<Rigidbody>().isKinematic = true;
    }

    /// <summary>
    /// Unbinds the item from the player on client-side.
    /// </summary>
    [ClientRpc]
    void RpcRemoveParent()
    {
        gameObject.transform.parent = null;
        GetComponent<Rigidbody>().isKinematic = false;
    }

}
