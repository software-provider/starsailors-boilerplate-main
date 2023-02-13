/*
 *  Holds an item in the ui screen
 */

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class ItemBox : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    //public GameObject inventory;
    public List<Sprite> images; // temp to test things

    public Image graphic;
    public Image highlight;
    public TMPro.TextMeshProUGUI number;

    public Item item { get; private set; } = new Item();
    int index = 0;

    public bool can_drop_items { get; private set; } = true;

    public ItemWindow parent { get; private set; } // a reference to the parent ItemWindow

    public void Init(ItemWindow _parent)
    {
        parent = _parent;

        SetHighlight(false);
    }

    public void SetCanDropItems(bool v)
    {
        can_drop_items = v;
    }

    public void SetHighlight(bool on)
    {
        highlight.enabled = on;
    }

    public bool IsValid() { return item != null && item.IsValid(); }

    public void Fill(int _index, Item _item)
    {
        item = _item;
        index = _index;

        Refresh_Icon();
    }

    void Refresh_Icon()
    {
        graphic.enabled = false;
        if (IsValid())
        {
            graphic.sprite = images[(int)(item.type - 1)];
            graphic.enabled = true;
        }

        number.text = IsValid() ? item.number.ToString() : "";
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        //Debug.Log("OnPointerDown: " + Debug_Get_Box_Info());
    }

    public void OnPointerUp(PointerEventData pointerEventData)
    {
        //Debug.Log("OnPointerUp: " + Debug_Get_Box_Info());
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item == null || item.number < 1) return;

        graphic.transform.SetParent(parent.parent.transform);

        //Debug.Log("OnBeginDrag: " + Debug_Get_Box_Info());
    }

    public void OnDrag(PointerEventData eventData)
    {
        graphic.transform.position = eventData.position;

        //Debug.Log("OnDrag: " + Debug_Get_Box_Info());
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        graphic.transform.SetParent(transform);
        graphic.transform.localPosition = new Vector3(0, 0, 0);

        //Debug.Log("OnEndDrag: " + Debug_Get_Box_Info());

        if (!item.IsValid()) return;

        // see if we've dropped this box on a different box

        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = eventData.position;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        foreach (var res in results)
        {
            if (res.gameObject.TryGetComponent(out ItemBox box))
            {
                if (this == box) continue;// if we're putting into the same box, don't go further

                if (!box.can_drop_items) continue; // don't let us drop items in if can drop items = false

                var result = box.parent.PutItemAt(item.type, item.number, box.index);
                if (result.Item1 == 1)
                {
                    if (result.Item2 == 0)
                        item.Clear();
                    else
                        item.Set(result.Item2);
                }
                else if (result.Item1 == 0) // do a swap
                {
                    var oldtype = item.type;
                    var oldnum = item.number;

                    item.Set(box.item.type, box.item.number);
                    box.item.Set(oldtype, oldnum);
                }

                box.parent.RefreshItems();
                parent.RefreshItems();
            }
        }

        if (results.Count < 1) // we're dragging out of the inventory, so drop the item
        {
            if (PlayerBackpack.Instance.isOpen) // only if we're in the backpack
            {
                float addx = Random.Range(1, 3f);
                if (Random.Range(0, 2) > 0) addx = -addx;
                float addy = Random.Range(1, 3f);
                if (Random.Range(0, 2) > 0) addy = -addy;

                Vector3 where = new Vector3(Player.Instance.transform.position.x + addx, Player.Instance.transform.position.y + 0.5f, Player.Instance.transform.position.z + addy); // spawn in a random location away from the player so you don't automatically pick it up when you leave the inventory

                Player.Instance.SpawnCrate(item.type, item.number, where);

                item.Clear();
                parent.RefreshItems();
            }
        }
    }

    public string Debug_Get_Box_Info()
    {
        return "Index: " + index + " " + name + " Item: " + (item != null ? item.type.ToString() : " empty ");
    }
}
