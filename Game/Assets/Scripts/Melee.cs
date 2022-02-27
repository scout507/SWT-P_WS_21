using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* created by: SWT-P_WS_21/22 */


public class Melee : ShootGun
{

    private AudioController audioController; // Audio Script that controlls Gun Sound

    /// <summary>
    /// In Start the different attributes for this gun are inizialized.
    /// </summary>
    void Start()
    {
        this.gunDamage = 100;
        this.fireRate = 0.5f;

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
        if (Input.GetButtonDown("Fire1") && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            Shoot();
        }
    }

    /// <summary>
    /// A melee weapon does not shoot, so here it calls a corountin for an animation.
    /// </summary>
    public override void Shoot()
    {
        audioController.PlayGunSound(3);
        StartCoroutine(Hit());
    }

    /// <summary>
    /// Enables the colliders of the weapon and plays animation.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Hit()
    {
        gunMount.GetComponentInChildren<CapsuleCollider>().enabled = true;
        for (int i = 0; i < 90; i += 2)
        {
            gunMount.transform.localRotation = Quaternion.Euler(90f, 0f, i);
            yield return new WaitForEndOfFrame();
        }
        gunMount.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        gunMount.GetComponentInChildren<CapsuleCollider>().enabled = false;
        yield return null;
    }

    /// <summary>
    /// This is called when a melee weapon hits a player.
    /// </summary>
    /// <param name="other">The collider of the gameobject which hit this gameobject.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.gameObject.CompareTag("Player"))
        {
            other.transform.root.GetComponent<Melee>().meleeHit(gameObject);
        }
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
    }
}
