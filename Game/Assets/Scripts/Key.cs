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
            GameObject.FindObjectOfType<KeyTask>().GetComponent<KeyTask>().taskDescription = "Use the key to open the door at PLACEHOLDER";
            Destroy(this.gameObject);
        }
    }
}
