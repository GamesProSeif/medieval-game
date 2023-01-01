using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    public enum Weapon { Sword, Bow, FireGrenade }
    public Weapon currentWeapon = Weapon.Bow;
    private StatsController playerStats;
    private bool readyToAttack = true;
    private List<GameObject> thrownProjectiles = new List<GameObject>();

    [Header("Keys")]
    public KeyCode attack = KeyCode.Mouse0;
    public KeyCode equipSword = KeyCode.Alpha1;
    public KeyCode equipBow = KeyCode.Alpha2;
    public KeyCode equipFireGrenade = KeyCode.Alpha3;

    [Header("References")]
    public GameObject attackPoint;
    public GameObject arrowPrefab;
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
        public int ammo;
        public float forwardForce;
        public float upwardForce;
    }

    [Serializable]
    public class GrenadeWeaponSettings : RangedWeaponSettings
    {
        public float duration;
    }

    [Header("Weapon Settings")]
    public BladeWeaponSettings swordSettings;
    public RangedWeaponSettings bowSettings;
    public GrenadeWeaponSettings fireGrenadeSettings;

    private void Start()
    {
        playerStats = GetComponent<StatsController>();
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
    }

    private void EquipWeapon(Weapon weapon)
    {
        currentWeapon = weapon;
    }

    private void Attack()
    {
        if (!readyToAttack) return;

        readyToAttack = false;
        float strengthModifer = GetComponent<StatsController>().strength;
        WeaponSettings weaponSettings = null;
        if (currentWeapon == Weapon.Sword)
        {
            weaponSettings = swordSettings;
            BladeWeaponSettings settings = (BladeWeaponSettings)weaponSettings;

            var collisions = Physics.OverlapSphere(transform.position, settings.range);
            
            foreach (var collision in collisions)
            {
                if (collision.GetComponent<StatsController>() == null || collision.name == "Player") continue;
                collision.GetComponent<StatsController>()
                    .TakeDamage(Convert.ToInt32(settings.damage * playerStats.strength), gameObject);
            }
        }
        else
        {
            GameObject objectToThrow = null;
            RangedWeaponSettings rangedSettings = null;
            if (currentWeapon == Weapon.Bow)
            {
                weaponSettings = bowSettings;
                rangedSettings = (RangedWeaponSettings) weaponSettings;
                objectToThrow = arrowPrefab;
            }
            else if (currentWeapon == Weapon.FireGrenade)
            {
                weaponSettings = fireGrenadeSettings;
                rangedSettings = (RangedWeaponSettings)weaponSettings;
                objectToThrow = fireGrenadePrefab;
            }

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
            rangedSettings.ammo--;
        }
        Invoke(nameof(ResetAttack), weaponSettings.cooldown);
    }

    private void ResetAttack()
    {
        readyToAttack = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position
            + transform.forward * (swordSettings.range), swordSettings.range);
    }
}
