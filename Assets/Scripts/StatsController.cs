using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsController : MonoBehaviour
{
    [Header("Stats")]
    public float health;
    public float maxHealth;
    public int level;
    public int xp;
    public float strength;
    public bool killed = false;

    private LevelingSystem levelingSystem;

    private void Start()
    {
        if (killed) return;
        if (health == 0)
            KillEntity(null);

        levelingSystem = GameObject.Find("Player").GetComponent<LevelingSystem>();
    }
    

    public void TakeDamage(int damage, GameObject damagedBy)
    {
        if (killed) return;
        health -= damage;

        if (health <= 0)
            KillEntity(damagedBy);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Exp"))
        {
            Destroy(collision.gameObject);
            xp++;
        }
    }

    public void KillEntity(GameObject killedBy)
    {
        if (killed) return;
        killed = true;
        if (gameObject.tag == "Player")
        {
            //@TODO: Player death animation
            MovementController moveController = GetComponent<MovementController>();
            CombatController combatController = GetComponent<CombatController>();
            moveController.enabled = false;
            combatController.enabled = false;
        }
        else if (gameObject.tag == "Enemy")
        {
            //@TODO: Enemy death animation
            Invoke(nameof(destroy), 3);
            if (killedBy.name == "Player")
                levelingSystem.GainExperienceScalable(xp, level);
        }
    }

    public void destroy()
    {
        Destroy(gameObject);
    }

    
}
