using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class MonsterController : NetworkBehaviour
{
    public float aggroRadius;
    public float damage;
    public float hp;
    public float moveSpeed;
    public bool awake;  //this is true when the monster gets aggro
    public float atkRange;
    public float atkCooldown;

    public List<GameObject> players;
    public GameObject target;

    float timer;
    float atkTimer;
    float refreshRate = 1f; //the refreshRate for target finding calculations
    bool dead;
    Vector3 home;   //stores the spawn spot
    NavMeshAgent nav;

    

    private void Start()
    {
        home = transform.position;
        if(isServer)
        {
            nav = GetComponent<NavMeshAgent>();
            FindPlayers();
        } 
    }


    void Update()
    {
        //Since the ai is only handled by the server, nobody else needs to run this code
        if(!isServer) return;

        timer += Time.deltaTime;
        atkTimer += Time.deltaTime;

        if(!dead){

            if(hp <= 0) Die();

            //Movement
            if(timer >= refreshRate) FindPlayers();
            if(!awake && timer >= refreshRate) awake = CheckAggro();
            if(awake && timer >= refreshRate) target = FindTarget();
            if(target != null)
            {
                if(Vector3.Distance(target.transform.position, transform.position) > atkRange)
                {
                    nav.isStopped = false;
                    nav.SetDestination(target.transform.position);
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

            if(timer >= refreshRate) timer = 0f;
        }
    }

    //Makes a list containing all active players.
    void FindPlayers()
    {
        //Make a new list
        players = new List<GameObject>();

        //Find all players
        GameObject[] activePlayers = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in activePlayers)
        {
            players.Add(player);
        }
    }

    //Checks if a player is within aggro range
    bool CheckAggro()
    {
        //Check if a player is in the aggro Radius
        foreach (GameObject player in players)
        {
            if(Vector3.Distance(transform.position, player.transform.position) <= aggroRadius) return true;
        }
        return false;
    }

    //Finds the nearest Target
    GameObject FindTarget()
    {
        float shortestDistance = 0f;
        GameObject newTarget = null;

        foreach (GameObject player in players)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);

            if(shortestDistance == 0) shortestDistance = distance;

            //if the player is reachable and the closest to the monster, the player becomes the new target
            NavMeshPath navMeshPath = new NavMeshPath();
            if(distance <= shortestDistance && nav.CalculatePath(player.transform.position, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
            {
                shortestDistance = distance;
                newTarget = player;
            } 
        }

        return newTarget;
    }

    void Die()
    {
        if(!dead)
        {
            dead = true;
            //TODO: Death-Animation
            Destroy(this.gameObject, 0.5f); // Destroys the Monster after 0.5f
        }  
    }

    void Attack()
    {
        if(atkTimer >= atkCooldown)
        {
            atkTimer = 0;
            //TODO: Attack animation
            //TODO: Call player script and damage him
        }
    }


    //this can be used to manualy trigger monsters.
    [Command]
    void AggroMob(GameObject player)
    {
        if(target == null)
        {
            awake = true;
            target = player;
        }
    }

    //can be called if a player damages the monster.
    [Command]
    void TakeDamage(float dmgTaken)
    {
        if(!dead)
        {
            //TODO: Play damage animation & sound
            hp -= dmgTaken;
            if(hp <= 0) Die();
        }
    }

}
