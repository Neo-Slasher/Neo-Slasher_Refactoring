using UnityEngine;


// index 0, normal enemy
public class BlackSuitMan : NormalEnemy
{

    private void Awake()
    {
        stats = new Monster(MonsterType.BlackSuitMan);
        SetEnemyStatus(GameManager.instance.player.level);
    }

}
