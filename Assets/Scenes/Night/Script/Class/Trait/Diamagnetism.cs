using UnityEngine;

public class Diamagnetism : MonoBehaviour
{
    // TODO: �ϵ� �ڵ��̶� ��ũ��Ʈ ���� �� �ݿ� �ȵ�
    float maxRange = 500f / 60f;
    float minRange = 400f / 60f;

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
