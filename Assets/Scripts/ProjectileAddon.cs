using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class ProjectileAddon : MonoBehaviour
{
    public GameObject fireParticles;
    private Rigidbody rb;
    private bool targetHit;
    private StatsController playerStats = null;
    private CombatController playerCombatController = null;

    public enum WeaponType { Projectile, Grenade };
    public WeaponType type;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        GameObject playerGameObject = GameObject.Find("Player");
        playerStats = playerGameObject.GetComponent<StatsController>();
        playerCombatController = playerGameObject.GetComponent<CombatController>();
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (type == WeaponType.Projectile)
        {
            if (targetHit)
                return;
            if (collision.gameObject.tag == "Player") return;
            targetHit = true;
            if (collision.gameObject.GetComponent<StatsController>() != null)
            {
                StatsController enemyStats = collision.gameObject.GetComponent<StatsController>();
                enemyStats.TakeDamage(
                    Convert.ToInt32(playerCombatController.bowSettings.damage * playerStats.strength),
                    GameObject.Find("Player"));
                Destroy(gameObject);
            }
            rb.isKinematic = true;
            transform.SetParent(collision.transform);
        } else if (type == WeaponType.Grenade)
        {
            if (collision.gameObject.tag == "Ground")
            {
                Instantiate(fireParticles, transform.position, fireParticles.transform.rotation);

                Destroy(gameObject);
                rb.isKinematic = true;
                transform.SetParent(collision.transform);
            }
        }
    }
}
