using System.Collections;
using UnityEngine;

public class EliteEnemy : Enemy
{
    [SerializeField] GameObject projectileObject;
    [SerializeField] GameObject[] projectilesPulling;


    int pullingScale = 80;
    [SerializeField] int nowPullingIndex = 0;

    bool isShoot = false;

    [SerializeField] float shootRange = 20f;
    [SerializeField] float dashRange = 10f;

    private Character character;


    protected override void Start()
    {
        base.Start();
        character = GameObject.Find("CharacterImage").GetComponent<Character>();
    }

    //공격 함수 들어갈 예정 + 범위는 overlap
    protected void SetProjectile()
    {
        if (stats.canProj)
        {
            projectilesPulling = new GameObject[pullingScale];

            //투사체 준비
            for (int i = 0; i < pullingScale; i++)
            {
                GameObject nowProj = Instantiate(projectileObject, transform);
                nowProj.GetComponent<Projectile>().isEnemy = true;
                nowProj.transform.SetParent(transform);
                nowProj.transform.position = transform.position;
                nowProj.SetActive(false);
                projectilesPulling[i] = nowProj;
            }
            DetectCharacter();
        }
    }

    void ShootProjectile()
    {
        StartCoroutine(ShootProjectileCoroutine());
    }

    IEnumerator ShootProjectileCoroutine()
    {
        if (!isShoot)
        {
            isShoot = true;

            float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
            projectilesPulling[nowPullingIndex].transform.rotation = Quaternion.AngleAxis(angle - 180, Vector3.forward);

            projectilesPulling[nowPullingIndex].SetActive(true);

            projectilesPulling[nowPullingIndex].GetComponent<Rigidbody2D>().linearVelocity
                = moveDir.normalized * (stats.moveSpeed /2);

            yield return new WaitForSeconds(2f);

            if (nowPullingIndex < 20)
                nowPullingIndex++;
            else
                nowPullingIndex = 0;

            isShoot = false;
        }
    }

    void DetectCharacter()
    {
        StartCoroutine(EliteEnemyCoroutine());
    }

    // Elite Enemy Coroutine에서는 두 가지 기능을 처리합니다.
    // 1. 플레이어와 거리가 일정 거리 이상 가까워지면 투사체 발사(canProj인 경우)
    // 2. 플레이어와 거리가 일정 거리 이상 가까워지면 이동속도 증가(dashAble인 경우)
    IEnumerator EliteEnemyCoroutine()
    {
        int layerMask = (1 << LayerMask.NameToLayer("Character"));

        Character character = GameObject.Find("CharacterImage").GetComponent<Character>();

        bool isDash = false; // Enemy가 대쉬 중인지
        while (!isStageEnd)
        {
            float distance = Vector3.Distance(character.transform.position, transform.position);

            if (stats.canProj && distance < shootRange)
            {
                //투사체 발사
                ShootProjectile();
            }

            if (stats.dashAble)
            {
                
                if (distance < dashRange && !isDash)
                {
                    Logger.Log("엘리트 몬스터 이속 증가");
                    stats.moveSpeed = stats.moveSpeed * stats.dashSpeed;
                    isDash = true;
                }
                else if (distance >= dashRange && isDash)
                {
                    Logger.Log("엘리트 몬스터 이속 감소");
                    stats.moveSpeed = stats.moveSpeed / stats.dashSpeed;
                    isDash = false;
                }
            }

            yield return new WaitForSeconds(3f);
        }
    }
}
