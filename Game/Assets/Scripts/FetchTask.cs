using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FetchTask : Task
{

    [SerializeField] List<GameObject> fetchItems;
    GameObject fetchItem;
    [SerializeField] int maxItems;
    List<GameObject> itemsInZone = new List<GameObject>();
    int currentItems;
    bool init;

    void Update()
    {
        if(!isServer) return;
        if(!init) InitTask();
        Debug.Log(currentItems);
        if(currentItems == maxItems) FinishTask();
        if(done && currentItems != maxItems) UndoTask();
    }

    void InitTask()
    {
        for(int i = 0; i<maxItems; i++)
        {
            GameObject itemToSpawn = fetchItems[Random.Range(0,fetchItems.Count)];
            fetchItem = Instantiate(itemToSpawn, itemToSpawn.transform.position,Quaternion.identity);
            NetworkServer.Spawn(fetchItem);
        }
        init = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "FetchItem" && isServer)
        {   
            if(!itemsInZone.Contains(other.gameObject))
            {
                bool light = !other.GetComponent<FetchItem>().lightOn;
                other.GetComponent<FetchItem>().lightOn = light;
                itemsInZone.Add(other.gameObject);
                currentItems ++;
                RpcFlipLight(other.gameObject, light);
            }
        } 
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "FetchItem" && isServer)
        {
            if(itemsInZone.Contains(other.gameObject))
            {
                itemsInZone.Remove(other.gameObject);
                bool light = !other.GetComponent<FetchItem>().lightOn;
                other.GetComponent<FetchItem>().lightOn = light;
                currentItems --;
                RpcFlipLight(other.gameObject, light);
            }
        } 
    }

    [ClientRpc]
    void RpcFlipLight(GameObject item, bool lightStatus)
    {
        item.GetComponentInChildren<Light>().enabled = lightStatus;
    }


}
