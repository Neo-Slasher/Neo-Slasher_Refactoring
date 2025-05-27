using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] public NightManager nightManager;
    [SerializeField] public ItemManager itemManager;

    // 플레이어 데이터
    [SerializeField]
    Player player;

    [SerializeField]
    GameObject hpBar;


    [SerializeField]
    Rigidbody2D characterRigid;
    [SerializeField]
    SpriteRenderer characterSpriteRanderer;

    [SerializeField]
    Rigidbody2D hitBoxRigid;



    [SerializeField]
    GameObject hitBox;
    [SerializeField]
    HitBox hitBoxScript;


    const float HIT_BOX_DISTANCE = 3; //히트박스와 캐릭터와의 거리

    public Vector3 nowDir;
    public Vector3 fixPos = Vector3.zero;

    bool isAttack;
    public bool canChange = false;
    public bool isAbsorb = false;
    public double nowMoveSpeed;

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

    private void Awake()
    {
        player = GameManager.instance.player;

        // Tolelom: 밤 씬 시작시에는 체력을 최대 체력으로 변경할 지 고민 중
        if (player.curHp == 0)
            player.curHp = player.maxHp;

        characterRigid = GetComponent<Rigidbody2D>();
        characterSpriteRanderer = GetComponent<SpriteRenderer>();
        characterSpriteRanderer.flipX = false;

        animator = GetComponent<Animator>();

        //캐릭터의 스테이터스를 장비 등 변화에 따라 변화시킨다.
        hitBoxScript.getAttackPower = player.attackPower;    //무기 공격력 임시로 줌
        nowMoveSpeed = player.moveSpeed;
    }

    private void Start()
    {
        SetHitBoxScale();
        StartAttack();

        if (player.hpRegen > 0)
            HpRegen();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //캐릭터 데미지 받을 때
        CharacterDamaged(collision);
    }













    void SetHitBoxScale()
    {
        float goalY = (float)GameManager.instance.player.attackRange * 0.15f;
        Vector3 goalScale = new Vector3(1, goalY, 1);

        hitBox.transform.localScale = goalScale;
    }



    // 아래는 이동 관련 로직(이 주석은 리팩토링이 끝나면 지울 것)
    public void StartMove(Vector3 joystickDir)
    {
        nowDir = joystickDir.normalized;
        characterRigid.linearVelocity = joystickDir.normalized * ConvertMoveSpeedToPixelSpeed(player.moveSpeed);
        animator.SetBool("move", true);
        characterSpriteRanderer.flipX = (nowDir.x < 0) ? false : true;

        //홀로그램도 같이 움직이도록
        if (isHologramAnimate)
        {
            for (int i = 0; i < 2; i++)
            {
                hologramAnimatorArr[i].SetBool("move", true);
                hologramRendererArr[i].flipX = (nowDir.x < 0) ? false : true;
            }
        }

        //transform.localScale = (nowDir.x < 0) ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);     아이템도 같이 이동해서 주석처리했어
    }

    public void EndMove()
    {
        nowDir = Vector3.zero;
        characterRigid.linearVelocity = Vector3.zero;
        animator.SetBool("move", false);

        //홀로그램도 같이 움직이도록
        if (isHologramAnimate)
        {
            for (int i = 0; i < 2; i++)
            {
                hologramAnimatorArr[i].SetBool("move", false);
            }
        }
    }

    float ConvertMoveSpeedToPixelSpeed(double getMoveSpeed)
    {
        return ((float)getMoveSpeed * 25) / 100;
    }

    //Character 스크립트에서는 공격 애니메이션과 히트 박스 온오프만 사용
    //실질적 데이터 교환은 enemy 스크립트에서 이루어짐.
    public void StartAttack()
    {
        if (!isAttack)
        {
            isAttack = true;
            StartCoroutine(AttackCoroutine());
        }
    }

    public void StopAttack()
    {
        StopCoroutine(AttackCoroutine());
    }

    IEnumerator AttackCoroutine()
    {
        float attackSpeed = 10 / (float)GameManager.instance.player.attackSpeed;

        while (!nightManager.isStageEnd)
        {
            if (!isDoubleAttack)
            {
                //공격 애니메이션 진행
                animator.SetTrigger("attack");

                //홀로그램도 같이 움직이도록
                if (isHologramAnimate)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        hologramAnimatorArr[i].SetTrigger("attack");
                    }
                }

                hitBox.SetActive(true);
                SetHitbox();
                yield return new WaitForSeconds(0.2f);
                hitBox.SetActive(false);

                isMoveBackOn = false;
            }
            else
            {
                itemManager.SetMultiSlasherSprite(true);
                hitBox.SetActive(true);
                SetHitbox();
                yield return new WaitForSeconds(0.1f);
                hitBox.SetActive(false);

                yield return new WaitForSeconds(0.1f);

                hitBox.SetActive(true);
                SetHitbox();
                yield return new WaitForSeconds(0.2f);
                hitBox.SetActive(false);
                itemManager.SetMultiSlasherSprite(false);

                isDoubleAttack = false;
                isMoveBackOn = false;
            }

            //다음 공격까지 대기
            yield return new WaitForSeconds(attackSpeed);
            isAbsorb = false;
            isAttack = false;
        }
    }

    //이동속도 컨트롤
    public void SetMoveSpeed(double moveSpeed)
    {
        player.moveSpeed = moveSpeed;
    }

    //이동 방향에 따라 히트박스 위치 조절하는 함수
    public void SetHitbox()
    {
        fixPos = (nowDir.normalized != Vector3.zero) ? nowDir.normalized : ((fixPos == Vector3.zero) ? Vector3.left : fixPos);

        hitBox.transform.localPosition = this.transform.position + fixPos * HIT_BOX_DISTANCE;
        float dot = Vector3.Dot(fixPos, new Vector3(1, 0, 0));
        float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

        if (fixPos.y >= 0)
            hitBox.transform.rotation = Quaternion.Euler(0, 0, angle);
        else
            hitBox.transform.rotation = Quaternion.Euler(0, 0, 360 - angle);

        StartCoroutine(SetHitBoxCoroutine());
    }

    IEnumerator SetHitBoxCoroutine()
    {
        while (hitBox.gameObject.activeSelf == true)
        {
            hitBoxRigid.linearVelocity = nowDir.normalized * ConvertMoveSpeedToPixelSpeed(player.moveSpeed);
            yield return null;
        }
    }


    // 나중에 리팩토링 할 예정
    // 1. 함수명
    // 2. 로직
    public void AbsorbAttack()   //canChange가 참이면 최대 체력일때 쉴드로 전환 가능
    {
        if (player.healByHit > 0 && !isAbsorb)
        {
            isAbsorb = true;

            if (player.curHp + player.healByHit < player.maxHp)
            {
                player.curHp += player.healByHit;
                hpBar.GetComponent<HealthBar>().UpdateBar();
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
                hpBar.GetComponent<HealthBar>().UpdateBar();
            }
        }
    }

    void HpRegen()
    {
        StartCoroutine(HpRegenCoroutine());
    }

    IEnumerator HpRegenCoroutine()
    {
        while (!nightManager.isStageEnd)
        {
            if (player.curHp < player.maxHp)
            {
                if (player.curHp + player.hpRegen >= player.maxHp)
                {
                    player.curHp = player.maxHp;
                }
                else
                {
                    player.curHp += player.hpRegen;
                }

                hpBar.GetComponent<HealthBar>().UpdateBar();
                yield return new WaitForSeconds(1);
            }
            else
                yield return new WaitForSeconds(1);
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

        if (enemy.tag == "Normal")
        {
            nowAttackPower = enemy.GetComponent<NormalEnemy>().GetEnemyAttackPower();
            enemy.GetComponent<NormalEnemy>().SetIsAttacked();
        }
        else if (enemy.tag == "Elite")
        {
            nowAttackPower = enemy.GetComponent<EliteEnemy>().GetEnemyAttackPower();
            enemy.GetComponent<EliteEnemy>().SetIsAttacked();
        }
        else if (enemy.tag == "Projectile")
        {
            if (enemy.GetComponent<Projectile>().isEnemy)
            {
                nowAttackPower = enemy.transform.parent.GetComponent<EliteEnemy>().GetEnemyAttackPower();
                enemy.transform.parent.GetComponent<EliteEnemy>().SetIsAttacked();
            }
        }
        else
        {
            Debug.Log(enemy.name + "이 정상적이지 않아서 공격력을 받아올 수 없음");
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
                hpBar.GetComponent<HealthBar>().UpdateBar();
            }
            else
            {
                double nowAttactDamage = getAttackData - (float)player.shieldPoint;
                player.shieldPoint = 0;
                hpBar.GetComponent<HealthBar>().UpdateBar();

                player.curHp -= nowAttactDamage;
                //감소한 만큼 플레이어 체력바 줄어들게
                hpBar.GetComponent<HealthBar>().UpdateBar();
            }
        }
        else
        { // 체력으로 데미지 받을 때
            //Hp - 적 데미지 계산
            if (player.curHp > getAttackData)
            {
                Debug.Log("damaged");
                player.curHp -= getAttackData;
                hpBar.GetComponent<HealthBar>().UpdateBar();
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
                hpBar.GetComponent<HealthBar>().UpdateBar();

                this.GetComponent<BoxCollider2D>().enabled = false;

                nightManager.SetStageEnd();
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
            Color nowColor = characterSpriteRanderer.color;
            nowColor.a = 0.7f;
            characterSpriteRanderer.color = nowColor;
            yield return new WaitForSeconds(0.2f);

            nowColor.a = 1f;
            characterSpriteRanderer.color = nowColor;
            yield return new WaitForSeconds(0.3f);
        }
    }

    // 28번 특성 활성화 시 실행되는 코드
    public void SetStartShieldPointData(float getShieldPoint)
    {
        player.shieldPoint = (float)player.maxHp * getShieldPoint;
        hpBar.GetComponent<HealthBar>().UpdateBar();
    }

    //해당 숫자만큼 쉴드 생성
    public void SetShieldPointData(float getShieldPoint)
    {
        player.shieldPoint = getShieldPoint;
        hpBar.GetComponent<HealthBar>().UpdateBar();
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
        hitBoxScript.getAttackPower = player.attackPower;
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

    public void UpdateKillCount()
    {
        nightManager.UpdateKillCount();
    }
    public void UpdateKillNormalCount()
    {
        nightManager.killNormal++;
    }
    public void UpdateKillEliteCount()
    {
        nightManager.killElite++;
    }

    public double ReturnCharacterMoveSpeed()
    {
        return player.moveSpeed;
    }

    public Vector3 ReturnSpeed()
    {
        return nowDir.normalized * ConvertMoveSpeedToPixelSpeed(player.moveSpeed);
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
                    hpBar.GetComponent<HealthBar>().UpdateBar();
                    break;
                }

                hpBar.GetComponent<HealthBar>().UpdateBar();
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
