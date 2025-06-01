using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public Monster stats;


    [SerializeField]
    GameObject player; // 몬스터의 적, 즉 플레이어를 뜻함

    
    Rigidbody2D enemyRigid;
    //테스트용
    SpriteRenderer enemyRenderer;

    protected Vector3 moveDir;

    public bool isSlow = false;
    protected bool isStageEnd = false; // isStageEnd = true라면 이동을 멈춤
    public bool isStop = false;         //오브젝트 움직임을 컨트롤하기 위해 만듦
    public bool isAttacked = false;     //공격을 했다면 2초간 true로 변환

    Coroutine moveCoroutine = null;






    protected void Start()
    {
        enemyRigid = GetComponent<Rigidbody2D>();
        enemyRenderer = GetComponent<SpriteRenderer>();

        player = GameObject.FindWithTag("Player").transform.Find("CharacterImage").gameObject;

        EnemyMove();
    }
    public void EnemyMove()
    {
        if (moveCoroutine == null)
            moveCoroutine = StartCoroutine(EnemyMoveCoroutine());
    }

    IEnumerator EnemyMoveCoroutine()
    {
        while (!isStageEnd)
        {
            while (isStop)
            {
                enemyRigid.linearVelocity = Vector3.zero;
                yield return null;
                continue;
            }

            Vector3 playerPosition = player.transform.position;
            moveDir = playerPosition - transform.position;

            float direction = moveDir.x > 0 ? -1f : 1f;
            if (transform.localScale.x != direction)
                transform.localScale = new Vector3(direction, 1, 1);

            float moveSpeed = SetMoveSpeed(stats.moveSpeed);
            enemyRigid.linearVelocity = moveDir.normalized * moveSpeed;

            yield return new WaitForSeconds(1);
        }

        enemyRigid.linearVelocity = Vector3.zero;
    }

    // Tolelom: set이라기 보다는 get에 가깝지 않나..
    // 추가로 저런 식이 왜 나온걸까?
    protected float SetMoveSpeed(double getMoveSpeed)
    {
        return ((float)getMoveSpeed * 25) / 100;
    }

    public void StopMove()
    {
        isStageEnd = true;
    }








    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyDamaged(collision);
    }

    public void SetEnemyStatus(int getLevel = 1)
    {
        // 선택한 난이도에 따라 스테이터스 변경
        int difficulty = GameManager.Instance.player.difficulty;
        stats.maxHp *= DataManager.Instance.difficultyList.difficulty[difficulty].enemyStatus;
        stats.curHp *= DataManager.Instance.difficultyList.difficulty[difficulty].enemyStatus;
        stats.attackPower *= DataManager.Instance.difficultyList.difficulty[difficulty].enemyStatus;
    }



    //몬스터가 강화되었는지
    public void SetEnforceData(int getLevel, bool isElite = false)
    {
        if (IsEnforce(getLevel, isElite))
        {
            stats.isEnforce = true;

            //강화되었으므로 스테이터스 변경
            stats.maxHp *= 2;
            stats.curHp *= 2;
            stats.attackPower *= 2;
        }
    }

    bool IsEnforce(int getLevel, bool isElite = false)
    {
        double nowProb;
        double randomProb = Random.value;

        int difficulty = GameManager.Instance.player.difficulty;
        if (!isElite)
            nowProb = DataManager.Instance.difficultyList.difficulty[difficulty].normalEnhance;
        else
            nowProb = DataManager.Instance.difficultyList.difficulty[difficulty].eliteEnhance;

        if (randomProb < nowProb)
            return true;
        else
            return false;
    }

    //캐릭터와 충돌할 경우 해당 적의 공격력을 반환하는 함수
    public double GetEnemyAttackPower()
    {
        if (isAttacked == false)
            return stats.attackPower;
        else return 0;
    }

    //공격하고 2초동안 공격 무시
    public void SetIsAttacked()
    {
        isAttacked = true;
        StartCoroutine(SetIsAttackedCoroutine());
    }

    IEnumerator SetIsAttackedCoroutine()
    {
        yield return new WaitForSeconds(2.0f);
        isAttacked = false;
    }

    void EnemyDamaged(Collider2D collision)
    {
        double getDamage = 0;

        if (collision.name == "HitBox")
        {
            getDamage = collision.gameObject.GetComponent<HitBox>().getAttackPower;
            collision.gameObject.GetComponent<HitBox>().isAttacked = true;
        }
        else if (collision.name == "CentryBallProjPrefab(Clone)")
        {
            getDamage = collision.GetComponent<Projectile>().projPower;
        }
        else if (collision.tag == "Item")
        {
            Debug.Log(collision.name);
            if (collision.name == "ChargingReaperImage")
            {
                getDamage = collision.transform.parent.GetComponent<ChargingReaper>().reaperAttackDamaege;
            }
        }

        if (getDamage > 0)
        {
            //피흡 있으면 여기서 회복
            player.GetComponent<Character>().AbsorbAttack();
            EnemyMoveBack();

            if (stats.curHp > getDamage)
            {
                StartCoroutine(EnemyDamagedAlphaCoroutine());
                stats.curHp -= getDamage;
            }
            else
            {
                player.GetComponent<Character>().UpdateKillCount();

                if (this.gameObject.tag == "Normal")
                    player.GetComponent<Character>().UpdateKillNormalCount();
                else if (this.gameObject.tag == "Elite")
                    player.GetComponent<Character>().UpdateKillEliteCount();

                Destroy(this.gameObject);
            }
        }
    }


    public void EnemyDamaged(double getDamage)
    {
        if (getDamage > 0)
        {

            if (stats.curHp > getDamage)
            {
                StartCoroutine(EnemyDamagedAlphaCoroutine());
                stats.curHp -= getDamage;
            }
            else
            {
                player.GetComponent<Character>().UpdateKillCount();
                Destroy(this.gameObject);
            }
        }
    }

    IEnumerator EnemyDamagedAlphaCoroutine()
    {
        Debug.Log(enemyRenderer.color.a + "@@@@@@@@@@@@@@@@@@@@@@@@@");
        if (enemyRenderer.color.a == 1)
        {
            Color color = enemyRenderer.color;
            color.a = 0.7f;
            enemyRenderer.color = color;

            yield return new WaitForSeconds(0.2f);

            color.a = 1;
            enemyRenderer.color = color;
        }
    }

    public void DrugEnemy()
    {
        Vector3 start;
        start = this.transform.position;
        StartCoroutine(DrugEnemyCoroutine(start));
    }

    IEnumerator DrugEnemyCoroutine(Vector3 start)
    {
        Vector3 nowVelocity = enemyRigid.linearVelocity;
        //캐릭터 위치 기준으로 반경 256px안까지 끌어당김
        //while((character.transform.position - this.transform.position).magnitude >= 2f)
        //{
        //    enemyRigid.AddForceAtPosition(moveDir, this.transform.position);
        //    yield return null;
        //}

        //본인 초기 위치 기준 150px 앞으로 당겨짐
        while ((start - this.transform.position).magnitude <= 1.5f)
        {
            enemyRigid.AddForceAtPosition(moveDir, this.transform.position);
            yield return null;
        }
        enemyRigid.linearVelocity = nowVelocity;
    }

    public void ThrustEnemy()
    {
        isStop = true;
        Vector3 start;
        start = this.transform.position;
        StartCoroutine(ThrustEnemyCoroutine(start));
    }

    IEnumerator ThrustEnemyCoroutine(Vector3 start)
    {
        Vector3 nowVelocity = enemyRigid.linearVelocity;
        moveDir = start - player.transform.position;
        enemyRigid.linearVelocity = moveDir.normalized * 5;

        //투명도로 확인하려고 임시로 만들어둠
        Color nowColor = enemyRenderer.color;
        nowColor.a = 0.5f;
        enemyRenderer.color = nowColor;

        //적 초기 위치 기준 256px 튕김
        while ((player.transform.position - this.transform.position).magnitude <= 2f)
        {
            //enemyRigid.AddForceAtPosition(moveDir, this.transform.position);
            yield return null;
        }
        nowColor.a = 1f;
        enemyRenderer.color = nowColor;
        enemyRigid.linearVelocity = nowVelocity;
        isStop = false;
    }

    public double ReturnEnemyMoveSpeed()
    {
        return stats.moveSpeed;
    }

    public void EnemyStop()
    {
        enemyRigid.linearVelocity = Vector3.zero;
        StopCoroutine(moveCoroutine);
        moveCoroutine = null;
    }

    public double ReturnEnemyHitPointMax()
    {
        return stats.maxHp;
    }

    public void SetEnemyMoveSpeed(double getEnemySpeed)
    {
        stats.moveSpeed = getEnemySpeed;
    }

    void EnemyMoveBack()
    {
        if (player.GetComponent<Character>().isMoveBackOn && !stats.isResist)
        {
            isStop = true;
            Vector3 start;
            start = this.transform.position;
            StartCoroutine(EnemyMoveBackCoroutine(start));
        }
    }

    IEnumerator EnemyMoveBackCoroutine(Vector3 start)
    {
        Vector3 nowVelocity = enemyRigid.linearVelocity;
        moveDir = start - player.transform.position;
        enemyRigid.linearVelocity = moveDir.normalized * 5; Debug.Log(moveDir + " " + enemyRigid.linearVelocity);

        //투명도로 확인하려고 임시로 만들어둠
        Color nowColor = enemyRenderer.color;
        nowColor.a = 0.5f;
        enemyRenderer.color = nowColor;

        //적 초기 위치 기준 150px 튕김
        while ((start - this.transform.position).magnitude <= 1.5f)
        {
            //enemyRigid.AddForceAtPosition(moveDir, this.transform.position);
            yield return null;
        }
        Debug.Log("end");
        nowColor.a = 1f;
        enemyRenderer.color = nowColor;
        enemyRigid.linearVelocity = nowVelocity;
        isStop = false;
    }
}
