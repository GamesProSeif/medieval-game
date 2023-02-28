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

    private enum Shooter { Player, Enemy }
    private Shooter shooter;
    private int damage;
    public GameObject initiator;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        GameObject playerGameObject = GameObject.Find("Player");
        playerStats = playerGameObject.GetComponent<StatsController>();
        playerCombatController = playerGameObject.GetComponent<CombatController>();
    }

    public void SetInitiator(GameObject initiator, int damage)
    {
        this.initiator = initiator;
        this.damage = damage;
        if (initiator.name == "Player")
            shooter = Shooter.Player;
        else
            shooter = Shooter.Enemy;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (type == WeaponType.Projectile)
        {
            if (targetHit)
                return;
            if (shooter == Shooter.Player && collision.gameObject.tag == "Player"
                || shooter == Shooter.Enemy && collision.gameObject.tag == "Enemy"
                ) return;
            targetHit = true;
            if (collision.gameObject.GetComponent<StatsController>() != null)
            {
                StatsController shotStats = collision.gameObject.GetComponent<StatsController>();
                StatsController shooterStats = initiator.GetComponent<StatsController>();

                shotStats.TakeDamage(
                    Convert.ToInt32(damage * shooterStats.strength),
                    initiator);
                Destroy(gameObject);
            }
            rb.isKinematic = true;
            transform.SetParent(collision.transform);
        } else if (type == WeaponType.Grenade)
        {
            if (collision.gameObject.tag == "Ground")
            {
                Instantiate(fireParticles, transform.position, fireParticles.transform.rotation);
                FindObjectOfType<AudioManager>().Play("MolotovBreak");
                Destroy(gameObject);
                rb.isKinematic = true;
                transform.SetParent(collision.transform);
            }
        }
    }
}
