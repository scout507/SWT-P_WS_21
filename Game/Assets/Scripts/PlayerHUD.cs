using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private WeaponUI weaponUI;

    // Set the maximum value of the healthbar to scale appropriatly.
    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    // Set the current health value to scale down the filling of the healthbar.
    public void SetHealth(int health)
    {
        slider.value = health;
    }

    // public void UpadteWeaponUI(ShootGun newShootgun)
    // {
    //     weaponUI.UpdateInfo(newShootgun.icon, newShootgunn.magazineSize, newShootgun.magazineCount);
    // }
}
