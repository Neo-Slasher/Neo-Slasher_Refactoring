using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static UnityEngine.GraphicsBuffer;

public class CentryBall : MonoBehaviour
{
    [SerializeField]
    NightManager nightManager;
    [SerializeField]
    NightSFXManager nightSFXManager;
    [SerializeField]
    Character character;
    [SerializeField]
    GameObject projectileObject;
    [SerializeField]
    GameObject[] projectilesPulling;
    [SerializeField]
    GameObject sparkImage;      //�߻� �̹���
    [SerializeField]
    GameObject centryBallImage;
    int pullingScale = 100;
    int nowPullingIndex = 0;

    int itemRank;
    float attackRangeRate;
    float attackSpeedRate;
    float attackPowerRate;

    Vector3 watchDir = Vector3.zero;
    bool isShoot = false;
    bool isStop = false;

    LayerMask enemyLayer;
    [SerializeField]
    float detectRadius;
    [SerializeField]
    float shootTime;
    [SerializeField]
    float projPower;

    private void Awake()
    {
        nightManager = GameObject.Find("NightManager").GetComponent<NightManager>();
        nightSFXManager = GameObject.Find("NightSFXManager").GetComponent<NightSFXManager>();
        character = GameObject.Find("CharacterImage").GetComponent<Character>();
    }

    private void Start()
    {
        SetCentryBallData();
        SetProjectile();
    }

    public void SetItemRank(int getRank)
    {
        itemRank = getRank;
    }

    void SetCentryBallData()
    {
        switch(itemRank)
        {
            case 0:
                attackRangeRate = (float)DataManager.instance.consumableList.item[0].attackRangeValue;
                attackSpeedRate = (float)DataManager.instance.consumableList.item[0].attackSpeedValue;
                attackPowerRate = (float)DataManager.instance.consumableList.item[0].attackPowerValue;
                break;
            case 1:
                attackRangeRate = (float)DataManager.instance.consumableList.item[15].attackRangeValue;
                attackSpeedRate = (float)DataManager.instance.consumableList.item[15].attackSpeedValue;
                attackPowerRate = (float)DataManager.instance.consumableList.item[15].attackPowerValue;
                break;
            case 2:
                attackRangeRate = (float)DataManager.instance.consumableList.item[30].attackRangeValue;
                attackSpeedRate = (float)DataManager.instance.consumableList.item[30].attackSpeedValue;
                attackPowerRate = (float)DataManager.instance.consumableList.item[30].attackPowerValue;
                break;
            case 3:
                attackRangeRate = (float)DataManager.instance.consumableList.item[45].attackRangeValue;
                attackSpeedRate = (float)DataManager.instance.consumableList.item[45].attackSpeedValue;
                attackPowerRate = (float)DataManager.instance.consumableList.item[45].attackPowerValue;
                break;
        }
        projPower = (float)character.ReturnCharacterAttackPower() * attackPowerRate;
    }

    //���� �Լ� �� ���� + ������ overlap
    void SetProjectile()
    {
        projectilesPulling = new GameObject[pullingScale];

        //����ü �غ�
        for (int i = 0; i < pullingScale; i++)
        {
            GameObject nowProj = Instantiate(projectileObject, this.transform);
            nowProj.GetComponent<Projectile>().isEnemy = false;
            nowProj.GetComponent<Projectile>().projPower = projPower;
            nowProj.transform.SetParent(this.transform.GetChild(1));
            nowProj.transform.position = centryBallImage.transform.position;
            nowProj.SetActive(false);
            projectilesPulling[i] = nowProj;
        }
        DetectEnemy();
    }

    void ShootProjectile(Collider2D getCol)
    {
        StartCoroutine(ShootProjectileCoroutine(getCol));
    }

    IEnumerator ShootProjectileCoroutine(Collider2D getCol)
    {
        if (!isShoot)
        {
            nightSFXManager.PlayAudioClip(AudioClipName.centryBall);
            StartCoroutine(CentryBallShootAnimation(getCol));
            StartCoroutine(StopCentryBallCoroutine());

            isShoot = true;
            projectilesPulling[nowPullingIndex].transform.position = this.transform.GetChild(0).position;
            projectilesPulling[nowPullingIndex].SetActive(true);
            float angle = Mathf.Atan2(watchDir.y, watchDir.x) * Mathf.Rad2Deg;
            projectilesPulling[nowPullingIndex].transform.rotation = Quaternion.AngleAxis(angle - 180, Vector3.forward);

            Vector3 moveDir = getCol.transform.position - centryBallImage.transform.position;
            
            projectilesPulling[nowPullingIndex].GetComponent<Rigidbody2D>().linearVelocity
                = moveDir.normalized * 10;

            yield return new WaitForSeconds(shootTime);

            if (nowPullingIndex < 30)
                nowPullingIndex++;
            else
                nowPullingIndex = 0;

            isShoot = false;
        }
    }

    void DetectEnemy()
    {
        enemyLayer = LayerMask.NameToLayer("Enemy");
        StartCoroutine(DetectEnemyCoroutine());
    }

    IEnumerator DetectEnemyCoroutine()
    {
        int layerMask = (1 << enemyLayer);
        Collider2D shortestCol = null;
        detectRadius = (float)character.ReturnCharacterAttackRange() * 15 * 0.01f * attackRangeRate;
        detectRadius += 1.95f;
        shootTime = 10 / ((float)character.ReturnCharacterAttackSpeed() * attackSpeedRate);

        while (!nightManager.isStageEnd)
        { 
            Collider2D[] colArr = Physics2D.OverlapCircleAll(character.transform.position, detectRadius, layerMask);
            
            if (colArr.Length > 0)
            {
                //����ü �߻�
                shortestCol = SetShortestDistanceCol(colArr);
                ShootProjectile(shortestCol);
            }

            yield return new WaitForSeconds(shootTime);
        }
    }

    Collider2D SetShortestDistanceCol(Collider2D[] getColArr)
    {
        Collider2D shortestDistanceCol = getColArr[0]; 
        float shortestDistance = (getColArr[0].transform.position - character.transform.position).magnitude;
        float nowDistance;

        for(int i =1; i< getColArr.Length; i++)
        {
            nowDistance = (getColArr[i].transform.position - character.transform.position).magnitude;
            if ( nowDistance < shortestDistance)
            {
                shortestDistanceCol = getColArr[i];
                shortestDistance = nowDistance;
            }
        }
        
        return shortestDistanceCol;

    }

    IEnumerator CentryBallShootAnimation(Collider2D getCol)
    {
        SetCentryBallWatchEnemy(getCol);
        sparkImage.SetActive(true);

        yield return new WaitForSeconds(0.3f);
        sparkImage.SetActive(false);
    }

    //�� �� �ٶ󺸴� �Լ�
    void SetCentryBallWatchEnemy(Collider2D getCol)
    {
        Transform enemyTransform = getCol.transform;
        float angle;

        watchDir = enemyTransform.position - centryBallImage.transform.position;

        angle = Mathf.Atan2(watchDir.y, watchDir.x) * Mathf.Rad2Deg;
        centryBallImage.transform.rotation = Quaternion.AngleAxis(angle - 180, Vector3.forward);
    }

    public bool StopCentryBall()
    {
        if (isStop)
            return true;
        else
            return false;
    }

    IEnumerator StopCentryBallCoroutine()
    {
        isStop = true;
        yield return new WaitForSeconds(0.2f);
        isStop = false;
    }
}
