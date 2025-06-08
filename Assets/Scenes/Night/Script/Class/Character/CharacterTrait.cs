using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    //none = 0,
    startShield = 28,       //전투 시작시 최대체력 25%의 반 영구 보호막 획득
    dragEnemy = 42,         //매 6초마다 적 끌어당김
    thrustEnemy = 43,       //매 8초마다 적 밀쳐냄
    getMoveSpeed = 44,      //전투 시작 6초 끝나기전 6초 이속 증가
    getAttackPower = 45,    //체력이 일정 이하되면 공격력 증가
    stopEnemy = 61,         //20초마다 2초 적 이동속도 0
    absorbDamage = 62       //일반 공격 적중당 체력 회복 -> 풀피면 보호막 만듬
}


// 밤 씬에서 사용하는 특성 매니저
public class CharacterTrait : MonoBehaviour
{
    private Character character;

    [SerializeField] Transform enemyCloneParent;


    // 최종 데이터매니저에서 가져오는 데이터들
    [SerializeField] List<Trait> activeTraitList;

    // 이펙트 추가
    [SerializeField] GameObject[] traitPrefab;    // 0: 끌어오기, 1: 밀어내기




    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void Start()
    {
        FindActiveTrait();
        SetTrait();
    }

    void FindActiveTrait()
    {
        Player player = GameManager.Instance.player;

        //index = 28, 42, 43, 44, 45, 61, 62 총 7개의 액티브가 존재.
        int[] activeTraitIndices = { 28, 42, 43, 44, 45, 61, 62 };

        activeTraitList.Clear();

        foreach (int index in activeTraitIndices)
        {
            if (player.trait[index])
                activeTraitList.Add(DataManager.Instance.traitList.trait[index - 1]);
        }
    }

    void SetTrait()
    {
        foreach (Trait trait in activeTraitList)
        {
            if (trait != null && trait.effectType1 == (int)EffectType.active)
                ActiveTrait(trait);
        }
    }

