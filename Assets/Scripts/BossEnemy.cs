using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : EnemyBase
{
    Animator animator;
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
        // Fixed position
        agent.SetDestination(transform.position);
        transform.LookAt(playerTransform);
        if (readyToAttack)
        {
           
            readyToAttack = false;
            Invoke(nameof(ResetAttack), attackCooldown);

            StartCoroutine(AttackCoroutine());
        }
    }

    IEnumerator AttackCoroutine()
    {
        Animator animator = gameObject.transform.GetChild(0).GetComponent<Animator>();
        for (int i = 0; i < 5; i++)
        {
            animator.SetBool("isAttacking", true);
            Vector3 currentPosition = transform.position + transform.forward * 2 + new Vector3(0, 1.3f, 0);
            Vector3 playerPosition = playerTransform.position + new Vector3(0, 0.6f, 0);
            Vector3 arrowDirection = (playerPosition - currentPosition).normalized;

            // Attack Phase
            GameObject projectile = Instantiate(objectToThrow, currentPosition,
                transform.rotation * objectToThrow.transform.rotation);
            ProjectileAddon projAddon = projectile.GetComponent<ProjectileAddon>();
            projAddon.SetInitiator(gameObject, damage);
            Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

            projectileRb.AddForce(arrowDirection * force, ForceMode.Impulse);
            yield return new WaitForSeconds(0.2f);
            animator.SetBool("isAttacking", false);
        }
        
    }
}
