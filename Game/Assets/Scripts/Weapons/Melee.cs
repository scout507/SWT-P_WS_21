using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* created by: SWT-P_WS_21/22 */

/// <summary>
/// Melee is responsible for the functions of the melee weapon, in our case a hammer.
/// It inherits ShootGun, because this abstract class implements functions for making damage.
/// </summary>
public class Melee : ShootGun
{
    /// <summary>
    /// In Start the different attributes for this gun are inizialized.
    /// </summary>
    void Start()
    {
        this.gunDamage = 50;
        this.fireRate = 0.5f;
        this.triggerRange = 0f;
        this.gunAmmo = 0; 
        this.magSize = 0;
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
            inventory.UpdateInfo(this.icon, 0, 0);
            if (Input.GetButtonDown("Fire1") && Time.time > nextFire)
            {
                nextFire = Time.time + fireRate;
                Shoot();
            }
            else
            {
                if (Input.GetButtonDown("Fire1") && Time.time > nextFire)
                {
                    nextFire = Time.time + fireRate;
                    Shoot();
                }
                else
                {
                    this.inAttack = false;
                }
            }
        }
    }

    /// <summary>
    /// A melee weapon does not shoot, so here it calls a corountin for an animation.
    /// </summary>
    public override void Shoot()
    {
        audioController.CmdPlayGunSound(3);
        this.inAttack = true;
    }

    /// <summary>
    /// Melee does not need Reload function, but it is necessary for abstract class ShootGun.
    /// </summary>
    public override void Reload()
    {
        return;
    }

    /// <summary>
    /// Deals damage to a gameobject which is hit.
    /// </summary>
    /// <param name="attackedOpponent"></param>
    public void meleeHit(GameObject attackedOpponent)
    {
        if (attackedOpponent.layer == LayerMask.NameToLayer("Player"))
        {
            CmdShootPlayer(attackedOpponent, gunDamage);
        }
        else if (attackedOpponent.layer == LayerMask.NameToLayer("Monster"))
        {
            CmdShootMonster(attackedOpponent, gunDamage);
        }
        else if (attackedOpponent.layer == LayerMask.NameToLayer("Device"))
        {
            CmdShootDevice(attackedOpponent, gunDamage);
        }
    }

    /// <summary>
    /// Getter for the melee's collider
    /// </summary>
    /// <returns>BoxCollider of the melee weapon's head</returns>
    public BoxCollider GetCollider()
    {
        return gunMount.GetComponentInChildren<BoxCollider>();
    }
}
