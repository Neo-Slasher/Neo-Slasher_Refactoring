using UnityEngine;

public class Magnetism : MonoBehaviour
{
    // TODO: �ϵ� �ڵ��̶� ��ũ��Ʈ ���� �� �ݿ� �ȵ�
    float maxRange = 10f;
    float minRange = 5f;

    private void OnDrawGizmos()
    {
        // �ּ� �ݰ� (�����)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, minRange);

        // �ִ� �ݰ� (������)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxRange);
    }
}
