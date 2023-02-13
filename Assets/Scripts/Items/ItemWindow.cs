/*
 *  A window that holds a list of items in the ui : Chris
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemWindow : MonoBehaviour
{
    public int items_x;
    public int items_y;
    public bool horizontal = false;

    public GameObject parent;

    public Slider scrollbar;

    public ItemBox itembox;

    List<ItemBox> boxes = new List<ItemBox>();

    Inventory inventory;

    int box_selected = -1;

    Action RefreshItemCallback = null; // this is an optional fuction that can be called when RefreshItems is called. I use this in places like where the crafting screen needs to know if items have been moved out of the forge so it can enable / disable the craft button

    public void Init(Inventory _inventory, Action _RefreshItemCallback = null)
    {
        inventory = _inventory;
        RefreshItemCallback = _RefreshItemCallback;

        // end

        boxes = new List<ItemBox>();

        for (int i = 0; i < items_x * items_y; i++) // go to 28 which is how many will fit in the window
        {
            var copy = Instantiate(itembox.gameObject); // make a copy
            copy.SetActive(true); // set it active
            copy.name = "item " + (i + 1); // name the object
            copy.transform.SetParent(gameObject.transform); // set parent to this

            var box = copy.GetComponent<ItemBox>();
            box.Init(this);

            boxes.Add(box); // add it to the list
        }

        if (scrollbar != null)
            scrollbar.value = 0; // set up the window to be at the beginning of the list
        ScrollBar(); // call scrollbar to refresh the items
    }

    public void DisableItemDrop()
    {
        foreach (ItemBox i in boxes)
            i.SetCanDropItems(false);
    }

    public void SetBoxSelected(int b)
    {
        foreach (ItemBox box in boxes)
            box.SetHighlight(false);

        if (b == box_selected || b == -1) // if we try to select the same box again unselect it
        {
            box_selected = -1;
            return;
        }

        box_selected = b;
        boxes[box_selected].SetHighlight(true);
    }

    public Item GetSelectedItem()
    {
        if (box_selected == -1) return new Item();

        return boxes[box_selected].item;
    }

    public void AddItem(Item item)
    {
        inventory.AddItem(item.type, item.number);
    }

    public (int, int) PutItemAt(Item.Type type, int number, int index)
    {
        return inventory.PutItemAt(type, number, index);
    }

    public void ScrollBar()
    {
        RefreshItems(scrollbar != null ? scrollbar.value : 0);
    }

    public void RefreshItems(float percent = -1)
    {
        if (inventory == null) return;

        if (scrollbar != null)
            if (percent < 0) percent = scrollbar.value;

        int columns;
        int top;

        if (!horizontal)
        {
            columns = inventory.CountMaxItems() / items_x + (inventory.CountMaxItems() % items_x > 0 ? 1 : 0); // get total number of columns

            top = (int)(percent * columns);

            if (top > columns - items_y) top = columns - items_y;
            if (top < 0) top = 0;
        }
        else
        {
            columns = inventory.CountMaxItems() / items_y + (inventory.CountMaxItems() % items_y > 0 ? 1 : 0); // get total number of columns

            top = (int)(percent * columns);

            if (top > columns - items_x) top = columns - items_x;
            if (top < 0) top = 0;
        }

        for (int i = 0; i < items_x * items_y; i++)
        {
            int itm = i + top * items_x; // get a reference to the right item

            if (horizontal) itm = i + top * items_y;

            boxes[i].Fill(itm, inventory.GetItemAt(itm)); // put the item in the box
        }

        RefreshItemCallback?.Invoke(); // if we passed a function here, fire it off
    }
}
