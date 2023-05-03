using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    private float lerpTimer;
    public float chipSpeed = 10f;
    public float waitTimer = 1f;
    public GameObject bossHealth;
    public Image frontHealthBar;
    public Image backHealthBar;
    public Image backGround;
    public Image Frame;
    private StatsController stats;
    // Start is called before the first frame update
    void Start()
    {
        bossHealth = GameObject.Find("BossHealth");
        stats = GetComponent<StatsController>();
        stats.health = stats.maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        stats.health = Mathf.Clamp(stats.health, 0, stats.maxHealth);
        updateHealthUI();
    }

    public void updateHealthUI()
    {
        float fillF = frontHealthBar.fillAmount;
        float fillB = backHealthBar.fillAmount;
        float hFraction = stats.health / stats.maxHealth;

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
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete *= percentComplete;
            frontHealthBar.fillAmount = Mathf.Lerp(fillF, backHealthBar.fillAmount, percentComplete);

        }

    }

    public void TakeDamage(float damage)
    {
        stats.health -= damage;
        lerpTimer = 0f;
        waitTimer = 2f;

    }


}
