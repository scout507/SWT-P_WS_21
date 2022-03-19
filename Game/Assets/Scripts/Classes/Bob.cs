using UnityEngine;

/* created by: SWT-P_WS_21/22*/

/// <summary>
/// Bob is a class with a shotgun and a pistol.
/// </summary>
public class Bob : Classes
{
    /// <summary>
    /// Update checks input if player wants to change weapon
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
    /// Bob does not have melee weapon, so the hasMelee flag is set false.
    /// </summary>
    public override void SetHasMelee()
    {
        this.hasMelee = false;
    }
}
