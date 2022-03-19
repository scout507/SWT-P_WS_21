using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

/* edited by: SWT-P_WS_21/22*/
public abstract class Classes : NetworkBehaviour
{
    public bool hasMelee;
    public int newWeapon = 0;

    [SyncVar(hook = nameof(SwitchWeapon))]
    public int selectedWeapon = 0;

    /// <summary>
    /// When a player prefab is spawns, this selects the first weapon.
    /// </summary>
    private void Start()
    {
        selectedWeapon = 1;
        SwitchWeapon(selectedWeapon, selectedWeapon);
        SetHasMelee();
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

    public int GetSelectedWeapon()
    {
        return selectedWeapon;
    }

    public abstract void SetHasMelee();
}
