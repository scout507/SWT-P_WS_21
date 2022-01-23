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

    NavMeshAgent nav;




    // Start is called before the first frame update
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
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

                Debug.Log(directionToTarget);
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

}

