using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RailPiercer : MonoBehaviour
{
    [SerializeField]
    GameObject railPiercerImage;
    [SerializeField]
    GameObject railPiercerHitBox;

    public Character character;

    [SerializeField] SpriteRenderer railPiercerImageRenderer;
    Rigidbody2D railPiercerRigid;

    float attackPowerRate;
    float attackSpeedRate;

    bool isShoot = false;

    public Image coolTimeImage;

    Coroutine RailPiercerPosCoroutine;

    private void Awake()
    {
        character = GameObject.Find("CharacterImage").GetComponent<Character>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Normal" || collision.tag == "Elite")
        {
            float damage = character.player.attackPower * attackPowerRate;
            collision.GetComponent<Enemy>().Damaged(damage);
        }
    }

    public void InitializeRailPiercer(Consumable item, Image coolTimeImage)
    {
        transform.SetParent(character.transform);
        this.coolTimeImage = coolTimeImage;
        attackPowerRate = item.attackPowerValue;
        attackSpeedRate = item.attackSpeedValue;
        railPiercerImageRenderer = railPiercerImage.GetComponent<SpriteRenderer>();
        StartRailPiercerPositionCoroutine();
    }

    public void StartRailPiercerPositionCoroutine()
    {
        if (RailPiercerPosCoroutine == null)
            RailPiercerPosCoroutine = StartCoroutine(SetRailPiercerPosCoroutine());
    }

    private void StopRailPiercerPositionCoroutine()
    {
        if (RailPiercerPosCoroutine != null)
        {
            StopCoroutine(RailPiercerPosCoroutine);
            RailPiercerPosCoroutine = null;
        }
    }

    IEnumerator SetRailPiercerPosCoroutine()
    {
        railPiercerRigid = GetComponent<Rigidbody2D>();
        Transform characterTransform = character.transform;

        Vector3 offset;
        while (!NightManager.Instance.isStageEnd)
        {
            offset = CalculateOffset();

            if (!isShoot)
            {
                railPiercerImageRenderer.flipX = (character.Movement.MoveDirection.x >= 0);
            }

            transform.position = characterTransform.position + offset;
            railPiercerRigid.linearVelocity = character.GetVelocity();
            yield return null;
        }
    }

    Vector3 CalculateOffset()
    {
        float xOffset = 2f;
        bool shouldFlip = (character.Movement.MoveDirection.x >= 0) ^ isShoot;
        return new Vector3(shouldFlip ? -xOffset : xOffset, 2f, 0);
    }

    public void StartRailPiercer()
    {
        ShootRailPiercer();
    }

    public void ShootRailPiercer()
    {
        railPiercerHitBox.SetActive(false);
        StartCoroutine(ShootRailPiercerCoroutine());
    }

    IEnumerator ShootRailPiercerCoroutine()
    {
        Vector3 railPiercerImageScale = Vector3.zero;

        float attackTime;
        float timer;
        while (!NightManager.Instance.isStageEnd)
        {
            attackTime = 10 / (character.player.attackSpeed * attackSpeedRate);
            coolTimeImage.fillAmount = 1;
            timer = 0;

            while (timer < attackTime)
            {
                timer += Time.deltaTime;
                coolTimeImage.fillAmount = 1 - timer / attackTime; 
                yield return null;
            }


            isShoot = true;

            bool isWatchRight = (character.Movement.MoveDirection.x >= 0);

            SetRailPiercerViewPos();

            NightSFXManager.Instance.PlayAudioClip(AudioClipName.railPiercer);
            railPiercerHitBox.SetActive(true);

            yield return new WaitForSeconds(0.5f);

            railPiercerHitBox.SetActive(false);

            isShoot = false;
        }
    }

    void SetRailPiercerViewPos()
    {
        Vector3 watchDir = character.Movement.MoveDirection;

        float angle = Mathf.Atan2(watchDir.y, watchDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - 180, Vector3.forward);
    }
}
