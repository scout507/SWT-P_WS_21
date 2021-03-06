using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


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

    /// <summary>True when the monster is near to a fence object. Used for prioritising fences over players</summary>
    bool prioritiseFence;

    /// <summary>The script of the nearby fence for choosing a plank to attack</summary>
    FenceInteraction fenceScript;

    /// <summary>The plank objects of a nearby fence</summary>
    GameObject[] planks;



    /// <summary>
    /// Initial FindBuildings.
    /// </summary>
    void Start()
    {
        if (!isServer)
            return;

        FindBuildings();
        nav = GetComponent<NavMeshAgent>();
        zombieAudioController = this.GetComponent<ZombieAudioController>();
    }

    /// <summary>
    /// Responsible for the ai behaviour. 
    /// Does target finding and player detection.
    /// Only runs on server.
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
                nav.SetDestination(transform.position);
            }
            else if (currentTarget != null)
            {
                if (
                    Vector3.Distance(currentTarget.transform.position, transform.position)
                    > atkRange
                )
                {
                    nav.SetDestination(currentTarget.transform.position);
                }
                else
                {
                    nav.SetDestination(transform.position);
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
    /// Selects a target for the monster to attack. The prioritisation is fence>player>destructable object.
    /// </summary>
    /// <returns>Returns the target as a GameObject or null if there is no possible target</returns>
    GameObject ChooseTarget()
    {
        GameObject newTarget = null;

        if (prioritiseFence && fenceScript)
        {
            foreach (GameObject plank in fenceScript.planksInspector)
            {
                if (plank.GetComponent<DestructableObject>().active) return plank;
            }

            prioritiseFence = false;
        }

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

    /// <summary>
    /// Used for checking if the monster is near a fence object.
    /// </summary>
    /// <param name="other">The collider that triggered this function</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Fence")
        {
            prioritiseFence = true;
            fenceScript = other.gameObject.GetComponent<FenceInteraction>();
        }
        //This is needed here, since the MonsterController OnTriggerEnter gets overriden
        if (other.transform.root.GetComponent<Melee>())
            other.transform.root.GetComponent<Melee>().meleeHit(gameObject);
    }

    /// <summary>
    /// Used to register the exit from a nearby fence.
    /// </summary>
    /// <param name="other">The collider that triggered this function</param>
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Fence")
        {
            prioritiseFence = false;
        }
    }


}
