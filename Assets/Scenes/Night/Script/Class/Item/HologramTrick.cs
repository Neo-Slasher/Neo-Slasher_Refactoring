using System.Collections;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class HologramTrick : MonoBehaviour
{
    private Character character;
    [SerializeField] private GameObject hologramPrefab;

    [SerializeField] private GameObject[] holograms = new GameObject[2];

    float attackSpeedRate;
    float attackRangeRate;

    public Image coolTimeImage;

    private void Awake()
    {
        character = GameObject.Find("CharacterImage").GetComponent<Character>();
    }

    public void InitializeHologramTrick(Consumable item, Image coolTimeImage)
    {
        transform.SetParent(character.transform);
        transform.position = character.transform.position;

        attackSpeedRate = Mathf.Abs(item.attackSpeedValue);
        attackRangeRate = item.attackRangeValue;

        this.coolTimeImage = coolTimeImage;
        InitializeHologram();

    }

    private void InitializeHologram()
    {
        for (int i = 0; i < 2; i++)
        {
            Vector3 spawnPosition = character.transform.position;
            GameObject hologramObj = Instantiate(hologramPrefab);
            holograms[i] = hologramObj;


            hologramObj.transform.SetParent(transform);
            hologramObj.transform.position = transform.position;

            spawnPosition.x += (i == 0) ? -1 : 1;
            hologramObj.transform.position = spawnPosition;
        }

    }

    public void StartHologramTrick()
    {
        StartCoroutine(HologramTrickCoroutine());
    }

    private IEnumerator HologramTrickCoroutine()
    {

        while (!NightManager.Instance.isStageEnd)
        {
            NightSFXManager.Instance.PlayAudioClip(AudioClipName.hologramTrick);
            character.isHologramTrickOn = true;
            ActiveHologram();

            float duration = character.player.attackRange * attackRangeRate;
            yield return new WaitForSeconds(duration);

            character.isHologramTrickOn = false;
            DeactiveHologram();

            float coolTime = 120 / character.player.attackSpeed * attackSpeedRate;
            float timer = 0;

            while (timer < coolTime)
            {
                timer += Time.deltaTime;
                coolTimeImage.fillAmount = 1 - timer / coolTime;
                yield return null;
            }
        }
    }


    private void ActiveHologram()
    {
        foreach (var hologram in holograms)
            hologram.SetActive(true);

    }

    private void DeactiveHologram()
    {
        foreach (var hologram in holograms)
            hologram.SetActive(false);
    }
}
