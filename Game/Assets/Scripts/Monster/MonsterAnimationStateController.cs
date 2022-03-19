using UnityEngine;
using UnityEngine.AI;
using Mirror;

/* created by: SWT-P_WS_21/22 */

/// <summary>Controller for monster animations</summary>
public class MonsterAnimationStateController : NetworkBehaviour
{
    /// <summary>Monster's Animator</summary>
    [SerializeField] Animator animator;

    /// <summary>Monsters's Nav Mesh Agent</summary>
    [SerializeField] NavMeshAgent navMeshAgent;

    /// <summary>Monsters's Controller</summary>
    [SerializeField] MonsterController monsterController;

    /// <summary>
    /// Update the monster's animation parameters.
    /// </summary>
    void Update()
    {
        animator.SetFloat("Velocity Z", monsterController.GetVelocityZ());
        animator.SetFloat("Velocity X", monsterController.GetVelocityX());
        animator.SetBool("inAttack", monsterController.attack);
        animator.SetBool("isDead", monsterController.dead);
    }
}
