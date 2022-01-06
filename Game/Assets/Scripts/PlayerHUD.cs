using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private WeaponUI weaponUI;

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    public void SetHealth(int health)
    {
        slider.value = health;
    }

    // public void UpadteWeaponUI(ShootGun newShootgun)
    // {
    //     weaponUI.UpdateInfo(newShootgun.icon, newShootgunn.magazineSize, newShootgun.magazineCount);
    // }
}
