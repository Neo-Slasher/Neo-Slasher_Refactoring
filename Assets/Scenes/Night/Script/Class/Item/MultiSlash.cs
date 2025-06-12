using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MultiSlash : MonoBehaviour
{
    Character character;
    float coolTime;
    float attackPowerRate;
    float attackRangeRate;
    int rank;

    [SerializeField] private Image coolTimeImage;

    [SerializeField] private GameObject swordAura;

    void Awake()
    {
        character = GameObject.Find("CharacterImage").GetComponent<Character>();
    }

    public void InitializeMultiSlash(Consumable item, Image coolTimeImage)
    {
        SetMultiSlashData(item);
        transform.SetParent(character.transform);
        this.coolTimeImage = coolTimeImage;
    }

    private void SetMultiSlashData(Consumable item)
    {
        attackPowerRate = item.attackPowerValue;
        attackRangeRate = item.attackRangeValue;
        rank = item.rank;

        switch (item.rank)
        {
            case 0:
                coolTime = 6;
                break;
            case 1:
                coolTime = 5;
                break;
            case 2:
                coolTime = 4;
                break;
            case 3:
                coolTime = 3;
                break;
        }
    }

    public void StartMultiSlash()
    {
        StartCoroutine(MultiSlashCoroutine());
    }


    IEnumerator MultiSlashCoroutine()
    {
        //2회 연속공격 + 에너지파
        while (!NightManager.Instance.isStageEnd)
        {
            coolTimeImage.fillAmount = 1;
            float timer = 0;

            while (timer < coolTime)
            {
                timer += Time.deltaTime;
                coolTimeImage.fillAmount = 1 - timer / coolTime;
                yield return null;
            }

            character.Attack.isDoubleAttack = true;
            yield return new WaitUntil(() => !character.Attack.isDoubleAttack);

            // 멀티 슬래시의 더블 어택 세부 구현은 CharacterAttack의 DoubleAttack 참고
            ShootSwordAura();
        }
    }


    // 아이템 "멀티 슬래시"의 검기 방출 코드
    void ShootSwordAura()
    {
        GameObject swordAuraObj = Instantiate(swordAura);

        SwordAura swordAuraScript = swordAuraObj.GetComponent<SwordAura>();
        swordAuraScript.InitializeSwordAura(attackPowerRate, attackRangeRate, rank);
        swordAuraScript.StartSwordAura();
    }



}
