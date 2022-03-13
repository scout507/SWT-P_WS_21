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

    [SerializeField] Classes playerClass;

    /// <summary>
    /// Player's Rig
    /// </summary>
    [SerializeField] Rig rig;

    void Start()
    {
        animator.SetBool("isKnut", playerClass.hasMelee);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = playerMovement.GetMoveRelative();
        animator.SetFloat("Velocity Z", velocity.z);
        animator.SetFloat("Velocity X", velocity.x);
        animator.SetFloat("Pitch", playerMovement.GetPitch());

        int selectedWeapon = playerClass.GetSelectedWeapon();
        animator.SetInteger("selectedWeapon", selectedWeapon);

        animator.SetBool("isCrouching", playerMovement.GetIsCrouching());
        animator.SetBool("isProne", playerMovement.GetIsProne());
        animator.SetBool("Jump", Input.GetButtonDown("Jump"));
        animator.SetBool("onGround", playerMovement.CheckGrounded());
        animator.SetBool("isKnut", playerClass.hasMelee);

        if (playerClass.hasMelee)
        {
            animator.SetBool("MeleeInAttack", GetComponent<Melee>().inAttack);
        }
        else
        {
            animator.SetBool("MeleeInAttack", false);
        }

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
            rig.weight = .8f;
        }
    }
}
