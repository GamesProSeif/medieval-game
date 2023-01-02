using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : EnemyBase
{
    [Header("Ranged Settings")]
    public int attackCooldown;
    public int damage;
    public float force;
    public GameObject objectToThrow;

    protected override void Idle()
    {
        transform.LookAt(playerTransform);
    }

    protected override void ChasePlayer()
    {
        Animator animator = gameObject.transform.GetChild(0).GetComponent<Animator>();
        agent.SetDestination(playerTransform.position);
        animator.SetFloat("Speedf", agent.speed);
    }

    protected override void AttackPlayer()
    {
        Animator animator = gameObject.transform.GetChild(0).GetComponent<Animator>();

        // Fixed position
        agent.SetDestination(transform.position);
        transform.LookAt(playerTransform);

        if (readyToAttack)
        {
            animator.SetBool("isAttacking", true);
            readyToAttack = false;
            Invoke(nameof(ResetAttack), attackCooldown);
            Vector3 currentPosition = transform.position + transform.forward * 2 + new Vector3(0, 0.3f, 0);
            Vector3 playerPosition = playerTransform.position;
            Vector3 arrowDirection = (playerPosition - currentPosition).normalized;

            // Attack Phase
            GameObject projectile = Instantiate(objectToThrow, currentPosition,
                transform.rotation * objectToThrow.transform.rotation);
            ProjectileAddon projAddon = projectile.GetComponent<ProjectileAddon>();
            projAddon.SetInitiator(gameObject, damage);
            Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

            projectileRb.AddForce(arrowDirection * force, ForceMode.Impulse);
        }
    }

    protected override void ResetAttack()
    {
        base.ResetAttack();

        Animator animator = gameObject.transform.GetChild(0).GetComponent<Animator>();
        animator.SetBool("isAttacking", true);
    }
}
