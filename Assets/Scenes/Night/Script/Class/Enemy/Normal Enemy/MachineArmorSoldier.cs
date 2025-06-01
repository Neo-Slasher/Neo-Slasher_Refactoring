using UnityEngine;

public class MachineArmorSoldier : NormalEnemy
{
    private void Awake()
    {
        stats = new Monster(MonsterType.MachineArmorSoldier);
        SetEnemyStatus(GameManager.Instance.player.level);
    }
}
