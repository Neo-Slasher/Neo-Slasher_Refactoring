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

        // XXX: 현재 게임에서 데미지 경감 요소가 안티 페넷이 유일하므로 증감량을 직접 계산
        damageReduceAmount = character.player.attackPower * damageReduceRate; 
    }



    public void StartAntiPhenet()
    {
        character.player.damageReductionRate += damageReduceAmount;
    }

    // Warning: float 특성 상 정확하게 증감이 떨어지지 않음
    public void EndAntiPhenet()
    {
        character.player.damageReductionRate -= damageReduceAmount;
    }
}
