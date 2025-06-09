using System.Collections;
using System.Linq;
using UnityEngine;


public class Character : MonoBehaviour
{
    // 플레이어 데이터
    public Player player;

    public CharacterMovement Movement {  get; private set; }
    public CharacterAttack Attack { get; private set; }
    public CharacterHealth Health { get; private set; }
    public CharacterAnimation Animation { get; private set; }

    [Header("HpBar")]
    [SerializeField] GameObject hpBar;
    public HealthBar hpBarScript;


    public bool isAbsorb = false;

    //아이템 관련
    public bool isHologramTrickOn = false;
    public bool isHologramAnimate = false;
    public bool isAntiPhenetOn = false;
    public bool isMoveBackOn = false;

    // 부스터 아이템 itemIdx: 13
    public bool isBoosterOn = false;
    public float basicSpeed; // 부스터 아이템으로 인한 speed 증감치를 종료 시에 원상복구 위함

    // 애니메이션
    public Animator[] hologramAnimatorArr;
    public SpriteRenderer[] hologramRendererArr;



    private void Awake()
    {
        player = GameManager.Instance.player;

        if (hpBar != null)
            hpBarScript = hpBar.GetComponent<HealthBar>();


        Movement = GetComponent<CharacterMovement>();

        Attack = GetComponent<CharacterAttack>();
        Health = GetComponent<CharacterHealth>();
        Animation = GetComponent<CharacterAnimation>();
    }

    private void Start()
    {
        Movement.Init(player.moveSpeed);
        InitializeCharacter();
        Attack.StartAttack();
    }

    private void InitializeCharacter()
    {
        Health.HealToMaxHp();
        Animation.LookRight();

        if (player.hpRegen > 0)
            Health.StartHpRegen();
    }





    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            CharacterDamaged(collision);
        }
    }

    public void CharacterDamaged(Collider2D enemyCollision)
    {

        GameObject enemy = enemyCollision.gameObject;
        float nowAttackPower = 0;

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
            Animation.PlayDamagedAnimation();

        if (isHologramTrickOn)
            return;

        Health.TakeDamage(nowAttackPower);
    }

    public void SetHpBarPosition()
    {
        Vector3 hpBarPos = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y - 3, 0));
        hpBar.transform.position = hpBarPos;
    }

    // 밤이 종료될 때 실행되는 함수
    // 부스터 아이템으로 인한 이동 속도 증가를 기본 값으로 되돌리는 역할 수행 중
    public void NightEnd()
    {
        EndCoroutines();
        Movement.EndMove();
        if (isBoosterOn)
        {
            Movement.SetMoveSpeed(basicSpeed);
        }
        // TODO: 코루틴 중단 코드 넣기
    }

    // 설정 창이 켜질 때 실행되는 일시정지 함수
    public void Pause()
    {
        if (isBoosterOn)
        {
            Movement.SetMoveSpeed(basicSpeed);
        }
        // TODO: 코루틴 중단 코드 넣기
    }

    private void EndCoroutines()
    {
        Health.StopHpRegen();
        Attack.StopAttack();

    }




    // 아래는 특성이나 아이템 관련 코드(나중에 리팩토링)


    //해당 숫자만큼 쉴드 생성
    public void SetShieldPointData(float getShieldPoint)
    {
        Health.AddShield(getShieldPoint);
        player.shieldPoint = getShieldPoint;
    }




    public Vector3 GetVelocity()
    {
        return Movement.MoveDirection.normalized * Movement.pixelMoveSpeed;
    }

    //아이템 6번 퍼스트 에이드에서 체력 회복할 때 쓰려고 만듬
    public void HealHp(float getHealHp, GameObject getImage)
    {
        StartCoroutine(HealHpCoroutine(getHealHp, getImage));
    }

    IEnumerator HealHpCoroutine(float getHealHp, GameObject getImage)
    {
        float time = 3;
        float deltaTime = 0;
        float value = 0;
        float nowHeal = 0;

        getImage.SetActive(true);

        while (time > deltaTime)
        {
            deltaTime += Time.deltaTime;
            value = getHealHp * Time.deltaTime;
            nowHeal += value;

            //힐량 초과시 종료
            if (nowHeal >= getHealHp)
                break;

            Health.Heal(value);

            //만약 피가 오버되면 종료
            if (player.curHp == player.maxHp)
                break;

            yield return null;
        }
        getImage.SetActive(false);
    }

    //데미지 경감용
    public void SetAntiPhenetData(float getReductionRate)
    {
        player.damageReductionRate = getReductionRate;
    }

    float AntiPhenetUse(float getAttackPowerData)
    {
        if (isAntiPhenetOn)
        {
            return getAttackPowerData * (1 - player.damageReductionRate);
        }
        else
            return getAttackPowerData;
    }

    //부스터관련 오류 고치기 위해 만든 함수
    //부스터가 켜진 상태로 게임이 종료되면 해당 스피드가 다음까지 이어져서 해결하기 위해 만듬
    public void SetCharacterBasicSpeedError()
    {
        basicSpeed = player.moveSpeed;
    }
}
