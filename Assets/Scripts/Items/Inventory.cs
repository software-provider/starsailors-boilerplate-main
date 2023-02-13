/*
 *  This is a container for a list of items. It could be used by a player, a store, a container etc. : Chris
 */

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    List<Item> items = new List<Item>();

    public void Init(int max_items)
    {
        for (int i = 0; i < max_items; i++)
            items.Add(new Item());
    }

    public int CountValidItems()
    {
        int valid = 0;

        for (int i = 0; i < CountMaxItems(); i++)
            valid += items[i].IsValid() ? 1 : 0;

        return valid;
    }

    public int CountMaxItems() { return items.Count; }

    public Item GetItemAt(int index) { return (index >= 0 && index < CountMaxItems()) ? items[index] : null; }

    public (int, int) PutItemAt(Item.Type type, int number, int index) // index -1 means first index with space
    {
        if (index < 0 || index >= CountMaxItems()) return (-1, 0); // out of range

        if (items[index].IsValid())
        {
            if (items[index].type != type)
                return (0, 0); // mismatch, we can't put it in
        }
        else
            items[index].Set(type); // empty, fill with the new item

        return (1, items[index].Plus(number)); // we did it, return the remainder
    }

    public void AddItem(Item.Type type, int number)
    {
        for (int i = 0; i < items.Count && number > 0; i++)
        {
            if (items[i].type == type)
                number = items[i].Plus(number);
            else if (items[i].type == Item.Type.none)
            {
                int n = number <= 99 ? number : 99;
                items[i].Set(type, n);
                number -= n;
            }
        }
    }

    public void ClearItems()
    {
        foreach (var i in items)
        {
            i.Clear();
        }
    }
}
