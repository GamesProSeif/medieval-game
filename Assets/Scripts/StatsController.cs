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
        else if (gameObject.tag == "Boss") animator = gameObject.transform.GetChild(0).GetComponent<Animator>();
        else if (gameObject.tag == "RangedEnemy") animator = gameObject.transform.GetChild(0).GetComponent<Animator>();
        else animator = GetComponent<Animator>();
        levelingSystem = GameObject.Find("Player").GetComponent<LevelingSystem>();
    }
    

    public void TakeDamage(int damage, GameObject damagedBy)
    {
        if (killed) return;
        if(gameObject.tag == "Player")animator.SetBool("isHit", true);
        health -= damage;
        Invoke(nameof(resetHit), 1);
        if (health <= 0)
            KillEntity(damagedBy);
    }
    
    private void resetHit()
    {
       if(gameObject.tag == "Player") animator.SetBool("isHit", false);
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
            //@TODO: Player death animation (done)
            animator.SetBool("isDead", true);
            MovementController moveController = GetComponent<MovementController>();
            CombatController combatController = GetComponent<CombatController>();
            moveController.enabled = false;
            combatController.enabled = false;
        }
        else if (gameObject.tag == "Enemy" || gameObject.tag == "Boss" || gameObject.tag == "RangedEnemy")
        {
            //@TODO: Enemy death animation
            animator.SetBool("isDead", true);
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
