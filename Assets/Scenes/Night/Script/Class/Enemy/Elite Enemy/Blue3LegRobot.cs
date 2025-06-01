using UnityEngine;

public class Blue3LegRobot : EliteEnemy
{

    private void Awake()
    {
        stats = new Monster(MonsterType.Blue3LegRobot);
        SetEnemyStatus(GameManager.Instance.player.level);
    }
}
