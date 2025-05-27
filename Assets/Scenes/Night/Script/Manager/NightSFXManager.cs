using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum AudioClipName
{
    none,
        autoAttack = 1, button,
        centryBall, chargingReaper, multiSlash, railPiercer,
        firstAde, barrior, hologramTrick, moveBack, booster, interceptDrone
}

public class NightSFXManager : MonoBehaviour
{
    public AudioSource bgmPlayer;
    public AudioSource sfxPlayer;

    [SerializeField]
    Slider bgmSlider;
    [SerializeField]
    Slider sfxSlider;

    [SerializeField]
    AudioClip[] bgmArr;
    [SerializeField]
    AudioClip[] sfxArr;

    private void Start()
    {
        bgmPlayer.volume = GameManager.instance.setting.bgm_volume;
        bgmSlider.value = GameManager.instance.setting.bgm_volume;

        sfxPlayer.volume = GameManager.instance.setting.sfx_volume;
        sfxSlider.value = GameManager.instance.setting.sfx_volume;

        PlayNightBGM();
    }

    public void SetBGMPlayerVolume()
    {
        bgmPlayer.volume = bgmSlider.value;
        GameManager.instance.setting.bgm_volume = bgmSlider.value;
        Player.Save(GameManager.instance.player);
    }

    public void SetSFXPlayerVolume()
    {
        sfxPlayer.volume = sfxSlider.value;
        GameManager.instance.setting.sfx_volume = sfxSlider.value;
        Player.Save(GameManager.instance.player);
    }

    void PlayNightBGM()
    {
        switch(GameManager.instance.player.assassinationCount)
        {
            case 1:
                bgmPlayer.clip = bgmArr[0];
                bgmPlayer.Play();
                break;
            case 2:
                bgmPlayer.clip = bgmArr[1];
                bgmPlayer.Play();
                break;
            case 3:
                bgmPlayer.clip = bgmArr[2];
                bgmPlayer.Play();
                break;
            case 4:
                bgmPlayer.clip = bgmArr[3];
                bgmPlayer.Play();
                break;
        }
    }

    public void PlayAudioClip(AudioClipName getName)
    {
        sfxPlayer.PlayOneShot(sfxArr[(int)getName]);
    }
}
