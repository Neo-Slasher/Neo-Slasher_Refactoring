using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class KillData
{
    public int killNormal;
    public int killElite;

    public KillData()
    {
        killNormal = 0;
        killElite = 0;
    }
}
