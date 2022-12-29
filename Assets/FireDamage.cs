using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireDamage : MonoBehaviour
{
    public int damage;
    public List<Enemy> damagedEnemies = new List<Enemy>();


    private void Start()
    {
        StartCoroutine(DamageEnemies());
        ParticleSystem ps = gameObject.GetComponent<ParticleSystem>();
        Invoke(nameof(destroy), ps.main.duration + 1);
    }
    private void destroy()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            damagedEnemies.Add(enemy);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            damagedEnemies.Remove(enemy);
        }
    }

    IEnumerator DamageEnemies()
    {
        foreach (Enemy enemy in damagedEnemies)
        {
            enemy.takeDamage(damage);
            yield return new WaitForSeconds(1);
        }
    }
}
