using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsController : MonoBehaviour
{
    private Animator animator;
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
        if (gameObject.tag == "Player") animator = GameObject.Find("PlayerBody").gameObject.GetComponent<Animator>();
        else
        {
            animator = gameObject.transform.GetChild(0).GetComponent<Animator>();
            updateHealth(level);
        }
        levelingSystem = GameObject.Find("Player").GetComponent<LevelingSystem>();
        
    }

   
    

    public void TakeDamage(int damage, GameObject damagedBy)
    {
        if (killed) return;
        animator.SetBool("isHit", true);
        health -= damage;
        Invoke(nameof(resetHit), 1);
        if (health <= 0)
            KillEntity(damagedBy);
        else if (gameObject.tag == "Player")
            FindObjectOfType<AudioManager>().Play("PlayerHurt");
        else if (gameObject.tag == "Enemy" || gameObject.tag == "Boss" || gameObject.tag == "RangedEnemy" || gameObject.tag == "Melee")
            FindObjectOfType<AudioManager>().Play("EnemyHurt");
    }
    
    public void resetHit()
    {
       animator.SetBool("isHit", false);
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
            FindObjectOfType<AudioManager>().Play("PlayerDeath");
            CombatController combatController = GetComponent<CombatController>();
            combatController.enabled = false;
        }
        else if (gameObject.tag == "Enemy" || gameObject.tag == "Boss" || gameObject.tag == "RangedEnemy" || gameObject.tag == "Melee")
        {
            FindObjectOfType<AudioManager>().Play("PlayerDeath");
            Invoke(nameof(destroy), 4);
            Debug.Log(killedBy.gameObject.name);
            if (killedBy.name == "Player")
            {
                levelingSystem.GainExperienceScalable(xp, level);
            }
        }
        animator.SetBool("isAttacking", false);
        animator.SetBool("isMoving", false);
        animator.SetBool("isHit", false);
        animator.SetBool("isDead", true);
    }

    private void updateHealth(int level)
    {
        if (gameObject.tag == "Enemy")
        {
            setHealth(0.01f, 0.258f, level);
        }
       else if (gameObject.tag == "RangedEnemy")
        {
            setHealth(0.01f, 0.24f, level);
        }
       else if (gameObject.tag == "Boss")
        {
            setHealth(0.01f, 0.473f, level);
        }
       else if (gameObject.tag == "Melee")
        {
            setHealth(0.01f, 0.44f, level);
        }
    }

    private void setHealth(float x, float y, int level)
    {
        if (level == 1)
        {
            maxHealth = 100;
            health = maxHealth;
            return;
        }
        for (int i = 2; i <= level; i++)
        {
            int ratio = Mathf.RoundToInt((100 * x) * (100 - i) * y);
            maxHealth += ratio;
        }
        health = maxHealth;
    }

    public void destroy()
    {
        Destroy(gameObject);
    }

    
}
