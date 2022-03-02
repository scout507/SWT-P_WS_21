using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Mirror;

/* created by: SWT-P_WS_21/22 */


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

    /// <summary>
    /// Placeholder for player's melee weapon, this should be replaced with a guns script / manager
    /// </summary>
    [SerializeField] Melee melee;

    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = playerMovement.GetMoveRelative();
        animator.SetFloat("Velocity Z", velocity.z);
        animator.SetFloat("Velocity X", velocity.x);
        animator.SetFloat("Pitch", playerMovement.GetPitch());

        int selectedWeapon = playerMovement.GetSelectedWeapon();
        animator.SetInteger("selectedWeapon", selectedWeapon);

        animator.SetBool("isCrouching", playerMovement.GetIsCrouching());
        animator.SetBool("isProne", playerMovement.GetIsProne());
        animator.SetBool("Jump", Input.GetButtonDown("Jump"));
        animator.SetBool("onGround", playerMovement.CheckGrounded());
        animator.SetBool("MeleeInAttack", melee.inAttack);

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
            rig.weight = selectedWeapon == 4 ? 1 : 0;
        }
    }
}
