using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* created by: SWT-P_WS_21/22 */

/// <summary>This script toggles the melee weapon collider during animation</summary>
public class MeleeAnimationColliderToggle : MonoBehaviour
{
    /// <summary>Player's melee script</summary>
    [SerializeField] Melee melee;

    /// <summary>Player's melee collider</summary>
    private CapsuleCollider col;

    /// <summary>Gets the melee collider on start</summary>
    void Start()
    {
        col = melee.gunMount.GetComponentInChildren<CapsuleCollider>();
    }

    /// <summary>Turns the melee collider on</summary>
    void setColliderOn()
    {
        melee.GetCollider().enabled = true;
    }

    /// <summary>Turns the melee collider off</summary>
    void setColliderOff()
    {
        melee.GetCollider().enabled = false;
    }
}
