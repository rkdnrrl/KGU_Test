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
        // ?띾뱷 ?대깽?몃? ?쒖꽌?濡?蹂댁〈?댁빞 ?곹샇?묒슜 寃利????꾨씫 ?щ? 異붿쟻???쎌뒿?덈떎.
        itemlist.Add(item);
    }

    public void RemoveItem(Item item)
    {
        Item getItem = null;

        foreach (var ite in itemlist)
        {
            if(ite.name == item.name)
            {
                // 留덉?留됱쑝濡?李얠? ??ぉ???쒓굅??以묐났 ?뚯? ?곹솴?먯꽌??理쒖냼????媛쒕쭔 李④컧?섍쾶 ?⑸땲??
                getItem = ite;
            }
        }

        itemlist.Remove(getItem);
    }
}


