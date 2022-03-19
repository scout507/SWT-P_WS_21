using UnityEngine;

/* created by: SWT-P_WS_21/22 */


/// <summary>
/// Shotgun implements a shotgun, inherits from ShootGun.
/// </summary>
public class Shotgun : ShootGun
{
    /// <summary>One shot with the shotgun fires a amount of pellets with random spread. This variable defines the amount of pellets.</summary>
    private int pelletAmount = 15;

    /// <summary>
    /// In Start the different attributes for this gun are inizialized.
    /// </summary>
    private void Start()
    {
        this.gunDamage = 5;
        this.fireRate = 0.25f;
        this.reloadDelay = 0.25f;
        this.weaponRange = 50f;
        this.gunAmmo = 8;
        this.recoil = 10f;
        this.magSize = 8;
        this.isReloading = false;
        this.triggerRange = 25f;
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
                inventory = GetComponentInChildren<Inventory>();
                inventory.UpdateInfo(this.icon, this.gunAmmo, this.magSize);
                nextFire = Time.time + fireRate;
                if (gunAmmo > 0)
                {
                    isReloading = false;
                    Shoot();
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
    /// Shotgun is reloaded one round after another till the magazin is full.
    /// </summary>
    public override void Reload()
    {
        if (Time.time > nextReload && gunAmmo < magSize && isReloading)
        {
            nextReload = Time.time + reloadDelay;
            gunAmmo++;
            if (gunAmmo % 4 == 0)
            {
                audioController.CmdPlayGunSound(7);
            }
        }
        if (gunAmmo == magSize)
        {
            isReloading = false;
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
        audioController.CmdPlayGunSound(2);
        TriggerAggro();
        for (int i = 0; i < pelletAmount; i++)
        {
            direction +=
                Quaternion.AngleAxis(Random.Range(-40f, 40f), Camera.main.transform.up)
                * Camera.main.transform.forward;
            direction +=
                Quaternion.AngleAxis(Random.Range(-40f, 40f), Camera.main.transform.right)
                * Camera.main.transform.forward;
            if (Physics.Raycast(rayOrigin, direction, out hit, weaponRange))
            {
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
        }
        Recoil();
    }
}
