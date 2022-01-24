using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class RoamingMonster : MonsterController
{

    public float detectionRate;
    public float deAggroRate;
    public bool detectedPlayer;
    public bool aggro;

    [Range(0, 360)]
    public float detectionAngle;

    public List<GameObject> targets = new List<GameObject>();

    float detectionTimer;
    float deAggroTimer;
    float patrolTimer;
    float nextPatrolTime = 0;

    NavMeshAgent nav;
    Vector3 patrolTarget;



    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        patrolTarget = transform.position;
    }

    void Update()
    {
        if (!isServer) return;

        detectionTimer += Time.deltaTime;
        if (detectionTimer >= detectionRate && !aggro)
        {
            FindPlayers();
            DetectPlayers();
            detectionTimer = 0;
        }

        if(detectedPlayer && !aggro)
        {
            nav.isStopped = true;
        }

        if (aggro)
        {
            deAggroTimer += Time.deltaTime;
            if(deAggroTimer >= deAggroRate)
            {
                aggro = false;
                deAggroTimer = 0;
            }
            
            currentTarget = ChooseTarget();
            
            if (currentTarget != null)
            {
                if (Vector3.Distance(currentTarget.transform.position, transform.position) > atkRange)
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

        if(!aggro && !detectedPlayer)
        {
            patrolTimer += Time.deltaTime;
            nav.isStopped = false;

            if(patrolTimer >= nextPatrolTime)
            {
                if(Vector3.Distance(transform.position, patrolTarget) <= 0.5f) patrolTarget = GetPatrollTarget();
                nav.SetDestination(patrolTarget);
                nextPatrolTime = Random.Range(0,10);
                patrolTimer = 0;
            }
        }    
    }


    private void DetectPlayers()
    {
        targets.Clear();

        if (players.Count > 0)
        {

            foreach (GameObject player in players)
            {
                Vector3 directionToTarget = (player.transform.position - transform.position).normalized;
                RaycastHit hit;
                if (Vector3.Angle(transform.forward, directionToTarget) < detectionAngle / 2)
                {

                    if (Physics.Raycast(transform.position, directionToTarget, out hit, aggroRadius))
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
            if (detectedPlayer) aggro = true;
            else detectedPlayer = true;
        }
        else
        {
            detectedPlayer = false;
        }
    }


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
                if (distance <= shortestDistance && nav.CalculatePath(target.transform.position, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
                {
                    shortestDistance = distance;
                    newTarget = target;
                }
            }
        }
        return newTarget;
    }

    Vector3 GetPatrollTarget()
    {
        Vector3 pTarget = new Vector3(transform.position.x + Random.Range(-10,11), transform.position.y, transform.position.z + Random.Range(-10,11));
        NavMeshPath navMeshPath = new NavMeshPath();

        if(nav.CalculatePath(pTarget, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete) return pTarget;
        else return transform.position;
    }

}

