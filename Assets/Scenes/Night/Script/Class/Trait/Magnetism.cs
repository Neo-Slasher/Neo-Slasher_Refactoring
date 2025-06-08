using UnityEngine;

public class Magnetism : MonoBehaviour
{
    // TODO: 하드 코딩이라 스크립트 수정 시 반영 안됨
    float maxRange = 10f;
    float minRange = 5f;

    private void OnDrawGizmos()
    {
        // 최소 반경 (노란색)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, minRange);

        // 최대 반경 (빨간색)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxRange);
    }
}
