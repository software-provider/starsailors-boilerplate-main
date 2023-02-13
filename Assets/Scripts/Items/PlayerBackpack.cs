/*
 *  A item window for the player containg the list of items in the player backpack : Chris
 */

using UnityEngine;

public class PlayerBackpack : InstanceMonoBehaviour<PlayerBackpack> // player item menu, bringing this up pauses the game
{
    public bool isOpen { get; private set; } = false;
    public ItemWindow window;

    protected override void Awake()
    {
        base.Awake();

        window.Init(Player.Instance.backpack);
        Close(true, true);
    }

    void Update()
    {
        if (!isOpen) return;

        // check for key commands

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Close(false);
        }
    }

    public void Open(bool instant)
    {
        isOpen = true;

        QuickSlots.Instance.Close(instant);
        BuildingScreen.Instance.Close(instant);

        window.RefreshItems(0);
        Cursor.lockState = CursorLockMode.None;

        Vector3 openpos = new Vector3(0, 0, 0f);

        if (instant)
            gameObject.transform.localPosition = openpos;
        else
            LeanTween.move(gameObject.GetComponent<RectTransform>(), openpos, 0.3f).setDelay(0.1f);
    }

    public void Close(bool instant, bool force = false)
    {
        if (!isOpen && !force) return;

        isOpen = false;
        Cursor.lockState = CursorLockMode.Locked;

        Vector3 closedpos = new Vector3(-1000, 0, 0f);

        if (instant)
            gameObject.transform.localPosition = closedpos;
        else
            LeanTween.move(gameObject.GetComponent<RectTransform>(), closedpos, 0.3f).setDelay(0.1f);

        QuickSlots.Instance.Open(instant);
    }

    //void Test_Gather_Widgets()
    //{
    //    Item.Type t = Item.Type.none;

    //    while (t != Item.Type.puzzle && t != Item.Type.rabbit && t != Item.Type.spade)
    //        t = Item.GetRandomType();

    //    Item item = new Item(t, Random.Range(1, 3));

    //    window_1.AddItem(item);
    //    window_1.RefreshItems(0);

    //    for (int n = 0; n < item.number; n++)
    //        Lootlocker.Instance.AddItem(item);

    //    Debug.Log("Test_Gather_Widgets + " + item.type + " " + item.number);
    //}
}
