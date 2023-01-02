using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private ItemController itemController;
    private CombatController combatController;
    private StatsController statsController;
    private PlayerHealth playerHealth;
    private GameObject[] wells;
    private bool wellRestoring = false;

    [Header("Settings")]
    public float wellRestoreRadius = 5;
    public float wellRestoreCooldown;
    public LayerMask whatIsPlayer;

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
        foreach (var well in wells)
        {
            var colliders = Physics.OverlapSphere(well.transform.position, wellRestoreRadius, whatIsPlayer);
            if (colliders.Length > 0)
                WellRestore();
        }
    }

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

    private void ResetWellRestore()
    {
        wellRestoring = false;
    }

    private void AddInventoryItems()
    {
        InventoryItem[] items =
        {
            new InventoryItem("Scitmar", 1, null, false),
            new InventoryItem("Bow", 1, null, false),
            new InventoryItem("Arrow", 25, combatController.arrowPrefab, true),
            new InventoryItem("FireGrenade", 10, combatController.fireGrenadePrefab, true),
            new InventoryItem("HealthPotion", 5, null, true),
            new InventoryItem("SpeedPotion", 5, null, true),
            new InventoryItem("StrengthPotion", 5, null, true),

        };

        foreach (var item in items)
            itemController.addToList(item);
    }

    private void OnDrawGizmosSelected()
    {
        wells = GameObject.FindGameObjectsWithTag("Well");
        Gizmos.color = Color.blue;
        foreach (var well in wells)
            Gizmos.DrawWireSphere(well.transform.position, wellRestoreRadius);
    }
}
