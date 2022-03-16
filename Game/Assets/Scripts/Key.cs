using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Key : NetworkBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && isServer)
        {
            GameObject.FindObjectOfType<KeyTask>().GetComponent<KeyTask>().playerWithKey = other.gameObject;
            GameObject.FindObjectOfType<KeyTask>().GetComponent<KeyTask>().taskDescription = "Use the key to access the electronics in the south-east building";
            TargetRpcSendMessage(other.gameObject.GetComponent<NetworkIdentity>().connectionToClient, "You picked up the key!");
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Sends a message to the choosen player.
    /// </summary>
    /// <param name="target">The players NetworkConnecton</param>
    /// <param name="message">The message for the player to recieve</param>
    [TargetRpc]
    void TargetRpcSendMessage(NetworkConnection target, string message)
    {
        if(NetworkClient.localPlayer.gameObject.GetComponent<Chatbox>()) NetworkClient.localPlayer.gameObject.GetComponent<Chatbox>().AddMessage(message);
    }
}
