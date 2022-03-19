using System.Collections.Generic;
using UnityEngine;
using Mirror;

/* created by: SWT-P_WS_21/22 */

/// <summary>
/// Responsible for spawning zombies during a round.
/// </summary>
public class ZombieSpawner : NetworkBehaviour
{
    /// <summary>Prefab of spawnable enemy</summary>
    [SerializeField]
    GameObject waveZombie;


    /// <summary>Prefab of roaming zombies.</summary>
    [SerializeField]
    GameObject roamingZombie;

    /// <summary>Seconds to next wave</summary>
    [SyncVar]
    double nextWave = 0d;

    /// <summary>Time between waves in seconds</summary>
    [SerializeField]
    int timeBetweenWave = 600;

    /// <summary>Number of enemys per wave</summary>
    [SerializeField]
    int zombiesAmount = 10;

    /// <summary>Number of roaming zombies</summary>
    [SerializeField]
    int roamerAmount = 15;

    /// <summary>All places where a wave can spawn</summary>
    [SerializeField]
    GameObject[] waveSpawns;

    /// <summary>All spawing positions for roaming zombies</summary>
    [SerializeField]
    List<GameObject> roamerSpawns;

    /// <summary>
    /// Spawns first wave at start of game
    /// </summary>
    void Start()
    {
        if (isServer)
        {
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
    }

    /// <summary>
    /// Returns string of remaining time
    /// </summary>
    /// <returns>String of remaining time</returns>
    public string TimeToNextWaveString()
    {
        float timeToNextWave = (float)(nextWave - NetworkTime.time);
        int minutes = (int)Mathf.Floor(timeToNextWave / 60);
        int seconds = (int)Mathf.Floor(timeToNextWave % 60);
        return minutes.ToString().PadLeft(2, '0') + ":" + seconds.ToString().PadLeft(2, '0');
    }

    /// <summary>
    /// Used to spawn the inital roaming zombies and setting the first wave-timer
    /// </summary>
    public void InitialSpawn()
    {
        for (int i = 0; i < roamerAmount; i++)
        {
            GameObject spawn = roamerSpawns[Random.Range(0, roamerSpawns.Count)];
            roamerSpawns.Remove(spawn);
            GameObject spawnedZombie = (GameObject)Instantiate(roamingZombie, spawn.transform.position, Quaternion.identity);
            NetworkServer.Spawn(spawnedZombie);
        }

        nextWave = NetworkTime.time + timeBetweenWave;

    }

    /// <summary>
    /// Spawns wave of enemys
    /// </summary>
    void SpawnWave()
    {
        for (int i = 0; i < zombiesAmount; i++)
        {
            GameObject spawn = waveSpawns[Random.Range(0, waveSpawns.Length)];
            GameObject spawnedZombie = (GameObject)Instantiate(waveZombie, spawn.transform.position, Quaternion.identity);
            NetworkServer.Spawn(spawnedZombie);
        }
    }
}
