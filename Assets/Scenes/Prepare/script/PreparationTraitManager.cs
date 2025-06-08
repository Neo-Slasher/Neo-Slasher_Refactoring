using UnityEngine;

// 2025.06.03 Refactoring Final Version
public class PreparationTraitManager : MonoBehaviour
{
    public float HpByTrait { get; private set; }
    public float MoveSpeedByTrait { get; private set; }
    public float AttackPowerByTrait { get; private set; }
    public float AttackSpeedByTrait { get; private set; }
    public float AttackRangeByTrait { get; private set; }
    public enum EffectType
    {
        None,
        Hp,
        MoveSpeed,
        AttackPower,
        AttackSpeed,
        AttackRange,
        StartMoney,
        EarnMoney,
        ShopSlot,
        ItemSlot,
        ShopMinRank,
        ShopMaxRank,
        DropRank,
        DropRate,
        HealByHit,
        HpRegen,
        DealOnMax,
        DealOnHp,
        Active
    }

    void Start()
    {
        HpByTrait = 0;
        MoveSpeedByTrait = 0;
        AttackPowerByTrait = 0;
        AttackSpeedByTrait = 0;
        AttackRangeByTrait = 0;
    }

    public void ActivateTrait(int traitNumber)
    {
        if (GameManager.Instance.player.traitPoint <= 0)
        {
            Logger.Log("플레이어의 특성 포인트가 없습니다.");
            return;
        }

        Trait trait = DataManager.Instance.traitList.trait[traitNumber - 1];
        GameManager.Instance.player.traitPoint--;
        GameManager.Instance.player.trait[traitNumber] = true;
        TraitParseAndApply(trait);
    }

    public void DeactivateTrait(int traitNumber)
    {
        Trait trait = DataManager.Instance.traitList.trait[traitNumber - 1];
        GameManager.Instance.player.trait[traitNumber] = false;
        GameManager.Instance.player.traitPoint++;
        TraitParseAndDisapply(trait);
    }

    public void TraitParseAndApply(Trait trait)
    {
        if (trait.effectType1 != 0)
            ApplyTrait((EffectType)trait.effectType1, trait.effectValue1, trait.effectMulti1);
        if (trait.effectType2 != 0)
            ApplyTrait((EffectType)trait.effectType2, trait.effectValue2, trait.effectMulti2);
        if (trait.effectType3 != 0)
            ApplyTrait((EffectType)trait.effectType3, trait.effectValue3, trait.effectMulti3);
        if (trait.effectType4 != 0)
            ApplyTrait((EffectType)trait.effectType4, trait.effectValue4, trait.effectMulti4);
    }

    public void TraitParseAndDisapply(Trait trait)
    {
        if (trait.effectType1 != 0)
            DisapplyTrait((EffectType)trait.effectType1, trait.effectValue1, trait.effectMulti1);
        if (trait.effectType2 != 0)
            DisapplyTrait((EffectType)trait.effectType2, trait.effectValue2, trait.effectMulti2);
        if (trait.effectType3 != 0)
            DisapplyTrait((EffectType)trait.effectType3, trait.effectValue3, trait.effectMulti3);
        if (trait.effectType4 != 0)
            DisapplyTrait((EffectType)trait.effectType4, trait.effectValue4, trait.effectMulti4);
    }

    private void ApplyTrait(EffectType type, float value, bool multi)
    {
        Player player = GameManager.Instance.player;
        switch (type)
        {
            case EffectType.Hp:
                player.maxHp += value;
                HpByTrait += value;
                break;
            case EffectType.MoveSpeed:
                player.moveSpeed += value;
                MoveSpeedByTrait += value;
                break;
            case EffectType.AttackPower:
                player.attackPower += value;
                AttackPowerByTrait += value;
                break;
            case EffectType.AttackSpeed:
                player.attackSpeed += value;
                AttackSpeedByTrait += value;
                break;
            case EffectType.AttackRange:
                player.attackRange += value;
                AttackRangeByTrait += value;
                break;
            case EffectType.StartMoney:
                player.startMoney += (int)value;
                break;
            case EffectType.EarnMoney:
                player.earnMoney += value;
                break;
            case EffectType.ShopSlot:
                player.shopSlot += (int)value;
                break;
            case EffectType.ItemSlot:
                player.itemSlot += (int)value;
                break;
            case EffectType.ShopMinRank:
                player.shopMinRank += (int)value;
                break;
            case EffectType.ShopMaxRank:
                player.shopMaxRank += (int)value;
                break;
            case EffectType.DropRank:
                player.dropRank += (int)value;
                break;
            case EffectType.DropRate:
                player.dropRate += value;
                break;
            case EffectType.HealByHit:
                player.healByHit += value;
                break;
            case EffectType.HpRegen:
                player.hpRegen += value;
                break;
            case EffectType.DealOnMax:
                player.dealOnMaxHp += value;
                break;
            case EffectType.DealOnHp:
                player.dealOnCurHp += value;
                break;
            // case EffectType.Active: // 필요시 구현
            default:
                break;
        }

    }

    private void DisapplyTrait(EffectType type, float value, bool multi)
    {
        ApplyTrait(type, -value, multi);
    }
}
