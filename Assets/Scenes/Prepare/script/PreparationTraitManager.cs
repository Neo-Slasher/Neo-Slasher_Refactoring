using UnityEngine;


// 스텟 데이터는 player에 저장 
// trait manager는 preparation scene에서 수치 값 제공 및 player에 반영하는 역할로 구상
public class PreparationTraitManager : MonoBehaviour
{
    public double hp_by_trait;
    public double move_speed_by_trait;
    public double attack_power_by_trait;
    public double attack_speed_by_trait;
    public double attack_range_by_trait;


    private void Start()
    {
        hp_by_trait = 0;
        move_speed_by_trait = 0;
        attack_power_by_trait = 0;
        attack_speed_by_trait = 0;
        attack_range_by_trait = 0;
    }


    public enum EffectType {
        none, hp, moveSpeed, attackPower, attackSpeed,
        attackRange, startMoney, earnMoney, shopSlot,
        itemSlot, shopMinRank, shopMaxRank, dropRank,
        dropRate, healByHit, hpRegen, dealOnMax, dealOnHp,
        active
    }

    public void activeTrait(int traitNumber)
    {
        if (GameManager.Instance.player.traitPoint <= 0)
        {
            Debug.Log("플레이어의 특성 포인트가 없습니다.");
            return;
        }
        
        Trait trait = DataManager.Instance.traitList.trait[traitNumber - 1];
        GameManager.Instance.player.traitPoint--;
        GameManager.Instance.player.trait[traitNumber] = true;
        TraitParseAndApply(trait);
    }

    public void unactiveTrait(int traitNumber)
    {
        Trait trait =  DataManager.Instance.traitList.trait[traitNumber - 1];
        GameManager.Instance.player.trait[traitNumber] = false;
        GameManager.Instance.player.traitPoint++;
        TraitParseAndDisapply(trait);
    }

    public void TraitParseAndApply(Trait trait) {
        if (trait.effectType1 != 0)
            applyTrait((EffectType)trait.effectType1, trait.effectValue1, trait.effectMulti1);
        if (trait.effectType2 != 0)
            applyTrait((EffectType)trait.effectType2, trait.effectValue2, trait.effectMulti2);
        if (trait.effectType3 != 0)
            applyTrait((EffectType)trait.effectType3, trait.effectValue3, trait.effectMulti3);
        if (trait.effectType4 != 0)
            applyTrait((EffectType)trait.effectType4, trait.effectValue4, trait.effectMulti4);
    }

    public void TraitParseAndDisapply(Trait trait)
    {
        if (trait.effectType1 != 0)
            disapplyTrait((EffectType)trait.effectType1, trait.effectValue1, trait.effectMulti1);
        if (trait.effectType2 != 0)
            disapplyTrait((EffectType)trait.effectType2, trait.effectValue2, trait.effectMulti2);
        if (trait.effectType3 != 0)
            disapplyTrait((EffectType)trait.effectType3, trait.effectValue3, trait.effectMulti3);
        if (trait.effectType4 != 0)
            disapplyTrait((EffectType)trait.effectType4, trait.effectValue4, trait.effectMulti4);
    }

    private void applyTrait(EffectType type, double value, bool multi) {
        Player player = GameManager.Instance.player;
        if (type == EffectType.hp) {
            player.maxHp += value;
            hp_by_trait += value;
        }
        else if (type == EffectType.moveSpeed) {
            player.moveSpeed += value;
            move_speed_by_trait += value;
        }
        else if (type == EffectType.attackPower) {
            player.attackPower += value;
            attack_power_by_trait += value;
        }
        else if (type == EffectType.attackSpeed) {
            player.attackSpeed += value;
            attack_speed_by_trait += value;
        }
        else if (type == EffectType.attackRange) {
            player.attackRange += value;
            attack_range_by_trait += value;
        }
        else if (type == EffectType.startMoney) {
            player.startMoney += (int)value;
        }
        else if (type == EffectType.earnMoney) {
            player.earnMoney += (float)value;
        }
        else if (type == EffectType.shopSlot) {
            player.shopSlot += (int)value;
        }
        else if (type == EffectType.itemSlot) {
            player.itemSlot += (int)value;
        }
        else if (type == EffectType.shopMinRank) {
            player.shopMinRank += (int)value;
        }
        else if (type == EffectType.shopMaxRank) {
            player.shopMaxRank += (int)value;
        }
        else if (type == EffectType.dropRank) {
            player.dropRank += (int)value;
        }
        else if (type == EffectType.dropRate) {
            player.dropRate += value;
        }
        else if (type == EffectType.healByHit) {
            player.healByHit += value;
        }
        else if (type == EffectType.hpRegen) {
            player  .hpRegen += value;
        }
        else if (type == EffectType.dealOnMax) {
            player.dealOnMaxHp += value;
        }
        else if (type == EffectType.dealOnHp) {
            player.dealOnCurHp += value;
        }
    }


    private void disapplyTrait(EffectType type, double value, bool multi) {
        applyTrait(type, -value, multi);  
    }

}
