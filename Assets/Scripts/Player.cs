using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Player
{
    private const string saveFileName = "UserData.json";

    // defalut
    public int day; //정윤석: 날짜를 세이브하기 위해서 넣었습니다. 제이슨도 바꿨어요.
    public int difficulty;
    public int assassinationCount;

    public int level;
    public int reqExp;
    public int curExp;
    public int money;
    public int startMoney;
    public float earnMoney;

    // status
    public double maxHp;
    public double curHp;
    public double attackPower;
    public double attackSpeed;
    public double attackRange;
    public double moveSpeed;
    public double shieldDuration;
    public double shieldPoint;
    public double immuneDuration;
    public int immuneCount;
    public bool dashable;
    public double dashFreq;
    public double dashSpeed;
    public double dashDuration;
    public double damageReductionRate;
    public double dealOnMaxHp;
    public double dealOnCurHp;
    public double healByHit;
    public double hpRegen;

    public int shopSlot;
    public int shopMinRank;
    public int shopMaxRank;
    public int dropRank;
    public double dropRate;
    public int itemSlot;

    // 1~62까지 인덱스를 사용합니다.
    public bool[] trait = new bool[63];
    public int traitPoint;

    public Equipment[] equipment = new Equipment[3];

    public Consumable[] item = new Consumable[3];


    // 장비 착용으로 인해 올라가는 능력치
    public double equipmentAttackPower;
    public double equipmentAttackSpeed;
    public double equipmentAttackRange;
    public double equipmentMoveSpeed;

    public Player()
    {
        day = 1;
        difficulty = 0; // 0인 경우 인트로를 보게 됨
        assassinationCount = 0;

        level = 1;
        reqExp = 2;

        maxHp = 25;
        curHp = 25;
        moveSpeed = 10;
        attackPower = 7;
        dashable = false;
        dashFreq = 0;
        dashSpeed = 0;
        attackSpeed = 10;
        attackRange = 10;
        shieldDuration = 0;
        shieldPoint = 0;
        immuneDuration = 0;
        immuneCount = 0;
        damageReductionRate = 0;
        dealOnMaxHp = 0;
        dealOnCurHp = 0;
        healByHit = 0;
        hpRegen = 0;
        money = 0;
        shopSlot = 1;
        shopMinRank = 0;
        shopMaxRank = 1;
        dropRank = 0;
        dropRate = 1;
        itemSlot = 1;

        traitPoint = level;

        // earn money 관련 데이터 없어서 추가 
        earnMoney = 1.0F;
    }


    public static void Save(Player instance)
    {
        string saveFilePath = Path.Combine(Application.persistentDataPath, saveFileName);
        string json = JsonUtility.ToJson(instance);
        File.WriteAllText(saveFilePath, json);


        Logger.Log("플레이어 데이터가 저장되었습니다.");
    }

    public static Player Load()
    {
        string saveFilePath = Path.Combine(Application.persistentDataPath, saveFileName);
        if (!File.Exists(saveFilePath))
        {
            Logger.Log("세이브 데이터가 존재하지 않습니다.");
            return null;
        }
        string savedData = File.ReadAllText(saveFilePath);
        return JsonUtility.FromJson<Player>(savedData);
    }

    public static Player SoftReset(Player player)
    {
        player = new Player();
        return player;
    }

    public int GetEmptyComsumableSlot()
    {
        for (int i = 0; i < itemSlot; ++i)
        {
            if (item[i] == null)
                return i;
        }
        return -1;
    }

    public bool haveConsumable(int consumableIndex)
    {
        for (int i = 0; i < itemSlot; i++)
        {
            if (item[i] != null && item[i].itemIdx == consumableIndex)
                return true;
        }
        return false;
    }
}
