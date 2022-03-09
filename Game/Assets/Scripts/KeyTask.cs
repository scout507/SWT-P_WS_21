using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class KeyTask : Task
{
    [SyncVar] public GameObject playerWithKey;
    GameObject zombieWithKey;
    List<GameObject> activeZombies;
    MonsterController zombieScript;
    public GameObject key;
    float refreshTime = 10f;
    float refreshTimer;
    bool keyDropped;
    [SyncVar] bool keyHere;

    private void Update()
    {
        
        if(isClient)
        {
            if(!done && keyHere && NetworkClient.localPlayer.gameObject.GetComponent<NetworkIdentity>().netId == playerWithKey.GetComponent<NetworkIdentity>().netId)
            {
                Debug.Log("Press 'E");
                if(Input.GetKeyDown(KeyCode.E)) CmdFinish();
            }
        }

        if(!isServer) return;

        refreshTimer -= Time.deltaTime;

        if(refreshTimer <= 0 && !keyDropped)
        {
            zombieWithKey = ChooseZombie();
            zombieScript = zombieWithKey.GetComponent<MonsterController>();
            refreshTimer = refreshTime;
        }

        if(zombieScript.dead && !keyDropped)
        {
            SpawnKey(zombieWithKey.transform.position);
        }

        if(playerWithKey != null && playerWithKey.GetComponent<Health>().health <= 0)
        {
            SpawnKey(playerWithKey.transform.position);
        }
    }


    GameObject ChooseZombie()
    {
        GameObject[] zombies = GameObject.FindGameObjectsWithTag("Roamer");
        activeZombies = new List<GameObject>();

        foreach (GameObject zombie in zombies)
        {
            if(!zombie.GetComponent<MonsterController>().dead) activeZombies.Add(zombie);
        }

        return activeZombies[Random.Range(0,activeZombies.Count)];
    }

    void SpawnKey(Vector3 spawnPoint)
    {
        //TODO: play sound
        keyDropped = true;
        GameObject keyInstance = Instantiate(key, spawnPoint, Quaternion.identity);
        NetworkServer.Spawn(keyInstance);
        taskDescription = "Pick up the key!";
    }


    void OnTriggerEnter(Collider other)
    {
        if(isServer && other.tag == "Player" && other.GetComponent<NetworkIdentity>().netId == playerWithKey.GetComponent<NetworkIdentity>().netId)
        {
            keyHere = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(isServer && other.tag == "Player" && other.GetComponent<NetworkIdentity>().netId == playerWithKey.GetComponent<NetworkIdentity>().netId)
        {
            keyHere = false;
        }
    }

    [Command(requiresAuthority = false)]
    void CmdFinish()
    {
        FinishTask();
    }



}
