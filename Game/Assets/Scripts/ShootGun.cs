using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ShootGun : NetworkBehaviour
{
    [SerializeField] int gunDamage = 10; // Damage output for gun, maybe attached to Gun-Object later
    [SerializeField] float fireRate = 0.25f; // Firerate for gun, maybe attached to Gun-Object later
    [SerializeField] float weaoponRange = 50f; // Range for gun, maybe attached to Gun-Object later
    [SerializeField] Transform gunEnd; // Gun end for Animations, maybe attached to Gun-Object later 
    [SerializeField] GameObject gun; // Gun-Object, can change
    private float nextFire; // Time of the next shot you can take 


    public override void OnStartLocalPlayer()
    {
        gameObject.layer = 0;
        foreach (Transform child in gameObject.transform)
        {
            child.gameObject.layer = 0;
        }
        
    }


    void Update()
    {
        if(!isLocalPlayer) 
        {
            return;
        }

        if (Input.GetButtonDown("Fire1") && Time.time > nextFire) 
        {
            nextFire = Time.time + fireRate;
            Shoot();
        }
        
    }

    // Gets called if Player is hit
    [Command]
    void CmdShootPlayer(GameObject player)
    {
       Debug.Log("Hit Player!");
       player.GetComponent<Health>().TakeDamage(gunDamage);
    }

    // Function for hit on wall, can start Animation or something on point of hit
    [ClientRpc]
    void RpcHitWall(Vector3 hit)
    {
        Debug.Log("Hit Wall!");
    }

    // Gets called if Wall is hit
    [Command]
    void CmdShootWall(Vector3 hit)
    {
        RpcHitWall(hit);
    }


    /**
    Function for shooting, casts Ray and calls functions based on object that is hit
    */
    void Shoot()
    {
        RaycastHit hit;
        Vector3 rayOrigin = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
        Vector3 direction = Camera.main.transform.forward;
        
        if(Physics.Raycast(rayOrigin, direction, out hit, weaoponRange, ~LayerMask.NameToLayer("Default"))) 
        {
            Debug.Log("In Range!");
            if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                CmdShootPlayer(hit.collider.gameObject.transform.parent.gameObject); // Gets Parent of Collider and calls function for hit on Player
            }
            else
            {
                CmdShootWall(hit.point);
            }
        }
        else 
        {
            Debug.Log("Out of Range!");
        }
        
    }
    
}
