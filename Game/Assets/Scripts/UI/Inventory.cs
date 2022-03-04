/* created by: SWT-P_SW_21/22 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Inventory : MonoBehaviour
{
    /// <summary>
    /// weapon Icon-Image
    /// </summary>
    public Image icon;
    /// <summary>
    /// ammount of ammo in a magazine
    /// </summary>
    public Text currentAmmo;
    /// <summary>
    /// amount of magazines per weapon
    /// </summary>
    public Text totalAmmo;

    /// <summary>
    /// Function to update the info on the Weapon UI, has to be used when the weapon is changed.
    /// When you change the weapon, update the UI.
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
