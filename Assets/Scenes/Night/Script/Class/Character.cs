using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


public class Character : MonoBehaviour
{
    // 플레이어 데이터
    public Player player;

    [Header("HpBar")]
    [SerializeField] GameObject hpBar;
    private HealthBar hpBarScript;

    [Header("HitBox")]
    [SerializeField] GameObject hitBox;
    private HitBox hitBoxScript;
    private Rigidbody2D hitBoxRigid;

    private Rigidbody2D characterRigid;
    private SpriteRenderer characterSpriteRenderer;



    const float HIT_BOX_DISTANCE = 3; //히트박스와 캐릭터와의 거리

    public Vector3 nowDir;
    public Vector3 fixPos = Vector3.zero;

    //bool isAttack;
    public bool canChange = false;
    public bool isAbsorb = false;

    //아이템 관련
    public bool isDoubleAttack = false;
    public bool isHologramTrickOn = false;
    public bool isHologramAnimate = false;
    public bool isAntiPhenetOn = false;
    public bool isMoveBackOn = false;

    // 부스터 아이템 itemIdx: 13
    public bool isBoosterOn = false;
    public double basicSpeed; // 부스터 아이템으로 인한 speed 증감치를 종료 시에 원상복구 위함

    // 애니메이션
    private Animator animator;
    public Animator[] hologramAnimatorArr;
    public SpriteRenderer[] hologramRendererArr;

    private float pixelMoveSpeed;

    // 코루틴
    private Coroutine hpRegenCoroutine;
    private Coroutine attackCoroutine;
    private Coroutine hitBoxCoroutine;

    private Vector3 currentAttackDirection; // 공격 시점의 방향 고정

    private void Awake()
    {
        player = GameManager.Instance.player;

        characterRigid = GetComponent<Rigidbody2D>();
        characterSpriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        if (hpBar != null)
            hpBarScript = hpBar.GetComponent<HealthBar>();

        if (hitBox != null)
        {
            hitBoxScript = hitBox.GetComponent<HitBox>();
            hitBoxRigid = hitBox.GetComponent<Rigidbody2D>();
        }
    }

    private void Start()
    {
        InitializeCharacter();
        InitializeHitBox();
        StartAttack();
    }

    public void SetMoveSpeed(double moveSpeed)
    {
        player.moveSpeed = moveSpeed;
        SetPixelMoveSpeed();
    }


    private void InitializeCharacter()
    {
        HealToMaxHp();
        LookRight();
        SetPixelMoveSpeed();

        if (player.hpRegen > 0)
            StartHpRegen();
    }

    private void HealToMaxHp()
    {
        player.curHp = player.maxHp;
    }
    private void LookRight()
    {
        characterSpriteRenderer.flipX = false;
    }

    private void SetPixelMoveSpeed()
    {
        pixelMoveSpeed = ConvertMoveSpeedToPixelSpeed(player.moveSpeed);
    }

    private void InitializeHitBox()
    {
        SetHitBoxScale();
    }

    private void SetHitBoxScale()
    {
        float goalY = (float)GameManager.Instance.player.attackRange * 0.15f;
        Vector3 goalScale = new Vector3(1, goalY, 1);

        hitBox.transform.localScale = goalScale;
    }
    private float ConvertMoveSpeedToPixelSpeed(double getMoveSpeed)
    {
        return ((float)getMoveSpeed * 25) / 100;
    }



    private void StartHpRegen()
    {
        if (player.hpRegen <= 0f) return;
        if (hpRegenCoroutine == null)
            hpRegenCoroutine = StartCoroutine(HpRegenCoroutine());
    }

    private void StopHpRegen()
    {
        if (hpRegenCoroutine != null)
        {
            StopCoroutine(hpRegenCoroutine);
            hpRegenCoroutine = null;
        }
    }

