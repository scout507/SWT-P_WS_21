using Mirror;
using UnityEngine;

/* created by: SWT-P_WS_21/22*/

/// <summary>
/// Classes is the abstract class of our classes.
/// Every Playerclass inherits from this one.
/// </summary>
public abstract class Classes : NetworkBehaviour
{
    /// <summary>hasMelee is important for the animator. Set true if class has melee weapon, false if not.</summary>
    [SyncVar] public bool hasMelee;

    /// <summary>newWeapon saves the new weapon when the weapon is changed.</summary>
    public int newWeapon = 0;

    ///<summary>selectedWeapon saves the selected weapon, if it is changed the function SwitchWeapon is called.</summary>
    [SyncVar(hook = nameof(SwitchWeapon))]
    public int selectedWeapon = 1;

    /// <summary>
    /// When a player prefab is spawns, this selects the first weapon.
    /// </summary>
    private void Start()
    {
        if (!isLocalPlayer)
        {
            SwitchWeapon(selectedWeapon, selectedWeapon);
            return;
        }
        selectedWeapon = 1;
        SwitchWeapon(selectedWeapon, selectedWeapon);
        SetHasMelee();
    }



    /// <summary>
    /// Deactivates the script of the old weapon and activates the script of the new weapon.
    /// </summary>
    /// <param name="oldWeapon">Index of old weapon.</param>
    /// <param name="newWeapon">Index of new weapon.</param>
    public abstract void SwitchWeapon(int oldWeapon, int newWeapon);

    /// <summary>
    /// Returns the number of the selected weapon.
    /// </summary>
    /// <returns>selectedWeapon for animator</returns>
    public int GetSelectedWeapon()
    {
        return selectedWeapon;
    }

    /// <summary>
    /// SetHasMelee sets hasMelee in correct class
    /// </summary>
    public abstract void SetHasMelee();

    /// <summary>
    /// This is called when a melee weapon hits the monster.
    /// </summary>
    /// <param name="other">The collider of the gameobject which hit this gameobject.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.GetComponent<Melee>())
            other.transform.root.GetComponent<Melee>().meleeHit(gameObject);
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
}
