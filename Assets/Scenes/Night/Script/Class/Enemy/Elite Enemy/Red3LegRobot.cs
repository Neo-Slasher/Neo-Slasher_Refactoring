// 2025.06.06 Refactoring Final Version
public class Red3LegRobot : EliteEnemy
{
    protected override void Start()
    {
        base.Start();

        stats = new Monster(MonsterType.Red3LegRobot);

        if (GameManager.Instance == null || GameManager.Instance.player == null)
        {
            Logger.LogError("GameManager �Ǵ� Player�� �Ҵ���� �ʾҽ��ϴ�.");
            return;
        }
        SetEnemyStatus();
    }
}
