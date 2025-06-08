using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class DisasterDrone : MonoBehaviour
{
    public NightManager nightManager;
    public Character character;
    LayerMask enemyLayer;
    [SerializeField]
    float detectRadius;
    [SerializeField]
    float damage;

    int itemRank;
    float getAttackRangeValue;
    float enemyDamageRate;

    Enemy getEnemyScript;

    private void Start()
    {
        DetectEnemy();
    }

    public void SetItemRank(int getRank)
    {
        itemRank = getRank;
        SetDisasterDroneData();
    }

    void SetDisasterDroneData()
    {
        getAttackRangeValue = (float)DataManager.Instance.consumableList.item[2].attackRangeValue;

        switch (itemRank)
        {
            case 0:
                detectRadius = (float)character.player.attackRange * 0.15f * getAttackRangeValue;
                enemyDamageRate = 0.07f;
                break;
            case 1:
                detectRadius = (float)character.player.attackRange * 0.15f * getAttackRangeValue;
                enemyDamageRate = 0.10f;
                break;
            case 2:
                detectRadius = (float)character.player.attackRange * 0.15f * getAttackRangeValue;
                enemyDamageRate = 0.13f;
                break;
            case 3:
                detectRadius = (float)character.player.attackRange * 0.15f * getAttackRangeValue;
                enemyDamageRate = 0.16f;
                break;
        }
        detectRadius += 1.95f;

        this.transform.localScale = new Vector3(detectRadius / 3, detectRadius / 3, detectRadius / 3);
    }

    void DetectEnemy()
    {
        enemyLayer = LayerMask.NameToLayer("Enemy");
        StartCoroutine(DetectEnemyCoroutine());
    }

    IEnumerator DetectEnemyCoroutine()
    {
        int layerMask = (1 << enemyLayer);
        while (!nightManager.isStageEnd)
        {
            Collider2D[] colArr = Physics2D.OverlapCircleAll(character.transform.position, detectRadius, layerMask);

            if (colArr.Length > 0)
            {
                for(int i =0; i< colArr.Length; i++)
                {
                    AttackEnemys(colArr[i]);
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }

    void AttackEnemys(Collider2D getCol)
    {
        getEnemyScript = getCol.GetComponent<Enemy>();
        damage = getEnemyScript.stats.maxHp;

        damage *= enemyDamageRate;

        getEnemyScript.EnemyDamaged(damage);
    }
}
