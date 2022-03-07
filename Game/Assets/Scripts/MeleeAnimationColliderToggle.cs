using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script toggles the melee weapon collider during animation
/// </summary>
public class MeleeAnimationColliderToggle : MonoBehaviour
{
    /// <summary>
    /// Player's melee script
    /// </summary>
    [SerializeField] Melee melee;

    /// <summary>
    /// Player's melee collider
    /// </summary>
    private CapsuleCollider collider;

    /// <summary>
    /// Gets the melee collider on start
    /// </summary>
    void Start()
    {
        collider = melee.gunMount.GetComponentInChildren<CapsuleCollider>();
        Debug.Log("Melee Collider: " + collider);
    }

    /// <summary>
    /// Turns the melee collider on
    /// </summary>
    void setColliderOn()
    {
        melee.GetCollider().enabled = true;
        Debug.Log("Melee Collider ON");
    }

    /// <summary>
    /// Turns the melee collider off
    /// </summary>
    void setColliderOff()
    {
        melee.GetCollider().enabled = false;
        Debug.Log("Melee Collider OFF");
    }
}
