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
        float startJoyStickSize = GameManager.Instance.setting.joystickSize / 100;
        joyStickSizeVector = new Vector3(startJoyStickSize, startJoyStickSize, startJoyStickSize);
        joyStickTransform.localScale = joyStickSizeVector;

        joyStickRectTransform = handle.GetComponent<RectTransform>();
        moveVector = Vector3.zero;
    }

    private void InitializeJoystickSize()
    {

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

            character.StartMove(moveVector);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!NightManager.Instance.isStageEnd)
        {
            joyStick.SetActive(false);
            character.EndMove();
            startPos = Vector3.zero;
            moveVector = Vector3.zero;
        }
        else
            joyStickRectTransform.anchoredPosition = Vector3.zero;
    }

    public void SetJoyStickSize()
    {
        joyStickSizeVector = new Vector3(joyStickSlider.value, joyStickSlider.value, joyStickSlider.value);
        joyStickTransform.localScale = joyStickSizeVector;

        int joyStickSize = (int)(joyStickSlider.value * 100);

        GameManager.Instance.setting.joystickSize = joyStickSize;
        Player.Save(GameManager.Instance.player);
    }
}
