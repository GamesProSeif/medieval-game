using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    public Image backGround;
    public Image Frame;
    public TextMeshProUGUI h;
    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        h.text = health + " / " + maxHealth;
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
        Debug.Log(maxHealth);
    }

    public void updateHealthUI()
    {
        float fillF = frontHealthBar.fillAmount;
        float fillB = backHealthBar.fillAmount;
        float hFraction = health / maxHealth;
        h.text = health + " / " + maxHealth;

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

    public void IncreaseHealth(int level)
    {
        level = GetComponent<LevelingSystem>().level;
        int ratio = Mathf.RoundToInt((health *0.01f) * (100 - level) * 0.327f);
        maxHealth += Mathf.RoundToInt((health *0.01f) * (100 - level) * 0.327f);
        health = maxHealth;
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
