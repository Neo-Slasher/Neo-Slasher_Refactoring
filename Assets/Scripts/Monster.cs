using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class MonsterList {
    public List<Monster> monster;
}
public enum MonsterType
{
   BlackSuitMan, WhiteSuitMan, MachineArmorSoldier
        , Red3LegRobot, Blue3LegRobot, Big4LegRobot
}

[Serializable]
public class Monster
{
    public int index;
    public double width;
    public double length;

    public double maxHp;
    public double curHp;
    public double moveSpeed;
    public double attackPower;

    public bool dashAble;
    public double dashFreq;
    public double dashSpeed;
    public double dashDuration;

    public bool isElite;
    public bool isEnforce;
    public bool isResist;
    public bool canKnockback;
    public bool canProj;

    public int imageIndex;
    public int soundIndex;

    public Monster(MonsterType monsterType)
    {
        Monster data = null;
        data = DataManager.instance.monsterList.monster[(int)monsterType];

        if (data != null)
        {
            index = data.index;
            width = data.width;
            length = data.length;
            maxHp = data.maxHp;
            curHp = data.curHp;
            moveSpeed = data.moveSpeed;
            attackPower = data.attackPower;
            dashAble = data.dashAble;
            dashFreq = data.dashFreq;
            dashSpeed = data.dashSpeed;
            dashDuration = data.dashDuration;
            isElite = data.isElite;
            isEnforce = data.isEnforce;
            isResist = data.isResist;
            canKnockback = data.canKnockback;
            canProj = data.canProj;
            imageIndex = data.imageIndex;
            soundIndex = data.soundIndex;
        }
    }
}
