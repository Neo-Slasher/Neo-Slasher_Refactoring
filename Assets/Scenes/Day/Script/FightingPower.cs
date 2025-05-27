using TMPro;
using UnityEngine;

public class FightingPower : MonoBehaviour
{
    public int currentCP;

    public TMP_Text money;
    public TMP_Text currentcp;

    public TMP_Text stacks;
    public TMP_Text diffs;

    double equipment_move_speed = 0;
    double equipment_attack_power = 0;
    double equipment_attack_speed = 0;
    double equipment_attack_range = 0;

    public void Awake()
    {
        CalculatePower();
    }

    public void Start()
    {
        UpdateFightPower();
    }

    public void CalculatePower()
    {
        Player player = GameManager.instance.player;

        double maxHp = player.maxHp;
        double moveSpeed = player.moveSpeed;
        double attackPower = player.attackPower;
        double attackSpeed = player.attackSpeed;
        double attackRange = player.attackRange;

        currentCP = (int)((maxHp * 1.5) + (moveSpeed * 3) + (attackPower * attackSpeed * attackRange * 0.02));
        Debug.Log("currentCP: " + currentCP);
    }

    public void RemoveEquipment(int part)
    {
        Player player = GameManager.instance.player;

        if (player.equipment[part].name != "")
        {
            Equipment equipment = player.equipment[part];
            player.equipmentAttackPower -= equipment.attackPower;
            player.equipmentAttackSpeed -= equipment.attackSpeed;
            player.equipmentAttackRange -= equipment.attackRange;
            player.equipmentMoveSpeed -= equipment.moveSpeed;
            player.equipment[part] = null;
        }
    }

    public void EquipEquipment(Equipment equipment)
    {
        Player player = GameManager.instance.player;

        player.equipmentAttackPower += equipment.attackPower;
        player.equipmentAttackSpeed += equipment.attackSpeed;
        player.equipmentAttackRange += equipment.attackRange;
        player.equipmentMoveSpeed += equipment.moveSpeed;

        Debug.Log($"장착하려는 장비명: {equipment.name}, {equipment.part}");
        player.equipment[equipment.part] = equipment;
    }

    public void UpdateFightPower()
    {
        CalculatePower();

        Player player = GameManager.instance.player;

        money.text = player.money.ToString() + "a / " + DataManager.instance.difficultyList.difficulty[player.difficulty].goalMoney + "a";
        
        currentcp.text = currentCP.ToString();

        stacks.text = player.maxHp.ToString() + "\n";
        stacks.text += player.moveSpeed.ToString() + "\n";
        stacks.text += player.attackSpeed.ToString() + "\n";
        stacks.text += player.attackPower.ToString() + "\n";
        stacks.text += player.attackRange.ToString() + "\n\n"; // 의도된 두 번 개행
        stacks.text += player.startMoney.ToString() + "\n";
        stacks.text += player.earnMoney.ToString() + "\n";

        PrintFightPower();
    }
    public void PrintFightPower()
    {
        Player player = GameManager.instance.player;

        foreach (var equipment in player.equipment)
        {
            equipment_move_speed += equipment.moveSpeed;
            equipment_attack_power += equipment.attackPower;
            equipment_attack_speed += equipment.attackSpeed;
            equipment_attack_range += equipment.attackRange;
        }

        if (player.equipmentMoveSpeed >= 0)
            diffs.text = $"\n+{equipment_move_speed}\n";
        else if (player.equipmentMoveSpeed < 0)
            diffs.text = $"\n{equipment_move_speed}\n";

        if (player.equipmentAttackPower >= 0)
            diffs.text += $"+{equipment_attack_power}\n";
        else if (player.equipmentAttackPower < 0)
            diffs.text += $"{equipment_attack_power}\n";

        if (player.equipmentAttackSpeed >= 0)
            diffs.text += $"+{equipment_attack_speed}\n";
        else if(player.equipmentAttackSpeed < 0)
            diffs.text += $"{equipment_attack_speed}\n";

        if (player.equipmentAttackRange >= 0)
            diffs.text += $"+{equipment_attack_range}\n";
        else if(player.equipmentAttackRange < 0)
            diffs.text += $"{equipment_attack_range}\n";

    }

}
