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
        animator.SetFloat("Velocity Z", monsterController.GetVelocityZ());
        animator.SetFloat("Velocity X",  monsterController.GetVelocityX());
        animator.SetBool("inAttack", monsterController.attack);
        animator.SetBool("isDead", monsterController.dead);
    }
}
