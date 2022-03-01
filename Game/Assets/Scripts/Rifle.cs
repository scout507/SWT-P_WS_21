using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* created by: SWT-P_WS_21/22 */


public class Rifle : ShootGun
{
    /// <summary>
    /// In Start the different attributes for this gun are inizialized.
    /// </summary>
    void Start()
    {
        this.gunDamage = 50;
        this.fireRate = 1f;
        this.weaoponRange = 200f;
        this.gunAmmo = 4;
        this.recoil = 20f;
    }

    /// <summary>
    /// Processes the input of the player.
    /// </summary>
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        if (Input.GetButtonDown("Fire1") && Time.time > nextFire)
        {
            inventory.UpdateInfo(this.icon,this.gunAmmo,0);
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
            gunAmmo = 4;
        }
    }

    /// <summary>
    /// Shoots one shot defined by the attributes of the specific gun, here it fires a single hart hitting round.
    /// </summary>
    public override void Shoot()
    {
        RaycastHit hit;
        Vector3 rayOrigin = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
        Vector3 direction = Camera.main.transform.forward;
        gunAmmo--;
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
