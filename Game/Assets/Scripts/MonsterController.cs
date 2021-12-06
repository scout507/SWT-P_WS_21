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
    
    [Range(0.1f,2)]
    public float playerToObjectRatio; //This is used to either prefer players (<1) or destructable objects (>1)  

    public List<GameObject> targets; //A list containing all possible targets
    public GameObject currentTarget;

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
            FindTargets();
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
            if(timer >= refreshRate) FindTargets();
            if(!awake && timer >= refreshRate) awake = CheckAggro();
            if(awake && timer >= refreshRate) currentTarget = FindTarget();
            if(currentTarget != null)
            {
                if(Vector3.Distance(currentTarget.transform.position, transform.position) > atkRange)
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

            if(timer >= refreshRate) timer = 0f;
        }
    }

    //Makes a list containing all active players and destructable objects.
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
            if(Vector3.Distance(transform.position, player.transform.position) <= aggroRadius) targets.Add(player);
        }
        foreach (GameObject destructable in destructables)
        {
            if(Vector3.Distance(transform.position, destructable.transform.position) <= aggroRadius && destructable.GetComponent<DestructableObject>().active) targets.Add(destructable);
        }
    }

    //This has changed from the previous version. It might be obsolete, but could still be usefull in the future if the aggro behaviour is going to change.
    bool CheckAggro()
    {
        return targets.Count>0;
    }

    //Finds the nearest Target
    GameObject FindTarget()
    {
        float shortestDistance = 0f;
        GameObject newTarget = null;

        foreach (GameObject target in targets)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            
            if(target.tag == "Player")
            {
                distance = distance * playerToObjectRatio;
            }
            else{
                //Debug.Log(distance <= shortestDistance && nav.CalculatePath(target.transform.position, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete);
            } 

            if(shortestDistance == 0) shortestDistance = distance;

            //if the player is reachable and the closest to the monster, the player becomes the new target
            NavMeshPath navMeshPath = new NavMeshPath();
            if(distance <= shortestDistance && nav.CalculatePath(target.transform.position, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
            {
                shortestDistance = distance;
                newTarget = target;
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
            if(currentTarget.tag == "Player")
            {
                currentTarget.GetComponent<Health>().TakeDamage(Mathf.RoundToInt(damage)); //Health script uses int for health. Needs to be resolved
            }
            else if(currentTarget.tag == "DestructableObject")
            {
                currentTarget.GetComponent<DestructableObject>().TakeDamage(damage);
            } 
        }
    }

    //can be called if a player damages the monster.
    public void TakeDamage(float dmgTaken)
    {
        if(!dead)
        {
            //TODO: Play damage animation & sound
            hp -= dmgTaken;
            if(hp <= 0) Die();
        }
    }


    //this can be used to manualy trigger monsters.
    void AggroMob(GameObject player)
    {
        if(currentTarget == null)
        {
            awake = true;
            currentTarget = player;
        }
    }

    
}
