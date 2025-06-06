using UnityEngine;

public class MachineArmorSoldier : NormalEnemy
{
    protected override void Start()
    {
        base.Start();

        stats = new Monster(MonsterType.MachineArmorSoldier);
        SetEnemyStatus();
    }
}
