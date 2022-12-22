using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{

    private float health;
    private float lerpTimer;
    public float maxHealth = 100f;
    public float chipSpeed = 10f;
    public float waitTimer = 1f;
    public Image frontHealthBar;
    public Image backHealthBar;
    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        health = Mathf.Clamp(health, 0, maxHealth);
        updateHealthUI();

        if (Input.GetKeyDown(KeyCode.A))
            TakeDamage(Random.Range(5, 10));
        if (Input.GetKeyDown(KeyCode.S))
            RestoreHealth(Random.Range(5, 10));
    }

    public void updateHealthUI()
    {
        float fillF = frontHealthBar.fillAmount;
        float fillB = backHealthBar.fillAmount;
        float hFraction = health / maxHealth;

        if (fillB > hFraction)
        {
            frontHealthBar.fillAmount = hFraction;
            backHealthBar.color = Color.white;
            if (waitTimer > 0)
            {
                waitTimer -= Time.deltaTime;
                
            }
            else
            {
                lerpTimer += Time.deltaTime;
                float percentComplete = lerpTimer / chipSpeed;
                percentComplete *= percentComplete;
                backHealthBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
            }
        }
         if (fillF < hFraction)
        {
            backHealthBar.fillAmount = hFraction;
            backHealthBar.color = Color.green;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer/chipSpeed;
            percentComplete *= percentComplete;
            frontHealthBar.fillAmount = Mathf.Lerp(fillF, backHealthBar.fillAmount, percentComplete);
           
        }
       
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        lerpTimer = 0f;
        waitTimer = 1f;

    }

    public void RestoreHealth(float healAmount)
    {
        health += healAmount;
        lerpTimer = 0f;

    }
}
