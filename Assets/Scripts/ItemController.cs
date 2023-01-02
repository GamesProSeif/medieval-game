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
    public RawImage image4;
    public RawImage image5;
    public RawImage image6;


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<Item>() != null) {
            Item temp = collision.gameObject.GetComponent<Item>();
            InventoryItem x = findByName(temp.itemName);
            if (temp.stackable)
            {           
                if (x != null)
                    x.count += temp.count;
                else addToList(temp);
            }
            else
            {
                if (x == null)
                    addToList(temp);
                else return;

            }
            Destroy(temp.gameObject);
        }      
    }

    void updateUI()
    {
        image1.gameObject.SetActive(findByName("Scitmar") != null);
        image2.gameObject.SetActive(findByName("Bow") != null);
        image3.gameObject.SetActive(findByName("FireGrenade") != null);
        image4.gameObject.SetActive(findByName("HealthPotion") != null);
        image5.gameObject.SetActive(findByName("SpeedPotion") != null);
        image6.gameObject.SetActive(findByName("StrengthPotion") != null);
    }

    public InventoryItem findByName(string name)
    {
        foreach(var item in items)
            if (item.itemName == name)
                return item;
        return null;
    }

    public void addToList(Item item)
    {
        InventoryItem temp1 = new InventoryItem();
        temp1.itemName = item.name;
        temp1.count = item.count;
        temp1.stackable = item.stackable;
        temp1.prefab = item.prefab;
        addToList(temp1);
    }

    public void addToList(InventoryItem item)
    {
        if (item.stackable && findByName(item.itemName) != null)
        {
            findByName(item.itemName).count += item.count;
        }
        else
        {
            Debug.Log(item.itemName);
            items.Add(item);
        }
        updateUI();
        Debug.Log(findByName("Bow"));
    }

    public void decrementCount(InventoryItem item)
    {
        item.count--;
        if (item.count <= 0)
            items.Remove(item);
    }

}

