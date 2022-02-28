using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;
using Mirror;


/// <summary>
/// This class is responsible for handling monsters. Most code should only be run on server.
/// </summary>
public class MonsterController : NetworkBehaviour
{
    public float aggroRadius;
    public float damage;
    public float hp;
    public float moveSpeed;

    [SyncVar] float velocityX;
    [SyncVar] float velocityZ;

    /// <summary>True when the monster aggro is triggered </summary>
    public bool awake;
    public float atkRange;
    public float atkCooldown;

    /// <summary>This is used to either prefer players (<1) or destructable objects (>1)</summary>
    [Range(0.1f, 2)]
    public float playerToObjectRatio;

    /// <summary>A list containing all possible targets</summary>
    public List<GameObject> targets;
    public GameObject currentTarget;

    float timer;
    float atkTimer;
    public bool attack;

    /// <summary>The refreshrate for target finding calculations</summary>
    float refreshRate = 1f;
    [SyncVar] public bool dead;
    public bool damageTaken;

    /// <summary>Stores the spawn spot</summary>
    Vector3 home;
    NavMeshAgent nav;

    /// <summary>Used to ground the monster</summary>
    [Header("Grounding")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundDistance = 0.4f;
    [SerializeField] LayerMask groundMask;

    /// <summary>The monster's animator</summary>
    [SerializeField] Animator animator;

    /// <summary>
    /// Monster's network Animator
    /// </summary>
    [SerializeField] NetworkAnimator networkAnimator;

     /// <summary>Player's melee collider</summary>
    [SerializeField] CapsuleCollider collider;

    /// <summary>
    /// Checks if the monster is currently on ground
    /// </summary>
    /// <returns>Returns true or false</returns>
    public bool CheckGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    /// <summary>
    /// Set monster status to dead
    /// </summary>
    [Command]
    void SetDead(bool newDead)
    {
        dead = newDead;
    }
    
    public float GetVelocityX() {
        return velocityX;
    }

    public float GetVelocityZ() {
        return velocityZ;
    }

    /// <summary>
    /// Set monsters's velocity
    /// </summary>
    [Command]
    void SetVelocityX(float newVelocityX)
    {
        //Vector3 velocity = nav.transform.InverseTransformDirection(nav.velocity);
        velocityX = newVelocityX;
        //Debug.Log("Set Velocity X: " + velocityX);
    }

    /// <summary>
    /// Set monsters's velocity
    /// </summary>
    [Command]
    void SetVelocityZ(float newVelocityZ)
    {
        //Vector3 velocity = nav.transform.InverseTransformDirection(nav.velocity);

        velocityZ = newVelocityZ;
        //Debug.Log("Set Velocity Z: " + velocityZ);
    }

    private void Start()
    {
        home = transform.position;
        if (isServer)
        {
            nav = GetComponent<NavMeshAgent>();
            FindTargets();
        }
    }


    void Update()
    {
        //Since the ai is only handled by the server, nobody else needs to run this code
        if (!isServer) return;
        Debug.Log("Monster Update!");
        Vector3 velocity = nav.transform.InverseTransformDirection(nav.velocity);
        velocityX = velocity.x;
        velocityZ = velocity.z;
        timer += Time.deltaTime;
        atkTimer += Time.deltaTime;

        if (!dead)
        {
            damageTaken = false;
            attack = false;

            if (hp <= 0) Die();

            //Movement
            if (timer >= refreshRate) FindTargets();
            if (!awake && timer >= refreshRate) awake = CheckAggro();
            if (awake && timer >= refreshRate) currentTarget = FindTarget();
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Zombie Attack") || 
                animator.GetCurrentAnimatorStateInfo(0).IsName("Zombie Damage") ||  
                animator.GetCurrentAnimatorStateInfo(0).IsName("Zombie Dying")) 
            {
                nav.isStopped = true;
            }
            else {
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
                else
                {
                    //if there's no legal target, the monster de-aggros and returns to it's spawn position.
                    awake = false;
                    nav.SetDestination(home);
                }
            }

            if (timer >= refreshRate) timer = 0f;
        }
        else {
            nav.isStopped = true;
        }
    }

