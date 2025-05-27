using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AssassinationStageList
{
    public List<AssassinationStage> assassinationStage;
}



[Serializable]
public class AssassinationStage
{
    public int stageNo;
    public int normalReward;
    public int eliteReward;
    public double normalDropRate;
    public double eliteDropRate;
    public double normal1Spawn;
    public double normal2Spawn;
    public double normal3Spawn;
    public double elite1Spawn;
    public double elite2Spawn;
    public double elite3Spawn;
    public int stageDropRank;
    public int stageRecCP;
}
