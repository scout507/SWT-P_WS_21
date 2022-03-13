using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* created by: SWT-P_WS_21/22 */


public class Pistol : ShootGun
{
    /// <summary>
    /// In Start the different attributes for this gun are inizialized.
    /// </summary>
    private void Start()
    {
        this.gunDamage = 10;
        this.fireRate = 0.25f;
        this.reloadDelay = 0.5f;
        this.weaoponRange = 50f;
        this.gunAmmo = 8;
        this.recoil = 3f;
        this.magSize = 8;
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
    /// Pistol is reloaded in full magazins, but should not instantly be reloaded, so it is reloaded after a certain time after the button is pressed
    /// </summary>
    public override void Reload()
    {
        if (Time.time > nextReload)
        {
            audioController.CmdPlayGunSound(4);
            gunAmmo = magSize;
            isReloading = false;
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
        audioController.PlayGunSound(0);
        TriggerAggro();
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
