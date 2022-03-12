using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* created by: SWT-P_WS_21/22 */


/// <summary>
/// Shotgun implements a shotgun, inherits from ShootGun.
/// </summary>
public class Shotgun : ShootGun
{
    private int pelletAmount = 15;

    /// <summary>
    /// In Start the different attributes for this gun are inizialized.
    /// </summary>
    private void Start()
    {
        this.gunDamage = 5;
        this.fireRate = 0.25f;
        this.weaoponRange = 50f;
        this.gunAmmo = 8;
        this.recoil = 10f;
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
            if (Input.GetButtonDown("Fire1") && Time.time > nextFire)
            {
                inventory = GetComponentInChildren<Inventory>();
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
                gunAmmo = magSize;
            }

            inventory.UpdateInfo(this.icon, this.gunAmmo, 0);
        }
    }

    /// <summary>
    /// Shoots one shot defined by the attributes of the specific gun, here it fires multiple rounds in a random pattern.
    /// </summary>
    public override void Shoot()
    {
        RaycastHit hit;
        Vector3 rayOrigin = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
        Vector3 direction = Camera.main.transform.forward;
        gunAmmo--;
        audioController.PlayGunSound(2);
        for (int i = 0; i < pelletAmount; i++)
        {
            direction +=
                Quaternion.AngleAxis(Random.Range(-40f, 40f), Camera.main.transform.up)
                * Camera.main.transform.forward;
            direction +=
                Quaternion.AngleAxis(Random.Range(-40f, 40f), Camera.main.transform.right)
                * Camera.main.transform.forward;
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
                Debug.DrawRay(rayOrigin, direction * weaoponRange, Color.red, 0.5f);
                Debug.Log("Out of Range!");
            }
        }
        Recoil();
    }
}
