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
    /// Player's Movement Script
    /// </summary>
    [SerializeField] NavMeshAgent navMeshAgent;

    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = navMeshAgent.transform.InverseTransformDirection(navMeshAgent.velocity);
        animator.SetFloat("Velocity Z", velocity.z);
        animator.SetFloat("Velocity X", velocity.x);
    }
}
