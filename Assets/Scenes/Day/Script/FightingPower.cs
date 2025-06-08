using TMPro;
using UnityEngine;

// 2025.06.03 Refactoring Final Version
public class FightingPower : MonoBehaviour
{
    [SerializeField] private TMP_Text money;
    [SerializeField] private TMP_Text currentCpText;
    [SerializeField] private TMP_Text stats;
    [SerializeField] private TMP_Text statDiff;

    // CP: Character Point(������)
    public int currentCP;


    public void Start()
    {
        UpdateFightPower();
    }

    public void UpdateFightPower()
    {
        CalculatePower();
        UpdateUI();
    }

    private void CalculatePower()
    {
        Player player = GameManager.Instance.player;

        double maxHp = player.maxHp;
        double moveSpeed = player.moveSpeed;
        double attackPower = player.attackPower;
        double attackSpeed = player.attackSpeed;
        double attackRange = player.attackRange;

        currentCP = (int)((maxHp * 1.5) + (moveSpeed * 3) + (attackPower * attackSpeed * attackRange * 0.02));
        Logger.Log("currentCP: " + currentCP);
    }

    private void UpdateUI()
    {
        Player player = GameManager.Instance.player;

        money.text = player.money.ToString() + "a / " + DataManager.Instance.difficultyList.difficulty[player.difficulty].goalMoney + "a";
        currentCpText.text = currentCP.ToString();

        UpdateStatUI();
        UpdateStatDiffUI();
    }

    private void UpdateStatUI()
    {
        Player player = GameManager.Instance.player;


        var sb = new System.Text.StringBuilder();
        sb.AppendLine(player.maxHp.ToString());
        sb.AppendLine(player.moveSpeed.ToString());
        sb.AppendLine(player.attackSpeed.ToString());
        sb.AppendLine(player.attackPower.ToString());
        sb.AppendLine(player.attackRange.ToString());
        sb.AppendLine(); // �ǵ��� ����
        sb.AppendLine(player.startMoney.ToString());
        sb.AppendLine(player.earnMoney.ToString());
        stats.text = sb.ToString();
    }

    private void UpdateStatDiffUI()
    {
        Player player = GameManager.Instance.player;
        var sb = new System.Text.StringBuilder();

        double equipmentMoveSpeed = 0;
        double equipmentAttackPower = 0;
        double equipmentAttackSpeed = 0;
        double equipmentAttackRange = 0;

        foreach (var equipment in player.equipment)
        {
            if (equipment == null) continue;
            equipmentMoveSpeed += equipment.moveSpeed;
            equipmentAttackPower += equipment.attackPower;
            equipmentAttackSpeed += equipment.attackSpeed;
            equipmentAttackRange += equipment.attackRange;
        }

        void AppendStatLine(double value)
        {
            sb.AppendLine(value > 0 ? $"+{value}" : $"{value}");
        }

        sb.AppendLine(" "); // �ǵ��� ����
        AppendStatLine(equipmentMoveSpeed);
        AppendStatLine(equipmentAttackPower);
        AppendStatLine(equipmentAttackSpeed);
        AppendStatLine(equipmentAttackRange);
        sb.AppendLine(" "); // �ǵ��� ����
        sb.AppendLine(" "); // �ǵ��� ����
        sb.AppendLine(" "); // �ǵ��� ����

        statDiff.text = sb.ToString();
    }

    public void RemoveEquipment(int part)
    {
        Player player = GameManager.Instance.player;

        // index == 0�̸� �� ����
        if (player.equipment[part].index == 0)
        {
            Logger.Log("FightingPower: �����Ϸ��� �ϴ� ��� ��Ʈ�� �������� �ʽ��ϴ�.");
            return;
        }

        player.equipment[part] = new Equipment { index = 0 };
    }

    public void EquipEquipment(Equipment equipment)
    {
        Player player = GameManager.Instance.player;

        if (player.equipment[equipment.part].index != 0)
        {
            RemoveEquipment(equipment.part);
        }

        player.equipment[equipment.part] = equipment;

        Logger.Log($"��� ���� �Ϸ�: {equipment.name}, {equipment.part}");
    }
}
