using UnityEngine;
using UnityEngine.UI;

// 2025.06.02 Refactoring Final Version
public class SfxSliderController : MonoBehaviour
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
        if (SoundManager.Instance != null)
            SoundManager.Instance.RegisterSfxSlider(slider);
    }

    private void OnDisable()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.UnregisterSfxSlider();
    }
}
