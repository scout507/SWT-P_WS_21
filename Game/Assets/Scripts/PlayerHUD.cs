using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror; 

/* created by: SWT-P_WS_21/22 */


public class PlayerHUD : NetworkBehaviour
{
    private Slider slider;
    private WeaponUI weaponUI;

    /// <summary>
    /// Set the maximum value of the healthbar to scale appropriatly.
    /// </summary>
    /// <param name="health"></param>
    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    /// <summary>
    /// Set the current health value to scale down the filling of the healthbar.
    /// </summary>
    /// <param name="health"></param>
    public void SetHealth(int health)
    {
        slider.value = health;
    }

    /// Has to be implemented later, when connecting the system to the weapons.
    /// // public void UpadteWeaponUI(ShootGun newShootgun)
    // {
    //     weaponUI.UpdateInfo(newShootgun.icon, newShootgunn.magazineSize, newShootgun.magazineCount);
    // }
}
