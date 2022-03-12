using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* edited by: SWT-P_WS_21/22*/
/// <summary>
/// Bob is a class with a shotgun and a pistol
/// </summary>
public class Bob : Classes
{
    /// <summary>
    /// Update checks input if player wants to change weapon
    /// </summary>
    void Update()
    {
        if (!isLocalPlayer) return;
        if (Input.GetAxis("Mouse ScrollWheel") < 0f && selectedWeapon < 2)
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
    }
public override int GetSelectedWeapon()
    {
        return selectedWeapon;
    }
    /// <summary>
    /// Handles change of weapons through enabling and disenabling the correct scripts on the player
    /// </summary>
    /// <param name="oldWeapon"></param>
    /// <param name="newWeapon"></param>
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

    /// <summary>
    /// Bob does not have melee weapon
    /// </summary>
    public override void SetHasMelee()
    {
        this.hasMelee = false;
         Debug.Log("SET HAS MELEE");
        Debug.Log("SET HAS MELEE: " + this.hasMelee);
    }
}