    /// <summary>
    ///     Makes a list containing all active players and destructable objects.
    /// </summary>
    void FindTargets()
    {
        //Make a new list
        targets = new List<GameObject>();

        //Find all players and objects
        GameObject[] activePlayers = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] destructables = GameObject.FindGameObjectsWithTag("DestructableObject");


        //If the player or object is within the aggro radius, it gets added to the list
        foreach (GameObject player in activePlayers)
        {
            if (Vector3.Distance(transform.position, player.transform.position) <= aggroRadius) targets.Add(player);
        }
        foreach (GameObject destructable in destructables)
        {
            if (Vector3.Distance(transform.position, destructable.transform.position) <= aggroRadius && destructable.GetComponent<DestructableObject>().active) targets.Add(destructable);
        }
    }

    /// <summary>
    /// This has changed from the previous version. It might be obsolete, but could still be usefull in the future if the aggro behaviour is going to change.
    /// </summary>
    /// <returns>Returns true when there are players or objects in aggro range</returns>
    bool CheckAggro()
    {
        return targets.Count > 0;
    }

    /// <summary>
    /// Selects a target for the monster to attack. 
    /// </summary>
    /// <returns>Returns the target as a GameObject.</returns>
    GameObject FindTarget()
    {
        float shortestDistance = aggroRadius;
        GameObject newTarget = null;

        foreach (GameObject target in targets)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);

            if (target.tag == "Player")
            {
                distance = distance * playerToObjectRatio;
            }

            //if the player/object is reachable and the closest to the monster, the player/object becomes the new target
            NavMeshPath navMeshPath = new NavMeshPath();
            if (distance <= shortestDistance && nav.CalculatePath(target.transform.position, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
            {
                shortestDistance = distance;
                newTarget = target;
            }
        }

        return newTarget;
    }

    /// <summary>
    /// Method to handle monster death.
    /// </summary>
    void Die()
    {
        if (!dead)
        {
            dead = true;
            collider.enabled = false;
            Destroy(this.gameObject, 300f); // Destroys the Monster after 5 minutes
        }
    }

    /// <summary>
    /// Method responsible for attacking.
    /// </summary>
    void Attack()
    {
        if (atkTimer >= atkCooldown)
        {
            attack = true;
            atkTimer = 0;
            if (currentTarget.tag == "Player")
            {
                currentTarget.GetComponent<Health>().TakeDamage(Mathf.RoundToInt(damage)); //Health script uses int for health. Needs to be resolved
            }
            else if (currentTarget.tag == "DestructableObject")
            {
                currentTarget.GetComponent<DestructableObject>().TakeDamage(damage);
            }
        }
    }


    /// <summary>
    /// Can be called to damage the monster.
    /// </summary>
    /// <param name="dmgTaken">The amount of damage the monster is going to take.</param>
    public void TakeDamage(float dmgTaken)
    {
        if (!dead)
        {
            animator.SetTrigger("hasTakenDamage");
            networkAnimator.SetTrigger("hasTakenDamage");

            hp -= dmgTaken;
            if (hp <= 0) Die();
        }
    }


    /// <summary>
    /// Not useable yet. This method is going to be used for triggering monsters manually.
    /// </summary>
    /// <param name="player">The player that triggered the monster.</param>
    void AggroMob(GameObject player)
    {
        if (currentTarget == null)
        {
            awake = true;
            currentTarget = player;
        }
    }

    /// <summary>
    /// This is called when a melee weapon hits the monster.
    /// </summary>
    /// <param name="other">The collider of the gameobject which hit this gameobject.</param>
    private void OnTriggerEnter(Collider other)
    {
        other.transform.root.GetComponent<Melee>().meleeHit(gameObject);
    }
}
