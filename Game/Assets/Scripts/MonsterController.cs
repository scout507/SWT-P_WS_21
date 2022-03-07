using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;
using Mirror;

/* created by: SWT-P_WS_21/22 */


/// <summary>
/// This class is responsible for handling monsters. Most code should only be run on server.
/// </summary>
public class MonsterController : NetworkBehaviour
{
    /// <summary>Aggro radius of the monster</summary>
    public float aggroRadius;
    /// <summary>Damage the monster does in one hit</summary>
    public float damage;
    /// <summary>The monsters Hp</summary>
    public float hp;
    /// <summary>The monsters movementspeed</summary>
    public float moveSpeed;

    /// <summary> Monster's sideways velocity </summary>
    [SyncVar] float velocityX;
    /// <summary> Monster's forwards velocity </summary>
    [SyncVar] float velocityZ;

    /// <summary>True when the monster aggro is triggered </summary>
    public bool awake;
    /// <summary>Range for a melee Attack</summary>
    public float atkRange;
    /// <summary>Cooldown between Attacks</summary>
    public float atkCooldown;
    /// <summary>Wether or not this monster is dead</summary>
    [SyncVar] public bool dead;

    /// <summary>A list containing all possible targets</summary>
    public List<GameObject> players;
    /// <summary>The current Target the monster is focused on</summary>
    public GameObject currentTarget;

    float timer;
    /// <summary>Timer for attacking</summary>
    public float atkTimer;
    [SyncVar] public bool attack;

    /// <summary>The refreshrate for target finding calculations</summary>
    float refreshRate = 1f;
    public bool damageTaken;

    /// <summary>Stores the spawn spot</summary>
    Vector3 home;
    public NavMeshAgent nav;

    /// <summary>Used to ground the monster</summary>
    [Header("Grounding")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundDistance = 0.4f;
    [SerializeField] LayerMask groundMask;

    /// <summary>The monster's animator</summary>
    [SerializeField] public Animator animator;

    /// <summary>
    /// Monster's network Animator
    /// </summary>
    [SerializeField] NetworkAnimator networkAnimator;

    /// <summary>Player's melee collider</summary>
    [SerializeField] CapsuleCollider collider;

    /// <summary>NavMeshAgent for navigation</summary>
    NavMeshAgent navAgent;

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

    /// <summary>
    /// Get the monster's sideways velocity
    /// </summary>
    public float GetVelocityX()
    {
        return velocityX;
    }

    /// <summary>
    /// Set the monster's sideways velocity variable
    /// </summary>    
    public void SetVelocityX(float velocityX)
    {
        this.velocityX = velocityX;
    }

    /// <summary>
    /// Get the monster's forwards velocity
    /// </summary>
    public float GetVelocityZ()
    {
        return velocityZ;
    }

    /// <summary>
    /// Set the monster's forwards velocity variable
    /// </summary>    
    public void SetVelocityZ(float velocityZ)
    {
        this.velocityZ = velocityZ;
    }

    /// <summary>
    ///  Makes a list containing all active players within aggro-radius.
    /// </summary>
    public void FindPlayers()
    {
        //Make a new list
        players = new List<GameObject>();
        navAgent = GetComponent<NavMeshAgent>();

        //Find all players and objects
        GameObject[] activePlayers = GameObject.FindGameObjectsWithTag("Player");

        //If the player or object is within the aggro radius, it gets added to the list
        foreach (GameObject player in activePlayers)
        {
            if (Vector3.Distance(transform.position, player.transform.position) <= aggroRadius)
            {
                NavMeshPath navMeshPath = new NavMeshPath();
                if (navAgent.CalculatePath(player.transform.position, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
                {
                    players.Add(player);
                }
            }
        }
    }


    /// <summary>
    /// Method to handle monster death.
    /// </summary>
    public void Die()
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
    public void Attack()
    {
        Debug.Log("ATTK");
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
    public void AggroMob(GameObject player)
    {
        if (currentTarget == null || currentTarget.tag != "Player")
        {
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
