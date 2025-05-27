using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterceptDrone : MonoBehaviour
{
    public NightManager nightManager;
    public Character character;
    public NightSFXManager nightSFXManager;
    LayerMask projLayer;

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

    int itemRank;

    float getCharacterAttackSpeed;
    float getCharacterAttackRange;

    Projectile getProjScript;

    //ÄðÅ¸ÀÓ
    public Image coolTimeImage;

    private void Start()
    {
        DetectProjectile();
        StartCoroutine(DroneRotate());
    }
    public void SetItemRank(int getRank)
    {
        itemRank = getRank;
        SetInterceptDroneData();
    }

    void SetInterceptDroneData()
    {
        float characterAttackSpeed = (float)character.ReturnCharacterAttackSpeed();
        float characterAttackRange = (float)character.ReturnCharacterAttackRange();

        switch (itemRank)
        {
            case 0:
                timeCount = 40 / (characterAttackSpeed * (float)DataManager.instance.consumableList.item[14].attackSpeedValue);
                detectRadius = characterAttackRange * 0.15f * (float)DataManager.instance.consumableList.item[14].attackRangeValue;
                break;
            case 1:
                timeCount = 40 / (characterAttackSpeed * (float)DataManager.instance.consumableList.item[29].attackSpeedValue);
                detectRadius = characterAttackRange * 0.15f * (float)DataManager.instance.consumableList.item[29].attackRangeValue;
                break;
            case 2:
                timeCount = 40 / (characterAttackSpeed * (float)DataManager.instance.consumableList.item[44].attackSpeedValue);
                detectRadius = characterAttackRange * 0.15f * (float)DataManager.instance.consumableList.item[44].attackRangeValue;
                break;
            case 3:
                timeCount = 40 / (characterAttackSpeed * (float)DataManager.instance.consumableList.item[59].attackSpeedValue);
                detectRadius = characterAttackRange * 0.15f * (float)DataManager.instance.consumableList.item[59].attackRangeValue;
                break;
        }
        detectRadius += 1.95f;
        Debug.Log(timeCount + "/ " + detectRadius);
    }

    IEnumerator DroneRotate()
    {
        while (!nightManager.isStageEnd)
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

    void DetectProjectile()
    {
        projLayer = LayerMask.NameToLayer("Projectile");
        StartCoroutine(DetectProjectileCoroutine());
    }

    IEnumerator DetectProjectileCoroutine()
    {
        int layerMask = (1 << projLayer);
        while (!nightManager.isStageEnd)
        {
            Collider2D[] colArr = Physics2D.OverlapCircleAll(character.transform.position, detectRadius, layerMask);

            if (colArr.Length > 0)
            {
                nightSFXManager.PlayAudioClip(AudioClipName.interceptDrone);
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
        getProjScript.SetProjPos();
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
