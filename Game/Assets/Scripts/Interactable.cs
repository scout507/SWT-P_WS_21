using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/* created by: SWT-P_WS_21/22 */

/// <summary>
/// Prototype script for interactable objects
/// </summary>
public class Interactable : NetworkBehaviour
{
    /// <summary>
    /// Initialization of interactable object, should be called with base.Start() in derived class
    /// </summary>
    public virtual void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Interactable");
    }

    /// <summary>
    /// Interaction handler to be overridden by derived class
    /// </summary>
    public virtual void OnInteract()
    {
        Debug.Log("OnInteract() not implemented for " + gameObject.name);
    }
}
