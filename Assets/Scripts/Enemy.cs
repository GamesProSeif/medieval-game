using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    public NavMeshAgent agent;
    public PlayerController controller;
    public Transform player;
    public Rigidbody rb;
    public LayerMask Ground, Player;
    public float startWaitTime = 4;
    public float timeToRotate = 2;
    public float walkSpeed;
    public float runSpeed;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    //stats
    public float sightRange, attackRange;
    public float m_WaitTime;
    public bool playerInSightRange, playerInAttackRange;
   
    void Awake()
    {
        player = GameObject.Find("PlayerObj").transform;
        agent = GetComponent<NavMeshAgent>();
        controller = GameObject.Find("PlayerObj").gameObject.GetComponent<PlayerController>();
        m_WaitTime = startWaitTime;

    }



    // Update is called once per frame
    void Update()
    {
        if(!controller.isGameOver)
        {

            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, Player);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, Player);
            //Check for sight and attack range
            if(!playerInSightRange && !playerInAttackRange)
            {
                Patrol();
            }
            else if (playerInSightRange && !playerInAttackRange)
            {
                ChasePlayer();
            }
            else if(playerInSightRange && playerInAttackRange)
            {
                AttackPlayer();
            }
        }
        if (true)
            rb.drag = controller.groundDrag;



    }


    private void startMove()
    {
        agent.isStopped = false;
        if (!playerInSightRange && !playerInAttackRange)
            agent.speed = walkSpeed;
        else if (playerInSightRange && !playerInAttackRange)
            agent.speed = runSpeed;
    }
    private void stopMove()
    {
        agent.isStopped = true;
        agent.speed = 0;
    }

    private void Patrol()
    {
        if (!walkPointSet)
            generatePoint();
        else if(walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalk = transform.position - walkPoint;

        //walkpoint reached

        if (distanceToWalk.magnitude < 1f)
        {
            if (m_WaitTime <= 0)
            {
                walkPointSet = false;
                startMove();
                m_WaitTime = startWaitTime;
                
            }
            else
            {
                stopMove();
                m_WaitTime -= Time.deltaTime;
            }
        }
            
    }
    
    private void generatePoint()
    {
        float randomZ = Random.Range(-walkPointRange, +walkPointRange);
        float randomX = Random.Range(-walkPointRange, +walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, Ground))
            walkPointSet = true;
    }
    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
        transform.LookAt(player);
        startMove();
    }
    private void AttackPlayer()
    {
        //make sure enemy doesnt move 

        agent.SetDestination(transform.position);
        transform.LookAt(player);

        if(!alreadyAttacked)
        {
            //code of attack

            alreadyAttacked = true;

        }
    }
    void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            controller.GameOver();
            agent.isStopped = true;
        }
    }
}
