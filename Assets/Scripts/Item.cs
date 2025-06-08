using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public enum ItemType { Equipment, Consumable };

public abstract class Item
{
    public abstract ItemType GetItemType();
    public abstract string GetRank();
    public abstract string GetCategory();
    public abstract int GetPrice();
}

[Serializable]
public class ConsumableList
{
    public List<Consumable> item;
}

[Serializable]
public class Consumable : Item
{
    public int itemIdx;
    public string name;
    public int rank;
    public bool showCooltime;
    public int price;
    public string script;
    public int imgIdx;
    public int category;
    public bool attackPowerCalc;
    public float attackPowerValue;
    public bool attackSpeedCalc;
    public float attackSpeedValue;
    public bool attackRangeCalc;
    public float attackRangeValue;

    public override ItemType GetItemType()
    {
        return ItemType.Consumable;
    }

    public string GetConvertedScript(Player player)
    {
        string convertedScript = script;
        if (attackPowerCalc)
            convertedScript = convertedScript.Replace("#at#", (player.attackPower * attackPowerValue).ToString());
        if (attackSpeedCalc)
            convertedScript = convertedScript.Replace("#as#", (player.attackSpeed * attackSpeedValue).ToString());
        if (attackRangeCalc)
            convertedScript = convertedScript.Replace("#ar#", (player.attackRange * attackRangeValue).ToString());

        return convertedScript;
    }
    public override string GetRank()
    {
        if (rank == 3)
            return "S";
        if (rank == 2)
            return "A";
        if (rank == 1)
            return "B";
        if (rank == 0)
            return "C";
        return "Unknown Rank!";
    }
    public override string GetCategory()
    {
        if (category == 0)
            return "공격";
        else if (category == 1)
            return "방어";
        else if (category == 2)
            return "보조";

        return "Unknown Category!";
    }
    public override int GetPrice()
    {
        return price;
    }
}

[Serializable]
public class EquipmentList
{
    public List<Equipment> equipment;
}



[Serializable]
public class Equipment : Item
{
    public int index;
    public string name;
    public int rank;
    public int price;
    public string script;
    public int imageIndex;
    public int part;
    public int attackPower;
    public int attackSpeed;
    public int attackRange;
    public int moveSpeed;

    public override ItemType GetItemType()
    {
        return ItemType.Equipment;
    }
    public override string GetRank()
    {
        if (rank == 3)
            return "S";
        if (rank == 2)
            return "A";
        if (rank == 1)
            return "B";
        if (rank == 0)
            return "C";

        return "Unknown Rank!";
    }
    public override string GetCategory()
    {
        if (part == 0)
            return "공격";
        else if (part == 1)
            return "방어";
        else if (part == 2)
            return "보조";

        return "Unknown Category!";
    }
    public override int GetPrice()
    {
        return price;
    }
}
