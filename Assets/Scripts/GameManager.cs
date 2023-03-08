using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private ItemController itemController;
    private CombatController combatController;
    private StatsController statsController;
    private PlayerHealth playerHealth;
    private GameObject[] wells;
    
    private bool wellRestoring = false;
    private float elapsed = 0;
    private static bool GameIsPaused = false;
    [Header("Settings")]
    public float wellRestoreRadius = 5;
    public float wellRestoreCooldown;
    public LayerMask whatIsPlayer;
    public GameObject gameOverText;
    public GameObject pauseMenu;
    public GameObject blackScreen;
    public Animator animScreen;
    public Animator deathanim;
    public Animator deathTip;
    public TextMeshProUGUI _Death;
    public TextMeshProUGUI DeathTextTip;
    //@TODO: implement settings for global variables (ie: healthPotionMultiplier)

    private void Start()
    {
        GameObject player = GameObject.Find("Player");
        itemController = player.GetComponent<ItemController>();
        combatController = player.GetComponent<CombatController>();
        statsController = player.GetComponent<StatsController>();
        playerHealth = player.GetComponent<PlayerHealth>();
        wells = GameObject.FindGameObjectsWithTag("Well");
        blackScreen.SetActive(true);
        animScreen.SetBool("fadeIn", true);
        animScreen.SetBool("fadeOut", false);

        AddInventoryItems();
    }

    private void Update()
    {
        if (!statsController.killed)
        {
            if (elapsed <= 2f)
                elapsed += Time.deltaTime;
            else
            {
                animScreen.SetBool("fadeIn", false);
                blackScreen.SetActive(false);
            }
            // Handle Well Range for Restore
            foreach (var well in wells)
            {
                var colliders = Physics.OverlapSphere(well.transform.position, wellRestoreRadius, whatIsPlayer);
                if (colliders.Length > 0)
                    WellRestore();
            }
        }
        else
        {
             StartCoroutine(DeathCouroutine());
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                ResumeGame();
            }
            else PauseGame();
        }
    }

    // Well Restore implementation
    private void WellRestore()
    {
        if (wellRestoring) return;
        wellRestoring = true;
        playerHealth.RestoreHealth(statsController.maxHealth);
        InventoryItem healthPotion = itemController.findByName("HealthPotion");
        if (healthPotion == null) healthPotion = new InventoryItem("HealthPotion", 5, null, true);
        healthPotion.count = 5;
        Invoke(nameof(ResetWellRestore), wellRestoreCooldown);
    }

    // For cooldown
    private void ResetWellRestore()
    {
        wellRestoring = false;
    }

    // Add starter items
    private void AddInventoryItems()
    {
        InventoryItem[] items =
        {
            new InventoryItem("Scitmar", 1, null, false),
            new InventoryItem("Arrow", 50, combatController.arrowPrefab, true),
            new InventoryItem("FireGrenade", 10, combatController.fireGrenadePrefab, true),
            new InventoryItem("HealthPotion", 10, null, true),
            new InventoryItem("SpeedPotion", 5, null, true),
            new InventoryItem("StrengthPotion", 5, null, true),

        };

        foreach (var item in items)
            itemController.addToList(item);
    }

    // Used for debugging to show well restore range
    private void OnDrawGizmosSelected()
    {
        wells = GameObject.FindGameObjectsWithTag("Well");
        Gizmos.color = Color.blue;
        foreach (var well in wells)
            Gizmos.DrawWireSphere(well.transform.position, wellRestoreRadius);
    }

    //Menu Functions
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1f;
        GameIsPaused = false;
        EnableCurser();
    }

    public void RestartGame()
    {
        
        SceneManager.LoadScene("level 1");
        blackScreen.SetActive(true);
        Time.timeScale = 1f;
        GameIsPaused = false;
        DisableCurser();
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        DisableCurser();
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        EnableCurser();
    }



    // curser Functions

    public void DisableCurser()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void EnableCurser()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    IEnumerator DeathCouroutine()
    {
        _Death.gameObject.SetActive(true);
        deathanim.SetBool("isDead", true);
        yield return new WaitForSeconds(4f);
        _Death.gameObject.SetActive(false);
        blackScreen.SetActive(true);
        animScreen.SetBool("fadeOut", true);
        animScreen.SetBool("fadeIn", false);
        yield return new WaitForSeconds(1f);
        DeathTextTip.gameObject.SetActive(true);
        deathTip.SetBool("Tip", true);
        yield return new WaitForSeconds(5f);
        RestartGame();
        statsController.killed = false;
    }


}
