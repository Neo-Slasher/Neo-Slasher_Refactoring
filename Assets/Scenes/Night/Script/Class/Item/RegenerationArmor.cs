using UnityEngine;

public class RegenerationArmor : MonoBehaviour
{
    private Character character;

    private float attackPowerRate;
    private float attackRangeRate;

    [SerializeField] private float additionalHp;
    [SerializeField] private float additionalHpRegen;

    void Awake()
    {
        character = GameObject.Find("CharacterImage").GetComponent<Character>();
    }


    public void InitializeRegenerationArmor(Consumable item)
    {
        transform.SetParent(character.transform);
        transform.position = character.transform.position;

        attackPowerRate = item.attackPowerValue;
        attackRangeRate = item.attackRangeValue;

        additionalHp = character.player.attackPower * attackPowerRate;
        additionalHpRegen = character.player.attackRange * attackRangeRate;
    }

    // 지금 게임 도중에 공격력 공격 범위를 동적으로 적용시키지 못함
    public void StartRegenerationArmor()
    {
        character.player.maxHp += additionalHp;
        character.Health.Heal(additionalHp);

        character.player.hpRegen += additionalHpRegen; 
    }

    public void EndRegenerationArmor()
    {
        character.player.maxHp -= additionalHp;
        character.player.hpRegen -= additionalHpRegen;
    }
}
