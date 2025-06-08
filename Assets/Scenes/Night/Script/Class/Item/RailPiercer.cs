using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RailPiercer : MonoBehaviour
{
    [SerializeField]
    GameObject railPiercerImage;
    [SerializeField]
    GameObject railPiercerHitBox;

    public NightManager nightManager;
    public NightSFXManager nightSFXManager;
    public Character character;
    [SerializeField]
    SpriteRenderer hitBoxRenderer;
    [SerializeField]
    SpriteRenderer railPiercerImageRenderer;
    Rigidbody2D railPiercerRigid;
    [SerializeField] float attackPower = 500;
    [SerializeField] float attackTime;
    bool isWatchRight = true;
    bool isShoot = false;

    int itemRank;

    //��Ÿ��
    public Image coolTimeImage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Normal" || collision.tag == "Elite")
        {
            collision.GetComponent<Enemy>().EnemyDamaged(attackPower);
        }
    }

    public void SetItemRank(int getRank)
    {
        itemRank = getRank;
        SetRailPiercerData();
    }

    void SetRailPiercerData()
    {
        float characterAttackSpeed = (float)character.player.attackSpeed;
        float characterAttackPower = (float)character.player.attackPower;

        switch (itemRank)
        {
            case 0:
                attackPower = characterAttackPower * DataManager.Instance.consumableList.item[4].attackPowerValue;
                attackTime = 10 / (characterAttackSpeed * DataManager.Instance.consumableList.item[4].attackSpeedValue);
                break;
            case 1:
                attackPower = characterAttackPower * DataManager.Instance.consumableList.item[19].attackPowerValue;
                attackTime = 10 / (characterAttackSpeed * DataManager.Instance.consumableList.item[19].attackSpeedValue);
                break;
            case 2:
                attackPower = characterAttackPower * DataManager.Instance.consumableList.item[34].attackPowerValue;
                attackTime = 10 / (characterAttackSpeed * DataManager.Instance.consumableList.item[34].attackSpeedValue);
                break;
            case 3:
                attackPower = characterAttackPower * DataManager.Instance.consumableList.item[49].attackPowerValue;
                attackTime = 10 / (characterAttackSpeed * DataManager.Instance.consumableList.item[49].attackSpeedValue);
                break;
        }
    }

    public void ShootRailPiercer()
    {
        railPiercerHitBox.SetActive(false);
        StartCoroutine(ShootRailPiercerCoroutine());
    }

    IEnumerator ShootRailPiercerCoroutine()
    {
        Vector3 railPiercerImageScale = Vector3.zero;

        while (!nightManager.isStageEnd)
        {
            isShoot = true;

            //��Ʈ�ڽ��� ���� ���⿡ ��� x ��ǥ�� ������
            Vector3 hitBoxPos = Vector3.zero;
            if (character.Movement.MoveDirection.x >= 0)
            {
                isWatchRight = true;
            }
            else
            {
                isWatchRight = false;
            }
            SetRailPiercerViewPos();

            //���� ��� �κ�
            nightSFXManager.PlayAudioClip(AudioClipName.railPiercer);
            railPiercerHitBox.SetActive(true);

            yield return new WaitForSeconds(0.5f);

            railPiercerHitBox.SetActive(false);
            isShoot = false;

            StartCoroutine(SetCooltimeCoroutine((float)attackTime));
            yield return new WaitForSeconds((float)attackTime);
            coolTimeImage.fillAmount = 1;
        }
    }

    void SetRailPiercerViewPos()
    {
        Transform railPiercerTransform = this.transform;
        float angle;
        Vector3 watchDir = character.Movement.MoveDirection;

        angle = Mathf.Atan2(watchDir.y, watchDir.x) * Mathf.Rad2Deg;
        railPiercerTransform.rotation = Quaternion.AngleAxis(angle - 180, Vector3.forward);
    }


    public void SetRailPiercerPos()
    {
        hitBoxRenderer = railPiercerHitBox.GetComponent<SpriteRenderer>();
        railPiercerImageRenderer = railPiercerImage.GetComponent<SpriteRenderer>();
        StartCoroutine(SetRailPiercerPosCoroutine());
    }

    IEnumerator SetRailPiercerPosCoroutine()
    {
        railPiercerRigid = this.GetComponent<Rigidbody2D>();
        Vector3 nowPos = character.transform.position;


        while (!nightManager.isStageEnd)
        {
            nowPos = character.transform.position;

            //������ �Ƚ�� ���� ��
            if (!isShoot)
            {
                if (character.Movement.MoveDirection.x >= 0)
                {
                    railPiercerImageRenderer.flipX = true;
                    nowPos.x -= 2;
                }
                else
                {
                    railPiercerImageRenderer.flipX = false;
                    nowPos.x += 2;
                }
            }
            //�� ��
            else
            {
                if (isWatchRight)
                    nowPos.x -= 2;
                else
                    nowPos.x += 2;
            }
            nowPos.y += 2;

            this.transform.position = nowPos;
            railPiercerRigid.linearVelocity = character.GetVelocity();
            yield return null;
        }
    }

    IEnumerator SetCooltimeCoroutine(float getCoolTime)
    {
        float nowTime = 0;
        coolTimeImage.gameObject.SetActive(true);
        while (coolTimeImage.fillAmount > 0)
        {
            nowTime += Time.deltaTime;
            coolTimeImage.fillAmount = 1 - nowTime / getCoolTime;
            yield return null;
        }
    }
}
