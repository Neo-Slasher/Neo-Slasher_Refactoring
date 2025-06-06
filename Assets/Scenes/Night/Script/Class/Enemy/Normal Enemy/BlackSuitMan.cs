
public class BlackSuitMan : NormalEnemy
{
    protected override void Start()
    {
        base.Start();

        stats = new Monster(MonsterType.BlackSuitMan);
        SetEnemyStatus();
    }

}
