using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : EnemyBase
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
        agent.SetDestination(playerTransform.position);
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
        for (int i = 0; i < 5; i++)
        {
            Vector3 currentPosition = transform.position + transform.forward * 2 + new Vector3(0, 1.3f, 0);
            Vector3 playerPosition = playerTransform.position;
            Vector3 arrowDirection = (playerPosition - currentPosition).normalized;

            // Attack Phase
            GameObject projectile = Instantiate(objectToThrow, currentPosition,
                transform.rotation * objectToThrow.transform.rotation);
            ProjectileAddon projAddon = projectile.GetComponent<ProjectileAddon>();
            projAddon.SetInitiator(gameObject, damage);
            Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

            projectileRb.AddForce(arrowDirection * force, ForceMode.Impulse);
            yield return new WaitForSeconds(0.2f);
        }
    }
}
