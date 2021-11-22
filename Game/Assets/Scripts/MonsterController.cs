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
    public bool awake; //this is true when the monster gets aggro
    public float atkRange;
    public float atkCooldown;

    public List<GameObject> players;
    public GameObject target;

    float timer;
    float atkTimer;
    float refreshRate = 1f;
    Vector3 home;
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

    bool CheckAggro()
    {
        //Check if a player is in the aggro Radius
        foreach (GameObject player in players)
        {
            if(Vector3.Distance(transform.position, player.transform.position) <= aggroRadius) return true;
        }
        return false;
    }

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


    void Attack()
    {
        if(atkTimer >= atkCooldown)
        {
            atkTimer = 0;
            //TODO: Attack animation
            //TODO: Call player script and damage him
        }
    }

}
