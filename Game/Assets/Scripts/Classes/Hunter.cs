using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* edited by: SWT-P_WS_21/22*/
/// <summary>
/// Hunter is a class with a Rifle and a pistol and can zoom in with rifle
/// </summary>
public class Hunter : Classes
{
    /// <summary>
    /// Flag for tracking the state of zoom
    /// </summary>
    bool isInZoom = false;

    /// <summary>
    /// Update checks input if player wants to change weapon or zoom in with rifle
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
        if (Input.GetButtonDown("Fire2") && selectedWeapon == 1)
        {
            if (isInZoom)
            {
                Camera.main.fieldOfView = 60f;
                isInZoom = false;
            }
            else
            {
                Camera.main.fieldOfView = 20f;
                isInZoom = true;
            }
        }
    }
    public override int GetSelectedWeapon()
    {
        return selectedWeapon;
    }
    /// <summary>
    /// Handles change of weapons through enabling and disenabling the correct scripts on the player and zooms out on weapon change
    /// </summary>
    /// <param name="oldWeapon"></param>
    /// <param name="newWeapon"></param>
    public override void SwitchWeapon(int oldWeapon, int newWeapon)
    {
        switch (oldWeapon)
        {
            case 1:
                GetComponent<Rifle>().enabled = false;
                if (isInZoom)
                {
                    Camera.main.fieldOfView = 60f;
                    isInZoom = false;
                }
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

    /// <summary>
    /// Hunter does not have melee weapon
    /// </summary>
    public override void SetHasMelee()
    {
        this.hasMelee = false;
    }
}
