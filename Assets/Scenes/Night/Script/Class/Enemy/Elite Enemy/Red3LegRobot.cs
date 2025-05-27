using UnityEngine;

public class Red3LegRobot : EliteEnemy
{
    private void Awake()
    {
        stats = new Monster(MonsterType.Red3LegRobot);
        SetEnemyStatus(GameManager.instance.player.level);
    }
}
