using UnityEngine;
using UnityEngine.UI;

// 2025.06.02 Refactoring Final Version
public class BgmSliderController : MonoBehaviour
{
    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        if (slider == null)
        {
            Logger.LogError("Slider ������Ʈ�� ã�� �� �����ϴ�!");
            enabled = false;
        }
    }

    private void OnEnable()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.RegisterBgmSlider(slider);
    }

    private void OnDisable()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.UnregisterBgmSlider();
    }
}

