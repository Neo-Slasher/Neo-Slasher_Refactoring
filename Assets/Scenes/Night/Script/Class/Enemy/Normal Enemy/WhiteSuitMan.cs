using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class WhiteSuitMan : NormalEnemy
{

    private void Awake()
    {
        stats = new Monster(MonsterType.WhiteSuitMan);
        SetEnemyStatus(GameManager.instance.player.level);
    }
}
