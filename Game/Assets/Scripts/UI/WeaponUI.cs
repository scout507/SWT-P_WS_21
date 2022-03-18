/* created by: SWT-P_WS_21/22 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

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
    /// amount of magazines per weapon
    /// </summary>
    private Text magazineCountText;

    /// <summary>
    /// Function to update the info on the Weapon UI, has to be used when the weapon is changed.
    /// </summary>
    /// Weapon Icon-Image
    /// <param name="weaponIcon"></param>
    /// ammount of ammo in a magazine
    /// <param name="magazineSize"></param>
    /// amount of magazines per weapon
    /// <param name="magazineCount"></param>
    public void UpdateInfo(Sprite weaponIcon, int magazineSize, int magazineCount)
    {
        icon.sprite = weaponIcon;
        magazineSizeText.text = magazineSize.ToString();
        int magazineCountAmount = magazineSize * magazineCount;
        magazineCountText.text = magazineCountAmount.ToString();
    }
}
