using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PiranhaController : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsWater, whatIsPlayer;
    public Transform[] waypoints;
    int waypointIndex;
    Vector3 target;

    //define attacking parameters
    public float timeBetweenAttack;
    bool alreadyAttacked;

    //define vision radius and check if player in proximity
    public float visionRad, attackRange;
    public bool playerInVisionRad, playerInAttackRange;

    private void Awake()
    {
        //find player object
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if(Vector3.Distance(transform.position, target) < 1)
        {
            IterateWaypointIndex();
            Patroling();
        }
        //check if player is in range
        playerInVisionRad = Physics.CheckSphere(transform.position, visionRad, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        //if the player is in neither vision or attack range then piranha should patrol
        if (!playerInVisionRad && !playerInAttackRange) Patroling();
        //if player is in sight but not in attack range, then piranha should start chasing player 
        if (playerInVisionRad && !playerInAttackRange) ChasingPlayer();
        //if player is also in attack range then piranha should start attacking
        if (playerInVisionRad && playerInAttackRange) Attacking();

    }

    private void Patroling()
    {
        //set new waypoint as destination
        target = waypoints[waypointIndex].position;
        agent.SetDestination(target);

    }

    void IterateWaypointIndex()
    {
        //increase index of array by 1 and return to first one when last one reached
        waypointIndex++;
        if(waypointIndex == waypoints.Length)
        {
            waypointIndex = 0;
        }
    }
    

    private void ChasingPlayer()
    {
        //make player the target/destination when in range
        agent.SetDestination(player.position);
    }

    private void Attacking()
    {
        //stop moving when attacking and look at the player
        agent.SetDestination(transform.position);
        transform.LookAt(player);


        //check if not attacked already and then invoke reset attack method with delay between attacks
        if (!alreadyAttacked)
        {
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttack);
        }

    }
    //kill player when touching its collider 
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Manager.AddHealth(-111);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, visionRad);

    }
}
