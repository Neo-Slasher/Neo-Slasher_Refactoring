using UnityEngine;

// 2025.06.06 Refactoring Final Version
public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Character character;

    [Header("Map Boundaries")]
    [SerializeField] private Vector2 center;
    [SerializeField] private Vector2 mapSize;

    [Header("Smooth Movement")]
    [SerializeField] private float smoothTime = 0.3f;

    private Camera mainCamera; 
    private float height;
    private float width;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        mainCamera = Camera.main; // �߰�
        height = mainCamera.orthographicSize;
        width = height * Screen.width / Screen.height;

        if (mapSize.x < width || mapSize.y < height)
        {
            Logger.LogWarning("�� ũ�Ⱑ ī�޶� ����Ʈ���� �۽��ϴ�!");
        }
    }

    void LateUpdate()
    {
        LimitCameraArea();
    }

    private void LimitCameraArea()
    {
        float lx = Mathf.Max(0, mapSize.x - width);
        float ly = Mathf.Max(0, mapSize.y - height);

        float clampX = Mathf.Clamp(playerTransform.position.x, -lx + center.x, lx + center.x);
        float clampY = Mathf.Clamp(playerTransform.position.y, -ly + center.y, ly + center.y);

        Vector3 targetPosition = new Vector3(clampX, clampY, -10f);
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        if (character != null)
            character.SetHpBarPosition();
    }
}
