using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Task : NetworkBehaviour
{
    [SyncVar]
    [SerializeField]
    Color color = Color.red;

    private bool touchedGround = false;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<Renderer>().material.color = color;



    }

    void OnCollisionEnter(Collision other)
    {
        changeColor();

        touchedGround = true;


    }

    void changeColor()
    {
        if (touchedGround)
        {
            Debug.Log("Collided");
            color = Color.green;


        }
    }
}
