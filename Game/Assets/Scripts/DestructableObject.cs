using UnityEngine;
using Mirror;

/* created by: SWT-P_WS_21/22 */


/// <summary>
/// This class is used to make non player objects attackable for the zombies. If you want to use this, 
/// make sure to set the 'DestructableObject' tag on the object holding this script. 
/// </summary>
public class DestructableObject : NetworkBehaviour
{
    /// <summary>Maximum Health of the object</summary>
    [SerializeField] public float maxHealth;
    /// <summary>Current health of the object</summary>
    [SyncVar] public float health;
    /// <summary>True when hp are above 0</summary>
    [SyncVar] public bool active;
    /// <summary>True when this object is attackable by zombies</summary>
    [SyncVar] public bool attackAble = true;

    /// <summary>
    /// Used for checking if health drops below 0.
    /// </summary>
    private void Update()
    {
        if (health <= 0)
        {
            active = false;
            health = 0;
        }
        else
            active = true;
    }

    /// <summary>
    /// Can be called to damage the object.
    /// </summary>
    /// <param name="amount">Amount of damage taken.</param>
    public void TakeDamage(float amount)
    {
        health -= amount;
    }

    /// <summary>
    /// Can be called to update the health, when constructing an object.
    /// </summary>
    /// <param name="percent">The building progress percentage</param>
    public void SetHealth(float percent)
    {
        health = maxHealth * percent;
    }
}
