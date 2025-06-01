using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EffectType
{
    none, hp, moveSpeed, attackPower, attackSpeed,
    attackRange, startMoney, earnMoney, shopSlot,
    itemSlot, shopMinRank, shopMaxRank, dropRank,
    dropRate, healByHit, hpRegen, dealOnMax, dealOnHp,
    active
}

public enum ActiveTrait
{
    none = 0,
    startShield = 28,       //전투 시작시 최대체력 25%의 반 영구 보호막 획득
    dragEnemy = 42,         //매 6초마다 적 끌어당김
    thrustEnemy = 43,       //매 8초마다 적 밀쳐냄
    getMoveSpeed = 44,      //전투 시작 6초 끝나기전 6초 이속 증가
    getAttackPower = 45,    //체력이 일정 이하되면 공격력 증가
    stopEnemy = 61,         //20초마다 2초 적 이동속도 0
    absorbDamage = 62       //일반 공격 적중당 체력 회복 -> 풀피면 보호막 만듬
}


// 밤 씬에서 사용하는 특성 매니저
public class TraitManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] NightManager nightManager;
    [SerializeField] TimerManager timerManager;


    [SerializeField]
    Character character;
    [SerializeField]
    Transform enemyCloneParent;


    // 최종 데이터매니저에서 가져오는 데이터들
    [SerializeField]
    List<Trait> activeTraitList;

    // 이펙트 추가
    [SerializeField]
    GameObject[] traitEffectArr;    // 0: 끌어오기, 1: 밀어내기

    private void Start()
    {
        FindActiveTrait();
        SetTrait();
    }

    // 플레이어 데이터를 가져와 액티브 특성을 저장합니다.
    void FindActiveTrait()
    {
        Player player = GameManager.Instance.player;

        //index = 28, 42, 43, 44, 45, 61, 62 총 7개의 액티브가 존재.
        int[] activeTraitIndices = { 28, 42, 43, 44, 45, 61, 62 };

        activeTraitList.Clear();

        foreach (int index in activeTraitIndices)
        {
            if (player.trait[index])
            {
                activeTraitList.Add(DataManager.Instance.traitList.trait[index - 1]);
            }
        }
    }


    //선택한 특성을 실행하는 함수
    void SetTrait()
    {
        foreach (Trait trait in activeTraitList)
        {
            if (trait != null && trait.effectType1 == (int)EffectType.active)
            {
                ActiveTrait(trait);
            }
        }
    }


    void ActiveTrait(Trait trait)
    {
        ActiveTrait activeTrait = (ActiveTrait)trait.index;
        Debug.Log("액티브: " + activeTrait);

        switch (activeTrait)
        {
            case global::ActiveTrait.startShield:
                SetStartShield(trait);
                break;
            case global::ActiveTrait.dragEnemy:
                StartCoroutine(DragEnemyCoroutine(trait));
                break;
            case global::ActiveTrait.thrustEnemy:
                StartCoroutine(ThrustEnemyCoroutine(trait));
                break;
            case global::ActiveTrait.getMoveSpeed:
                StartCoroutine(GetMoveSpeed(trait));
                break;
            case global::ActiveTrait.getAttackPower:
                StartCoroutine(GetAttackPower(trait));
                break;
            case global::ActiveTrait.stopEnemy:
                StartCoroutine(StopEnemy(trait));
                break;
            case global::ActiveTrait.absorbDamage:
                AbsorbDamage(trait);
                break;
            default:
#if UNITY_EDITOR
                Debug.LogError($"No handler for {activeTrait}");
#endif
                break;
        }
    }


    //특성 인덱스 28번 게임 시작시 쉴드 생성 코드
    void SetStartShield(Trait getTrait)
    {
        character.SetStartShieldPointData((float)getTrait.effectValue1);
    }

    // character의 SetShieldImage부터 볼 것












    //특성 인덱스 42번 n초마다 주변 적 끌어당기기 코드
    IEnumerator DragEnemyCoroutine(Trait getTrait)
    {
        GameObject drugEnemyEffect = Instantiate(traitEffectArr[0]);
        drugEnemyEffect.transform.localPosition = character.transform.position;
        drugEnemyEffect.transform.SetParent(character.transform);
        float maxRange = ((float)getTrait.effectValue2 / 100) / 3;
        drugEnemyEffect.transform.localScale = new Vector3(maxRange, maxRange, maxRange);
        drugEnemyEffect.SetActive(false);
        Animator effectAnimator = drugEnemyEffect.GetComponent<Animator>();

        while (!nightManager.isStageEnd)
        {
            yield return new WaitForSeconds((float)getTrait.effectValue1);    //n초의 대기시간을 갖는 코드

            //이펙트 키기
            drugEnemyEffect.SetActive(true);
            effectAnimator.SetBool("isPlay", true);

#if UNITY_EDITOR
            Debug.Log("DrugEnemy");
#endif

            Collider2D[] getCols =
                character.ReturnOverLapColliders((float)getTrait.effectValue3 / 100, (float)getTrait.effectValue2 / 100);


            foreach (var col in getCols)
            {
                if (col.CompareTag("Normal") || col.CompareTag("Elite"))
                {
                    var enemy = col.GetComponent<Enemy>();
                    if (enemy != null)
                        enemy.DrugEnemy();
                }
            }

            StartCoroutine(SetAnimatorParameter(effectAnimator, "DrugEnemyAnimation"));
        }
    }

    //특성 인덱스 43번 n초마다 주변 적 밀어내기 코드
    IEnumerator ThrustEnemyCoroutine(Trait getTrait)
    {
        Collider2D[] arr;
        GameObject thrustEnemyEffect = Instantiate(traitEffectArr[0]);
        thrustEnemyEffect.transform.localPosition = character.transform.position;
        thrustEnemyEffect.transform.SetParent(character.transform);
        thrustEnemyEffect.SetActive(false);
        Animator effectAnimator = thrustEnemyEffect.GetComponent<Animator>();

        while (!nightManager.isStageEnd)
        {
            yield return new WaitForSeconds((float)getTrait.effectValue1);    //n초의 대기시간을 갖는 코드
            //이펙트
            thrustEnemyEffect.SetActive(true);
            effectAnimator.SetBool("isPlay", true);

            Debug.Log("ThrustEnemy");
            Collider2D[] getCols =
                character.ReturnOverLapColliders((float)getTrait.effectValue2 / 100);
            arr = getCols;
            if (getCols != null)
                //이제 당겨
                for (int i = 0; i < getCols.Length; i++)
                {
                    if (getCols[i].tag == "Normal" || getCols[i].tag == "Elite")
                    {
                        getCols[i].GetComponent<Enemy>().ThrustEnemy();
                        Debug.Log(getCols.Length);
                    }
                }

            StartCoroutine(SetAnimatorParameter(effectAnimator, "ThrustEnemyAnimation"));
        }
    }

    IEnumerator SetAnimatorParameter(Animator getAnimator, string animationName)
    {
        while (getAnimator.GetBool("isPlay"))
        {
            if (getAnimator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
            {
                getAnimator.SetBool("isPlay", false);
            }
            yield return new WaitForSeconds(1f);
        }
    }
    IEnumerator GetMoveSpeed(Trait getTrait)
    {
        double defaultMoveSpeed = character.nowMoveSpeed;
        double upgradeMoveSpeed = character.nowMoveSpeed * (1 + getTrait.effectValue3);

        //60~54초
        character.SetMoveSpeed(upgradeMoveSpeed);
        while (timerManager.timerCount >= getTrait.effectValue2)
        {
            yield return new WaitForSeconds(0.5f);
        }

        character.SetMoveSpeed(defaultMoveSpeed);

        //6~0초@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        while (timerManager.timerCount >= getTrait.effectValue1)
        {
            yield return new WaitForSeconds(0.5f);
        }
        character.SetMoveSpeed(upgradeMoveSpeed);
    }

    IEnumerator GetAttackPower(Trait getTrait)
    {
        double hitPointMax = character.ReturnCharacterHitPointMax();
        double hitPointGoal = hitPointMax * getTrait.effectValue1;
        double attackPower = character.ReturnCharacterAttackPower();
        attackPower *= (1 + getTrait.effectValue2);

        while (character.ReturnCharacterHitPoint() >= hitPointGoal)
            yield return new WaitForSeconds(0.1f);

        character.SetCharacterAttackPower(attackPower);
    }

    IEnumerator StopEnemy(Trait getTrait)
    {
        while (!nightManager.isStageEnd)
        {
            //yield return new WaitForSeconds(getTrait.traitEffectValue1);
            yield return new WaitForSeconds(5);
            //적 오브젝트 정지
            Debug.Log("stop");
            for (int i = 0; i < enemyCloneParent.childCount; i++)
            {
                if (enemyCloneParent.childCount != 0)
                {
                    enemyCloneParent.GetChild(i).GetComponent<Enemy>().EnemyStop();
                }
            }

            yield return new WaitForSeconds((float)getTrait.effectValue2);
            Debug.Log("move");
            //적 오브젝트 정지
            for (int i = 0; i < enemyCloneParent.childCount; i++)
            {
                if (enemyCloneParent.childCount != 0)
                {
                    if (enemyCloneParent.GetChild(i) != null)
                        enemyCloneParent.GetChild(i).GetComponent<Enemy>().EnemyMove();
                }
            }
        }
    }

    void AbsorbDamage(Trait getTrait)
    {
        character.SetAbsorbAttackData((float)getTrait.effectValue2);
        character.canChange = true;
    }
}
