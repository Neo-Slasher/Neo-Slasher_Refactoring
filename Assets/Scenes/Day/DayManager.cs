using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DayManager : MonoBehaviour
{
    public GameObject setting;
    public TMP_Text day;

    public AudioClip dayBGM;

    public void Start()
    {
        PrintDay();
        SoundManager.Instance.PlayBGM(dayBGM);
    }

    public void OnClickSettingButton()
    {
        SoundManager.Instance.PlaySFX(SoundManager.Instance.buttonClip);
        setting.SetActive(true);
    }

    public void OnClickSettingContinueButton()
    {
        SoundManager.Instance.PlaySFX(SoundManager.Instance.buttonClip);
        setting.SetActive(false);
    }

    public void OnClickSettingExitButton()
    {
        SoundManager.Instance.PlaySFX(SoundManager.Instance.buttonClip);
        Player.Save(GameManager.Instance.player);
        SceneManager.LoadScene("MainScene");
    }

    public void PrintDay()
    {
        int current_day = GameManager.Instance.player.day;
        day.text = "D-" + (7 - current_day).ToString() + "\n" + current_day.ToString() + "ÀÏÂ÷";
    }
}
