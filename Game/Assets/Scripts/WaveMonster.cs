using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

/* created by: SWT-P_WS_21/22 */


/// <summary>
/// This class contains specific target finding for the WaveMonsters.
/// </summary>
public class WaveMonster : MonsterController
{
    /// <summary>A list containing all attackable Objects (no players)</summary>
    List<GameObject> buildingTargets = new List<GameObject>();

    /// <summary>Timer for target finding</summary>
    float timer;

    /// <summary>The refreshrate for target finding calculations</summary>
    float refreshRate = 2f;

    /// <summary>
    /// Initial FindBuildings.
    /// </summary>
    void Start()
    {
        if (!isServer)
            return;

        FindBuildings();
        nav = GetComponent<NavMeshAgent>();
    }

    /// <summary>
    /// Handling the target finding
    /// </summary>
    void Update()
    {
        if (!isServer)
            return;

        Vector3 velocity = nav.transform.InverseTransformDirection(nav.velocity);
        SetVelocityX(velocity.x);
        SetVelocityZ(velocity.z);

        if (!dead)
        {
            damageTaken = false;
            attack = false;

            if (hp <= 0)
                Die();

            timer += Time.deltaTime;
            atkTimer += Time.deltaTime;

            if (timer >= refreshRate)
            {
                FindBuildings();
                FindPlayers();
            }

            if (timer >= refreshRate)
                currentTarget = ChooseTarget();

            if (
                animator.GetCurrentAnimatorStateInfo(0).IsName("Zombie Attack")
                || animator.GetCurrentAnimatorStateInfo(0).IsName("Zombie Damage")
                || animator.GetCurrentAnimatorStateInfo(0).IsName("Zombie Dying")
            )
            {
                nav.isStopped = true;
            }
            else if (currentTarget != null)
            {
                if (
                    Vector3.Distance(currentTarget.transform.position, transform.position)
                    > atkRange
                )
                {
                    nav.isStopped = false;
                    nav.SetDestination(currentTarget.transform.position);
                }
                else
                {
                    nav.isStopped = true;
                    Attack();
                }
            }

            if (timer >= refreshRate)
                timer = 0f;
        }
    }

    /// <summary>
    /// Finds all GameObject with the 'DestructableObject' tag and adds them to the buildingTargets list.
    /// </summary>
    void FindBuildings()
    {
        buildingTargets = new List<GameObject>();
        GameObject[] destructables = GameObject.FindGameObjectsWithTag("DestructableObject");

        foreach (GameObject destructable in destructables)
        {
            DestructableObject script = destructable.GetComponent<DestructableObject>();
            if (script.active && script.attackAble)
                buildingTargets.Add(destructable);
        }
    }

    /// <summary>
    /// Selects a target for the monster to attack. 
    /// </summary>
    /// <returns>Returns the target as a GameObject or null if there is no possible target</returns>
    GameObject ChooseTarget()
    {
        GameObject newTarget = null;

        if (players.Count > 0)
        {
            float shortestDistance = aggroRadius;

            foreach (GameObject target in players)
            {
                float distance = Vector3.Distance(transform.position, target.transform.position);

                //if the player is reachable and the closest to the monster, the player becomes the new target
                NavMeshPath navMeshPath = new NavMeshPath();
                if (
                    distance <= shortestDistance
                    && nav.CalculatePath(target.transform.position, navMeshPath)
                    && navMeshPath.status == NavMeshPathStatus.PathComplete
                )
                {
                    shortestDistance = distance;
                    newTarget = target;
                }
            }
        }
        else if (buildingTargets.Count > 0)
        {
            float shortestDistance = Vector3.Distance(
                this.transform.position,
                buildingTargets[0].transform.position
            );

            foreach (GameObject target in buildingTargets)
            {
                float distance = Vector3.Distance(transform.position, target.transform.position);

                //if the object is reachable and the closest to the monster, the object becomes the new target
                NavMeshPath navMeshPath = new NavMeshPath();
                if (
                    distance <= shortestDistance
                    && nav.CalculatePath(target.transform.position, navMeshPath)
                    && navMeshPath.status == NavMeshPathStatus.PathComplete
                )
                {
                    shortestDistance = distance;
                    newTarget = target;
                }
            }
        }
        else
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            if (players.Length > 0)
            {
                float shortestDistance = Vector3.Distance(
                    this.transform.position,
                    players[0].transform.position
                );

                for (int i = 0; i < players.Length; i++)
                {
                    float distance = Vector3.Distance(
                        transform.position,
                        players[i].transform.position
                    );

                    //if the object is reachable and the closest to the monster, the object becomes the new target
                    NavMeshPath navMeshPath = new NavMeshPath();
                    if (
                        distance <= shortestDistance
                        && nav.CalculatePath(players[i].transform.position, navMeshPath)
                        && navMeshPath.status == NavMeshPathStatus.PathComplete
                    )
                    {
                        shortestDistance = distance;
                        newTarget = players[i];
                    }
                }
            }
        }

        return newTarget;
    }


}