    void ActiveTrait(Trait trait)
    {
        ActiveTrait activeTrait = (ActiveTrait)trait.index;

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
                StartCoroutine(AbsorbDamage(trait));
                break;
            default:
                Logger.LogError($"No handler for {activeTrait}");
                break;
        }
    }


    //특성 인덱스 28번 "생체 배리어" (매 전투 시작시, 캐릭터 최대 체력 25% 분량의 보호막 획득. 보호막은 파괴되기 전까지 반영구 유지.)
    void SetStartShield(Trait getTrait)
    {
        Logger.Log("특성: \"생체 배리어\" 활성화");
        character.Health.AddShield(character.player.maxHp * getTrait.effectValue1);
    }

    //특성 인덱스 42번 "자성" (매 6초에 1회, 캐릭터 중심 300 px. ~ 600 px. 거리 원형 범위에 있는 모든 적 150 px. 만큼 끌어옴.)
    IEnumerator DragEnemyCoroutine(Trait trait)
    {
        Logger.Log("특성: \"자성\" 활성화");

        GameObject magnetism = Instantiate(traitPrefab[0]);
        magnetism.transform.localPosition = character.transform.position;
        magnetism.transform.SetParent(character.transform);

        float maxRange = trait.effectValue3 / 60f;
        float minRange = trait.effectValue2 / 60f;
        magnetism.transform.localScale = Vector3.one * (maxRange / 3);

        Animator effectAnimator = magnetism.GetComponent<Animator>();

        while (!NightManager.Instance.isStageEnd)
        {
            yield return new WaitForSeconds(trait.effectValue1);    // 6초의 대기시간

            effectAnimator.SetBool("isPlay", true);

            Logger.Log("DrugEnemy");

            Collider2D[] getCols = GetOverLapColliders(minRange, maxRange);

            foreach (var col in getCols)
            {
                if (col.CompareTag("Normal") || col.CompareTag("Elite"))
                {
                    var enemy = col.GetComponent<Enemy>();
                    enemy.DrugEnemy(minRange);
                }
            }

            StartCoroutine(SetAnimatorParameter(effectAnimator, "DrugEnemyAnimation"));
        }
    }

    // 범위 내 오브젝트를 탐색하는 함수
    public Collider2D[] GetOverLapColliders(float minRadius, float maxRadius)
    {
        Collider2D[] allColliders = Physics2D.OverlapCircleAll(transform.position, maxRadius);

        List<Collider2D> result = new List<Collider2D>();

        foreach (Collider2D col in allColliders)
        {
            float distance = Vector2.Distance(transform.position, col.transform.position);
            if (minRadius <= distance && distance <= maxRadius)
                result.Add(col);
        }

        return result.ToArray();
    }






    //특성 인덱스 43번 "반자성" (매 8초에 1회, 캐릭터 중심으로 300 px. 원형 이내에 있는 모든 적을 200 px. 만큼 밀어냄.)
    IEnumerator ThrustEnemyCoroutine(Trait trait)
    {
        Logger.Log("특성: \"반자성\" 활성화");

        GameObject diamagnetism = Instantiate(traitPrefab[1]);
        diamagnetism.transform.localPosition = character.transform.position;
        diamagnetism.transform.SetParent(character.transform);


        float maxRange = (trait.effectValue2 + trait.effectValue3) / 60f;
        float minRange = (trait.effectValue2 + 100f) / 60f;
        diamagnetism.transform.localScale = Vector3.one * (minRange / 3);

        Animator effectAnimator = diamagnetism.GetComponent<Animator>();

        while (!NightManager.Instance.isStageEnd)
        {
            yield return new WaitForSeconds(trait.effectValue1);    // 8초의 대기시간

            effectAnimator.SetBool("isPlay", true);

            Debug.Log("ThrustEnemy");

            Collider2D[] getCols = GetOverLapColliders(minRange);

            foreach (Collider2D col in getCols)
            {
                if (col.CompareTag("Normal") || col.CompareTag("Elite"))
                {
                    var enemy = col.GetComponent<Enemy>();
                    enemy.ThrustEnemy(maxRange);
                }
            }

            StartCoroutine(SetAnimatorParameter(effectAnimator, "ThrustEnemyAnimation"));
        }
    }
    public Collider2D[] GetOverLapColliders(float radius)
    {
        Collider2D[] overLapColArr = Physics2D.OverlapCircleAll(transform.position, radius);

        return overLapColArr;
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

    //특성 인덱스 44번 "초전도" (전투 타이머 6초 이전, 54초 이후는 캐릭터의 이동속도 +30%, (6초 + 6초 = 총 12초))
    IEnumerator GetMoveSpeed(Trait trait)
    {
        // TODO: 현재 다른 특성, 아이템과 이속 증가가 중복되면 중복 값은 적용되지 않음
        Logger.Log("특성: \"초전도\" 활성화");

        float defaultMoveSpeed = character.player.moveSpeed;
        float upgradeMoveSpeed = character.player.moveSpeed * (1 + trait.effectValue3);

        //60~54초

        Logger.Log($"첫 번째 이속 증가 {trait.effectValue2}");
        character.Movement.SetMoveSpeed(upgradeMoveSpeed);
        yield return new WaitUntil(() => TimerManager.Instance.timerCount <= trait.effectValue2);

        Logger.Log($"이속 정상화 {trait.effectValue1}");
        character.Movement.SetMoveSpeed(defaultMoveSpeed);
        yield return new WaitUntil(() => TimerManager.Instance.timerCount <= trait.effectValue1);

        Logger.Log($"두 번째 이속 증가");
        character.Movement.SetMoveSpeed(upgradeMoveSpeed);
        yield return new WaitUntil(() => NightManager.Instance.isStageEnd);

        character.Movement.SetMoveSpeed(defaultMoveSpeed);
    }

    //특성 인덱스 45번 "오버클럭" (캐릭터의 현재체력이 캐릭터 최대체력의 40% 이하가 되면, 공격력 +30%)
    IEnumerator GetAttackPower(Trait getTrait)
    {
        Logger.Log("특성: \"오버클럭\" 활성화");

        float maxHp = character.player.maxHp;
        float goalHp = maxHp * getTrait.effectValue1;
        float currentAttackPower = character.player.attackPower;
        float enforcedAttackPower = character.player.attackPower * (1 + getTrait.effectValue2);

        bool isBoosted = false;

        void CheckHpCondition()
        {
            bool shouldBoost = (character.player.curHp <= goalHp);

            if (shouldBoost && !isBoosted)
            {
                character.player.attackPower = enforcedAttackPower;
                isBoosted = true;
                Logger.Log("오버클럭 활성화: 공격력 +30%");
            }
            else if (!shouldBoost && isBoosted)
            {
                character.player.attackPower = currentAttackPower;
                isBoosted = false;
                Logger.Log("오버클럭 비활성화: 공격력 원상복구");
            }
        }

        // 초기 상태 체크
        CheckHpCondition();

        character.Health.OnHpChanged += CheckHpCondition;

        yield return new WaitUntil(() => NightManager.Instance.isStageEnd);

        character.Health.OnHpChanged -= CheckHpCondition;

        if (isBoosted)
        {
            character.player.attackPower = currentAttackPower;
            Logger.Log("전투 종료: 공격력 원상복구");
        }
    }




    //특성 인덱스 61번 "EMP 폭발" (매 20초에 1회, 2초간 모든 적의 이동을 정지시킴.)
    IEnumerator StopEnemy(Trait trait)
    {
        Logger.Log("특성: \"EMP 폭발\" 활성화");

        float stopTime = trait.effectValue2;
        float intervalTime = trait.effectValue1;

        WaitForSeconds intervalWait = new WaitForSeconds(intervalTime);
        WaitForSeconds durationWait = new WaitForSeconds(stopTime);

        while (!NightManager.Instance.isStageEnd)
        {
            yield return intervalWait;

            Logger.Log("적 스탑");
            foreach (Transform child in enemyCloneParent)
            {
                GameObject enemyObj = child.gameObject;
                Enemy enemyComponent = enemyObj.GetComponent<Enemy>();

                if (enemyComponent != null)
                {
                    enemyComponent.isStop = true;
                }
                else
                {
                    Logger.Log("적 null");
                }
            }
            yield return durationWait;

            Logger.Log("적 무빙");
            foreach (Transform child in enemyCloneParent)
            {
                GameObject enemyObj = child.gameObject;
                Enemy enemyComponent = enemyObj.GetComponent<Enemy>();

                if (enemyComponent != null && enemyObj.activeInHierarchy)
                {
                    enemyComponent.isStop = false;
                }

            }
        }
    }


    //특성 인덱스 62번 "고급 신체 강화" (일반공격 적중당 회복 +1, 모든 종류의 회복이 최대체력 초과하여 회복시 보호막 전환. 페널티로 캐릭터의 보호막은 최대체력을 넘을 수 없음.)
    IEnumerator AbsorbDamage(Trait getTrait)
    {
        Logger.Log("특성: \"고급 신체 강화\" 활성화");

        character.player.healByHit += getTrait.effectValue2;
        character.Health.isOverhealToShield = true;


        yield return new WaitUntil(() => NightManager.Instance.isStageEnd);
        character.Health.isOverhealToShield = false;
    }
}
