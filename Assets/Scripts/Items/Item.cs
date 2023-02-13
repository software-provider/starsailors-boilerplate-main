/*
 *  Actual item information
 */

using UnityEngine;

[System.Serializable]
public class Item
{
    const int Max = 99; // max number of items

    public int number { get; private set; } // how many?

    public enum Type
    {
        none,
        item1, item2, item3, item4, item5, item6, item7, item8, item9, item10,
        item11, item12, item13, item14, item15, item16, item17, item18, item19, item20,
        item21, item22, item23, item24, item25, item26, item27, item28, item29, item30,
        item31, item32, item33, item34, item35, item36, item37, item38, item39, item40,
        item41, item42, item43, item44, item45,
    }

    public Type type { get; private set; } // what kind of item?

    public Item(Type _type = Type.none, int _number = 0)
    {
        Set(_type, _number);
    }

    public void Set(Type _type) { type = _type; }
    public void Set(int _number) { number = _number; }
    public void Set(Type _type, int _number) { type = _type; number = _number; }

    public bool IsValid() { return type != Type.none && number > 0; }
    public void Clear() { type = Type.none; number = 0; }

    public int Plus(int plus)
    {
        number += plus;

        if (number > Max)
        {
            int leftover = number - Max;

            number = Max;

            return leftover;
        }
        if (number < 0)
        {
            int leftover = number;

            number = 0;

            return leftover;
        }

        return 0;
    }


    public static Type GetRandomType()
    {
        var values = System.Enum.GetValues(typeof(Type));

        return (Type)Random.Range(1, values.Length);
    }


}
