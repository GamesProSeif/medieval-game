using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : EnemyBase
{
    [Header("Melee Settings")]
    public int damage;
    public int attackCooldown;

    protected override void Idle()
    {
        transform.LookAt(playerTransform);
    }

    protected override void ChasePlayer()
    {
        agent.SetDestination(playerTransform.position);
    }

    protected override void AttackPlayer()
    {
        // Fixed position
        agent.SetDestination(transform.position);
        transform.LookAt(playerTransform);
        animator.SetBool("isAttacking", true);

        if (readyToAttack)
        {

            readyToAttack = false;
            Invoke(nameof(ResetAttack), attackCooldown);
            animator.SetBool("isAttacking", false);

            // Attack Phase
           // playerStatsController.TakeDamage(Convert.ToInt32(damage * stats.strength), gameObject);
        }
    }
}
