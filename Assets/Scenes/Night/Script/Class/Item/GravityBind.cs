using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEngine;
using static UnityEditor.Progress;

public class GravityBind : MonoBehaviour
{
    private Character character;

    [SerializeField] float detectScale;
    [SerializeField] float spinSpeed;

    [SerializeField] float slowRate;

    Enemy getEnemyScript;

    private float attackRangeRate;
    private float attackSpeedRate;

    void Awake()
    {
        character = GameObject.Find("CharacterImage").GetComponent<Character>();
    }


    public void InitializeGravityBind(Consumable item)
    {
        transform.SetParent(character.transform);
        transform.position = character.transform.position;

        attackRangeRate = item.attackRangeValue;
        attackSpeedRate = item.attackSpeedValue;
        SetGravityBindData(item);
    }

    public void StartGravityBind()
    {
        StartCoroutine(SpinGravityBindCoroutine());
    }



    void SetGravityBindData(Consumable item)
    {
        slowRate = character.player.attackSpeed * attackSpeedRate;
        
        spinSpeed = 50f;
        UpdateDetectScale();
    }

    private void UpdateDetectScale()
    {
        detectScale = character.player.attackRange * 0.4f * attackRangeRate;
        detectScale /= 3;   //이미지 기본 픽셀이 600px라서 100px로 맞춰주고 스케일 조절하기 위해 넣었음.
        transform.localScale = Vector3.one * detectScale;
    }

    private void OnTriggerEnter2D(Collider2D getCol)
    {
        if (getCol.tag == "Normal" || getCol.tag == "Elite")
        {
            SlowEnemy(getCol);
        }
    }

    private void OnTriggerExit2D(Collider2D getCol)
    {
        if (getCol.tag == "Normal" || getCol.tag == "Elite")
        {
            ExitSlowEnemy(getCol);
        }
    }

    void SlowEnemy(Collider2D getCol)
    {
        Logger.Log("Slow Enemy");
        getEnemyScript = getCol.GetComponent<Enemy>();
        float getEnemySpeed = getEnemyScript.stats.moveSpeed;
        getEnemyScript.stats.moveSpeed = (getEnemySpeed * (1 - slowRate / 100));
    }

    void ExitSlowEnemy(Collider2D getCol)
    {
        Logger.Log("Exit Slow Enemy");
        getEnemyScript = getCol.GetComponent<Enemy>();
        float getEnemySpeed = getEnemyScript.stats.moveSpeed;
        getEnemyScript.stats.moveSpeed = (getEnemySpeed / (1 - slowRate / 100));
    }

    IEnumerator SpinGravityBindCoroutine()
    {
        float rotationSpeed = spinSpeed;
        while (!NightManager.Instance.isStageEnd)
        {
            transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);

            // player의 attackSpeed, attackRange 변경됬을 때만 호출하는게 바람직함 (이벤트화)
            slowRate = character.player.attackSpeed * attackSpeedRate;
            UpdateDetectScale();
            yield return null;
        }
    }
}
