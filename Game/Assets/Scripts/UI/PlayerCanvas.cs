/* created by: SWT-P_WS_21/22 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

/// <summary>
/// This class handles the UI of each player.
/// It stops multiple UI´s from overlapping, by setting specific objects active.
/// </summary>
public class PlayerCanvas : NetworkBehaviour
{
    /// <summary>
    /// the canvas on the player model.
    /// </summary>
    public GameObject PlayerCanvasObject;

    /// <summary>
    /// the camera for the minimap.
    /// </summary>
    public GameObject MinimapCam;

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
        MinimapCam.SetActive(true);
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
