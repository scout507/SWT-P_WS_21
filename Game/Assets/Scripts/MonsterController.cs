using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;


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

    /// <summary>Range for a melee Attack</summary>
    public float atkRange;
    /// <summary>Cooldown between Attacks</summary>
    public float atkCooldown;
    /// <summary>Wether or not this monster is dead</summary>
    public bool dead;

    /// <summary>A list containing all possible targets</summary>
    public List<GameObject> players;
    /// <summary>The current Target the monster is focused on</summary>
    public GameObject currentTarget;

    /// <summary>Timer for attacking</summary>
    public float atkTimer;
    /// <summary>NavMeshAgent for navigation</summary>
    NavMeshAgent navAgent;



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
            //TODO: Death-Animation
            Destroy(this.gameObject, 0.5f); // Destroys the Monster after 0.5f
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
            atkTimer = 0;
            //TODO: Attack animation
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
            //TODO: Play damage animation & sound
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
