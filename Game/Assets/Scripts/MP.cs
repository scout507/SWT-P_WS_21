using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MP : ShootGun
{
    // Start is called before the first frame update
    void Start()
    {
        this.gunDamage = 5;
        this.fireRate = 0.1f;
        this.weaoponRange = 50f;
        this.gunAmmo = 30;
        this.recoil = 2.5f;
    }

    void Update()
    {
        if(!isLocalPlayer) 
        {
            return;
        }
        if (Input.GetButton("Fire1") && Time.time > nextFire) 
        {
            nextFire = Time.time + fireRate;
            if(gunAmmo > 0)
            {
              Shoot();  
            }
            else
            {
                Debug.Log("Out of Ammo!");
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            gunAmmo = 30;
        }
    }

    public override void Shoot()
    {
        RaycastHit hit;
        Vector3 rayOrigin = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
        Vector3 direction = Camera.main.transform.forward;

        gunAmmo--;
        
        if(Physics.Raycast(rayOrigin, direction, out hit, weaoponRange, ~0)) 
        {
            Debug.Log("In Range!");
            Debug.DrawLine(rayOrigin, hit.point, Color.green, 0.5f);
            if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                CmdShootPlayer(hit.collider.transform.root.gameObject, gunDamage); // Gets Parent of Collider and calls function for hit on Player
            }
            else if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Monster"))
            {
                CmdShootMonster(hit.collider.transform.root.gameObject, gunDamage); // Calls TakeDamage on the monster hit
            }
            else
            {
                CmdShootWall(hit.point);
            }
        }
        else 
        {
            Debug.Log("Out of Range!");
            Debug.DrawRay(rayOrigin, direction * weaoponRange, Color.red, 0.5f);
        }
        Recoil();
    }
}
