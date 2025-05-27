using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DayManager : MonoBehaviour
{
    public GameObject setting;
    public TMP_Text day;

    public void Start()
    {
        PrintDay();
    }

    public void OnClickSettingButton() {
        setting.SetActive(true);
    }

    public void OnClickSettingContinueButton() {
        setting.SetActive(false);
    }

    public void OnClickSettingExitButton() {
        Player.Save(GameManager.instance.player);
        SceneManager.LoadScene("MainScene");
    }

    public void PrintDay()
    {
        int current_day = GameManager.instance.player.day;
        day.text = "D-" + (7 - current_day).ToString() + "\n" + current_day.ToString() + "ÀÏÂ÷";
    }
}
