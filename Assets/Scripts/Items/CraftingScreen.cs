/*
 *  A window for crafting containing 3 item menus, 1 for the players backpack, 1 for the items to be used, 1 for the item to be made : Chris
 */

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingScreen : InstanceMonoBehaviour<CraftingScreen>
{
    public bool isOpen { get; private set; } = false;
    public ItemWindow backpack_window;
    public ItemWindow forge_window;
    public ItemWindow output_window;
    public Button Craft_Btn;
    Color original_Craft_Btn_Color;
    Color original_Craft_Txt_Color;

    Inventory forge = new Inventory();
    Inventory output = new Inventory();

    protected override void Awake()
    {
        base.Awake();

        original_Craft_Btn_Color = Craft_Btn.GetComponent<Image>().color;
        original_Craft_Txt_Color = Craft_Btn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color;

        backpack_window.Init(Player.Instance.backpack);

        forge.Init(4);
        output.Init(4);

        forge_window.Init(forge, EnableCraftButton);
        output_window.Init(output);
        output_window.DisableItemDrop();

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

        backpack_window.RefreshItems(0);
        forge_window.RefreshItems(0);
        output_window.RefreshItems(0);

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

        // throw all the items from the forge and output slots into the players inventory before we close

        for (int i = 0; i < forge.CountMaxItems(); i++)
        {
            backpack_window.AddItem(forge.GetItemAt(i));
        }

        forge.ClearItems();

        for (int i = 0; i < output.CountMaxItems(); i++)
        {
            backpack_window.AddItem(output.GetItemAt(i));
        }

        output.ClearItems();

        isOpen = false;
        Cursor.lockState = CursorLockMode.Locked;

        Vector3 closedpos = new Vector3(-1000, 0, 0f);

        if (instant)
            gameObject.transform.localPosition = closedpos;
        else
            LeanTween.move(gameObject.GetComponent<RectTransform>(), closedpos, 0.3f).setDelay(0.1f);

        QuickSlots.Instance.Open(instant);
    }

    bool CheckCraftingReqs() // check and see if we have enough stuff in the forge to craft
    {
        if (output.CountValidItems() > 0) return false; // if there's already something in the output slots, don't craft

        int unique_items = 0;

        for (int i1 = 0; i1 < forge.CountMaxItems(); i1++)
        {
            for (int i2 = 0; i2 < forge.CountMaxItems(); i2++)
            {
                if (i1 == i2) continue; // don't check the same item against itself

                var item1 = forge.GetItemAt(i1);
                var item2 = forge.GetItemAt(i2);
                if (item1.IsValid() && item2.IsValid() && item1.type != item2.type)
                {
                    unique_items++;
                }
            }
        }

        if (unique_items > 1) return true;

        return false;
    }

    public void Craft()
    {
        if (!CheckCraftingReqs()) return;

        int num = 0;

        for (int i = 0; i < forge.CountMaxItems(); i++)
        {
            var itm = forge.GetItemAt(i);

            num += itm.number;
        }

        int min = num / 2 > 0 ? num / 2 : 1;
        if (min > 50) min = 50;
        int max = num + 1 <= 100 ? num + 1 : 100;

        output_window.AddItem(new Item(Item.GetRandomType(), Random.Range(min, max)));
        output_window.RefreshItems();
        forge.ClearItems();
        forge_window.RefreshItems();
    }

    public void EnableCraftButton()
    {
        bool enabled = Craft_Btn.interactable = CheckCraftingReqs();

        if (enabled)
        {
            Craft_Btn.GetComponent<Image>().color = original_Craft_Btn_Color;
            Craft_Btn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = original_Craft_Txt_Color;
        }
        else
        {
            Craft_Btn.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f);
            Craft_Btn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(0.4f, 0.4f, 0.4f);
        }
    }
}