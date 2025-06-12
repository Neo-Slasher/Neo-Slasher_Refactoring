using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Barrior : MonoBehaviour
{
    private Character character;

    [SerializeField]
    Transform barriorImageTransform;

    [SerializeField]
    float createSpeed;
    float startSize = 0;
    float nowSize = 0;
    float maxSize = 1;

    public Image coolTimeImage;

    private float attackPowerRate;
    private float attackSpeedRate;

    private void Awake()
    {
        character = GameObject.Find("CharacterImage").GetComponent<Character>();
    }


    public void InitializeBarrior(Consumable item, Image coolTimeImage)
    {
        transform.SetParent(character.transform);
        transform.position = character.transform.position;
        attackPowerRate = item.attackPowerValue;
        attackSpeedRate = item.attackSpeedValue;

        this.coolTimeImage = coolTimeImage;
    }

    public void StartBarrior()
    {
        StartCoroutine(BarrierCoroutine());
    }

    public void CreateBarrier()
    {
        StartCoroutine(CreateBarriorCoroutine());
    }
    


    IEnumerator CreateBarriorCoroutine()
    {
        Vector3 nowScale = Vector3.zero;
        barriorImageTransform.localScale = nowScale;
        nowSize = startSize;
        while (nowSize <= maxSize)
        {
            nowSize += Time.deltaTime * createSpeed;
            nowScale.x = nowSize;
            nowScale.y = nowSize;
            barriorImageTransform.localScale = nowScale;

            yield return null;
        }
    }

    IEnumerator BarrierCoroutine()
    {
        while (!NightManager.Instance.isStageEnd)
        {
            yield return new WaitUntil(() => character.player.shieldPoint == 0);

            float shieldPoint = character.player.attackPower * attackPowerRate;
            character.Health.AddShield(shieldPoint);

            NightSFXManager.Instance.PlayAudioClip(AudioClipName.barrior);

            transform.GetComponent<SpriteRenderer>().enabled = true;
            CreateBarrier();

            yield return new WaitForSeconds(2f);
            transform.GetComponent<SpriteRenderer>().enabled = false;

            float coolTime = 90 / (character.player.attackSpeed * Mathf.Abs(attackSpeedRate));
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
