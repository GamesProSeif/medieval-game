using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderList : MonoBehaviour
{
    public List<GameObject> collisions = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        addCollision(other.gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {
        addCollision(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        removeCollision(other.gameObject);
    }

    private void OnCollisionExit(Collision other)
    {
        removeCollision(other.gameObject);
    }

    private void addCollision(GameObject gameObject)
    {
        if (gameObject.GetComponent<StatsController>())
            collisions.Add(gameObject);
    }

    private void removeCollision(GameObject gameObject)
    {
        collisions.Remove(gameObject);
    }
}
