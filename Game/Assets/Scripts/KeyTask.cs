using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class KeyTask : Task
{
    /// <summary>The player object that holds the key</summary>
    [SyncVar] public GameObject playerWithKey;
    /// <summary>The zombie holding the key</summary>
    GameObject zombieWithKey;
    /// <summary>A list of all living roaming zombies</summary>
    List<GameObject> activeZombies;
    /// <summary>Monstercontroller of the zombieWithKey</summary>
    MonsterController zombieScript;
    /// <summary>The key prefab</summary>
    public GameObject key;
    /// <summary>Time before a new zombie is chosen</summary>
    float refreshTime = 10f;
    /// <summary>Timer for choosing a new zombie</summary>
    float refreshTimer;
    /// <summary>True when the key is dropped by a zombie</summary>
    bool keyDropped;
    /// <summary>True when the player with the key is nearby</summary>
    [SyncVar] bool keyHere;


    /// <summary>
    /// Handles player-interaction. 
    /// </summary>
    private void Update()
    {

        if (isClient)
        {
            if (!done && keyHere && NetworkClient.localPlayer.gameObject.GetComponent<NetworkIdentity>().netId == playerWithKey.GetComponent<NetworkIdentity>().netId)
            {
                Debug.Log("Press 'E");
                if (Input.GetKeyDown(KeyCode.E)) CmdFinish();
            }
        }

        if (!isServer) return;

        refreshTimer -= Time.deltaTime;

        if (refreshTimer <= 0 && !keyDropped)
        {
            zombieWithKey = ChooseZombie();
            zombieScript = zombieWithKey.GetComponent<MonsterController>();
            refreshTimer = refreshTime;
        }

        if (zombieScript.dead && !keyDropped)
        {
            SpawnKey(zombieWithKey.transform.position);
        }

        if (playerWithKey != null && playerWithKey.GetComponent<Health>().health <= 0)
        {
            SpawnKey(playerWithKey.transform.position);
        }
    }

    /// <summary>
    /// Chooses a zombie to hold the key.
    /// </summary>
    /// <returns>The chosen zombie gameobject</returns>
    GameObject ChooseZombie()
    {
        GameObject[] zombies = GameObject.FindGameObjectsWithTag("Roamer");
        activeZombies = new List<GameObject>();

        foreach (GameObject zombie in zombies)
        {
            if (!zombie.GetComponent<MonsterController>().dead) activeZombies.Add(zombie);
        }

        return activeZombies[Random.Range(0, activeZombies.Count)];
    }

    /// <summary>
    /// Spawns the key on kill.
    /// </summary>
    /// <param name="spawnPoint">The place where the key is spawned</param>
    void SpawnKey(Vector3 spawnPoint)
    {
        //TODO: play sound
        keyDropped = true;
        GameObject keyInstance = Instantiate(key, spawnPoint, Quaternion.identity);
        NetworkServer.Spawn(keyInstance);
        taskDescription = "Pick up the key!";
    }

    /// <summary>
    /// For detecting nearby players.
    /// </summary>
    /// <param name="other">Collider that triggered this method.</param>
    void OnTriggerEnter(Collider other)
    {
        if (isServer && other.tag == "Player" && playerWithKey && other.GetComponent<NetworkIdentity>().netId == playerWithKey.GetComponent<NetworkIdentity>().netId)
        {
            keyHere = true;
        }
    }

    /// <summary>
    /// For detecting when a nearby player leaves.
    /// </summary>
    /// <param name="other">Collider that triggered this method.</param>
    void OnTriggerExit(Collider other)
    {
        if (isServer && other.tag == "Player" && playerWithKey && other.GetComponent<NetworkIdentity>().netId == playerWithKey.GetComponent<NetworkIdentity>().netId)
        {
            keyHere = false;
        }
    }

    /// <summary>
    /// Used to finish this task.
    /// </summary>
    [Command(requiresAuthority = false)]
    void CmdFinish()
    {
        FinishTask();
    }



}
