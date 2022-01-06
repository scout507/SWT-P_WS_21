using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Text magazineSizeText;
    [SerializeField] private Text magazineCountText;

    public void UpdateInfo(Sprite weaponIcon, int magazineSize, int magazineCount)
    {
        icon.sprite = weaponIcon;
        magazineSizeText.text = magazineSize.ToString();
        int magazineCountAmount = magazineSize * magazineCount;
        magazineCountText.text = magazineCountAmount.ToString();
        //when you change the weapon, Update the UI:
        //Update weapon:
        //Reference to Weapon UI:
        //Player HUD script:
    }
       
}
