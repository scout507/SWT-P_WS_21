using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* edited by: SWT-P_WS_21/22*/
public class Hunter : Classes
{
    public override void SwitchWeapon(int oldWeapon, int newWeapon)
    {
        switch (oldWeapon)
        {
            case 1:
                GetComponent<Rifle>().enabled = false;
                break;
            case 2:
                GetComponent<Pistol>().enabled = false;
                break;
            default:
                break;
        }
        switch (newWeapon)
        {
            case 1:
                GetComponent<Rifle>().enabled = true;
                break;
            case 2:
                GetComponent<Pistol>().enabled = true;
                break;
            default:
                break;
        }
    }
}
