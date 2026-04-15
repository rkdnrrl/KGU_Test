using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory instance;

    List<Item> itemlist = new List<Item>();

    private void Awake()
    {
        instance = this;
    }

    public void AddItem(Item item)
    {
        // 유지보수 시 의도를 빠르게 파악할 수 있도록 정리한 주석입니다.
        itemlist.Add(item);
    }

    public void RemoveItem(Item item)
    {
        Item getItem = null;

        foreach (var ite in itemlist)
        {
            if(ite.name == item.name)
            {
                // 유지보수 시 의도를 빠르게 파악할 수 있도록 정리한 주석입니다.
                getItem = ite;
            }
        }

        itemlist.Remove(getItem);
    }
}


