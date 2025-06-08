using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class DifficultyList
{
    public List<Difficulty> difficulty;
}

[Serializable]
public class Difficulty
{
    public int recommandLv;
    public int rewardExp;
    public int goalMoney;
    public float enemyStatus;
    public float enemyRespawn;
    public float normalEnhance;
    public float eliteEnhance;
    public int dropRank;
    public float recommandCP;
}
