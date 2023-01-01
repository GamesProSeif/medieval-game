using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemController : MonoBehaviour
{
    public List<InventoryItem> items = new List<InventoryItem>(); 
    public RawImage image1;
    public RawImage image2;
    public RawImage image3;



    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<Item>() != null) {
            Item temp = collision.gameObject.GetComponent<Item>();
            InventoryItem temp1 = new InventoryItem();
            temp1.itemName=temp.name;
            temp1.count=temp.count;
            temp1.stackable=temp.stackable;
            temp1.prefab=temp.prefab;
            items.Add(temp1);
            updateUI();
            Destroy(temp.gameObject);
        }      
    }

    void updateUI()
    {

        if (findByName("Scitmar"))
        {
            image1.gameObject.SetActive(true);
        }

        if (findByName("Bow"))
        {
            image2.gameObject.SetActive(true);
        }
        if (findByName("FireBomb"))
            image3.gameObject.SetActive(true);
    }

    private bool findByName(string name)
    {
        foreach(var item in items)
        {
            if (item.itemName == name)
            {
                return true;
            }
          
        }
        return false;
    }
}

