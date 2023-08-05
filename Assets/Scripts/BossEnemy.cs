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
    private GameObject health;

    protected void Start()
    {
        health = GameObject.Find("BossHealth");
    }
    protected override void Idle()
    {
        transform.LookAt(playerTransform);
        health.gameObject.SetActive(false);
    }

    protected override void ChasePlayer()
    {

        agent.SetDestination(playerTransform.position);
        health.gameObject.SetActive(true);
    }

    protected override void AttackPlayer()
    {
        health.gameObject.SetActive(true);
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
