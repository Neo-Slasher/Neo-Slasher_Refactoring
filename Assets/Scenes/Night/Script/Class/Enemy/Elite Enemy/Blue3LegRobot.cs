// 2025.06.06 Refactoring Final Version
public class Blue3LegRobot : EliteEnemy
{
    protected override void Start()
    {
        base.Start();

        stats = new Monster(MonsterType.Blue3LegRobot);

        if (GameManager.Instance == null || GameManager.Instance.player == null)
        {
            Logger.LogError("GameManager 또는 Player가 할당되지 않았습니다.");
            return;
        }
        SetEnemyStatus();


        SetProjectile();
    }
}
