using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class InterceptDrone : MonoBehaviour
{
    public Character character;


    [SerializeField]
    GameObject droneArea;
    [SerializeField]
    GameObject droneAreaImage;
    [SerializeField]
    GameObject interceptDroneImage;
    [SerializeField]
    GameObject interceptDroneSearchingImage;
    [SerializeField]
    SpriteRenderer droneRenderer;

    [SerializeField]
    float droneAngle;
    [SerializeField]
    float detectRadius;
    float timeCount;


    float getCharacterAttackSpeed;
    float getCharacterAttackRange;

    Projectile getProjScript;

    private float attackSpeedRate;
    private float attackRangeRate;

    public Image coolTimeImage;

    private void Awake()
    {
        character = GameObject.Find("CharacterImage").GetComponent<Character>();
    }

    public void InitializeInterceptDrone(Consumable item, Image coolTimeImage)
    {
        transform.SetParent(character.transform);
        transform.position = character.transform.position;

        SetInterceptDroneData(item);

        this.coolTimeImage = coolTimeImage;

    }
    private void SetInterceptDroneData(Consumable item)
    {
        attackRangeRate = item.attackRangeValue;
        attackSpeedRate = Mathf.Abs(item.attackSpeedValue);
    }

    public void StartInterceptDrone()
    {
        StartCoroutine(DetectProjectileCoroutine());
        StartCoroutine(DroneRotate());
    }

    private IEnumerator DroneRotate()
    {
        float rotateSpeed = 5f;
        float angle = 0f; // 현재 각도
        while (!NightManager.Instance.isStageEnd)
        {
            if (Time.timeScale != 0)
            {
                angle += rotateSpeed * Time.deltaTime;
                // 중심점에서 detectRadius만큼 떨어진 위치 계산
                Vector3 offset = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0) * detectRadius;
                transform.position = character.transform.position + offset;

                //interceptDroneImage.transform.RotateAround(character.transform.position, Vector3.back, droneAngle);
                //interceptDroneImage.transform.rotation = Quaternion.AngleAxis(0, Vector3.forward);
            }
            yield return null;
        }
    }

    IEnumerator DetectProjectileCoroutine()
    {
        LayerMask projLayer = LayerMask.NameToLayer("Projectile");
        int layerMask = (1 << projLayer);

        while (!NightManager.Instance.isStageEnd)
        {
            detectRadius = character.player.attackRange * 0.4f * attackRangeRate;
            Collider2D[] colArr = Physics2D.OverlapCircleAll(character.transform.position, detectRadius, layerMask);

            if (colArr.Length > 0)
            {
                NightSFXManager.Instance.PlayAudioClip(AudioClipName.interceptDrone);
                for (int i = 0; i < colArr.Length; i++)
                {
                    StartCoroutine(SearchIngProjEffectCoroutine());
                    InterceptProj(colArr[i]);
                }
            }


            float coolTime = 40 / (character.player.attackSpeed * attackSpeedRate);
            float timer = 0;

            while (timer < coolTime)
            {
                timer += Time.deltaTime;
                coolTimeImage.fillAmount = 1 - timer / coolTime;
                yield return null;
            }
        }
        yield return null;
    }

    IEnumerator SearchIngProjEffectCoroutine()
    {
        float nowAngle = 0;
        float spinSpeed = 720;
        droneAreaImage.SetActive(true);

        while (nowAngle <= 360)
        {
            nowAngle += Time.deltaTime * spinSpeed;
            interceptDroneSearchingImage.transform.rotation = Quaternion.Euler(0, 0, nowAngle * (-1));

            yield return null;
        }

        droneAreaImage.SetActive(false);
    }

    void InterceptProj(Collider2D getCol)
    {
        getProjScript = getCol.GetComponent<Projectile>();
        getProjScript.SetPosition();
    }

    private void OnDrawGizmos()
    {
        // 적 탐지 반경
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(character.transform.position, detectRadius);
    }
}
