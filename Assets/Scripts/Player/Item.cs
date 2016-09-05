using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Item
{
    public string name;
    public int ItemId;
    public int ItemValue;
    public bool ItemStacked;
    public int ItemValueMax;

    public Item(int pItemId, string pname, bool pItemStacked, int pItemValueMax)
    {

        ItemId = pItemId;
        name = pname;
        ItemValue = 1;
        ItemStacked = pItemStacked;
        ItemValueMax = pItemValueMax;
    }

    public Item()
    {
        ItemId = -1;
    }
}

public class ItemDataBase : MonoBehaviour
    {
        public List<Item> items = new List<Item>();

    void Start()
    {
        items.Add(new Item(0,"Nuts",true,1));
        items.Add(new Item(1,"Speed",true,0));
        items.Add(new Item(2,"NutStack",true,0));
    }
    }


