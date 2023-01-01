using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireDamage : MonoBehaviour
{
    private List<GameObject> gameObjects;
    private float elapsedTime = 0;
    private float damageElapsedTime = 0;
    private StatsController playerStats = null;
    private CombatController playerCombatController = null;

    private void Start()
    {
        gameObjects = GetComponent<ColliderList>().collisions;
        GameObject playerGameObject = GameObject.Find("Player");
        playerStats = playerGameObject.GetComponent<StatsController>();
        playerCombatController = playerGameObject.GetComponent<CombatController>();
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        damageElapsedTime += Time.deltaTime;

        if (elapsedTime >= playerCombatController.fireGrenadeSettings.duration)
        {
            Destroy(gameObject);
            return;
        }

        if (damageElapsedTime >= 1)
        {
            foreach (var gameObject in gameObjects)
            {
                try
                {
                    StatsController gameObjectStats = gameObject.GetComponent<StatsController>();
                    gameObjectStats.TakeDamage(
                        Convert.ToInt32(playerCombatController.fireGrenadeSettings.damage * playerStats.strength),
                        gameObject);
                } catch { }
            }
            damageElapsedTime = 0;
        }
    }
}

