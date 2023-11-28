using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelingSystem : MonoBehaviour
{
    public int level;
    private int maxLevel = 10;
    public float currentxp;
    public float requiredxp;
    private float lerptimer;
    private float delaytimer;
    [Header("UI")]
    public Image FrontxPBar;
    public Image BackxPBar;
    public Image backGround;
    public Image Frame;
    public TextMeshProUGUI lvl;
    public TextMeshProUGUI xp;
    public StatsController stats;
    [Header("Multipliers")]
    [Range(1f,300f)]
    public float additionMultiplier = 300;
    [Range(2f, 4f)]
    public float powerMultiplier = 2;
    [Range(7f, 14f)]
    public float divisionMultiplier = 7;


    // Start is called before the first frame update

    private void Awake()
    {
        FrontxPBar = GameObject.Find("FrontxPBar").GetComponent<Image>();
        BackxPBar = GameObject.Find("BackxPBar").GetComponent<Image>();
        Frame = GameObject.Find("FramexP").GetComponent<Image>();
        backGround = GameObject.Find("ExpBackGround").GetComponent<Image>();
        lvl = GameObject.Find("level").GetComponent<TextMeshProUGUI>();
        xp = GameObject.Find("requiredExp").GetComponent<TextMeshProUGUI>();
    }
    void Start()
    {
        currentxp = 0;
        level = 1;
        requiredxp = CalculateRequiredXp();
        FrontxPBar.fillAmount = currentxp / requiredxp;
        BackxPBar.fillAmount = currentxp / requiredxp;
        lvl.text = "level " + level;
        xp.text = currentxp + " / " + requiredxp;
        stats = gameObject.GetComponent<StatsController>();
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateXpUI();
        if (level < maxLevel)
        {
          
            if (currentxp > requiredxp)
            {

                levelup();
                backGround.rectTransform.sizeDelta = new Vector2(backGround.rectTransform.sizeDelta.x + 50f, backGround.rectTransform.sizeDelta.y);
                backGround.rectTransform.transform.position = new Vector2(backGround.rectTransform.transform.position.x + 25f, backGround.rectTransform.transform.position.y);
                BackxPBar.rectTransform.sizeDelta = new Vector2(BackxPBar.rectTransform.sizeDelta.x + 50f, BackxPBar.rectTransform.sizeDelta.y);
                BackxPBar.rectTransform.transform.position = new Vector2(BackxPBar.rectTransform.transform.position.x + 25f, BackxPBar.rectTransform.transform.position.y);
                Frame.rectTransform.sizeDelta = new Vector2(Frame.rectTransform.sizeDelta.x + 50f, Frame.rectTransform.sizeDelta.y);
                Frame.rectTransform.transform.position = new Vector2(Frame.rectTransform.transform.position.x + 25f, Frame.rectTransform.transform.position.y);
                FrontxPBar.rectTransform.sizeDelta = new Vector2(FrontxPBar.rectTransform.sizeDelta.x + 50f, FrontxPBar.rectTransform.sizeDelta.y);
                FrontxPBar.rectTransform.transform.position = new Vector2(FrontxPBar.rectTransform.transform.position.x + 25f, FrontxPBar.rectTransform.transform.position.y);
            }
        }
        else
        {
            currentxp = requiredxp;
        }

    }
    public void UpdateXpUI()
    {
        if (level < maxLevel)
        {
            float xpfraction = currentxp / requiredxp;
            float fxp = FrontxPBar.fillAmount;

            if (fxp < xpfraction)
            {
                delaytimer += Time.deltaTime;
                BackxPBar.fillAmount = xpfraction;
                if (delaytimer > 1.5)
                {
                    lerptimer += Time.deltaTime;
                    float percentcomplete = lerptimer / 4;
                    FrontxPBar.fillAmount = Mathf.Lerp(fxp, BackxPBar.fillAmount, percentcomplete);
                }
            }
            xp.text = currentxp + " / " + requiredxp;

        }
        else
        {
            lerptimer += Time.deltaTime;
            float percentComplete = lerptimer / 4;
            FrontxPBar.fillAmount = Mathf.Lerp(FrontxPBar.fillAmount, 1, percentComplete);
            xp.text = currentxp + " / " + requiredxp;

        }
    }
    public void GainExperienceFlatRate(float xpGain)
    {
            currentxp += xpGain;
            lerptimer = 0f;
            delaytimer = 0f;
    }
    public void GainExperienceScalable(float xpGained,int passedlevel)
    {
        if (passedlevel < level)
        {
            float multiplier = 1 + (level - passedlevel) * 0.1f;
            currentxp +=xpGained*multiplier;
        }
        else
        {
            currentxp+=xpGained;
        }
        lerptimer = 0f;
    }
    public void levelup()
    {
        level++;
        FrontxPBar.fillAmount = 0f;
        BackxPBar.fillAmount = 0f;
        currentxp = Mathf.RoundToInt(currentxp - requiredxp);
        //add any skill point attributes here
        GetComponent<PlayerHealth>().IncreaseHealth(level);
        requiredxp = CalculateRequiredXp();
        lvl.text = "level " + level;
        stats.strength += 0.09f * level;
    }
    private int CalculateRequiredXp()
    {
        int solveForRequiredXp = 0;
        for(int levelCycle = 1; levelCycle <= level; levelCycle++)
        {
            solveForRequiredXp +=(int)Mathf.Floor(levelCycle+additionMultiplier*Mathf.Pow(powerMultiplier,levelCycle/divisionMultiplier));
        }
        return solveForRequiredXp / 4;
    }


}
