using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{

    private float lerpTimer;
    public float chipSpeed = 10f;
    public float waitTimer = 1f;
    public Image frontHealthBar;
    public Image backHealthBar;
    public Image backGround;
    public Image Frame;
    public TextMeshProUGUI h;
    private StatsController stats;
    // Start is called before the first frame update
    void Start()
    {
        stats = GetComponent<StatsController>();
        stats.health = stats.maxHealth;
        h.text = stats.health + " / " + stats.maxHealth;
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
        h.text = stats.health + " / " + stats.maxHealth;

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
        stats.health -= damage;
        lerpTimer = 0f;
        waitTimer = 1f;

    }

    public void RestoreHealth(float healAmount)
    {
        stats.health += healAmount;
        lerpTimer = 0f;

    }

    public void IncreaseHealth(int level)
    {
        level = GetComponent<LevelingSystem>().level;
        int ratio = Mathf.RoundToInt((stats.health *0.01f) * (100 - level) * 0.327f);
        stats.maxHealth += Mathf.RoundToInt((stats.health *0.01f) * (100 - level) * 0.327f);
        stats.health = stats.maxHealth;
        backGround.rectTransform.sizeDelta = new Vector2(backGround.rectTransform.sizeDelta.x + ratio, backGround.rectTransform.sizeDelta.y);
        backGround.rectTransform.transform.position = new Vector2(backGround.rectTransform.transform.position.x + ratio/2, backGround.rectTransform.position.y);
        backHealthBar.rectTransform.sizeDelta = new Vector2(backHealthBar.rectTransform.sizeDelta.x + ratio, backHealthBar.rectTransform.sizeDelta.y);
        backHealthBar.rectTransform.transform.position = new Vector2(backHealthBar.rectTransform.transform.position.x + ratio/2, backHealthBar.rectTransform.transform.position.y);
        Frame.rectTransform.sizeDelta = new Vector2(Frame.rectTransform.sizeDelta.x + ratio, Frame.rectTransform.sizeDelta.y);
        Frame.rectTransform.transform.position = new Vector2(Frame.rectTransform.transform.position.x + ratio / 2, Frame.rectTransform.transform.position.y);
        frontHealthBar.rectTransform.sizeDelta = new Vector2(frontHealthBar.rectTransform.sizeDelta.x + ratio, frontHealthBar.rectTransform.sizeDelta.y);
        frontHealthBar.rectTransform.transform.position = new Vector2(frontHealthBar.rectTransform.transform.position.x + ratio/2, frontHealthBar.rectTransform.transform.position.y);
    }

   

}
