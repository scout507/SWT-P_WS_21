using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/* created by: SWT-P_WS_21/22 */

/// <summary>
/// This class contains spefic target detection for Roaming Monsters. Inherits from MonsterController.
/// </summary>
public class RoamingMonster : MonsterController
{
    /// <summary>The rate for detection checks in seconds</summary>
    public float detectionRate;

    /// <summary>Time it takes for the monster do de-aggro</summary>
    public float deAggroRate;

    /// <summary>Wether or not the monster sees a player</summary>
    public bool detectedPlayer;

    [Range(0, 360)]
    /// <summary>Angle in witch the monster can 'see'</summary>
    public float detectionAngle;

    /// <summary>List of all posible Targets</summary>
    List<GameObject> targets = new List<GameObject>();

    /// <summary>Timer for detection</summary>
    float detectionTimer;

    /// <summary>Timer for de-agrro</summary>
    float deAggroTimer;

    /// <summary>Timer for patrolling</summary>
    float patrolTimer;

    /// <summary>Time for the next patrol-step</summary>
    float nextPatrolTime = 0;

    /// <summary>The destination for the next patrol-step</summary>
    Vector3 patrolTarget;

    /// <summary>
    /// Gets Components.
    /// </summary>
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        patrolTarget = transform.position;
        zombieAudioController = this.GetComponent<ZombieAudioController>();
    }

    /// <summary>
    /// Responsible for the ai behaviour.
    /// </summary>
    void Update()
    {
        if (!isServer || dead)
            return;

        Vector3 velocity = nav.transform.InverseTransformDirection(nav.velocity);
        SetVelocityX(velocity.x);
        SetVelocityZ(velocity.z);

        detectionTimer += Time.deltaTime;
        atkTimer += Time.deltaTime;

        damageTaken = false;
        attack = false;

        if (detectionTimer >= detectionRate && !aggro)
        {
            FindPlayers();
            DetectPlayers();
            detectionTimer = 0;
        }

        if (detectedPlayer && !aggro)
        {
            nav.isStopped = true;
        }

        if (
            animator.GetCurrentAnimatorStateInfo(0).IsName("Zombie Attack")
            || animator.GetCurrentAnimatorStateInfo(0).IsName("Zombie Damage")
            || animator.GetCurrentAnimatorStateInfo(0).IsName("Zombie Dying")
        )
        {
            nav.isStopped = true;
        }
        else
        {
            if (aggro)
            {
                deAggroTimer += Time.deltaTime;
                if (deAggroTimer >= deAggroRate)
                {
                    aggro = false;
                    deAggroTimer = 0;
                }

                currentTarget = ChooseTarget();

                if (currentTarget != null)
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
            }
            else
            {
                currentTarget = null;
            }

            if (!aggro && !detectedPlayer)
            {
                patrolTimer += Time.deltaTime;
                nav.isStopped = false;

                if (patrolTimer >= nextPatrolTime)
                {
                    if (
                        Vector3.Distance(
                            new Vector3(transform.position.x, 0, transform.position.z),
                            new Vector3(patrolTarget.x, 0, patrolTarget.z)
                        ) <= 0.5f
                    )
                        patrolTarget = GetPatrollTarget();
                    nav.SetDestination(patrolTarget);
                    nextPatrolTime = Random.Range(0, 10);
                    patrolTimer = 0;
                }
            }
        }
    }

    /// <summary>
    /// This method is responsible for checking if players are within eyesight of the monster.
    /// It checks all players within aggro range, and adds them to targets if they are visible.
    /// </summary>
    private void DetectPlayers()
    {
        targets.Clear();

        if (players.Count > 0)
        {
            foreach (GameObject player in players)
            {
                Vector3 directionToTarget =
                    (player.transform.position - transform.position).normalized;
                RaycastHit hit;
                if (Vector3.Angle(transform.forward, directionToTarget) < detectionAngle / 2)
                {
                    if (
                        Physics.Raycast(transform.position, directionToTarget, out hit, aggroRadius)
                    )
                    {
                        if (hit.collider.gameObject.tag == "Player")
                        {
                            targets.Add(hit.collider.gameObject);
                        }
                    }
                }
            }
        }
        else
        {
            aggro = false;
            detectedPlayer = false;
        }

        if (targets.Count > 0)
        {
            //TODO: Add some sound/animation to show that the monster is seeing the player.
            if (detectedPlayer)
                aggro = true;
            else
                detectedPlayer = true;
        }
        else
        {
            detectedPlayer = false;
        }
    }

    /// <summary>
    /// Finds the closest player (that is reachable) within the possible targets.
    /// </summary>
    /// <returns>The target (player) GameObject</returns>
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
                    && target.tag != "Untagged"
                )
                {
                    shortestDistance = distance;
                    newTarget = target;
                }
            }
        }
        return newTarget;
    }

    /// <summary>
    /// Randomly chooses a new Vector3 coordinate for the AI to patrol towards. Only returns reachable targets.
    /// </summary>
    /// <returns>The new target as Vector3</returns>
    Vector3 GetPatrollTarget()
    {
        Vector3 pTarget = new Vector3(
            transform.position.x + Random.Range(-10, 11),
            transform.position.y,
            transform.position.z + Random.Range(-10, 11)
        );
        NavMeshPath navMeshPath = new NavMeshPath();

        if (
            nav.CalculatePath(pTarget, navMeshPath)
            && navMeshPath.status == NavMeshPathStatus.PathComplete
        )
            return pTarget;
        else
            return transform.position;
    }
}