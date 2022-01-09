using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FenceDeath : MonoBehaviour
{
    DestructableObject board;

    
    void Start()
    {
        board = GetComponent<DestructableObject>();
    }

    
    void Update()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = board.active;
    }
}
