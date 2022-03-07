using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* created by: SWT-P_WS_21/22 */


public class MP : ShootGun
{

    private AudioController audioController; // Audio Script that controlls Gun Sound

    /// <summary>
    /// In Start the different attributes for this gun are inizialized.
    /// The AudioController is set
    /// </summary>
    /// 


    void Start()
    {
        this.gunDamage = 5;
        this.fireRate = 0.1f;
        this.weaoponRange = 50f;
        this.gunAmmo = 30;
        this.recoil = 2.5f;

        audioController = this.GetComponent<AudioController>();
    }

    /// <summary>
    /// Processes the input of the player. Because the MP is a full automatic gun, the Shoot() method is called as long as the fire button is pressed and there are rounds left in
    /// the magazin.
    /// </summary>
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        if (Input.GetButton("Fire1") && Time.time > nextFire)
        {
            inventory.UpdateInfo(this.icon, this.gunAmmo, 0);
            nextFire = Time.time + fireRate;
            if (gunAmmo > 0)
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

    /// <summary>
    /// Shoots one shot defined by the attributes of the specific gun, here it fires a single round.
    /// </summary>
    public override void Shoot()
    {
        RaycastHit hit;
        Vector3 rayOrigin = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
        Vector3 direction = Camera.main.transform.forward;
        gunAmmo--;
        audioController.PlayGunSound(1);
        if (Physics.Raycast(rayOrigin, direction, out hit, weaoponRange, ~0))
        {
            Debug.Log("In Range!");
            Debug.DrawLine(rayOrigin, hit.point, Color.green, 0.5f);
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                CmdShootPlayer(hit.collider.transform.root.gameObject, gunDamage); // Gets Parent of Collider and calls function for hit on Player
            }
            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Monster"))
            {
                CmdShootMonster(hit.collider.transform.root.gameObject, gunDamage); // Calls TakeDamage on the monster hit
            }
            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Device"))
            {
                CmdShootDevice(hit.collider.transform.root.gameObject, gunDamage); // Calls TakeDamage on the device hit
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
