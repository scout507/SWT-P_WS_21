using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

/* created by: SWT-P_WS_21/22 */


public class WeaponUI : NetworkBehaviour
{
    /// <summary>
    /// Weapon Icon-Image
    /// </summary>
    private Image icon;
    /// <summary>
    /// ammount of ammo in a magazine
    /// </summary>
    private Text magazineSizeText;
    /// <summary>
    /// amoutn of magazines per weapon
    /// </summary>
    private Text magazineCountText;

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
    public void UpdateInfo(Sprite weaponIcon, int magazineSize, int magazineCount)
    {
        icon.sprite = weaponIcon;
        magazineSizeText.text = magazineSize.ToString();
        int magazineCountAmount = magazineSize * magazineCount;
        magazineCountText.text = magazineCountAmount.ToString();     
    }
}
