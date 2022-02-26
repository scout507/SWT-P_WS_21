using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class ZombieSpawner : NetworkBehaviour
{
    /// <summary>
    /// Prefab of spawnable enemy
    /// </summary>
    [SerializeField]
    GameObject waveZombie;

    /// <summary>
    /// Seconds to next wave
    /// </summary>
    [SyncVar]
    double nextWave = 0d;

    /// <summary>
    /// Time between waves in seconds
    /// </summary>
    [SerializeField]
    int timeBetweenWave = 600;

    /// <summary>
    /// Number of enemys per wave
    /// </summary>
    [SerializeField]
    int zombiesAmount = 10;

    [SerializeField]
    GameObject[] waveSpawns;

    [SerializeField]
    GameObject[] roamerSpawns;

    /// <summary>
    /// Spawns first wave at start of game
    /// </summary>
    void Start()
    {
        if (isServer)
        {
            SpawnWave();
            nextWave = NetworkTime.time + timeBetweenWave;
        }
    }

    /// <summary>
    /// Checks if time to next wave is over, spawns new wave, if time is over and at the moment returns timer as String in the console
    /// </summary>
    void Update()
    {
        if (isServer)
        {
            if (NetworkTime.time > nextWave)
            {
                SpawnWave();
                nextWave = NetworkTime.time + timeBetweenWave;
            }
        }
        Debug.Log(TimeToNextWaveString());
    }

    /// <summary>
    /// Spawns wave of enemys
    /// </summary>
    void SpawnWave()
    {
        for (int i = 0; i < zombiesAmount; i++)
        {
            GameObject spawnedZombie = (GameObject)Instantiate(waveZombie, this.gameObject.transform.position + new Vector3(i, 0, 0), Quaternion.identity);
            NetworkServer.Spawn(spawnedZombie);
        }
    }

    /// <summary>
    /// Returns string of remaining time
    /// </summary>
    /// <returns>String of remaining time</returns>
    public string TimeToNextWaveString()
    {
        double timeToNextWave = nextWave - NetworkTime.time;
        int minutes = (int)Math.Floor(timeToNextWave / 60);
        int seconds = (int)Math.Floor(timeToNextWave % 60);
        return minutes.ToString().PadLeft(2, '0') + ":" + seconds.ToString().PadLeft(2, '0');
    }
}