    private IEnumerator HpRegenCoroutine()
    {
        const float HP_REGEN_TICK = 1;
        while (!NightManager.Instance.isStageEnd)
        {
            if (player.curHp < player.maxHp)
            {
                player.curHp = Mathf.Min((float)(player.curHp + player.hpRegen), (float)player.maxHp);
                hpBarScript.UpdateBar();
            }
            yield return new WaitForSeconds(HP_REGEN_TICK);
        }

        hpRegenCoroutine = null;
    }







    public void StartMove(Vector3 joystickDir)
    {
        nowDir = joystickDir.normalized;
        characterRigid.linearVelocity = nowDir * pixelMoveSpeed;

        animator.SetBool("move", true);
        characterSpriteRenderer.flipX = nowDir.x > 0;

        UpdateHologramMoveAndFlip(true, nowDir.x > 0);

        // 자식 오브젝트까지 반전해야하면 사용
        //transform.localScale = (nowDir.x < 0) ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);     아이템도 같이 이동해서 주석처리했어
    }

    public void EndMove()
    {
        nowDir = Vector3.zero;
        characterRigid.linearVelocity = Vector3.zero;
        animator.SetBool("move", false);

        UpdateHologramMoveAndFlip(false, nowDir.x > 0);
    }

    private void UpdateHologramMoveAndFlip(bool isMoving, bool flipX)
    {
        if (!isHologramAnimate) return;

        foreach (Animator hologramAnimator in hologramAnimatorArr)
        {
            if (hologramAnimator != null)
                hologramAnimator.SetBool("move", isMoving);
        }

        foreach (SpriteRenderer hologramRenderer in hologramRendererArr)
        {
            if (hologramRenderer != null)
                hologramRenderer.flipX = flipX;
        }
    }












    public void StartAttack()
    {
        if (attackCoroutine == null)
        {
            attackCoroutine = StartCoroutine(AttackCoroutine());
        }
    }

