using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using Mirror;

public class MonsterAnimationStateController : NetworkBehaviour
{
    /// <summary>
    /// Monster's Animator
    /// </summary>
    [SerializeField] Animator animator;

    /// <summary>
    /// Monsters's Nav Mesh Agent
    /// </summary>
    [SerializeField] NavMeshAgent navMeshAgent;
    
    /// <summary>
    /// Monsters's Controller
    /// </summary>
    [SerializeField] MonsterController monsterController;

    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = navMeshAgent.transform.InverseTransformDirection(navMeshAgent.velocity);
        animator.SetFloat("Velocity Z", velocity.z);
        animator.SetFloat("Velocity X", velocity.x);

        animator.SetBool("inAttack", monsterController.attack);
        animator.SetBool("isDead", monsterController.dead);
    }
}
