using UnityEngine;

public class Big4LegRobot : EliteEnemy
{
    private void Awake()
    {
        stats = new Monster(MonsterType.Big4LegRobot);
        SetEnemyStatus(GameManager.Instance.player.level);
    }
}

