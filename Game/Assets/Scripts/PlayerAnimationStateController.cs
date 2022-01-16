/* created by: SWT-P_SW_21/22*/ 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Mirror;

public class PlayerAnimationStateController : NetworkBehaviour
{
    /// <summary>
    /// Player's Animator
    /// </summary>
    [SerializeField] Animator animator;

    /// <summary>
    /// Player's Movement Script
    /// </summary>
    [SerializeField] PlayerMovement playerMovement;

    /// <summary>
    /// Player's Rig
    /// </summary>
    [SerializeField] Rig rig;

    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = playerMovement.GetMoveRelative();
        animator.SetFloat("Velocity Z", velocity.z);
        animator.SetFloat("Velocity X", velocity.x);
        animator.SetFloat("Pitch", playerMovement.GetPitch());
        animator.SetBool("isCrouching", playerMovement.GetIsCrouching());
        animator.SetBool("isProne", playerMovement.GetIsProne());
        animator.SetBool("Jump", Input.GetButtonDown("Jump"));
        animator.SetBool("onGround", playerMovement.CheckGrounded());

        int taunt = playerMovement.GetCurrentTaunt();
        animator.SetInteger("Taunt", taunt);
        if (taunt > 0)
        {
            animator.SetLayerWeight(2, 1);
            rig.weight = 0;
        }
        else 
        {
            animator.SetLayerWeight(2, 0);
            rig.weight = 1;
        }
    }
}
