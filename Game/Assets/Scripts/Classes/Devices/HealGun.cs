using UnityEngine;

/* created by: SWT-P_WS_21/22 */

/// <summary>
/// This Script implements a healing device.
/// It is a pistol with negativ damage and the option to apply "damage" on self.
/// </summary>
public class HealGun : ShootGun
{
    /// <summary>
    /// In Start the different attributes for this gun are inizialized.
    /// </summary>
    private void Start()
    {
        this.gunDamage = -15;
        this.fireRate = 0.25f;
        this.reloadDelay = 0.5f;
        this.weaponRange = 5f;
        this.gunAmmo = 1;
        this.isReloading = false;
        this.recoil = 0f;
        this.magSize = 1;
        this.triggerRange = 0f;
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
            inventory.UpdateInfo(this.icon, this.gunAmmo, this.magSize);

            if (Input.GetButtonDown("Fire1") && Time.time > nextFire)
            {
                nextFire = Time.time + fireRate;
                if (gunAmmo > 0)
                {
                    Shoot();
                }
            }
            if (Input.GetButtonDown("Fire3") && Time.time > nextFire)
            {
                nextFire = Time.time + fireRate;
                if (gunAmmo > 0)
                {
                    CmdShootPlayer(this.gameObject, gunDamage);
                    gunAmmo--;
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
    /// Healgun is reloaded in full magazins, but should not instantly be reloaded, so it is reloaded after a certain time after the button is pressed
    /// </summary>
    public override void Reload()
    {
        if (Time.time > nextReload)
        {
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
        if (Physics.Raycast(rayOrigin, direction, out hit, weaponRange, ~0))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                // Gets Parent of Collider and calls function for hit on Player
                CmdShootPlayer(hit.collider.transform.root.gameObject, gunDamage);
            }
            else
            {
                CmdShootWall(hit.point);
            }
        }
        Recoil();
    }
}
