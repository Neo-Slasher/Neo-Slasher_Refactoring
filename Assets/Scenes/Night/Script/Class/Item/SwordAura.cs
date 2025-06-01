using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAura : MonoBehaviour
{
    public double attackDamage = 500;
    public int itemRank;
    public float attackRange;
    public float goalDistance = 0;
    Vector3 startPos;

    private void Start()
    {
        startPos = this.transform.localPosition;
        goalDistance = SetGoalDistance();
        StartCoroutine(DestroySwordAura());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Normal" || collision.tag == "Elite")
            collision.GetComponent<Enemy>().EnemyDamaged(attackDamage);
        else if(collision.name == "BackGround")
            Destroy(this.gameObject);
    }

    IEnumerator DestroySwordAura()
    {
        Vector3 nowPos;
        while(true)
        {
            nowPos = this.transform.localPosition;
            if ((nowPos - startPos).magnitude <= goalDistance)
                yield return null;
            else
                break;
        }
        Destroy(this.gameObject);
    }

    float SetGoalDistance()
    {
        float returnGoalDistance = attackRange * 0.15f;

        switch(itemRank)
        {
            case 0:
                returnGoalDistance *= (float)DataManager.Instance.consumableList.item[3].attackRangeValue;
                break;
            case 1:
                returnGoalDistance *= (float)DataManager.Instance.consumableList.item[18].attackRangeValue;
                break;
            case 2:
                returnGoalDistance *= (float)DataManager.Instance.consumableList.item[33].attackRangeValue;
                break;
            case 3:
                returnGoalDistance *= (float)DataManager.Instance.consumableList.item[48].attackRangeValue;
                break;
        }

        Debug.Log((float)DataManager.Instance.consumableList.item[4].attackRangeValue);
        return returnGoalDistance;
    }
}
