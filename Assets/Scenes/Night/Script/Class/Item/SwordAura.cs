using System.Collections;
using UnityEngine;

public class SwordAura : MonoBehaviour
{
    private Character character;

    public float attackDamage = 500;
    public int rank;
    public float attackRange;
    public float goalDistance = 0;
    Vector3 startPos;


    void Awake()
    {
        character = GameObject.Find("CharacterImage").GetComponent<Character>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Normal" || collision.tag == "Elite")
            collision.GetComponent<Enemy>().Damaged(attackDamage);
        else if (collision.name == "BackGround")
            Destroy(gameObject);
    }

    public void InitializeSwordAura(float attackPowerRate, float attackRangeRate, int rank)
    {
        transform.SetParent(character.transform);
        transform.localPosition = Vector3.zero; // 위치 초기화
        startPos = transform.localPosition;

        attackDamage = character.player.attackPower * attackPowerRate;
        attackRange = character.player.attackRange * attackRangeRate;
        this.rank = rank;

        SetSwordAuraAngle();

        GetComponent<Rigidbody2D>().linearVelocity = character.Movement.LastMoveDirection.normalized * 10;


        goalDistance = SetDistance();
    }
    void SetSwordAuraAngle()
    {
        if (character != null && character.Movement != null)
        {
            if (character.Movement.LastMoveDirection != Vector2.zero)
            {
                float angle = Vector2.SignedAngle(Vector2.right, character.Movement.LastMoveDirection);
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
            // LastMoveDirection이 zero일 때는 기존 각도 유지
        }
    }


    public void StartSwordAura()
    {
        StartCoroutine(SwordAuraCoroutine());
    }

    IEnumerator SwordAuraCoroutine()
    {
        Vector3 nowPos;
        while (true)
        {
            nowPos = transform.localPosition;
            if ((nowPos - startPos).magnitude <= goalDistance)
                yield return null;
            else
                break;
        }
        Destroy(gameObject);
    }

    float SetDistance()
    {
        int itemIdx = rank * 15 + 3;
        float distance = attackRange * 0.15f;
        distance *= DataManager.Instance.consumableList.item[itemIdx].attackRangeValue;

        return distance;
    }
}
