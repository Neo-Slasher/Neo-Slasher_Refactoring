using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FirstAde : MonoBehaviour
{
    private Character character;

    [SerializeField] private float coolTime;
    [SerializeField] private float healRate;

    public Image coolTimeImage;

    private void Awake()
    {
        character = GameObject.Find("CharacterImage").GetComponent<Character>();
    }


    public void InitializeFirstAde(Consumable item, Image coolTimeImage)
    {
        SetFirstAdeData(item);
        transform.SetParent(character.transform);
        transform.localPosition = character.transform.position;

        this.coolTimeImage = coolTimeImage;
    }

    private void SetFirstAdeData(Consumable item)
    {
        switch (item.rank)
        {
            case 0:
                coolTime = 30;
                break;
            case 1:
                coolTime = 25;
                break;
            case 2:
                coolTime = 20;
                break;
            case 3:
                coolTime = 15;
                break;
        }

        healRate = item.attackPowerValue;
    }

    public void StartFirstAde()
    {
        StartCoroutine(FirstAdeCoroutine());

    }

    private IEnumerator FirstAdeCoroutine()
    {
        float timer;
        coolTimeImage.fillAmount = 0;
        transform.GetComponent<SpriteRenderer>().enabled = false;

        while (!NightManager.Instance.isStageEnd)
        {

            yield return new WaitUntil(() => character.player.curHp < character.player.maxHp * 0.4);

            float healHp = character.player.attackPower * healRate;
            NightSFXManager.Instance.PlayAudioClip(AudioClipName.firstAde);

            // 부드럽게 채워줄 방법도 생각해보기
            character.Health.Heal(healHp / 3);
            transform.GetComponent<SpriteRenderer>().enabled = true;
            yield return new WaitForSeconds(0.9f);
            transform.GetComponent<SpriteRenderer>().enabled = false;
            yield return new WaitForSeconds(0.1f);

            character.Health.Heal(healHp / 3);
            transform.GetComponent<SpriteRenderer>().enabled = true;
            yield return new WaitForSeconds(0.9f);
            transform.GetComponent<SpriteRenderer>().enabled = false;
            yield return new WaitForSeconds(0.1f);

            character.Health.Heal(healHp / 3);
            transform.GetComponent<SpriteRenderer>().enabled = true;
            yield return new WaitForSeconds(0.9f);
            transform.GetComponent<SpriteRenderer>().enabled = false;
            yield return new WaitForSeconds(0.1f);


            timer = 0;
            while (timer < coolTime)
            {
                timer += Time.deltaTime;
                coolTimeImage.fillAmount = 1 - timer / coolTime;
                yield return null;
            }
        }
    }


}
