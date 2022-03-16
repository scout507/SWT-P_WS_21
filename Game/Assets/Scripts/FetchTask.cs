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
    [SerializeField]
    List<GameObject> fetchItems;

    /// <summary>The item to spawn</summary>
    [SerializeField]
    GameObject fetchItem;

    /// <summary>A list of all possible spawn locations</summary>
    [SerializeField]
    List<Vector3> spawns;

    /// <summary>Amount of items to spawn for one task</summary>
    [SerializeField]
    int maxItems;

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
        if (!isServer)
            return;
        if (!init)
            InitTask();
        if (currentItems == maxItems)
            FinishTask();
        if (done && currentItems != maxItems)
            UndoTask();
        taskDescription =
            "Find the remaining "
            + (maxItems - currentItems).ToString()
            + " barrels and bring them to the main building";
    }

    /// <summary>
    /// Spawns the fetch-items.
    /// </summary>
    void InitTask()
    {
        for (int i = 0; i < maxItems + 3; i++)
        {
            Vector3 spawn = spawns[Random.Range(0, spawns.Count)];
            spawns.Remove(spawn);
            GameObject fetchItemInstance = Instantiate(fetchItem, spawn, Quaternion.identity);
            NetworkServer.Spawn(fetchItemInstance);
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
