using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    public NavMeshAgent agent;
    public PlayerController controller;
    public Transform player;
    public Vector3 lastPlayerPos;
    public Rigidbody rb;
    public LayerMask Ground, Player;
    public LayerMask obsticle;
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
    public float m_WaitTime, m_RotateTime;
    public float angle;
    public bool playerInSightRange, playerInAttackRange, wasChasing;

    void Awake()
    {
        player = GameObject.Find("LookAt").transform;
        agent = GetComponent<NavMeshAgent>();
        controller = GameObject.Find("Player").gameObject.GetComponent<PlayerController>();
        angle = 45;
        lastPlayerPos = Vector3.zero;

    }


    // Update is called once per frame
    void Update()
    {
        if (!controller.isGameOver)
        {

            RoundEnviroment();

           // playerInSightRange = Physics.CheckSphere(transform.position, sightRange, Player);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, Player);
            //Check for sight and attack range
            if (!playerInSightRange && !playerInAttackRange)
            {
                lastPlayerPos = player.position;
                if (m_RotateTime > 0)
                {
                    transform.LookAt(player);
                    m_RotateTime -= Time.deltaTime;
                    stopMove();
                }
                else
                {
                    if (wasChasing)
                    {
                        m_WaitTime = 2f;
                        lookingForPlayer(lastPlayerPos);
                    }
                    Patrol();
            
                }
            }
            else if (playerInSightRange && !playerInAttackRange)
            {
                if (m_WaitTime >= 0)
                {
                    m_WaitTime = 0;
                    ChasePlayer();
                }
            }
            else if (playerInSightRange && playerInAttackRange)
            {
                AttackPlayer();
            }
        }
        if (true)
            rb.drag = controller.groundDrag;
    }


    private void Patrol()
    {
        startMove(walkSpeed);
        if (!walkPointSet)
            generatePoint();
        else if (walkPointSet)
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
                startMove(walkSpeed);

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
        m_WaitTime = startWaitTime;
        float randomZ = Random.Range(-walkPointRange, +walkPointRange);
        float randomX = Random.Range(-walkPointRange, +walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, Ground))
            walkPointSet = true;
    }
    private void ChasePlayer()
    {
        Vector3 dirToplayer = (transform.position - player.position).normalized;
        walkPointSet = false;
        m_RotateTime = timeToRotate;
        agent.SetDestination(player.position);
        transform.LookAt(player);
        startMove(runSpeed);
        wasChasing = true;


    }
    private void AttackPlayer()
    {
        //make sure enemy doesnt move 

        agent.SetDestination(transform.position);
        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            //code of attack

            alreadyAttacked = true;

        }
    }
    void ResetAttack()
    {
        alreadyAttacked = false;
    }
    private void startMove(float s)
    {
        agent.isStopped = false;
        agent.speed = s;
    }
    private void stopMove()
    {
        agent.isStopped = true;
        agent.speed = 0;
    }

    private void lookingForPlayer(Vector3 p)
    {
        agent.SetDestination(p);
        if (m_WaitTime > 0 && Vector3.Distance(transform.position, p) <= 0.3)
        {
            stopMove();
            m_WaitTime -= Time.deltaTime;
        }
        else wasChasing = false;
       

    }




    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            controller.GameOver();
            agent.isStopped = true;
        }
    }

    void RoundEnviroment()
    {
        Collider[] playerInRange = Physics.OverlapSphere(transform.position, sightRange, Player);  

        for (int i = 0; i < playerInRange.Length; i++)
        {
            Transform player = playerInRange[i].transform;
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToPlayer) < angle)
            {
                float dstToPlayer = Vector3.Distance(transform.position, player.position);        
                if (!Physics.Raycast(transform.position, dirToPlayer, dstToPlayer, obsticle))
                {
                    playerInSightRange = true;                     
                }
                else
                {
                    playerInSightRange = false;
                }
            }
        }
    }
}
