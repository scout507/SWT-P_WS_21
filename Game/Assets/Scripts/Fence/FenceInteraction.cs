using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FenceInteraction : NetworkBehaviour
{
    [SyncVar]
    public List<GameObject> boards = new List<GameObject>();

    [SyncVar]
    public List<GameObject> brokenBoards = new List<GameObject>();

    [SyncVar]
    public bool used = false;

    [SyncVar]
    public List<GameObject> players = new List<GameObject>();

    public float timer = 0;
    public float timePerBoardInSeconds = 3;
    public GameObject activPlayer = null;

    void Update()
    {
        if (isLocalPlayer)
        {
            Debug.Log("Test");
            if (players.Contains(gameObject) && !used && brokenBoards.Count > 0)
            {
                Debug.Log("Press [E]");

                if (Input.GetKeyDown(KeyCode.E))
                {
                    used = true;
                    activPlayer = gameObject;
                }
            }
        }

        if (isServer)
        {
            if (used)
            {
                timer += Time.deltaTime;

                if (timer >= timePerBoardInSeconds)
                {
                    timer = 0;
                    brokenBoards[0].GetComponent<MeshRenderer>().enabled = true;
                    brokenBoards.RemoveAt(0);
                }
            }

            if (players.Count == 0 || brokenBoards.Count == 0 || !players.Contains(activPlayer))
            {
                used = false;
                activPlayer = null;
            }

            brokenBoards.Clear();

            foreach (var item in boards)
                if (!item.GetComponent<MeshRenderer>().enabled)
                    brokenBoards.Add(item);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !players.Contains(other.gameObject))
        {
            players.Add(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            players.Remove(other.gameObject);
        }
    }
}
