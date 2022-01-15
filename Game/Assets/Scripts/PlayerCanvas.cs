using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PlayerCanvas : NetworkBehaviour
{
    public GameObject PlayerCanvasObject;
    // Start is called before the first frame update
    void Start()
    {
        if(isLocalPlayer)
        {
            PlayerCanvasObject.SetActive(true);
        }
    }
}
