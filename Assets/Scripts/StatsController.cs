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

    [Header("LevelingSystem")]
    public LevelingSystem levelingSystem;

    private void Start()
    {
        if (killed) return;
        if (health == 0)
            KillEntity(null);

        if (gameObject.tag == "Player")
        {
            levelingSystem = GetComponent<LevelingSystem>();
        }
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
            MovementController moveController = GetComponent<MovementController>();
            moveController.enabled = false;
        }
        else if (gameObject.tag == "Enemy")
        {
            Invoke(nameof(destroy), 3);
            levelingSystem.GainExperienceScalable(xp, level);
        }
    }

    public void destroy()
    {
        Destroy(gameObject);
    }

    
}
