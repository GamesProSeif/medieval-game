using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SwordScript : MonoBehaviour
{
    private CombatController combatController;
    private StatsController statsController;
    private Animator animator;
    private bool hit = false;
    private float damageDone = 0;
    // Start is called before the first frame update
    void Awake()
    {
        combatController = GameObject.Find("Player").GetComponent<CombatController>();
        animator = gameObject.GetComponentInParent<Animator>();
        statsController = gameObject.GetComponentInParent<StatsController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(hit)
            checkReset();
        if(animator.GetBool("isAttacking"))
        {
            gameObject.GetComponent<BoxCollider>().isTrigger = true;
        }
        else
            gameObject.GetComponent<BoxCollider>().isTrigger = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<StatsController>() != null && other.tag != statsController.gameObject.tag)
        {

            if (statsController.gameObject.tag == "Player")
            {
                other.gameObject.GetComponent<StatsController>().TakeDamage(Convert.ToInt32(combatController.swordSettings.damage * statsController.strength), other.gameObject);
                combatController.readyToAttack = false;
                damageDone += combatController.swordSettings.damage;
                Debug.Log(damageDone);
            }
            else if (statsController.gameObject.tag == "Enemy")
            {
                other.gameObject.GetComponent<StatsController>().TakeDamage(Convert.ToInt32(statsController.gameObject.GetComponent<MeleeEnemy>().damage * statsController.strength), other.gameObject);
                
            }
            
            hit = true;
            FindObjectOfType<AudioManager>().Play("SwordAttack");
            
        }
    }

    private void checkReset()
    {
        
        if (statsController.gameObject.tag == "Player")
        {
            Invoke(nameof(resetAttack), combatController.swordSettings.cooldown);
          
        }
        //else if (statsController.gameObject.tag == "Enemy")
        //{
        //    Invoke(nameof(resetAttack), statsController.gameObject.GetComponent<MeleeEnemy>().attackCooldown);
        //    Debug.Log("checking in progress");
        //}
        hit = false;
        animator.SetBool("isAttacking", false);
        Debug.Log(hit);
        Debug.Log(animator.GetBool("isAttacking"));
    }
    private void resetAttack()
    {
        if (statsController.gameObject.tag == "Player")
            combatController.readyToAttack = true;
      //  else if (statsController.gameObject.tag == "Enemy")
           // gameObject.GetComponentInParent<MeleeEnemy>().readyToAttack = true;
    }
}
