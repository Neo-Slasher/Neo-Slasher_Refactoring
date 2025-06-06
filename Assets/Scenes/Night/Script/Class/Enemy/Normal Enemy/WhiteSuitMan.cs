using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class WhiteSuitMan : NormalEnemy
{
    protected override void Start()
    {
        base.Start();

        stats = new Monster(MonsterType.WhiteSuitMan);
        SetEnemyStatus();
    }
}
