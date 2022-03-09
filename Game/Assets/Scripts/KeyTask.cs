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
    

    private void Update()
    {
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

        if(playerWithKey.GetComponent<Health>().health <= 0)
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

}
