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
        bgmMixer.SetFloat("BGM", MapVolumeToDecibel(GameManager.instance.setting.bgm_volume));
        sfxMixer.SetFloat("SFX", MapVolumeToDecibel(GameManager.instance.setting.sfx_volume));
        BGMSlider.value = GameManager.instance.setting.bgm_volume;
        SFXSlider.value = GameManager.instance.setting.sfx_volume;
        JoyStickSlider.value = GameManager.instance.setting.joy_stick_size;
    }

    public void ChangeBgmVolume(float value) {
        bgmMixer.SetFloat("BGM", MapVolumeToDecibel(value));
        GameManager.instance.setting.bgm_volume = value;
        Setting.Save(GameManager.instance.setting);
    }

    public void ChangeSfxVolume(float value) {
        sfxMixer.SetFloat("SFX", MapVolumeToDecibel(value));
        GameManager.instance.setting.sfx_volume = value;
        Setting.Save(GameManager.instance.setting);
    }

    public void ChangeJoyStickSize(float value)
    {
        GameManager.instance.setting.joy_stick_size = value;
        Setting.Save(GameManager.instance.setting);
    }

    float MapVolumeToDecibel(float normalizedValue) {
        float minDecibel = -80f;
        float maxDecibel = 0f;
        return minDecibel + normalizedValue * (maxDecibel - minDecibel);
    }
}
