using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    public NavMeshAgent agent;
    public PlayerController controller;
    public Transform player;
    public Vector3 lastPlayerPos;
    public Vector3 lastPoint;
    public Rigidbody rb;
    public LayerMask Ground, Player;

    public LayerMask obsticle;
    public int health;

    public float startWaitTime = 4;
    public float timeToRotate = 10;
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
    public float m_WaitTime, m_RotateTime, m_BeforeMoving;
    public float angle;
    public bool playerInSightRange, playerInAttackRange, wasChasing, isLookingForPlayer;

    public enum AiStatus {Attacker , Archer, Patroller}
    public AiStatus status;

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

            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, Player);
            //Check for sight and attack range
            if (!playerInSightRange && !playerInAttackRange && status == AiStatus.Patroller)
            {
                if(m_RotateTime > 0)
                {
                    stopMove();
                    m_RotateTime -= Time.deltaTime;
                }
                else
                    Patrol();

            }
            else if (playerInSightRange && !playerInAttackRange)
            {
                if (m_WaitTime >= 0)
                {
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

    public void takeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
            Destroy(gameObject);
    }


    private void Patrol()
    {
        startMove(walkSpeed);

        if(wasChasing)
        {
            lookingForPlayer(lastPlayerPos);          

        }
        if (!walkPointSet && !wasChasing)
            generatePoint();
        else if (walkPointSet && !wasChasing)
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
        lastPlayerPos = transform.position;
        Vector3 dirToplayer = (transform.position - player.position).normalized;
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
            if(status == AiStatus.Attacker || status == AiStatus.Patroller)
            {


            }

            else if (status == AiStatus.Archer)
            {

            }
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
        if (Vector3.Distance(transform.position, p) <= 0.3f)
        {
            if (m_WaitTime > 0)
            {
                stopMove();
                m_WaitTime -= Time.deltaTime;
            }
            else
            {
                wasChasing = false;
                m_WaitTime = startWaitTime / 2;
            }
        }



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
                if (!Physics.Raycast(transform.position, dirToPlayer, dstToPlayer, obsticle) && dstToPlayer <= sightRange)
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
