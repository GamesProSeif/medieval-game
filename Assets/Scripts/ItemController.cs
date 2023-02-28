using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;

public class ItemController : MonoBehaviour
{
    public List<InventoryItem> items = new List<InventoryItem>(); 
    public RawImage scitmarImage;
    public RawImage bowImage;
    public RawImage fireGrenadeImage;
    public RawImage healthPotionImage;
    public RawImage speedPotionImage;
    public RawImage strengthPotionImage;
    public TextMeshProUGUI fireGrenadeAmmoText;
    public TextMeshProUGUI arrowAmmoText;
    public TextMeshProUGUI healthPotionCount;
    public TextMeshProUGUI speedPotionCount;
    public TextMeshProUGUI strengthPotionCount;
    public TextMeshProUGUI pickup;
    public SelectionManger selection;

    private void Update()
    {
       if (selection.mainSelection != null)
        {
            pickup.gameObject.SetActive(true);
            if(Input.GetKey(KeyCode.E))
            {
                if (selection.mainSelection.gameObject.GetComponent<Item>() != null)
                {
                    Item temp = selection.mainSelection.gameObject.GetComponent<Item>();
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
        }
       else pickup.gameObject.SetActive(false);
    }


    void updateUI()
    {
        if (items.Count == 0) hideUI();
        scitmarImage.gameObject.SetActive(findByName("Scitmar") != null);
        bowImage.gameObject.SetActive(findByName("Bow") != null);
        arrowAmmoText.gameObject.SetActive(findByName("Bow") != null && findByName("Arrow") != null);
        arrowAmmoText.text = findByName("Arrow") != null ? findByName("Arrow").count.ToString() : "0";

        fireGrenadeImage.gameObject.SetActive(findByName("FireGrenade") != null);
        fireGrenadeAmmoText.gameObject.SetActive(findByName("FireGrenade") != null);
        fireGrenadeAmmoText.text = findByName("FireGrenade") != null ? findByName("FireGrenade").count.ToString() : "0";

        healthPotionImage.gameObject.SetActive(findByName("HealthPotion") != null);
        healthPotionCount.gameObject.SetActive(findByName("HealthPotion") != null);
        healthPotionCount.text = findByName("HealthPotion") != null ? findByName("HealthPotion").count.ToString() : "0";

        speedPotionImage.gameObject.SetActive(findByName("SpeedPotion") != null);
        speedPotionCount.gameObject.SetActive(findByName("SpeedPotion") != null);
        speedPotionCount.text = findByName("SpeedPotion") != null ? findByName("SpeedPotion").count.ToString() : "0";

        strengthPotionImage.gameObject.SetActive(findByName("StrengthPotion") != null);
        strengthPotionCount.gameObject.SetActive(findByName("StrengthPotion") != null);
        strengthPotionCount.text = findByName("StrengthPotion") != null ? findByName("StrengthPotion").count.ToString() : "0";
    }

    void hideUI()
    {
        scitmarImage.gameObject.SetActive(false);
        bowImage.gameObject.SetActive(false);
        arrowAmmoText.gameObject.SetActive(false);
        fireGrenadeImage.gameObject.SetActive(false);
        fireGrenadeAmmoText.gameObject.SetActive(false);
        healthPotionImage.gameObject.SetActive(false);
        healthPotionCount.gameObject.SetActive(false);
        speedPotionImage.gameObject.SetActive(false);
        speedPotionCount.gameObject.SetActive(false);
        strengthPotionImage.gameObject.SetActive(false);
        strengthPotionCount.gameObject.SetActive(false);
        
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
            items.Add(item);
        }
        updateUI();
    }

    public void decrementCount(InventoryItem item)
    {
        item.count--;
        if (item.count <= 0)
            items.Remove(item);
        updateUI();
    }

}

