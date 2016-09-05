using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class Inventory : MonoBehaviour
{

    private List<Item> inventory = new List<Item>();
    private ItemDataBase dataBase;
    public bool addItem;

    void Update()
    {
        if (addItem)
        {
            AddItem(0);
            Debug.Log("hii");
            Debug.Log(inventory.Count);
        }
    }

   

    void AddItem(int id)//With stacking the currentStack count is being added to all of the items of the same id, and more items still fill the inv.
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].ItemStacked && inventory[i].ItemId == id && inventory[i].ItemValue < inventory[i].ItemValueMax)//Stacking
            {
                inventory[i].ItemValue++;//This makes the actual item have a higher current stack
                break;
            }
            if (inventory[i].name == null)
            {
               
                for (int d = 0; d < dataBase.items.Count; d++)
                {
                    if (dataBase.items[d].ItemId == id)
                    {
                        
                        inventory[i] = dataBase.items[d];
                        break;
                    }
                }
                break;
            }
        }
    }

    void RemoveItem(int id)
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].ItemId == id && inventory[i].ItemValue > 1)
            {
                inventory[i].ItemValue--;
                break;
            }
            if (inventory[i].ItemId == id)
            {
                inventory[i] = new Item();
                break;
            }
        }
    }



}
