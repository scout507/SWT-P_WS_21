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
    [SerializeField] Camera playerCamera;

    private WaitForSeconds shotDuration = new WaitForSeconds(0.07f);
    private LineRenderer laserLine;
    private float nextFire;


    public override void OnStartLocalPlayer()
    {
        laserLine = gun.GetComponent<LineRenderer>();
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

            StartCoroutine(ShotEffect());

            Vector3 rayOrigin = playerCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            laserLine.SetPosition(0, gunEnd.position);

            if(Physics.Raycast(rayOrigin, playerCamera.transform.forward, out hit, weaoponRange)) 
            {
                laserLine.SetPosition(1, hit.point);
                Debug.Log("Hit!");
            }
            else 
            {
                laserLine.SetPosition(1, rayOrigin + (playerCamera.transform.forward * weaoponRange));
            }
        }
        
    }

    private IEnumerator ShotEffect() 
    {
        
        laserLine.enabled = true;
        yield return shotDuration;
        laserLine.enabled = false;
    }
}
