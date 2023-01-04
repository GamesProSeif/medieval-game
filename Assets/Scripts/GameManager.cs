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
    private bool reloading = false;

    [Header("Settings")]
    public float wellRestoreRadius = 5;
    public float wellRestoreCooldown;
    public LayerMask whatIsPlayer;
    public GameObject gameOverText;

    //@TODO: implement settings for global variables (ie: healthPotionMultiplier)

    private void Start()
    {
        GameObject player = GameObject.Find("Player");
        itemController = player.GetComponent<ItemController>();
        combatController = player.GetComponent<CombatController>();
        statsController = player.GetComponent<StatsController>();
        playerHealth = player.GetComponent<PlayerHealth>();
        wells = GameObject.FindGameObjectsWithTag("Well");

        AddInventoryItems();
    }

    private void Update()
    {
        // Handle Well Range for Restore
        foreach (var well in wells)
        {
            var colliders = Physics.OverlapSphere(well.transform.position, wellRestoreRadius, whatIsPlayer);
            if (colliders.Length > 0)
                WellRestore();
        }

        // Restart Game
        if (Input.GetKeyDown(KeyCode.Escape) && !reloading)
        {
            reloading = true;
            SceneManager.LoadScene("level 1");
        }

        // Show mouse and game over screen
        if (statsController.killed)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
        gameOverText.SetActive(statsController.killed);
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

    // Back to menu button
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
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
}
