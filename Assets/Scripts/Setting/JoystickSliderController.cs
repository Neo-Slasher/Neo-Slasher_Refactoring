using UnityEngine;
using UnityEngine.UI;

// 2025.06.02 Refactoring Final Version
public class JoystickSliderController : MonoBehaviour
{
    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        if (slider == null)
        {
            Logger.LogError("Slider 컴포넌트를 찾을 수 없습니다!");
            enabled = false;
        }
    }

    private void OnEnable()
    {
        if (JoystickManager.Instance != null)
            JoystickManager.Instance.RegisterJoystickSlider(slider);
    }

    private void OnDisable()
    {
        if (JoystickManager.Instance != null)
            JoystickManager.Instance.UnregisterJoystickSlider();
    }
}
