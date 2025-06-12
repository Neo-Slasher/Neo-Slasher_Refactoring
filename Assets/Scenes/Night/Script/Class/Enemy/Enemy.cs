using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Monster stats;

    [SerializeField] protected GameObject character; // 플레이어 오브젝트


    protected Rigidbody2D enemyRigid;
    protected SpriteRenderer enemyRenderer;

    protected Coroutine moveCoroutine = null;
    protected Coroutine knockbackCoroutine;
    protected Coroutine damagedEffectCoroutine;

    protected Vector3 moveDir;

    protected bool isStageEnd = false; // true라면 이동을 멈춤
    public bool isStop = false;        // 오브젝트 움직임 제어용
    public bool isAttacked = false;    // 공격 시 2초간 true


    public bool isBeingDragged = false;
    public bool isKnockBack = false;

    public float damageCooldown = 0.2f;    // 피격 효과 시간


    protected virtual void Awake()
    {
        enemyRigid = GetComponent<Rigidbody2D>();
        enemyRenderer = GetComponent<SpriteRenderer>();
    }

    protected virtual void Start()
    {
        character = GameObject.FindWithTag("Player").transform.Find("CharacterImage").gameObject;
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
            if (isStop || isBeingDragged || isKnockBack)
            {
                enemyRigid.linearVelocity = Vector3.zero;
                yield return new WaitWhile(() => (isStop || isBeingDragged || isKnockBack));
            }

            Vector3 playerPosition = character.transform.position;
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











    // 몬스터가 피격
    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyDamaged(collision);
    }

    private void EnemyDamaged(Collider2D collision)
    {
        float damage = 0;

        if (collision.CompareTag("CharacterAttackHitbox"))
        {
            // TODO: 나중에 리팩토링 할 일이 생기면 함수로 뺄 것
            Character character = collision.gameObject.GetComponent<HitBox>().character;

            // 기본 데미지
            if (character.player.attackPower > 0)
                damage += character.player.attackPower;

            // 최대 체력 비례 데미지
            float dealOnMax = character.player.dealOnMaxHp;
            if (dealOnMax > 0)
            {
                damage += stats.maxHp * dealOnMax;
            }

            // 현재 체력 비례 데미지
            float dealOnHp = character.player.dealOnCurHp;
            if (dealOnHp > 0)
            {
                damage += stats.curHp * dealOnHp;
            }

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
                damage = collision.transform.parent.GetComponent<ChargingReaper>().reaperAttackDamege;
            }
        }
        else
        {
            return;
        }

        if (damage <= 0)
            return;

        //피흡 있으면 회복 (이것도 캐릭터로 이전할 것)
        character.GetComponent<CharacterHealth>().HealByHit();

        Damaged(damage);
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
        if (character.GetComponent<CharacterAttack>().isMoveBackOn && !stats.isResist)
        {
            if (knockbackCoroutine != null)
                StopCoroutine(knockbackCoroutine);

            StartCoroutine(EnemyKnockbackCoroutine(transform.position));
        }
    }

    IEnumerator EnemyKnockbackCoroutine(Vector3 startPosition)
    {
        const float KNOCKBACK_DISTANCE = 1.5f; // 150px
        const float KNOCKBACK_SPEED = 5f;      // 넉백 속도

        Vector3 nowVelocity = enemyRigid.linearVelocity;
        moveDir = (transform.position - character.transform.position).normalized;

        // 투명도 임시 효과
        Color nowColor = enemyRenderer.color;
        nowColor.a = 0.5f;
        enemyRenderer.color = nowColor;
        isKnockBack = true;

        //적 초기 위치 기준 150px 튕김
        try
        {
            while ((startPosition - transform.position).magnitude <= KNOCKBACK_DISTANCE)
            {
                Logger.Log($"while {moveDir} {KNOCKBACK_SPEED} {enemyRigid.linearVelocity}");
                enemyRigid.linearVelocity = moveDir * KNOCKBACK_SPEED;
                yield return null;
            }

        }
        finally
        {
            // 상태 복구
            nowColor.a = 1f;
            enemyRenderer.color = nowColor;
            enemyRigid.linearVelocity = nowVelocity;
            isKnockBack = false;

        }
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

        // TODO: NightManager의 update kill count에 통합
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



    public void Damaged(float damage)
    {
        if (damage <= 0) return;

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
    public float GetEnemyAttackPower()
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






    public void DrugEnemy(float range)
    {
        Logger.Log("적이 당겨집니다.");
        StartCoroutine(DrugEnemyCoroutine(range));
    }

    IEnumerator DrugEnemyCoroutine(float range)
    {
        isBeingDragged = true;

        float minDistance = range;
        float forceMagnitude = 100f;

        Vector2 nowVelocity = enemyRigid.linearVelocity;

        while ((character.transform.position - transform.position).magnitude >= minDistance)
        {
            Vector2 moveDir = (character.transform.position - transform.position).normalized;
            enemyRigid.AddForce(moveDir * forceMagnitude, ForceMode2D.Force);
            yield return null;
        }

        isBeingDragged = false;
    }



    public void ThrustEnemy(float range)
    {
        Logger.Log("적이 밀쳐집니다.");
        StartCoroutine(ThrustEnemyCoroutine(range));
    }

    IEnumerator ThrustEnemyCoroutine(float range)
    {
        isStop = true;

        float maxDistance = range;
        float forceMagnitude = 100f;

        Vector3 nowVelocity = enemyRigid.linearVelocity;

        while ((character.transform.position - transform.position).magnitude <= maxDistance)
        {
            Vector2 moveDir = (transform.position - character.transform.position).normalized;
            enemyRigid.AddForce(moveDir * forceMagnitude, ForceMode2D.Force);
            yield return null;
        }

        isStop = false;
    }






}
