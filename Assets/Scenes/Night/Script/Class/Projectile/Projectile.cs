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
            //���� �� ����ü�� ���
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
            //���� �� ����ü�� ���
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

        target = targetTransform; // Ÿ�� Ʈ������ ����
        StartCoroutine(TrackTarget());
    }

    IEnumerator TrackTarget()
    {
        float speed = 10f; // �ӽ� ��
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

        // projectile�� Ÿ���� ���� ���� �� Ÿ���� �������(Destroy) ��� Deactive�ϵ��� ������
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
