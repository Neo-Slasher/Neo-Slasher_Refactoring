using System.Collections;
using UnityEngine;

public class CentryBall : MonoBehaviour
{
    [SerializeField] private Character character;
    [SerializeField] private GameObject projectileObject;

    [SerializeField] private GameObject sparkImage;
    private Vector3 sparkOffset; // 초기 오프셋

    // Object Pooling
    [SerializeField] private GameObject[] projectilePool;
    const int POOL_SCALE = 100;
    private int currentPoolIndex = 0;

    private float attackRangeRate;
    private float attackSpeedRate;
    private float attackPowerRate;

    private Vector3 watchDirection = Vector3.zero;

    private bool isStop = false;

    [SerializeField] private float detectRadius;
    [SerializeField] private float shootTime;
    [SerializeField] private float projPower;

    public AudioClip shootClip;

    void Awake()
    {
        character = GameObject.Find("CharacterImage").GetComponent<Character>();
    }


    void Start()
    {
        // sparkImage의 초기 위치 (CentryBall 중심 기준)
        sparkOffset = sparkImage.transform.localPosition;

        currentPoolIndex = 0;
    }

    public void InitializeCentryBall(Consumable item)
    {
        SetCentryBallData(item);
        SetTransform();
        transform.SetParent(character.transform);
        SetProjectile();

        isStop = false;
    }

    // 이거 이벤트로 업데이트 해야되지 않을까?
    // 또는 투사체 발사 시 마다 공격력 업데이트
    private void SetCentryBallData(Consumable item)
    {
        attackPowerRate = item.attackPowerValue;
        attackSpeedRate = item.attackSpeedValue;
        attackRangeRate = item.attackRangeValue;
    }

    private void SetTransform()
    {
        detectRadius = (character.player.attackRange * attackRangeRate) * 0.5f;

        Vector3 centryBallPos = character.transform.position + new Vector3(0, detectRadius, 0);
        transform.localPosition = centryBallPos;
    }

    private void SetProjectile()
    {
        projectilePool = new GameObject[POOL_SCALE];

        for (int i = 0; i < POOL_SCALE; i++)
        {
            GameObject projectile = Instantiate(projectileObject, transform);
            projectile.GetComponent<Projectile>().isEnemy = false;
            projectile.transform.SetParent(transform);

            projectile.SetActive(false);
            projectilePool[i] = projectile;
        }
    }

    public void ActiveCentryBall()
    {
        StartCoroutine(DetectEnemyAndShoot());
        StartCoroutine(Rotate());
    }

    private IEnumerator DetectEnemyAndShoot()
    {
        while (!NightManager.Instance.isStageEnd)
        {
            // TODO: 가능하다면 attackRange, attackSpeed가 변경될 때만 재계산
            projPower = character.player.attackPower * attackPowerRate;
            detectRadius = (character.player.attackRange * attackRangeRate) * 0.5f;
            shootTime = 10 / (character.player.attackSpeed * attackSpeedRate);

            Collider2D shortestCol = SetShortestDistanceCol();
            if (shortestCol != null)
                StartCoroutine(ShootProjectileCoroutine(shortestCol));

            yield return new WaitForSeconds(shootTime);
        }
    }

    private Collider2D SetShortestDistanceCol()
    {
        LayerMask enemyLayer = LayerMask.NameToLayer("Enemy");
        Collider2D[] colArr = Physics2D.OverlapCircleAll(character.transform.position, detectRadius, (1 << enemyLayer));
        if (colArr == null || colArr.Length == 0)
            return null;

        Collider2D shortestDistanceCol = colArr[0];

        float shortestDistance = (colArr[0].transform.position - character.transform.position).sqrMagnitude;

        for (int i = 1; i < colArr.Length; i++)
        {
            float currentDistance = (colArr[i].transform.position - character.transform.position).sqrMagnitude;
            if (currentDistance < shortestDistance)
            {
                shortestDistanceCol = colArr[i];
                shortestDistance = currentDistance;
            }
        }

        return shortestDistanceCol;
    }

    private IEnumerator ShootProjectileCoroutine(Collider2D target)
    {
        NightSFXManager.Instance.PlayAudioClip(AudioClipName.centryBall);
        StartCoroutine(CentryBallShootAnimation(target));

        // 필요한가? 발사 주기가 0.2f 보다 짧으면?
        yield return new WaitForSeconds(0.2f);

        Logger.Log($"current pool index: {currentPoolIndex}");
        GameObject projectile = projectilePool[currentPoolIndex];
        projectile.transform.SetParent(null); // 부모 분리 TODO: 발사체 사라질 때 부모 세팅
        projectile.transform.position = transform.position;
        projectile.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

        projectile.SetActive(true);
        projectile.GetComponent<CentryBallProjectile>().projPower = projPower;
        projectile.transform.GetComponent<CentryBallProjectile>().Shoot(target.transform);
        SoundManager.Instance.PlaySFX(shootClip);


        currentPoolIndex = (currentPoolIndex + 1) % POOL_SCALE;
    }

    private IEnumerator CentryBallShootAnimation(Collider2D enemy)
    {
        SetCentryBallWatchEnemy(enemy);
        sparkImage.SetActive(true);

        yield return new WaitForSeconds(0.3f);
        sparkImage.SetActive(false);
    }

    private void SetCentryBallWatchEnemy(Collider2D enemy)
    {
        if (enemy == null) return;

        watchDirection = enemy.transform.position - transform.position;
        float angle = Mathf.Atan2(watchDirection.y, watchDirection.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.AngleAxis(angle - 180, Vector3.forward);

        // CentryBallSprite 회전
        Transform centryBallSprite = transform.Find("CentryBallSprite");
        if (centryBallSprite != null)
            centryBallSprite.rotation = targetRotation;

        // sparkImage 위치 및 회전 업데이트
        if (sparkImage != null)
        {
            // 오프셋을 회전시킨 새 위치 계산
            Vector3 rotatedOffset = targetRotation * sparkOffset;
            sparkImage.transform.position = transform.position + rotatedOffset;
            sparkImage.transform.rotation = targetRotation;
        }
    }

    private IEnumerator Rotate()
    {
        float rotateSpeed = 0.2f;

        while (!NightManager.Instance.isStageEnd)
        {
            if (Time.timeScale != 0 && !isStop)
            {
                transform.RotateAround(character.transform.position, Vector3.back, rotateSpeed);

                transform.rotation = Quaternion.AngleAxis(0, Vector3.forward);
            }
            yield return null;
        }
    }
    

    private void OnDrawGizmos()
    {
        // 적 탐지 반경
        Gizmos.color = Color.azure;
        Gizmos.DrawWireSphere(character.transform.position, detectRadius);
    }
}
