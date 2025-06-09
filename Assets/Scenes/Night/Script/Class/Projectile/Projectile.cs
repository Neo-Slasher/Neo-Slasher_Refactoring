using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public bool isEnemy;
    public float projPower;

    Transform target;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isEnemy)
        {
            //적이 쏜 투사체일 경우
            if (collision.name == "BackGround")
            {
                gameObject.SetActive(false);
                if (transform.parent != null)
                    transform.position = transform.parent.position;
            }
            else if (collision.name == "Character")
            {
                gameObject.SetActive(false);
                if (transform.parent != null)
                    transform.position = transform.parent.position;
            }

        }
        else
        {
            //내가 쏜 투사체일 경우
            if (collision.name == "BackGround")
            {
                gameObject.SetActive(false);
            }
            else if (collision.tag == "Normal" || collision.tag == "Elite")
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void Shoot(Transform targetTransform)
    {
        Logger.Log("Shoot");

        target = targetTransform; // 타겟 트랜스폼 저장
        StartCoroutine(TrackTarget());
    }

    IEnumerator TrackTarget()
    {
        float speed = 10f; // 임시 값
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        while (target != null)
        {
            if (target == null)
                yield break;
            Vector2 currentDirection = (target.position - transform.position).normalized;
            rb.linearVelocity = currentDirection * speed;

            float angle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
            yield return null;
        }

        // projectile이 타겟을 향해 가던 중 타겟이 사라지면(Destroy) 즉시 Deactive하도록 구현됨
        gameObject.SetActive(false);
    }















    public void SetPosition()
    {
        if (isEnemy)
        {
            gameObject.SetActive(false);
            if (transform.parent != null)
                transform.position = transform.parent.position;
        }
    }
}
