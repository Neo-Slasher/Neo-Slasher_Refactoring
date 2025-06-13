using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MoveBack : MonoBehaviour
{
    private Character character;

    private float attackSpeedRate;
    private float attackRangeRate;

    public Image coolTimeImage;

    void Awake()
    {
        character = GameObject.Find("CharacterImage").GetComponent<Character>();
    }

    public void InitializeMoveBack(Consumable item, Image coolTimeImage)
    {
        transform.SetParent(character.transform);
        transform.position = character.transform.position;

        attackSpeedRate = Mathf.Abs(item.attackSpeedValue);
        attackRangeRate = item.attackRangeValue;

        this.coolTimeImage = coolTimeImage;
    }

    public void StartMoveBack()
    {
        StartCoroutine(MoveBackCoroutine());
    }


    private IEnumerator MoveBackCoroutine()
    {
        float coolTime; 

        while (!NightManager.Instance.isStageEnd)
        {
            NightSFXManager.Instance.PlayAudioClip(AudioClipName.moveBack);
            character.Attack.isMoveBackOn = true;
            transform.GetComponent<SpriteRenderer>().enabled = true;

            yield return new WaitUntil(() => !character.Attack.isMoveBackOn);

            transform.GetComponent<SpriteRenderer>().enabled = false;

            coolTime = 20 / (character.player.attackSpeed * attackSpeedRate);
            float timer = 0;

            while (timer < coolTime)
            {
                timer += Time.deltaTime;
                coolTimeImage.fillAmount = 1 - timer / coolTime;
                yield return null;
            }
        }
    }
}
