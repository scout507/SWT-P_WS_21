using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PlayerCanvas : NetworkBehaviour
{
    public GameObject PlayerCanvasObject;
    private Inventory inventory;
    // Start is called before the first frame update
    void Start()
    {
        if (isLocalPlayer)
        {
            PlayerCanvasObject.SetActive(true);
        }
    }

    /// <summary>
    /// Updates the current inventory text and icon to the currently wielded weapon
    /// </summary>
    /// <param name="newWeapon"></param>
    public void UpdateWeaponUI(ShootGun newWeapon)
    {
        inventory.UpdateInfo(newWeapon.icon, newWeapon.gunAmmo, 0);
    }
}