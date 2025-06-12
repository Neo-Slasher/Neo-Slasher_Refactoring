using System.Collections;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.TextCore.Text;


public class Character : MonoBehaviour
{
    // 플레이어 데이터
    public Player player;

    public CharacterMovement Movement { get; private set; }
    public CharacterAttack Attack { get; private set; }
    public CharacterHealth Health { get; private set; }
    public CharacterAnimation Animation { get; private set; }

    [Header("HpBar")]
    [SerializeField] GameObject hpBar;
    public HealthBar hpBarScript;


    public bool isAbsorb = false;

    //아이템 관련
    // 홀로그램 트릭 온오프(On이면 캐릭터가 무적)
    public bool isHologramTrickOn = false;

    // 부스터 아이템 itemIdx: 13
    public bool isBoosterOn = false;
    public float basicSpeed; // 부스터 아이템으로 인한 speed 증감치를 종료 시에 원상복구 위함

    // 애니메이션
    public Animator[] hologramAnimatorArr;
    public SpriteRenderer[] hologramRendererArr;

    Coroutine knockbackCoroutine;


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

        bool isKnockback = false;

        switch (enemy.tag)
        {
            case "Normal":
                nowAttackPower = enemy.GetComponent<NormalEnemy>().GetEnemyAttackPower();
                enemy.GetComponent<NormalEnemy>().SetIsAttacked();
                isKnockback = enemy.GetComponent<Enemy>().stats.canKnockback;
                break;
            case "Elite":
                nowAttackPower = enemy.GetComponent<EliteEnemy>().GetEnemyAttackPower();
                enemy.GetComponent<EliteEnemy>().SetIsAttacked();
                isKnockback = enemy.GetComponent<Enemy>().stats.canKnockback;
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

        if (isKnockback)
        {
            Knockback(enemyCollision.transform);
            // 넉백
        }

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


    private void Knockback(Transform enemyTransform)
    {
        if (knockbackCoroutine != null)
            StopCoroutine(knockbackCoroutine);

        StartCoroutine(KnockbackCoroutine(transform.position, enemyTransform));
    }

    IEnumerator KnockbackCoroutine(Vector3 startPosition, Transform enemyTransform)
    {
        const float KNOCKBACK_DISTANCE = 1.5f; // 150px
        const float KNOCKBACK_SPEED = 5f;      // 넉백 속도

        Vector3 nowVelocity = GetComponent<Rigidbody2D>().linearVelocity;
        Vector3 moveDir = (transform.position - enemyTransform.position).normalized;


        Movement.isKnockback = true;

        Rigidbody2D rigid = GetComponent<Rigidbody2D>();

        //적 초기 위치 기준 150px 튕김
        try
        {
            while ((startPosition - transform.position).magnitude <= KNOCKBACK_DISTANCE)
            {
                rigid.linearVelocity = moveDir * KNOCKBACK_SPEED;
                yield return null;
            }

        }
        finally
        {
            Movement.isKnockback = false;
        }
    }




    // 아래는 특성이나 아이템 관련 코드(나중에 리팩토링)

    public Vector3 GetVelocity()
    {
        return Movement.MoveDirection.normalized * Movement.pixelMoveSpeed;
    }



    //부스터관련 오류 고치기 위해 만든 함수
    //부스터가 켜진 상태로 게임이 종료되면 해당 스피드가 다음까지 이어져서 해결하기 위해 만듬
    public void SetCharacterBasicSpeedError()
    {
        basicSpeed = player.moveSpeed;
    }
}