    public void StopAttack()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
    }

    private IEnumerator AttackCoroutine()
    {
        try
        {
            while (!NightManager.Instance.isStageEnd)
            {
                float attackSpeed = 10 / (float)GameManager.Instance.player.attackSpeed;

                if (!isDoubleAttack) // single attack
                {
                    currentAttackDirection = nowDir.normalized;

                    animator.SetTrigger("attack");

                    if (isHologramAnimate && hologramAnimatorArr != null)
                    {
                        foreach (var hologramAnimator in hologramAnimatorArr)
                            hologramAnimator.SetTrigger("attack");
                    }

                    hitBox.SetActive(true);
                    SetHitbox();
                    yield return new WaitForSeconds(0.2f);
                    hitBox.SetActive(false);

                    isMoveBackOn = false;
                }
                else // double attack
                {
                    ItemManager.Instance.SetMultiSlasherSprite(true);
                    hitBox.SetActive(true);
                    SetHitbox();
                    yield return new WaitForSeconds(0.1f);
                    hitBox.SetActive(false);

                    yield return new WaitForSeconds(0.1f);

                    hitBox.SetActive(true);
                    SetHitbox();
                    yield return new WaitForSeconds(0.2f);
                    hitBox.SetActive(false);
                    ItemManager.Instance.SetMultiSlasherSprite(false);

                    isDoubleAttack = false;
                    isMoveBackOn = false;
                }

                //다음 공격까지 대기
                yield return new WaitForSeconds(attackSpeed);
                isAbsorb = false;
            }
        }
        finally
        {
            attackCoroutine = null;
        }
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        //캐릭터 데미지 받을 때
        CharacterDamaged(collision);
    }


    //이동 방향에 따라 히트박스 위치 조절하는 함수
    public void SetHitbox()
    {
        Vector3 newDirection = nowDir.normalized;
        if (newDirection == Vector3.zero)
            newDirection = ((fixPos == Vector3.zero) ? Vector3.left : fixPos);
        fixPos = newDirection;

        if (hitBox == null) return;
        hitBox.transform.localPosition = this.transform.position + fixPos * HIT_BOX_DISTANCE;

        float angle = Mathf.Atan2(fixPos.y, fixPos.x) * Mathf.Rad2Deg;
        hitBox.transform.rotation = Quaternion.Euler(0, 0, angle);

        if (!isActiveAndEnabled) return;
        StartCoroutine(SetHitBoxCoroutine());
    }

    IEnumerator SetHitBoxCoroutine()
    {
        while (hitBox.gameObject.activeSelf == true)
        {
            hitBoxRigid.linearVelocity = nowDir.normalized * pixelMoveSpeed;
            yield return null;
        }
    }


























    // 나중에 리팩토링 할 예정
    // 1. 함수명
    // 2. 로직
    // 플레이어가 피흡이 존재할 때, 피흡 계산하는 로직
    public void AbsorbAttack()   //canChange가 참이면 최대 체력일때 쉴드로 전환 가능
    {
        if (player.healByHit > 0 && !isAbsorb)
        {
            isAbsorb = true;

            if (player.curHp + player.healByHit < player.maxHp)
            {
                player.curHp += player.healByHit;
                hpBarScript.UpdateBar();
                //SetShieldImage();
            }
            else
            {
                //초과량 쉴드로 전환
                if (canChange)
                {
                    //쉴드가 최대 체력 초과면 리턴
                    if (player.shieldPoint >= player.maxHp)
                        return;

                    double excessHeal = player.curHp + player.healByHit
                                                                            - player.maxHp;

                    //쉴드가 최대 체력을 넘지 못하게 제어
                    if (player.shieldPoint + excessHeal >= player.maxHp)
                    {
                        player.shieldPoint = player.maxHp;
                    }

                    //그냥 보호막 회복
                    else
                        player.shieldPoint += excessHeal;
                }

                player.curHp = player.maxHp;
                hpBarScript.UpdateBar();
            }
        }
    }







    public void CharacterDamaged(Collider2D enemyCollision)
    {
        //공격이 성공할 때 히트박스가 캐릭터의 하위 오브젝트라 데미지를 받는 경우가 있어서 만든 코드
        if (hitBoxScript.isAttacked)
        {
            hitBoxScript.isAttacked = false;
            return;
        }

        GameObject enemy = enemyCollision.gameObject;
        double nowAttackPower = 0;

        switch (enemy.tag)
        {
            case "Normal":
                nowAttackPower = enemy.GetComponent<NormalEnemy>().GetEnemyAttackPower();
                enemy.GetComponent<NormalEnemy>().SetIsAttacked();
                break;
            case "Elite":
                nowAttackPower = enemy.GetComponent<EliteEnemy>().GetEnemyAttackPower();
                enemy.GetComponent<EliteEnemy>().SetIsAttacked();
                break;
            case "Projectile":
                if (enemy.GetComponent<Projectile>().isEnemy)
                {
                    nowAttackPower = enemy.transform.parent.GetComponent<EliteEnemy>().GetEnemyAttackPower();
                    enemy.transform.parent.GetComponent<EliteEnemy>().SetIsAttacked();
                }
                break;
            default:
                Logger.Log(enemy.name + "이 정상적이지 않아서 공격력을 받아올 수 없음");
                return;
        }


        if (nowAttackPower != 0)
            SetDamagedAnim();

        if (isHologramTrickOn)
            return;

        SetDamageData(nowAttackPower);
    }

    void SetDamageData(double getAttackData)
    {
        //안티 페넷 사용시 데미지 경감 계산
        getAttackData = AntiPhenetUse(getAttackData);

        //쉴드로 데미지 받을 때
        if (player.shieldPoint > 0)
        {
            //어차피 체력 100%에서 쉴드가 생기므로 
            //1. 체력바를 쉴드 비율만큼 옆으로 민다
            //2. 해당 체력바 위치에 쉴드 이미지를 놓는다.
            //3. 쉴드의 fillamount를 설정한다.
            if (player.shieldPoint >= getAttackData)
            {
                player.shieldPoint -= getAttackData;
                hpBarScript.UpdateBar();
            }
            else
            {
                double nowAttactDamage = getAttackData - (float)player.shieldPoint;
                player.shieldPoint = 0;
                hpBarScript.UpdateBar();

                player.curHp -= nowAttactDamage;
                //감소한 만큼 플레이어 체력바 줄어들게
                hpBarScript.UpdateBar();
            }
        }
        else
        { // 체력으로 데미지 받을 때
            //Hp - 적 데미지 계산
            if (player.curHp > getAttackData)
            {
                Debug.Log("damaged");
                player.curHp -= getAttackData;
                hpBarScript.UpdateBar();
            }
            else
            {
                //체력이 다 떨어졌으므로 사망
                Debug.Log("GameEnd");
                // TODO: 죽은 모션이 나온 후 결과 창이 뜨도록 딜레이 필요
                animator.SetTrigger("die");

                //홀로그램도 같이 움직이도록
                if (isHologramAnimate)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        hologramAnimatorArr[i].SetTrigger("die");
                    }
                }

                player.curHp = 0;
                hpBarScript.UpdateBar();

                this.GetComponent<BoxCollider2D>().enabled = false;

                NightManager.Instance.SetStageEnd();
            }
        }
    }

    //데미지를 받았을 경우 액션
    void SetDamagedAnim()
    {
        animator.SetTrigger("knockback");

        //홀로그램도 같이 움직이도록
        if (isHologramAnimate)
        {
            for (int i = 0; i < 2; i++)
            {
                hologramAnimatorArr[i].SetTrigger("knockback");
            }
        }

        StartCoroutine(SetDamagedAnimCoroutine());
    }

    IEnumerator SetDamagedAnimCoroutine()
    {
        for (int i = 0; i < 2; i++)
        {
            Color nowColor = characterSpriteRenderer.color;
            nowColor.a = 0.7f;
            characterSpriteRenderer.color = nowColor;
            yield return new WaitForSeconds(0.2f);

            nowColor.a = 1f;
            characterSpriteRenderer.color = nowColor;
            yield return new WaitForSeconds(0.3f);
        }
    }

    // 28번 특성 활성화 시 실행되는 코드
    public void SetStartShieldPointData(float getShieldPoint)
    {
        player.shieldPoint = (float)player.maxHp * getShieldPoint;
        hpBarScript.UpdateBar();
    }

    //해당 숫자만큼 쉴드 생성
    public void SetShieldPointData(float getShieldPoint)
    {
        player.shieldPoint = getShieldPoint;
        hpBarScript.UpdateBar();
    }


    //void SetShieldImage()
    //{
    //    Vector3 fixShieldPosition = shieldBarImage.transform.localPosition;

    //    hpBar.GetComponent<HealthBar>().UpdateBar();

    //    fixShieldPosition.x = hpBarImage.rectTransform.sizeDelta.x * hpBarImage.fillAmount;

    //    shieldBarImage.transform.localPosition = fixShieldPosition;
    //    shieldBarImage.fillAmount = (float)(player.shieldPoint / (player.shieldPoint + player.curHp));
    //}

    public void SetHpBarPosition()
    {
        Vector3 hpBarPos = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y - 3, 0));
        hpBar.transform.position = hpBarPos;
    }

    //특성에서 주변을 탐색하고 싶을 때 사용할 함수
    public Collider2D[] ReturnOverLapColliders(float maxRadius, float minRadius)
    {
        Collider2D[] overLapMaxColArr = Physics2D.OverlapCircleAll(this.transform.position, maxRadius);
        Collider2D[] overLapMinColArr = Physics2D.OverlapCircleAll(this.transform.position, minRadius);
        Collider2D[] overLapColArr = null;
        if (overLapMinColArr.Length != 0)
            overLapColArr = overLapMaxColArr.Except(overLapMinColArr).ToArray();

        return overLapColArr;
    }

    public Collider2D[] ReturnOverLapColliders(float radius)
    {
        Collider2D[] overLapColArr = Physics2D.OverlapCircleAll(this.transform.position, radius);

        return overLapColArr;
    }

    public double ReturnCharacterHitPoint()
    {
        return player.curHp;
    }

    public double ReturnCharacterHitPointMax()
    {
        return player.maxHp;
    }

    public double ReturnCharacterAttackPower()
    {
        return player.attackPower;
    }

    public double ReturnCharacterAttackSpeed()
    {
        return player.attackSpeed;
    }

    public void SetCharacterAttackPower(double getAttackPower)
    {
        player.attackPower = getAttackPower;
    }

    public void SetAbsorbAttackData(float getHealByHit)
    {
        player.healByHit += getHealByHit;
    }

    public void SetItemAbsorbAttackData(float getHealByHit)
    {
        player.healByHit = getHealByHit;
    }

    //아이템쪽
    public int ReturnCharacterItemSlot()
    {
        return player.itemSlot;
    }

    public double GetMoveSpeed()
    {
        return player.moveSpeed;
    }

    public Vector3 ReturnSpeed()
    {
        return nowDir.normalized * pixelMoveSpeed;
    }

    //아이템 6번 퍼스트 에이드에서 체력 회복할 때 쓰려고 만듬
    public void HealHp(double getHealHp, GameObject getImage)
    {
        StartCoroutine(HealHpCoroutine(getHealHp, getImage));
    }

    IEnumerator HealHpCoroutine(double getHealHp, GameObject getImage)
    {
        float time = 3;
        float deltaTime = 0;
        double value = 0;
        double nowHeal = 0;

        getImage.SetActive(true);

        while (time > deltaTime)
        {
            deltaTime += Time.deltaTime;
            value = getHealHp * Time.deltaTime;
            nowHeal += value;

            //힐량 초과시 종료
            if (nowHeal >= getHealHp)
                break;

            if (player.curHp < player.maxHp)
            {
                player.curHp += value;

                //만약 피가 오버되면 종료
                if (player.curHp >= player.maxHp)
                {
                    player.curHp = player.maxHp;
                    hpBarScript.UpdateBar();
                    break;
                }

                hpBarScript.UpdateBar();
            }
            yield return null;
        }
        getImage.SetActive(false);
        Debug.Log("End");
    }

    public double ReturnCharacterShieldPoint()
    {
        return player.shieldPoint;
    }

    public double ReturnCharacterAttackRange()
    {
        return player.attackRange;
    }

    //데미지 경감용

    public void SetAntiPhenetData(float getReductionRate)
    {
        player.damageReductionRate = getReductionRate;
    }

    double AntiPhenetUse(double getAttackPowerData)
    {
        if (isAntiPhenetOn)
        {
            return getAttackPowerData * (1 - player.damageReductionRate);
        }
        else
            return getAttackPowerData;
    }

    public void SetCharacterHitPointMax(double getHitPoint)
    {
        player.maxHp += getHitPoint;
        player.curHp = player.maxHp;
    }

    public void SetCharacterHpRegen(double getHpRegen)
    {
        player.hpRegen = getHpRegen;
    }

    //부스터관련 오류 고치기 위해 만든 함수
    //부스터가 켜진 상태로 게임이 종료되면 해당 스피드가 다음까지 이어져서 해결하기 위해 만듬
    public void SetCharacterBasicSpeedError()
    {
        basicSpeed = player.moveSpeed;
    }


    // 밤이 종료될 때 실행되는 함수
    // 부스터 아이템으로 인한 이동 속도 증가를 기본 값으로 되돌리는 역할 수행 중
    public void NightEnd()
    {
        if (isBoosterOn)
        {
            SetMoveSpeed(basicSpeed);
        }
    }
}
