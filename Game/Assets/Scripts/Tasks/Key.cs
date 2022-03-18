using UnityEngine;
using Mirror;

/* created by: SWT-P_WS_21/22 */

/// <summary>
/// Class for the 'Key' object of the 'KeyTask'.
/// </summary>
public class Key : NetworkBehaviour
{
    /// <summary>
    /// Used for pickup by the player. The Gameobject gets destroyed on pickup.
    /// </summary>
    /// <param name="other">The collider that triggered this function.</param>
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
