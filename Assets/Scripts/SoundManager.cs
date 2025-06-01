using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;


// Sound Manager이지만 Joystick 관련 업무도 처리 중
public class SoundManager : MonoBehaviour
{
    public AudioMixer bgmMixer;
    public AudioMixer sfxMixer;
    public Slider BGMSlider;
    public Slider SFXSlider;
    public Slider JoyStickSlider;

    void Start() {
        bgmMixer.SetFloat("BGM", MapVolumeToDecibel(GameManager.Instance.setting.bgm_volume));
        sfxMixer.SetFloat("SFX", MapVolumeToDecibel(GameManager.Instance.setting.sfx_volume));
        BGMSlider.value = GameManager.Instance.setting.bgm_volume;
        SFXSlider.value = GameManager.Instance.setting.sfx_volume;
        JoyStickSlider.value = GameManager.Instance.setting.joy_stick_size;
    }

    public void ChangeBgmVolume(float value) {
        bgmMixer.SetFloat("BGM", MapVolumeToDecibel(value));
        GameManager.Instance.setting.bgm_volume = value;
        Setting.Save(GameManager.Instance.setting);
    }

    public void ChangeSfxVolume(float value) {
        sfxMixer.SetFloat("SFX", MapVolumeToDecibel(value));
        GameManager.Instance.setting.sfx_volume = value;
        Setting.Save(GameManager.Instance.setting);
    }

    public void ChangeJoyStickSize(float value)
    {
        GameManager.Instance.setting.joy_stick_size = value;
        Setting.Save(GameManager.Instance.setting);
    }

    float MapVolumeToDecibel(float normalizedValue) {
        float minDecibel = -80f;
        float maxDecibel = 0f;
        return minDecibel + normalizedValue * (maxDecibel - minDecibel);
    }
}
