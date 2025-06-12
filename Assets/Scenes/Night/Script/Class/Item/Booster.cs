using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Booster : MonoBehaviour
{
    public Character character;

    private Image coolTimeImage;

    private float attackPowerRate;
    private float attackSpeedRate;
    private float attackRangeRate;

    Coroutine SpriteCoroutine;

    void Awake()
    {
        character = GameObject.Find("CharacterImage").GetComponent<Character>();
    }

    public void InitializeBooster(Consumable item, Image coolTimeImage)
    {
        transform.SetParent(character.transform);
        transform.position = character.transform.position;
        transform.localPosition = new Vector3(2.13f, 0, 0);

        attackPowerRate = item.attackPowerValue;
        attackSpeedRate = Mathf.Abs(item.attackSpeedValue);
        attackRangeRate = item.attackRangeValue;

        this.coolTimeImage = coolTimeImage;

        character.isBoosterOn = true;
        character.SetCharacterBasicSpeedError();

    }

    public void StartBooster()
    {
        StartCoroutine(BoosterCoroutine());
    }

    private IEnumerator BoosterCoroutine()
    {
        float coolTime;
        float duration;

        while (!NightManager.Instance.isStageEnd)
        {
            NightSFXManager.Instance.PlayAudioClip(AudioClipName.booster);

            float additionalSpeed = character.player.attackPower * attackPowerRate;

            character.Movement.SetMoveSpeed(character.player.moveSpeed + additionalSpeed);

            if (SpriteCoroutine == null)
                StartCoroutine(ActiveSprite());

            duration = character.player.attackRange * attackRangeRate;

            yield return new WaitForSeconds(duration);

            StopCoroutine(ActiveSprite());
            SpriteCoroutine = null;
            transform.GetComponent<SpriteRenderer>().enabled = false;

            character.Movement.SetMoveSpeed(character.player.moveSpeed - additionalSpeed);


            coolTime = 30 / (character.player.attackSpeed * attackSpeedRate);
            float timer = 0;

            coolTime = 1;
            while (timer < coolTime)
            {
                timer += Time.deltaTime;
                coolTimeImage.fillAmount = 1 - timer / coolTime;
                yield return null;
            }
        }
    }

    private IEnumerator ActiveSprite()
    {
        transform.GetComponent<SpriteRenderer>().enabled = true;

        while (!NightManager.Instance.isStageEnd)
        {
            if (character.GetComponent<SpriteRenderer>().flipX)
            {
                transform.localPosition = new Vector3(-2.13f, 0, 0);
                GetComponent<SpriteRenderer>().flipX = true;
            }
            else
            {
                transform.localPosition = new Vector3(2.13f, 0, 0);
                GetComponent<SpriteRenderer>().flipX = false;
            }

            yield return null;
        }
    }
}
