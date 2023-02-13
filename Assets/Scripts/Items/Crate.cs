/*
 *  Holds an item in the world
 */

using UnityEngine;

public class Crate : MonoBehaviour
{
    Item item;

    public void Spawn(Item.Type type, int number)
    {
        item = new Item(type, number);
    }

    public Item Pickup()
    {
        Destroy(gameObject);

        return item;
    }
}
