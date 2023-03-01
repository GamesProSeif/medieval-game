using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
   
    public string itemName;
    public int count;
    public GameObject prefab;
    public bool stackable;
    public bool rotatable;

    private void Update()
    {
        if(!rotatable)
            return;
        gameObject.transform.rotation *= Quaternion.AngleAxis(1, Vector3.up);
    }
}
