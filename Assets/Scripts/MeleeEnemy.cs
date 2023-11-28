using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : EnemyBase
{
    [Header("Melee Settings")]
    public int damage;
    public int attackCooldown;
    public float time = 0;
    public float lapseTime = 2.02f;
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
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        animator.SetBool("isAttacking", true);
        animator.SetBool("isReadyToAttack", false);
        if (time >= lapseTime )
        {
            animator.SetBool("isAttacking", false);
        }
        
        else if (readyToAttack)
        {

            readyToAttack = false;
            Invoke(nameof(ResetAttack), attackCooldown);
            //animator.SetBool("isAttacking", false);
            animator.SetBool("isReadyToAttack", true);
            time = 0;

            // Attack Phase
           // playerStatsController.TakeDamage(Convert.ToInt32(damage * stats.strength), gameObject);
        }
    }
}
