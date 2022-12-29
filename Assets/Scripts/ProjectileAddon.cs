using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class ProjectileAddon : MonoBehaviour
{
    public int damage;
    public int radius;
    public GameObject fireParticles;
    private Rigidbody rb;
    private bool targetHit;

    public enum WeaponType { Projectile, Grenade };
    public WeaponType type;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (type == WeaponType.Projectile)
        {
            if (targetHit)
                return;
            targetHit = true;
            if (collision.gameObject.GetComponent<Enemy>() != null)
            {
                Enemy enemy = collision.gameObject.GetComponent<Enemy>();
                enemy.takeDamage(damage);
                Destroy(gameObject);
            }
            rb.isKinematic = true;
            transform.SetParent(collision.transform);
        } else if (type == WeaponType.Grenade)
        {
            if (collision.gameObject.tag == "Ground")
            {
                GameObject particles = Instantiate(fireParticles, transform.position, fireParticles.transform.rotation);

                SphereCollider collider = particles.GetComponent<SphereCollider>();

                // Invoke(nameof(destroy), 5);
                Destroy(gameObject);
                rb.isKinematic = true;
                transform.SetParent(collision.transform);
            }
        }


    }

}
