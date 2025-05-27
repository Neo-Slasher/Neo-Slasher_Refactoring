using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    Transform playerTransform;
    [SerializeField]
    Character character;
    [SerializeField]
    Vector3 cameraPos;

    [SerializeField]
    Vector2 center;
    [SerializeField]
    Vector2 mapSize;

    float height;
    float width;

    private void Start()
    {
        height = Camera.main.orthographicSize;
        width = height * Screen.width / Screen.height;
    }

    private void FixedUpdate()
    {
        LimitCameraArea();
    }

    void LimitCameraArea()
    {
        float lx = mapSize.x - width;
        float clampX = Mathf.Clamp(playerTransform.position.x, -lx + center.x, lx + center.x);

        float ly = mapSize.y - height;
        float clampY = Mathf.Clamp(playerTransform.position.y, -ly + center.y, ly + center.y);

        this.transform.position = new Vector3(clampX, clampY, -10f);
        cameraPos= transform.position;

        if (playerTransform.position.x != clampX || playerTransform.position.y != clampY)
            character.SetHpBarPosition();
    }
}
