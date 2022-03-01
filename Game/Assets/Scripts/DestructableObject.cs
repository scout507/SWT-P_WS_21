/* created by: SWT-P_SW_21/22 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/* created by: SWT-P_WS_21/22 */


/// <summary>
/// Prototype script for destructable objects. 
/// </summary>
public class DestructableObject : NetworkBehaviour
{
    /// <summary>Maximum Health of the object</summary>
    [SerializeField] public float maxHealth;
    /// <summary>Current health of the object</summary>
    [SyncVar] public float health;
    /// <summary>True when hp are above 0</summary>
    [SyncVar] public bool active;

    /// <summary>
    /// Used for checking if health drops below 0.
    /// </summary>
    private void Update()
    {
        if (health <= 0)
        {
            active = false;
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
