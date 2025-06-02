using UnityEngine;
using UnityEngine.UI;


// 2025.06.02 Refactoring Final Version
public class JoystickManager : MonoBehaviour
{
    public static JoystickManager Instance { get; private set; }

    private Slider joystickSlider;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (GameManager.Instance == null || GameManager.Instance.setting == null)
        {
            Logger.LogError("GameManager �ʱ�ȭ���� ����!");
            enabled = false; 
            return;
        }
    }

    public void RegisterJoystickSlider(Slider slider)
    {
        if (GameManager.Instance == null || GameManager.Instance.setting == null)
        {
            Logger.LogError("GameManager �ʱ�ȭ���� ����!");
            return;
        }

        if (joystickSlider != null)
            joystickSlider.onValueChanged.RemoveListener(OnJoystickSizeChanged);

        joystickSlider = slider;
        joystickSlider.onValueChanged.AddListener(OnJoystickSizeChanged);
        joystickSlider.value = GameManager.Instance.setting.joystickSize;
        OnJoystickSizeChanged(joystickSlider.value);
    }
    public void UnregisterJoystickSlider()
    {
        if (joystickSlider != null)
            joystickSlider.onValueChanged.RemoveListener(OnJoystickSizeChanged);
        joystickSlider = null;
    }

    private void OnJoystickSizeChanged(float value)
    {
        if (GameManager.Instance != null && GameManager.Instance.setting != null)
        {
            GameManager.Instance.setting.joystickSize = value;
        }
    }
}
