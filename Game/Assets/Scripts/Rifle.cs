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
        this.reloadDelay = 0.7f;
        this.weaoponRange = 200f;
        this.gunAmmo = 4;
        this.recoil = 20f;
        this.magSize = 4;
        this.isReloading = false;
        audioController = this.GetComponent<AudioController>();
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

        if (canInteract)
        {
            inventory.UpdateInfo(this.icon, this.gunAmmo, 0);

            if (Input.GetButtonDown("Fire1") && Time.time > nextFire)
            {
                nextFire = Time.time + fireRate;
                if (gunAmmo > 0)
                {
                    isReloading = false;
                    Shoot();
                }
                else
                {
                    Debug.Log("Out of Ammo!");
                }
            }

            if (Input.GetKeyDown(KeyCode.R) && Time.time > nextReload && !Input.GetButton("Fire1"))
            {
                isReloading = true;
                nextReload = Time.time + reloadDelay;
            }
            if (isReloading)
            {
                Reload();
            }
        }
    }

    /// <summary>
    /// Rifle is reloaded one round after another till the magazin is full
    /// </summary>
    public override void Reload()
    {
        if (Time.time > nextReload && gunAmmo < magSize && isReloading)
        {
            nextReload = Time.time + reloadDelay;
            gunAmmo++;
        }
        if (gunAmmo == magSize)
        {
            isReloading = false;
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
        audioController.PlayGunSound(4);
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
