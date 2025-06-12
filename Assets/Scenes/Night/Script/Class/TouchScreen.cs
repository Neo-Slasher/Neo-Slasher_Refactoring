using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TouchScreen : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] Character character;
    [SerializeField] GameObject joyStick;
    [SerializeField] GameObject handle;
    [SerializeField] RectTransform joyStickRectTransform;

    [SerializeField] Slider joyStickSlider;
    [SerializeField] Transform joyStickTransform;

    Vector3 startPos;
    Vector3 moveVector;
    Vector3 joyStickSizeVector;

    [SerializeField]
    float joyStickRange = 10;

    void Start()
    {
        float startJoyStickSize = GameManager.Instance.setting.joystickSize;
        joyStickSizeVector = Vector3.one * startJoyStickSize * 2;
        joyStickTransform.localScale = joyStickSizeVector;

        joyStickRectTransform = handle.GetComponent<RectTransform>();
        moveVector = Vector3.zero;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!NightManager.Instance.isStageEnd)
        {
            startPos = eventData.position;
            joyStick.transform.position = startPos;
            joyStick.SetActive(true);
        }
        else
            joyStickRectTransform.anchoredPosition = Vector3.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {

        if (!NightManager.Instance.isStageEnd)
        {
            Vector3 currVector = eventData.position;

            Vector3 touchPos = eventData.position - (Vector2)startPos;

            Vector3 clampedPos = touchPos.magnitude < joyStickRange ?
                touchPos : touchPos.normalized * joyStickRange;


            joyStickRectTransform.anchoredPosition = clampedPos;

            moveVector = (currVector - startPos).normalized;

            character.Movement.StartMove((Vector2)moveVector);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!NightManager.Instance.isStageEnd)
        {
            joyStick.SetActive(false);
            character.Movement.EndMove();
            startPos = Vector3.zero;
            moveVector = Vector3.zero;
        }
        else
            joyStickRectTransform.anchoredPosition = Vector3.zero;
    }

    public void SetJoyStickSize()
    {
        joyStickSizeVector = Vector3.one * joyStickSlider.value * 2;
        joyStickTransform.localScale = joyStickSizeVector;

        int joyStickSize = (int)(joyStickSlider.value * 100);

        GameManager.Instance.setting.joystickSize = joyStickSize;
        Player.Save(GameManager.Instance.player);
    }
}
