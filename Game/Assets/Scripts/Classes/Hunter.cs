using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* created by: SWT-P_WS_21/22*/

/// <summary>
/// Hunter is a class with a Rifle and a pistol and can zoom in with rifle.
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
        if (GetComponent<PlayerMovement>().active)
        {
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
    }

    /// <summary>
    /// Deactivates the script of the old weapon and activates the script of the new weapon.
    /// </summary>
    /// <param name="oldWeapon">Index of old weapon.</param>
    /// <param name="newWeapon">Index of new weapon.</param>
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
    /// Hunter does not have melee weapon, so the hasMelee flag is set false.
    /// </summary>
    public override void SetHasMelee()
    {
        this.hasMelee = false;
    }
}
