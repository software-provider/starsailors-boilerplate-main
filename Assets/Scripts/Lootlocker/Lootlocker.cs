using UnityEngine;
using LootLocker.Requests;

public class Lootlocker : InstanceMonoBehaviour<Lootlocker>
{
    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(gameObject);
    }


    void Start()
    {
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (!response.success)
            {
                Debug.Log("error starting LootLocker session");

                return;
            }

            Debug.Log("successfully started LootLocker session");
        });

        LootLockerSDKManager.SetPlayerName("Chris", (response) =>
        {
            if (response.success)
            {
                Debug.Log("Successfully set player name");
            }
            else
            {
                Debug.Log("Error setting player name");
            }
        });

        //LootLockerSDKManager.AddFavouriteAsset("97900", (response) =>
        //{
        //    if (response.success)
        //    {
        //        Debug.Log("Successfully favourited asset");
        //    }
        //    else
        //    {
        //        Debug.Log("Error favouriting asset");
        //    }
        //});

        LootLockerSDKManager.CreateKeyValuePairForAssetInstances(97900, "blah", "value here", (response) =>
        {
            if (response.success)
            {
                Debug.Log("Successfully created key value pair for asset instance");
            }
            else
            {
                Debug.Log("Error creating key value pair");
            }
        });

        LootLockerSDKManager.GetInventory((response) =>
        {
            if (response.success)
            {
                //Debug.Log("Successfully retrieved player inventory: " + response.inventory.Length);

                //foreach (var item in response.inventory)
                //{
                //    Debug.Log("fill item with " + item.asset.name);

                //    switch (item.asset.name)
                //    {
                //        case "Item_1":
                //            Store.Instance.window_1.AddItem(new Item(Item.Type.rabbit, 1));
                //            break;
                //        case "Item_2":
                //            Store.Instance.window_1.AddItem(new Item(Item.Type.puzzle, 1));
                //            break;
                //        case "Item_3":
                //            Store.Instance.window_1.AddItem(new Item(Item.Type.spade, 1));
                //            break;
                //    }
                //}

                //Store.Instance.window_1.RefreshItems(0);
                //Store.Instance.window_2.RefreshItems(0);
            }
            else
            {
                Debug.Log("Error getting player inventory");
            }
        });
    }

    public void AddItem(Item item) // add the item to the lootlocker server for this player
    {
        //switch (item.type)

        //{
        //    case Item.Type.rabbit:
        //        LootLockerSDKManager.TriggeringAnEvent("Give_Rabbit", (response) =>
        //        {
        //            if (response.success)
        //            {
        //                Debug.Log("Successfully triggered event Rabbit");
        //            }
        //            else
        //            {
        //                Debug.Log("Error triggering event GiveItem");
        //            }
        //        });
        //        break;
        //    case Item.Type.puzzle:
        //        LootLockerSDKManager.TriggeringAnEvent("Give_Puzzle", (response) =>
        //        {
        //            if (response.success)
        //            {
        //                Debug.Log("Successfully triggered event Puzzle");
        //            }
        //            else
        //            {
        //                Debug.Log("Error triggering event GiveItem");
        //            }
        //        });
        //        break;
        //    case Item.Type.spade:
        //        LootLockerSDKManager.TriggeringAnEvent("Give_Spade", (response) =>
        //        {
        //            if (response.success)
        //            {
        //                Debug.Log("Successfully triggered event Spade");
        //            }
        //            else
        //            {
        //                Debug.Log("Error triggering event GiveItem");
        //            }
        //        });
        //        break;

        //}
    }

    public void RemoveItem(Item item) // remove an item from the lootlocker server for this player
    {

    }
}
