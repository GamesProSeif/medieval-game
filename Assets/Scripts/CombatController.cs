using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.InteropServices;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    public enum Weapon { Sword, Bow, FireGrenade, None }
    public Weapon currentWeapon = Weapon.Bow;
    private StatsController playerStats;
    private bool readyToAttack = true;
    private bool usedHealthPotion;
    private bool usedSpeedPotion;
    private bool usedStrengthPotion;
    private List<GameObject> thrownProjectiles = new List<GameObject>();
    private ItemController itemController;
    private PlayerHealth playerHealth;
    private MovementController movementController;
    private Animator animator;
    [Header("Keys")]
    public KeyCode attack = KeyCode.Mouse0;
    public KeyCode equipSword = KeyCode.Alpha1;
    public KeyCode equipBow = KeyCode.Alpha2;
    public KeyCode equipFireGrenade = KeyCode.Alpha3;
    public KeyCode drinkHealthPotion = KeyCode.H;
    public KeyCode drinkSpeedPotion = KeyCode.J;
    public KeyCode drinkBuffPotion = KeyCode.K;


    [Header("References")]
    public GameObject attackPoint;
    public GameObject arrowPrefab;
    public GameObject swordPrefab;
    public GameObject bowPrefab;
    public GameObject fireGrenadePrefab;
    public GameObject fireParticlesPrefab;
    public Transform cam;
    

    [Header("General Settings")]
    public int maxProjectiles;

    public class WeaponSettings
    {
        public int damage;
        public float cooldown;
    }
    [Serializable]
    public class BladeWeaponSettings : WeaponSettings {
        public float range;
    }

    [Serializable]
    public class RangedWeaponSettings : WeaponSettings
    {
        public float forwardForce;
        public float upwardForce;
        public float offsetDelay;
    }

    [Serializable]
    public class GrenadeWeaponSettings : RangedWeaponSettings
    {
        public float duration;
    }

    public enum PotionType { Health, Speed, Strength }

    [Serializable]

    public class PotionSettings 
    {

        public float buffMultiplier;
        public float buffTime;

    }

    [Header("Weapon Settings")]
    public BladeWeaponSettings swordSettings;
    public RangedWeaponSettings bowSettings;
    public GrenadeWeaponSettings fireGrenadeSettings;
    public PotionSettings healthPotionSettings;
    public PotionSettings speedPotionSettings;
    public PotionSettings strengthPotionSettings;


    private void Start()
    {
        playerStats = GetComponent<StatsController>();
        itemController = GetComponent<ItemController>();
        playerHealth = GetComponent<PlayerHealth>();
        movementController = GetComponent<MovementController>();
        animator = GameObject.Find("PlayerBody").gameObject.GetComponent<Animator>();
        currentWeapon = Weapon.None;
    }
    private void Update()
    {
        if (playerStats.killed) return;
        if (Input.GetKeyDown(attack))
            Attack();
        if (Input.GetKeyDown(equipSword))
            EquipWeapon(Weapon.Sword); 
        else if (Input.GetKeyDown(equipBow))
            EquipWeapon(Weapon.Bow);
        else if (Input.GetKeyDown(equipFireGrenade))
            EquipWeapon(Weapon.FireGrenade);
        else if (Input.GetKeyDown(drinkHealthPotion))
            DrinkPotion(PotionType.Health);
        else if (Input.GetKeyDown(drinkSpeedPotion))
            DrinkPotion(PotionType.Speed);
        else if (Input.GetKeyDown(drinkBuffPotion))
            DrinkPotion(PotionType.Strength);
    }

    private void EquipWeapon(Weapon weapon)
    {
        switch (weapon)
        {
            case Weapon.Sword:
                if (itemController.findByName("Scitmar") == null || currentWeapon == Weapon.Sword)
                    return;
                animator.SetBool("isBowEquiped", false);
                animator.SetBool("isSwordEquiped", true);
                ResetActivePrefabs(Weapon.Sword);
                animator.SetBool("currentWeaponSword", true);
                FindObjectOfType<AudioManager>().Play("SwordDraw");
                swordPrefab.SetActive(true);
                break;
            case Weapon.Bow:
                if (itemController.findByName("Bow") == null || currentWeapon == Weapon.Bow)
                    return;
                animator.SetBool("isSwordEquiped", false);
                animator.SetBool("isBowEquiped", true);
                animator.SetBool("currentWeaponBow", true);
                ResetActivePrefabs(Weapon.Bow);
                bowPrefab.SetActive(true);
                break;
            case Weapon.FireGrenade:
                if (itemController.findByName("FireGrenade") == null
                    || itemController.findByName("FireGrenade").count == 0
                    || currentWeapon == Weapon.FireGrenade)
                    return;
                ResetActivePrefabs(Weapon.FireGrenade);
                break;
        }
        currentWeapon = weapon;
    }

    private void ResetActivePrefabs(Weapon weapon)
    {
        GameObject[] prefabs = { swordPrefab, bowPrefab };
        foreach (GameObject prefab in prefabs)
            prefab.SetActive(false);
    }

    private void Attack()
    {
        if (currentWeapon == Weapon.None) return;
        if (!readyToAttack) return;
        animator.SetBool("isAttacking", true);
        readyToAttack = false;
        float strengthModifer = GetComponent<StatsController>().strength;
        WeaponSettings weaponSettings = null;
        if (currentWeapon == Weapon.Sword)
        {
            weaponSettings = swordSettings;
            BladeWeaponSettings settings = (BladeWeaponSettings)weaponSettings;

            var collisions = Physics.OverlapSphere(transform.position
            + transform.forward * (swordSettings.range), settings.range);

            bool hit = false;
            foreach (var collision in collisions)
            {
                if (collision.GetComponent<StatsController>() == null || collision.name == "Player")
                    continue;
                collision.GetComponent<StatsController>()
                    .TakeDamage(Convert.ToInt32(settings.damage * playerStats.strength), gameObject);
                hit = true;
            }
            if (hit)
                FindObjectOfType<AudioManager>().Play("SwordAttack");
        }
        else
        {
            GameObject objectToThrow = null;
            RangedWeaponSettings rangedSettings = null;
            InventoryItem item = null;
            if (currentWeapon == Weapon.Bow)
            {
                item = itemController.findByName("Arrow");
                weaponSettings = bowSettings;
                rangedSettings = (RangedWeaponSettings) weaponSettings;
                objectToThrow = arrowPrefab;
                
            }
            else if (currentWeapon == Weapon.FireGrenade)
            {
                item = itemController.findByName("FireGrenade");
                weaponSettings = fireGrenadeSettings;
                rangedSettings = (RangedWeaponSettings)weaponSettings;
                objectToThrow = fireGrenadePrefab;
                
            }
            if (item == null || item.count <= 0) return;
            itemController.decrementCount(item);

            animator.SetBool("isAttacking", true);
            StartCoroutine(ThrowProjectile(objectToThrow, rangedSettings));
        }
        Invoke(nameof(ResetAttack), weaponSettings.cooldown);
    }

    private IEnumerator ThrowProjectile(GameObject objectToThrow, RangedWeaponSettings rangedSettings)
    {
        yield return new WaitForSeconds(rangedSettings.offsetDelay);

        GameObject projectile = Instantiate(objectToThrow, attackPoint.transform.position, cam.transform.rotation * objectToThrow.transform.rotation);
        ProjectileAddon projAddon = projectile.GetComponent<ProjectileAddon>();
        projAddon.SetInitiator(gameObject, rangedSettings.damage);
        thrownProjectiles.Add(projectile);
        if (thrownProjectiles.Count > maxProjectiles)
        {
            GameObject firstProjectile = thrownProjectiles[0];
            thrownProjectiles.Remove(firstProjectile);
            Destroy(firstProjectile);
        }

        // Get rigidbody component
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        // Calculate direction
        Vector3 forceDirection = cam.transform.forward;

        /*
        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, 500f))
        {
            forceDirection = (hit.point - attackPoint.transform.position).normalized;
        }
        */

        // Add force
        Vector3 forceToAdd =
            forceDirection * rangedSettings.forwardForce
            + transform.up * rangedSettings.upwardForce;

        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);
    }

    private void ResetAttack()
    {
        readyToAttack = true;
        animator.SetBool("isAttacking", false);
    }

    private void DrinkPotion(PotionType potion)
    {
        InventoryItem item = null;
        if (potion == PotionType.Health)
        {
            item = itemController.findByName("HealthPotion");
            if (item == null || item.count <= 0) return;
            playerHealth.RestoreHealth(healthPotionSettings.buffMultiplier);
        }

        if(potion == PotionType.Speed)
        {
            item = itemController.findByName("SpeedPotion");
            if (item == null || item.count <= 0) return;
            increaseSpeed();   
        }
   
        if (potion == PotionType.Strength)
        {
            item = itemController.findByName("StrengthPotion");
            if (item == null || item.count <= 0) return;
            increaseStrength();
        }
        itemController.decrementCount(item);
    }

    private void increaseSpeed()
    {
        if (usedSpeedPotion) return;
        movementController.Speed *= speedPotionSettings.buffMultiplier;
        usedSpeedPotion = true;
        Invoke(nameof(RevertSpeed), speedPotionSettings.buffTime);
        
      
    }
    
    private void RevertSpeed()
    {
        movementController.Speed /= speedPotionSettings.buffMultiplier;
        usedSpeedPotion = false;
    }    
    
    private void increaseStrength()
    {
        if (usedStrengthPotion) return;
        playerStats.strength *= strengthPotionSettings.buffMultiplier;   
        usedStrengthPotion = true;
        Invoke(nameof(RevertStrength), strengthPotionSettings.buffTime);
      
    }

    private void RevertStrength()
    {
        playerStats.strength /= strengthPotionSettings.buffMultiplier;
        usedStrengthPotion = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position
            + transform.forward * (swordSettings.range), swordSettings.range);
    }
}
