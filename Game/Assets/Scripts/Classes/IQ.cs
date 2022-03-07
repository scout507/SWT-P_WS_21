using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* edited by: SWT-P_WS_21/22*/
public class IQ : Classes
{   
    void Update()
    {
        if(!isLocalPlayer) return;
        if (Input.GetAxis("Mouse ScrollWheel") < 0f && selectedWeapon < 3)
        {
            newWeapon = selectedWeapon + 1;
            CmdSwitchWeapon(newWeapon);
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0f && selectedWeapon > 1)
        {
            newWeapon = selectedWeapon - 1;
            CmdSwitchWeapon(newWeapon);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CmdSwitchWeapon(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CmdSwitchWeapon(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            CmdSwitchWeapon(3);
        }
    }

    public override void SwitchWeapon(int oldWeapon, int newWeapon)
    {
        switch (oldWeapon)
        {
            case 1:
                GetComponent<MP>().enabled = false;
                break;
            case 2:
                GetComponent<Pistol>().enabled = false;
                break;
            case 3:
                GetComponent<IQCam>().enabled = false;
                break;
            default:
                break;
        }
        switch (newWeapon)
        {
            case 1:
                GetComponent<MP>().enabled = true;
                break;
            case 2:
                GetComponent<Pistol>().enabled = true;
                break;
            case 3:
                GetComponent<IQCam>().enabled = true;
                break;
            default:
                break;
        }
    }
    public override void SetHasMelee()
    {
        this.hasMelee = false;
    }
}
