using System.Collections;
using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ChargingReaper : MonoBehaviour
{
    [SerializeField] private Character character;
    [SerializeField] private GameObject reaperImage;
    [SerializeField] private GameObject reaperAfterImage;

    public bool isAttack = false;

    public int chargingGauge;

    public float reaperAttackDamege;

    [SerializeField] private Image coolTimeImage;

    const int MAX_GAUGE = 100;

    private void Awake()
    {
        character = GameObject.Find("CharacterImage").GetComponent<Character>();
    }
    private void Start()
    {

        NightManager.Instance.OnMonsterDie += ChargingGauge;
        NightManager.Instance.OnMonsterDie += SetCoolTime;
    }

    private void ChargingGauge()
    {
        chargingGauge = Mathf.Min(chargingGauge + 5, MAX_GAUGE);
    }

    public void SetCoolTime()
    {
        coolTimeImage.fillAmount = 1 - ((float)chargingGauge / MAX_GAUGE);
    }

    public void InitializeChargingReaper(Consumable item, Image coolTimeImage)
    {

        SetChargingReaperData(item);
        transform.SetParent(character.transform);
        chargingGauge = 0;
        this.coolTimeImage = coolTimeImage;
    }

    private void SetChargingReaperData(Consumable item)
    {
        // Tolelom: scale 크기는 Data Table을 보고 시각적으로 가장 비슷한 값을 임의로 사용
        switch (item.rank)
        {
            case 0:
                reaperAttackDamege = 13;
                reaperImage.transform.localScale = new Vector3(0.7f, 0.7f, 1);
                break;
            case 1:
                reaperAttackDamege = 18;
                reaperImage.transform.localScale = new Vector3(0.9f, 0.9f, 1);
                break;
            case 2:
                reaperAttackDamege = 27;
                reaperImage.transform.localScale = new Vector3(1.2f, 1.2f, 1);
                break;
            case 3:
                reaperAttackDamege = 42;
                reaperImage.transform.localScale = new Vector3(1.6f, 1.6f, 1);
                break;
        }
    }

    public void ActiveChargingReaper()
    {
        StartCoroutine(RunChargingReaper());
    }

    // TODO: 변수명 수정하기
    private IEnumerator RunChargingReaper()
    {
        Transform reaperImageTransform = transform.Find("ChargingReaperImage");
        if (reaperImageTransform == null) yield break;

        float reaperCircleR = reaperImage.transform.localScale.x * 3; //반지름
        float reaperDeg = 0; //각도
        float reaperSpeed = 600; //원운동 속도

        while (!NightManager.Instance.isStageEnd)
        {
            yield return new WaitUntil(() => IsChargingGaugeFull());

            Logger.Log("차징 리퍼 발동!");

            reaperImage.SetActive(true);
            NightSFXManager.Instance.PlayAudioClip(AudioClipName.chargingReaper);
            StartCoroutine(ReaperAfterImageCoroutine());

            // 이펙트 지속 시간
            float effectDuration = 0.6f; // 정확히 한 바퀴
            float timer = effectDuration;

            while (timer > 0)
            {
                timer -= Time.deltaTime;
                chargingGauge = Mathf.FloorToInt((timer / effectDuration) * 100); // 게이지 감소
                SetCoolTime();

                reaperDeg += Time.deltaTime * reaperSpeed;
                reaperDeg %= 360;
                

                var rad = Mathf.Deg2Rad * (reaperDeg);
                Vector3 offset = new Vector3(
                    Mathf.Sin(rad) * reaperCircleR,
                    Mathf.Cos(rad) * reaperCircleR,
                    0
                );
                reaperImageTransform.transform.position = character.transform.position + offset;
                reaperImageTransform.transform.rotation = Quaternion.Euler(0, 0, -reaperDeg);
                
                yield return null;
            }

            reaperImage.SetActive(false);
            chargingGauge = 0;
        }
    }

    public bool IsChargingGaugeFull()
    {
        if (chargingGauge >= MAX_GAUGE)
            return true;
        return false;
    }

    IEnumerator ReaperAfterImageCoroutine()
    {
        reaperAfterImage.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        reaperAfterImage.SetActive(true);
    }
}
