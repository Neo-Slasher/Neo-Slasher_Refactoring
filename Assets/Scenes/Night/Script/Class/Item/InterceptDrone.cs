using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    
    public void StartInterceptDrone()
    {
        StartCoroutine(DetectProjectileCoroutine());
        StartCoroutine(DroneRotate());
    }



    void SetInterceptDroneData(Consumable item)
    {
        attackRangeRate = item.attackRangeValue;
        attackSpeedRate = item.attackSpeedValue;

        float characterAttackSpeed = character.player.attackSpeed;
        float characterAttackRange = character.player.attackRange;


        timeCount = 40 / (characterAttackSpeed * item.attackSpeedValue);
        detectRadius = characterAttackRange * 0.15f * item.attackRangeValue;

        detectRadius += 1.95f;
    }

    IEnumerator DroneRotate()
    {
        while (!NightManager.Instance.isStageEnd)
        {
            interceptDroneImage.transform.RotateAround(character.transform.position, Vector3.back, droneAngle);
            interceptDroneImage.transform.rotation = Quaternion.AngleAxis(0, Vector3.forward);

            yield return null;
        }
    }
    //void SetCentryBallWatchEnemy(Collider2D getCol)
    //{
    //    Transform enemyTransform = getCol.transform;
    //    float angle;

    //    watchDir = enemyTransform.position - centryBallImage.transform.position;

    //    angle = Mathf.Atan2(watchDir.y, watchDir.x) * Mathf.Rad2Deg;
    //    centryBallImage.transform.rotation = Quaternion.AngleAxis(angle - 180, Vector3.forward);
    //}

    public void SetInterceptDrone(float getAttackSpeed, float getAttackRange)
    {
        getCharacterAttackRange = getAttackRange;
        getCharacterAttackSpeed = getAttackSpeed;
    }


    IEnumerator DetectProjectileCoroutine()
    {
        LayerMask projLayer = LayerMask.NameToLayer("Projectile");
        int layerMask = (1 << projLayer);

        while (!NightManager.Instance.isStageEnd)
        {
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

            if (coolTimeImage.fillAmount == 0)
                coolTimeImage.fillAmount = 1;
            StartCoroutine(SetCoolTime());
            Debug.Log("InterCept CoolTime: " + timeCount);
            yield return new WaitForSeconds(timeCount/40);
        }
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

    public IEnumerator SetCoolTime()
    {
        coolTimeImage.gameObject.SetActive(true);
        float nowTime = 0;
        while (coolTimeImage.fillAmount > 0)
        {
            nowTime += Time.deltaTime;
            coolTimeImage.fillAmount = 1 - nowTime/timeCount;
            yield return null;
        }
    }
}
