using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PlayerCanvas : NetworkBehaviour
{
    public GameObject PlayerCanvasObject;
    private Inventory inventory;
    void Start()
    {
        if(isLocalPlayer){
            PlayerCanvasObject.SetActive(true);
        }
    }

    public void UpdateWeaponUI(ShootGun newWeapon)
    {
        inventory.UpdateInfo(newWeapon.icon,newWeapon.gunAmmo, 0);
    }
}