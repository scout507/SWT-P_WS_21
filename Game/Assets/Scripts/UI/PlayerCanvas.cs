using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PlayerCanvas : NetworkBehaviour
{
    /// <summary>
    /// the canvas on the player model.
    /// </summary>
    public GameObject PlayerCanvasObject;
    /// <summary>
    /// the weapon loadout of the player.
    /// </summary>
    private Inventory inventory;

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        PlayerCanvasObject.SetActive(true);
    }

    /// <summary>
    /// updates the current inventory text and icon to the currently wielded weapon
    /// </summary>
    /// <param name="newWeapon"></param>
    public void UpdateWeaponUI(ShootGun newWeapon)
    {
        inventory.UpdateInfo(newWeapon.icon, newWeapon.gunAmmo, 0);
    }
}