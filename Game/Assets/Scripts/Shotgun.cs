using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : ShootGun
{
    private int pelletAmount = 15;

    private void Start() 
    {
        this.gunDamage = 5;
        this.fireRate = 0.25f;
        this.weaoponRange = 50f;
        this.gunAmmo = 8;
        this.recoil = 10f;
        Instantiate(gun, gunMount);
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
            gunAmmo = 8;
        }
    }

    public override void Shoot()
    {
        RaycastHit hit;
        Vector3 rayOrigin = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
        Vector3 direction = Camera.main.transform.forward;
        gunAmmo--;
        for(int i = 0; i < pelletAmount; i++)
        {

            direction += Quaternion.AngleAxis(Random.Range(-40f, 40f), Camera.main.transform.up) * Camera.main.transform.forward;
            direction += Quaternion.AngleAxis(Random.Range(-40f, 40f), Camera.main.transform.right) * Camera.main.transform.forward;
            if(Physics.Raycast(rayOrigin, direction, out hit, weaoponRange, ~0)) 
            {
                Debug.Log("In Range!");
                Debug.DrawLine(rayOrigin, hit.point, Color.green, 0.5f);
                if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    CmdShootPlayer(hit.collider.transform.root.gameObject); // Gets Parent of Collider and calls function for hit on Player
                }
                else if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Monster"))
                {
                    CmdShootMonster(hit.collider.transform.root.gameObject); // Calls TakeDamage on the monster hit
                }
                else
                {
                    CmdShootWall(hit.point);
                }
            }
            else 
            {
                Debug.DrawRay(rayOrigin, direction * weaoponRange, Color.red, 0.5f);
                Debug.Log("Out of Range!");
            }
        }
        Recoil();
    }
}
