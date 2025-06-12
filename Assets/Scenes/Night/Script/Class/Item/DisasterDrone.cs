using JetBrains.Annotations;
using System.Collections;
using UnityEngine;

public class DisasterDrone : MonoBehaviour
{
    [SerializeField] private Character character;

    [SerializeField] private float attackRangeRate;
    [SerializeField] private float damageRate;
    [SerializeField] private float detectRadius;

    public bool isStop;

    void Awake()
    {
        character = GameObject.Find("CharacterImage").GetComponent<Character>();
    }

    void Start()
    {
    }

    public void InitializeDisasterDrone(Consumable item)
    {
        SetDisasterDroneData(item);
        transform.SetParent(character.transform);
        transform.localPosition = character.transform.position;

        isStop = false;
    }

    public void StartDisasterDrone()
    {
        StartCoroutine(DetectEnemyCoroutine());
    }

    void SetDisasterDroneData(Consumable item)
    {
        switch (item.rank)
        {
            case 0:
                attackRangeRate = DataManager.Instance.consumableList.item[2].attackRangeValue;
                detectRadius = character.player.attackRange * attackRangeRate;
                damageRate = 0.07f;
                break;
            case 1:
                attackRangeRate = DataManager.Instance.consumableList.item[17].attackRangeValue;
                detectRadius = character.player.attackRange * attackRangeRate;
                damageRate = 0.10f;
                break;
            case 2:
                attackRangeRate = DataManager.Instance.consumableList.item[32].attackRangeValue;
                detectRadius = character.player.attackRange * attackRangeRate;
                damageRate = 0.13f;
                break;
            case 3:
                attackRangeRate = DataManager.Instance.consumableList.item[47].attackRangeValue;
                detectRadius = character.player.attackRange * attackRangeRate;
                damageRate = 0.16f;
                break;
        }
        detectRadius = detectRadius * 0.5f;

        transform.localScale = new Vector3(detectRadius / 3, detectRadius / 3, detectRadius / 3);
    }

    IEnumerator DetectEnemyCoroutine()
    {
        LayerMask enemyLayer = LayerMask.NameToLayer("Enemy");
        int layerMask = (1 << enemyLayer);

        // 1초로 공격 주기 고정
        WaitForSeconds waitInterval = new WaitForSeconds(1f);

        while (!NightManager.Instance.isStageEnd)
        {
            if (isStop)
                yield return new WaitWhile(() => isStop);

            Collider2D[] colArr = Physics2D.OverlapCircleAll(character.transform.position, detectRadius, layerMask);

            if (colArr != null && colArr.Length > 0)
            {
                for (int i = 0; i < colArr.Length; i++)
                {
                    AttackEnemys(colArr[i]);
                }
            }

            yield return waitInterval;
        }
    }

    void AttackEnemys(Collider2D getCol)
    {
        Enemy enemyScript = getCol.GetComponent<Enemy>();
        float damage = enemyScript.stats.maxHp;

        damage *= damageRate;

        enemyScript.Damaged(damage);
    }

    private void OnDrawGizmos()
    {
        // 적 탐지 반경
        Gizmos.color = Color.azure;
        Gizmos.DrawWireSphere(character.transform.position, detectRadius);
    }
}
