using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Monster stats;

    [SerializeField] protected GameObject player; // 플레이어 오브젝트


    protected Rigidbody2D enemyRigid;
    protected SpriteRenderer enemyRenderer;

    protected Coroutine moveCoroutine = null;
    protected Coroutine knockbackCoroutine;
    protected Coroutine damagedEffectCoroutine;

    protected Vector3 moveDir;

    public bool isSlow = false;
    protected bool isStageEnd = false; // true라면 이동을 멈춤
    public bool isStop = false;        // 오브젝트 움직임 제어용
    public bool isAttacked = false;    // 공격 시 2초간 true

    public float damageCooldown = 0.2f;    // 피격 효과 시간


    protected virtual void Awake()
    {
        enemyRigid = GetComponent<Rigidbody2D>();
        enemyRenderer = GetComponent<SpriteRenderer>();
    }

    protected virtual void Start()
    {
        player = GameObject.FindWithTag("Player").transform.Find("CharacterImage").gameObject;
        EnemyMove();
    }

    public void EnemyMove()
    {
        if (moveCoroutine == null)
            moveCoroutine = StartCoroutine(EnemyMoveCoroutine());
    }

    public void StopEnemyMove()
    {
        if (moveCoroutine != null)
        {
            // 아래 주석 코드가 필요한 지 아직 모름, 테스트 후 결정할 것
            //enemyRigid.linearVelocity = Vector3.zero;
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
    }

    IEnumerator EnemyMoveCoroutine()
    {
        while (!isStageEnd)
        {
            while (isStop)
            {
                enemyRigid.linearVelocity = Vector3.zero;
                yield return null;
            }

            Vector3 playerPosition = player.transform.position;
            moveDir = playerPosition - transform.position;

            float direction = moveDir.x > 0 ? -1f : 1f;
            if (transform.localScale.x != direction)
                transform.localScale = new Vector3(direction, 1, 1);

            float moveSpeed = SetMoveSpeed(stats.moveSpeed);
            enemyRigid.linearVelocity = moveDir.normalized * moveSpeed;

            yield return null;
        }

        enemyRigid.linearVelocity = Vector3.zero;
        moveCoroutine = null;
    }

    // XXX: 저런 식이 왜 나온걸까?
    protected float SetMoveSpeed(double getMoveSpeed)
    {
        return ((float)getMoveSpeed * 25) / 100;
    }











    // TODO: 몬스터가 닿은 오브젝트는
    // 1. 몬스터가 피격당하거나 (현재는 피격만 구현되어 있음)
    // 2. 몬스터가 공격하거나(피격 로직은 피격 오브젝트에서 담당)
    // 3. 무시해도 되는 오브젝트이거나
    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyDamaged(collision);
    }

    private void EnemyDamaged(Collider2D collision)
    {
        double damage = 0;

        if (collision.CompareTag("CharacterAttackHitbox"))
        {
            damage = collision.gameObject.GetComponent<HitBox>().character.player.attackPower;
            collision.gameObject.GetComponent<HitBox>().isAttacked = true;
        }
        else if (collision.CompareTag("Projectile"))
        {
            if (collision.name == "CentryBallProjPrefab(Clone)")
            {
                damage = collision.GetComponent<Projectile>().projPower;
            }
        }
        else if (collision.CompareTag("Item"))
        {
            if (collision.name == "ChargingReaperImage")
            {
                damage = collision.transform.parent.GetComponent<ChargingReaper>().reaperAttackDamaege;
            }
        }
        else
        {
            return;
        }

        if (damage <= 0)
            return;

        //피흡 있으면 회복 (이것도 캐릭터로 이전할 것)
        player.GetComponent<Character>().AbsorbAttack();
        EnemyKnockback();

        if (stats.curHp > damage)
        {
            StartCoroutine(EnemyDamagedEffectCoroutine());
            stats.curHp -= damage;
        }
        else
        {
            Die();
        }
    }

    IEnumerator EnemyDamagedEffectCoroutine()
    {
        try
        {
            if (!Mathf.Approximately(enemyRenderer.color.a, 1f)) yield break;

            Color originalColor = enemyRenderer.color;
            enemyRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.7f);

            yield return new WaitForSeconds(damageCooldown);
        }
        finally // 예외 발생 시에도 색상 복구
        {
            Color color = enemyRenderer.color;
            color.a = 1f;
            enemyRenderer.color = color;
            damagedEffectCoroutine = null;
        }
    }

    private void EnemyKnockback()
    {
        if (player.GetComponent<Character>().isMoveBackOn && !stats.isResist)
        {
            isStop = true;
            Vector3 start = transform.position;

            if (knockbackCoroutine != null)
                StopCoroutine(knockbackCoroutine);

            StartCoroutine(EnemyKnockbackCoroutine(start));
        }
    }

    IEnumerator EnemyKnockbackCoroutine(Vector3 start)
    {
        const float KNOCKBACK_DISTANCE = 1.5f; // 150px
        const float KNOCKBACK_SPEED = 5f;      // 넉백 속도

        Vector3 nowVelocity = enemyRigid.linearVelocity;
        moveDir = (transform.position - player.transform.position).normalized;
        enemyRigid.linearVelocity = moveDir * KNOCKBACK_SPEED;

        // 투명도 임시 효과
        Color nowColor = enemyRenderer.color;
        nowColor.a = 0.5f;
        enemyRenderer.color = nowColor;

        //적 초기 위치 기준 150px 튕김
        while ((start - this.transform.position).magnitude <= KNOCKBACK_DISTANCE)
        {
            enemyRigid.linearVelocity = moveDir * KNOCKBACK_SPEED;
            yield return null;
        }

        // 상태 복구
        nowColor.a = 1f;
        enemyRenderer.color = nowColor;
        enemyRigid.linearVelocity = nowVelocity;
        isStop = false;
    }

    private void Die()
    {
        UpdateKillCount();
        Destroy(gameObject);
    }
    private void UpdateKillCount()
    {
        NightManager nightManager = NightManager.Instance;
        if (nightManager == null) return;

        nightManager.UpdateKillCount();

        if (gameObject.CompareTag("Normal"))
            nightManager.killNormal++;
        else if (gameObject.CompareTag("Elite"))
            nightManager.killElite++;
    }

    public void SetEnemyStatus()
    {
        // 선택한 난이도에 따라 스테이터스 배율 설정
        // DataManager에 접근 할 때 -1
        int difficulty = GameManager.Instance.player.difficulty - 1;
        stats.maxHp *= DataManager.Instance.difficultyList.difficulty[difficulty].enemyStatus;
        stats.curHp *= DataManager.Instance.difficultyList.difficulty[difficulty].enemyStatus;
        stats.attackPower *= DataManager.Instance.difficultyList.difficulty[difficulty].enemyStatus;
    }

    // Tolelom: 동일한 함수명이 존재함
    // 다양한 형태의 공격에 맞았을 때 공격한 오브젝트 측에서 사용하는 함수로 추측됨
    // 추측대로라면 위에 피격 로직에 합치는게 합리적일 수도
    public void EnemyDamaged(double getDamage)
    {
        if (getDamage > 0)
        {

            if (stats.curHp > getDamage)
            {
                StartCoroutine(EnemyDamagedEffectCoroutine());
                stats.curHp -= getDamage;
            }
            else
            {
                NightManager.Instance.UpdateKillCount();
                Destroy(gameObject);
            }
        }
    }

    //몬스터가 강화되었을 때 스텟 증폭시키는 함수
    public void SetEnforceData()
    {
        if (IsEnforce())
        {
            stats.isEnforce = true;

            //강화되었으므로 스테이터스 변경
            stats.maxHp *= 2;
            stats.curHp *= 2;
            stats.attackPower *= 2;
        }
    }

    bool IsEnforce()
    {
        double nowProb;
        double randomProb = Random.value;

        // DataManager에 접근할 때는 -1 해줘야 함
        int difficulty = GameManager.Instance.player.difficulty - 1;
        if (!stats.isElite)
            nowProb = DataManager.Instance.difficultyList.difficulty[difficulty].normalEnhance;
        else
            nowProb = DataManager.Instance.difficultyList.difficulty[difficulty].eliteEnhance;

        if (randomProb < nowProb)
            return true;
        else
            return false;
    }

    //캐릭터와 충돌할 경우 해당 적의 공격력을 반환하는 함수
    public double GetEnemyAttackPower()
    {
        if (isAttacked == false)
            return stats.attackPower;
        else return 0;
    }

    //공격하고 2초동안 공격 무시
    public void SetIsAttacked()
    {
        isAttacked = true;
        StartCoroutine(SetIsAttackedCoroutine());
    }

    IEnumerator SetIsAttackedCoroutine()
    {
        yield return new WaitForSeconds(2.0f);
        isAttacked = false;
    }








    public void DrugEnemy()
    {
        Vector3 start = transform.position;
        StartCoroutine(DrugEnemyCoroutine(start));
    }

    IEnumerator DrugEnemyCoroutine(Vector3 start)
    {
        Vector3 nowVelocity = enemyRigid.linearVelocity;
        //캐릭터 위치 기준으로 반경 256px안까지 끌어당김
        //while((character.transform.position - this.transform.position).magnitude >= 2f)
        //{
        //    enemyRigid.AddForceAtPosition(moveDir, this.transform.position);
        //    yield return null;
        //}

        //본인 초기 위치 기준 150px 앞으로 당겨짐
        while ((start - this.transform.position).magnitude <= 1.5f)
        {
            enemyRigid.AddForceAtPosition(moveDir, this.transform.position);
            yield return null;
        }
        enemyRigid.linearVelocity = nowVelocity;
    }

    public void ThrustEnemy()
    {
        isStop = true;
        Vector3 start;
        start = this.transform.position;
        StartCoroutine(ThrustEnemyCoroutine(start));
    }

    IEnumerator ThrustEnemyCoroutine(Vector3 start)
    {
        Vector3 nowVelocity = enemyRigid.linearVelocity;
        moveDir = start - player.transform.position;
        enemyRigid.linearVelocity = moveDir.normalized * 5;

        //투명도로 확인하려고 임시로 만들어둠
        Color nowColor = enemyRenderer.color;
        nowColor.a = 0.5f;
        enemyRenderer.color = nowColor;

        //적 초기 위치 기준 256px 튕김
        while ((player.transform.position - this.transform.position).magnitude <= 2f)
        {
            //enemyRigid.AddForceAtPosition(moveDir, this.transform.position);
            yield return null;
        }
        nowColor.a = 1f;
        enemyRenderer.color = nowColor;
        enemyRigid.linearVelocity = nowVelocity;
        isStop = false;
    }






}
