using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class RoamingMonster : MonsterController
{

    public float detectionRate;
    float detectionTimer;
    public bool detectedPlayer;
    public bool aggro;

    [Range(0, 360)]
    public float angle;

    public List<GameObject> targets = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {


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
                if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
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

}

