using UnityEngine;
using UnityEngine.UI;

/* created by: SWT-P_SW_21/22 */

/// <summary>
/// This script Updates the Weapon-UI in the HUD.
/// </summary>
public class Inventory : MonoBehaviour
{
    /// <summary>Weapon Icon-Image</summary>
    public Image icon;

    /// <summary>Ammount of ammo in a magazine</summary>
    public Text currentAmmo;

    /// <summary>Amount of magazines per weapon</summary>
    public Text totalAmmo;

    /// <summary>
    /// Function to update the info on the Weapon UI, has to be used when the weapon is changed.
    /// When you change the weapon, update the UI.
    /// </summary>
    /// <param name="weaponIcon">The weapons icon sprite</param>
    /// <param name="newCurrentAmmo">Current amount of ammo</param>
    /// <param name="newTotalAmmo">Maximum mag size of the weapon</param>
    public void UpdateInfo(Sprite weaponIcon, int newCurrentAmmo, int newTotalAmmo)
    {
        icon.sprite = weaponIcon;
        currentAmmo.text = newCurrentAmmo.ToString();
        totalAmmo.text = newTotalAmmo.ToString();
    }
}
