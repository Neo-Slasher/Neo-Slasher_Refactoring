using System.Collections;
using UnityEngine;

public class CharacterAttack : MonoBehaviour
{
    private Character character;
    private Animator animator;


    [SerializeField] private GameObject hitBox;
    private HitBox hitBoxScript;
    private Rigidbody2D hitBoxRigid;



    private Coroutine attackCoroutine;
    private Coroutine hitBoxFollowCoroutine;


    public bool canChange = false;
    public bool isAbsorb = false;
    public bool isDoubleAttack = false;
    //    public bool isMoveBackOn = false;
    //    public bool isHologramTrickOn = false;
    //    public bool isHologramAnimate = false;
    //    public bool isAntiPhenetOn = false;


    private void Awake()
    {
        character = GetComponent<Character>();
        animator = GetComponent<Animator>();

        if (hitBox != null)
        {
            hitBoxScript = hitBox.GetComponent<HitBox>();
            hitBoxRigid = hitBox.GetComponent<Rigidbody2D>();
        }
    }

    private void Start()
    {
        InitializeHitBox();
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
                // 공격 속도 10당 1초에 1회 공격
                float attackInterval = 10 / (float)GameManager.Instance.player.attackSpeed;

                if (!isDoubleAttack) // single attack
                {
                    yield return ExecuteSingleAttack(attackInterval);
                }
                else // double attack
                {
                    yield return ExecuteDoubleAttack(attackInterval);
                }

                //isAbsorb = false;
            }
        }
        finally
        {
            attackCoroutine = null;
        }
    }

    private IEnumerator ExecuteSingleAttack(float attackInterval)
    {
        float attackAnimationTime = 0.2f;
        float minAttackDelay = 0.1f;

        TriggerAttackAnimations();

        ActivateHitBox(attackAnimationTime);

        //isMoveBackOn = false;

        float remainingDelay = Mathf.Max(minAttackDelay, attackInterval - attackAnimationTime);
        yield return new WaitForSeconds(remainingDelay);

    }

    private IEnumerator ExecuteDoubleAttack(float attackInterval)
    {
        ItemManager.Instance.SetMultiSlasherSprite(true);
        float attackAnimationTime = 0.4f;

        ActivateHitBox(0.1f);

        yield return new WaitForSeconds(0.1f);

        ActivateHitBox(0.2f);

        //isMoveBackOn = false;
        isDoubleAttack = false;

        float remainingDelay = Mathf.Max(0.1f, attackInterval - attackAnimationTime);
        yield return new WaitForSeconds(remainingDelay);
    }

    private void ActivateHitBox(float activeTime)
    {
        if (hitBox == null) return;

        hitBox.SetActive(true);

        // attack range 적용 코드
        hitBox.transform.localScale = new Vector3(character.player.attackRange / 10, 1.5f, 1);
        
        CancelInvoke(nameof(DeactivateHitBox));
        Invoke(nameof(DeactivateHitBox), activeTime);

        if (hitBoxFollowCoroutine != null)
            StopCoroutine(hitBoxFollowCoroutine);
        hitBoxFollowCoroutine = StartCoroutine(SetHitBoxCoroutine());
    }

    private void DeactivateHitBox()
    {
        if (hitBox != null)
            hitBox.SetActive(false);

        if (hitBoxFollowCoroutine != null)
        {
            StopCoroutine(hitBoxFollowCoroutine);
            hitBoxFollowCoroutine = null;
        }
    }

    private void TriggerAttackAnimations()
    {
        animator.SetTrigger("attack");

        //if (!isHologramAnimate || hologramAnimatorArr == null) return;

        //foreach (var hologramAnimator in hologramAnimatorArr)
        //{
        //    if (hologramAnimator != null)
        //        hologramAnimator.SetTrigger("attack");
        //}
    }

    IEnumerator SetHitBoxCoroutine()
    {
        const float HIT_BOX_DISTANCE = 3; //히트박스와 캐릭터와의 거리

        Vector2 attackDirection = character.Movement.LastMoveDirection.normalized;

        float angle = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg;

        while (hitBox != null && hitBox.activeSelf)
        {

            hitBox.transform.localPosition = transform.position + (Vector3)attackDirection * HIT_BOX_DISTANCE;
            hitBox.transform.rotation = Quaternion.Euler(0, 0, angle);

            if (hitBoxRigid != null)
                hitBoxRigid.linearVelocity = character.Movement.MoveDirection.normalized * character.Movement.pixelMoveSpeed;

            yield return null; // 매 프레임마다 갱신
        }
    }



}
