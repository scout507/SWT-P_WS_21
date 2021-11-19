using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ShootGun : NetworkBehaviour
{

    [SerializeField] int gunDamage = 1;
    [SerializeField] float fireRate = 0.25f;
    [SerializeField] float weaoponRange = 50f;
    [SerializeField] Transform gunEnd;
    [SerializeField] GameObject gun;

    private WaitForSeconds shotDuration = new WaitForSeconds(0.07f);
    //private LineRenderer laserLine;
    private float nextFire;


    public override void OnStartLocalPlayer()
    {
        //laserLine = gun.GetComponent<LineRenderer>();
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

    [Command]
    void CmdShoot(Vector3 hit)
    {
       Debug.Log("Hit!");
    }

    void Shoot()
    {
        Vector3 rayOrigin = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if(Physics.Raycast(rayOrigin, Camera.main.transform.forward, out hit, weaoponRange)) 
        {
            Debug.Log("In Range!");
            CmdShoot(hit.point);
        }
        else 
        {
            Debug.Log("Out of Range!");
        }
        
    }
    
}
