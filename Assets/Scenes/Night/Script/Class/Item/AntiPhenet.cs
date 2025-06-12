using UnityEngine;
using UnityEngine.TextCore.Text;
using static UnityEditor.Progress;

public class AntiPhenet : MonoBehaviour
{
    private Character character;

    float damageReduceRate;
    float damageReduceAmount;

    private void Awake()
    {
        character = GameObject.Find("CharacterImage").GetComponent<Character>();
    }

    public void InitializeAntiPhenet(Consumable item)
    {
        transform.SetParent(character.transform);
        transform.position = character.transform.position;

        damageReduceRate = item.attackPowerValue;

        // XXX: ���� ���ӿ��� ������ �氨 ��Ұ� ��Ƽ ����� �����ϹǷ� �������� ���� ���
        damageReduceAmount = character.player.attackPower * damageReduceRate; 
    }



    public void StartAntiPhenet()
    {
        character.player.damageReductionRate += damageReduceAmount;
    }

    // Warning: float Ư�� �� ��Ȯ�ϰ� ������ �������� ����
    public void EndAntiPhenet()
    {
        character.player.damageReductionRate -= damageReduceAmount;
    }
}
