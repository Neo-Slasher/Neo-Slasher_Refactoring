using System;
using System.Collections;
using UnityEngine;

public class CharacterHealth : MonoBehaviour
{
    private Character character;
    public CharacterAnimation Animation { get; private set; }

    public event Action OnHpChanged; // 체력 변경 시 호출
    public event Action OnDeath; // 사망 시 호출

    private BoxCollider2D BoxCollider2D;

    private Coroutine hpRegenCoroutine;

    public bool isOverhealToShield;

    private void Awake()
    {
        character = GetComponent<Character>();
        Animation = GetComponent<CharacterAnimation>();
        BoxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        isOverhealToShield = false;
    }

    public void TakeDamage(float damage)
    {
        if (damage <= 0) return; 

        float remainingDamage = damage;

        // 데미지 경감 계산
        remainingDamage = remainingDamage * (1 - character.player.damageReductionRate / 100);
        
        if (remainingDamage <= 0) return;

        if (character.player.shieldPoint > 0)
        {
            float shieldDamage = Mathf.Min(character.player.shieldPoint, remainingDamage);
            character.player.shieldPoint -= shieldDamage;
            remainingDamage -= shieldDamage;
        }

        if (remainingDamage > 0)
        {
            character.player.curHp = Mathf.Max(character.player.curHp - remainingDamage, 0f);
            OnHpChanged?.Invoke();

            if (character.player.curHp == 0)
            {
                Animation.SetTrigger("die");

                BoxCollider2D.enabled = false;

                ////홀로그램도 같이 움직이도록
                //if (isHologramAnimate)
                //{
                //    for (int i = 0; i < 2; i++)
                //    {
                //        hologramAnimatorArr[i].SetTrigger("die");
                //    }
                //}

                NightManager.Instance.SetStageEnd();
                OnDeath?.Invoke();
            }
        }
        else
            OnHpChanged?.Invoke();
    }



    public void Heal(float amount)
    {
        if (amount < 0) return; // 음수 값 차단

        if (!isOverhealToShield)
        {
            character.player.curHp = Mathf.Min(character.player.curHp + amount, character.player.maxHp);
        }
        else
        {
            float healAmount = Mathf.Min(character.player.maxHp - character.player.curHp, amount);

            character.player.curHp += healAmount;

            float shieldAmount = amount - healAmount;

            if (shieldAmount > 0)
            {
                // 쉴드 최대 값은 플레이어의 maxHp보다 클 수 없음
                AddShield(Mathf.Min(character.player.maxHp - character.player.shieldPoint, shieldAmount));
            }
        }
        OnHpChanged?.Invoke();
    }

    public void HealToMaxHp()
    {
        Logger.Log("Heal To Max Hp Called");
        character.player.curHp = character.player.maxHp;
        OnHpChanged?.Invoke();
    }

    public void AddShield(float amount)
    {
        if (amount < 0) return; // 음수 값 차단
        character.player.shieldPoint += amount;
        OnHpChanged?.Invoke();
    }

    public void StartHpRegen()
    {
        if (character.player.hpRegen <= 0f) return;
        if (hpRegenCoroutine == null)
            hpRegenCoroutine = StartCoroutine(HpRegenCoroutine());
    }

    public void StopHpRegen()
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
            Heal(character.player.hpRegen);
            yield return new WaitForSeconds(HP_REGEN_TICK);
        }

        hpRegenCoroutine = null;
    }

    // 플레이어가 피흡이 존재할 때, 피흡 계산하는 로직
    public void HealByHit()   //canChange가 참이면 최대 체력일때 쉴드로 전환 가능
    {
        if (character.player.healByHit <= 0) return;

        Heal(character.player.healByHit);
    }
}
