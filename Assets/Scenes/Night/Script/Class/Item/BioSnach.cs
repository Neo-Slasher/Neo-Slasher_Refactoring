using UnityEngine;
using static UnityEditor.Progress;

public class BioSnach : MonoBehaviour
{
    private Character character;

    private float healByHitAmount;

    private void Awake()
    {
        character = GameObject.Find("CharacterImage").GetComponent<Character>();
    }

    public void InitializeBioSnach(Consumable item)
    {
        transform.SetParent(character.transform);
        transform.position = character.transform.position;

        switch (item.rank)
        {
            case 0:
                healByHitAmount = 1;
                break;
            case 1:
                healByHitAmount = 2;
                break;
            case 2:
                healByHitAmount = 3;
                break;
            case 3:
                healByHitAmount = 5;
                break;
        }
    }

    public void StartBioSnach()
    {
        character.player.healByHit += healByHitAmount;
    }

    public void EndBioSnach()
    {
        character.player.healByHit -= healByHitAmount;
    }
}
