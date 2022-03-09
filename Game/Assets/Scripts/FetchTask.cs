using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/// <summary>
/// Class for the 'FetchTask'-task. Inherits from Task.
/// </summary>
public class FetchTask : Task
{
    /// <summary>List of all spawnable fetchItems</summary>
    [SerializeField] List<GameObject> fetchItems;
    /// <summary>Amount of items to spawn for one task</summary>
    [SerializeField] int maxItems;
    /// <summary>Items in the drop-off zone</summary>
    List<GameObject> itemsInZone = new List<GameObject>();
    /// <summary>Amount of items in the drop-off zone</summary>
    int currentItems;
    /// <summary>True when the items have been spawned</summary>
    bool init;

    /// <summary>
    /// Spawns the fetchitems and checks for completion.
    /// </summary>
    void Update()
    {
        if (!isServer) return;
        if (!init) InitTask();
        if (currentItems == maxItems) FinishTask();
        if (done && currentItems != maxItems) UndoTask();
        taskDescription = "Find the remaining " + (maxItems-currentItems).ToString() + " barrels and bring them to PLACEHOLDER";
    }

    /// <summary>
    /// Spawns the fetch-items.
    /// </summary>
    void InitTask()
    {
        for (int i = 0; i < maxItems; i++)
        {
            GameObject itemToSpawn = fetchItems[Random.Range(0, fetchItems.Count)];
            fetchItems.Remove(itemToSpawn);
            GameObject fetchItem = Instantiate(itemToSpawn, itemToSpawn.transform.position, Quaternion.identity);
            NetworkServer.Spawn(fetchItem);
        }
        init = true;
    }

    /// <summary>
    /// Handles the detection of FetchItems inside the drop-off area.
    /// </summary>
    /// <param name="other">The collider of the gameobject that triggered this method</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "FetchItem" && isServer)
        {
            if (!itemsInZone.Contains(other.gameObject))
            {
                bool light = !other.GetComponent<FetchItem>().lightOn;
                other.GetComponent<FetchItem>().lightOn = light;
                itemsInZone.Add(other.gameObject);
                currentItems++;
                RpcFlipLight(other.gameObject, light);
            }
        }
    }

    /// <summary>
    /// Handles the detection of FetchItems inside the drop-off area.
    /// </summary>
    /// <param name="other">The collider of the gameobject that triggered this method</param>
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "FetchItem" && isServer)
        {
            if (itemsInZone.Contains(other.gameObject))
            {
                itemsInZone.Remove(other.gameObject);
                bool light = !other.GetComponent<FetchItem>().lightOn;
                other.GetComponent<FetchItem>().lightOn = light;
                currentItems--;
                RpcFlipLight(other.gameObject, light);
            }
        }
    }

    /// <summary>
    /// Used for dis/enabling the light on the FetchItems.
    /// </summary>
    /// <param name="item">Item to switch the light on</param>
    /// <param name="lightStatus">If the light is on (true) or of(false)</param>
    [ClientRpc]
    void RpcFlipLight(GameObject item, bool lightStatus)
    {
        item.GetComponentInChildren<Light>().enabled = lightStatus;
    }


}
