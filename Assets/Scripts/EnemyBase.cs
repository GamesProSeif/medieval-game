using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBase : MonoBehaviour
{
    protected NavMeshAgent agent;
    protected StatsController stats;
    protected Rigidbody rb;
    protected Animator animator;
    protected float speed, groundDrag;
    protected bool readyToAttack;

    protected StatsController playerStatsController;
    protected Transform playerTransform;
    private BossHealth bossHealth;

    public LayerMask whatIsPlayer, whatIsGround;

    [Header("States")]
    public float sightRange;
    public float attackRange;
    public bool playerInSightRange, playerInAttackRange;

    protected virtual void Awake()
    {
        animator = gameObject.transform.GetChild(0).GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        stats = GetComponent<StatsController>();
        rb = GetComponent<Rigidbody>();
    
        GameObject player = GameObject.Find("Player");
        playerStatsController = player.GetComponent<StatsController>();
        playerTransform = player.transform;

        MovementController playerMovementController = player.GetComponent<MovementController>();
        speed = playerMovementController.Speed - 1;
        if (speed <= 0) speed = 1;
        groundDrag = playerMovementController.groundDrag;
        agent.speed = speed / 2;
        readyToAttack = true;
    }

    protected virtual void Update()
    {
        if (stats.killed || playerStatsController.killed)
        {
            agent.SetDestination(transform.position);
            return;
        }
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange)
        {
            Idle();
            animator.SetBool("isMoving", false);
            agent.isStopped = true;
        }
        else if (playerInSightRange && !playerInAttackRange)
        {
            ChasePlayer();
            animator.SetBool("isMoving", true);
            agent.isStopped = false;
        }
        else if (playerInSightRange && playerInAttackRange && (!animator.GetBool("isHit") || gameObject.tag == "Boss")) { AttackPlayer(); if (gameObject.tag == "Boss") bossHealth.bossHealth.SetActive(true);}
        if (animator.GetBool("isHit") && gameObject.tag != "Boss")
            ResetAttack();
        
    }

    protected abstract void Idle();

    protected abstract void ChasePlayer();

    protected abstract void AttackPlayer();

    protected virtual void ResetAttack()
    {
        readyToAttack = true;
        animator.SetBool("isAttacking", false);
        if (animator.GetBool("isHit"))
            stats.resetHit();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}

