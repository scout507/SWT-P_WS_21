/* created by: SWT-P_SW_21/22 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Inventory : NetworkBehaviour
{
    /// <summary>
    /// Weapon Icon-Image
    /// </summary>
    public Image icon;
    /// <summary>
    /// ammount of ammo in a magazine
    /// </summary>
    public Text currentAmmo;
    /// <summary>
    /// amoutn of magazines per weapon
    /// </summary>
    public Text totalAmmo;

    /// <summary>
    /// Function to update the info on the Weapon UI, has to be used when the weapon is changed.
    /// when you change the weapon, Update the UI:
    /// Update weapon:
    /// Reference to Weapon UI:
    /// Player HUD script:
    /// </summary>
    /// <param name="weaponIcon"></param>
    /// <param name="magazineSize"></param>
    /// <param name="magazineCount"></param>
    public void UpdateInfo(Sprite weaponIcon, int newCurrentAmmo, int newTotalAmmo)
    {
        icon.sprite = weaponIcon;
        currentAmmo.text = newCurrentAmmo.ToString();
        totalAmmo.text = newTotalAmmo.ToString();     
    }
}
