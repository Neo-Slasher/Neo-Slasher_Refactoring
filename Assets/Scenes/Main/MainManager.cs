using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public GameObject start_popup;
    public GameObject exit_popup;
    public GameObject setting_popup;
    public Button continue_button;


    void Start()
    {
        if (!GameManager.instance.has_save_data)
        {
            continue_button.interactable = false;
        }

        start_popup.SetActive(false);
        exit_popup.SetActive(false);
        setting_popup.SetActive(false);
    }

    public void OnClickStartButton()
    {
        if (GameManager.instance.has_save_data == true)
        {
            start_popup.SetActive(true);
        }
        else
        {
            StartNewGame();
        }
    }
    private void StartNewGame()
    {
        GameManager.instance.player = Player.SoftReset(GameManager.instance.player);
        SceneManager.LoadScene("CutScene");
    }

    public void OnClickStartPopupYesButton()
    {
        StartNewGame();
    }

    public void OnClickStartPopupNoButton()
    {
        start_popup.SetActive(false);
    }

    public void OnClickContinueButton()
    {
        SceneManager.LoadScene("DayScene");
    }

    public void OnClickSettingButton()
    {
        setting_popup.SetActive(true);
    }

    public void OnClickSettingExitButton()
    {
        setting_popup.SetActive(false);
    }

    public void OnClickExitButton()
    {
        exit_popup.SetActive(true);
    }

    public void OnClickExitPopupYesButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public void OnClickExitPopupNoButton()
    {
        exit_popup.SetActive(false);
    }
}
