using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBase : MonoBehaviour
{
    protected NavMeshAgent agent;
    protected StatsController stats;
    protected Rigidbody rb;
    protected float speed, groundDrag;
    protected bool readyToAttack;

    protected StatsController playerStatsController;
    protected Transform playerTransform;

    public LayerMask whatIsPlayer, whatIsGround;

    [Header("States")]
    public float sightRange;
    public float attackRange;
    public bool playerInSightRange, playerInAttackRange;

    protected virtual void Awake()
    {
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

        if (!playerInSightRange && !playerInAttackRange) Idle();
        else if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        else if (playerInSightRange && playerInAttackRange) AttackPlayer();
    }

    protected abstract void Idle();

    protected abstract void ChasePlayer();

    protected abstract void AttackPlayer();

    protected virtual void ResetAttack()
    {
        readyToAttack = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}

