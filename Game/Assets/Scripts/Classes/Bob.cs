using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* edited by: SWT-P_WS_21/22*/
public class Bob : Classes
{
    public override void SwitchWeapon(int oldWeapon, int newWeapon)
    {
        switch (oldWeapon)
        {
            case 1:
                GetComponent<Shotgun>().enabled = false;
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
                GetComponent<Shotgun>().enabled = true;
                break;
            case 2:
                GetComponent<Pistol>().enabled = true;
                break;
            default:
                break;
        }
    }
}
