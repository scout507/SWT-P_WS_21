using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public abstract class Classes : NetworkBehaviour
{

    [SyncVar(hook = nameof(SwitchWeapon))]
    public int selectedWeapon = 0;

    /// <summary>
    /// When a player prefab is spawns, this selects the first weapon.
    /// </summary>
    private void Start()
    {
        selectedWeapon = 1;
        SwitchWeapon(selectedWeapon, selectedWeapon);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f && selectedWeapon < 2)
        {
            int newWeapon = selectedWeapon + 1;
            CmdSwitchWeapon(newWeapon);
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f && selectedWeapon > 1)
        {
            int newWeapon = selectedWeapon - 1;
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

    /// <summary>
    /// Switching weapons is handled by the server. This methode changes the index of the selected weapon to the new weapon.
    /// </summary>
    /// <param name="newWeapon">Index of new weapon which is now selected.</param>
    [Command]
    public void CmdSwitchWeapon(int newWeapon)
    {
        selectedWeapon = newWeapon;
    }

    /// <summary>
    /// Deactivates the script of the old weapon and activates the script of the new weapon.
    /// </summary>
    /// <param name="oldWeapon">Index of old weapon.</param>
    /// <param name="newWeapon">Index of new weapon.</param>
    public abstract void SwitchWeapon(int oldWeapon, int newWeapon);
}
