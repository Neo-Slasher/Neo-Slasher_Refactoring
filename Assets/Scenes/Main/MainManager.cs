using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// 2025.06.02 Refactoring Final Version
public class MainManager : MonoBehaviour
{
    [SerializeField] private GameObject startPopup;
    [SerializeField] private GameObject exitPopup;
    [SerializeField] private GameObject settingPopup;
    [SerializeField] private Button continueButton;

    private const string CUT_SCENE_NAME = "CutScene";
    private const string DAY_SCENE_NAME = "DayScene";


    void Start()
    {
        if (startPopup == null || exitPopup == null || settingPopup == null || continueButton == null)
        {
            Logger.LogError("UI 오브젝트가 할당되지 않았습니다!");
            enabled = false;
            return;
        }

        if (GameManager.Instance == null)
        {
            Logger.LogError("GameManager 인스턴스가 존재하지 않습니다!");
            return;
        }

        if (!GameManager.Instance.has_save_data)
        {
            continueButton.interactable = false;
        }

        startPopup.SetActive(false);
        exitPopup.SetActive(false);
        settingPopup.SetActive(false);
    }

    public void OnClickStartButton()
    {
        if (GameManager.Instance.has_save_data)
        {
            startPopup.SetActive(true);
        }
        else
        {
            StartNewGame();
        }
    }
    private void StartNewGame()
    {
        GameManager.Instance.player = Player.SoftReset(GameManager.Instance.player);
        SceneManager.LoadScene(CUT_SCENE_NAME);
    }

    public void OnClickStartPopupYesButton()
    {
        StartNewGame();
    }

    public void OnClickStartPopupNoButton()
    {
        startPopup.SetActive(false);
    }

    public void OnClickContinueButton()
    {
        SceneManager.LoadScene(DAY_SCENE_NAME);
    }

    public void OnClickSettingButton()
    {
        settingPopup.SetActive(true);
    }

    public void OnClickSettingExitButton()
    {
        settingPopup.SetActive(false);
        Setting.Save(GameManager.Instance.setting);
    }

    public void OnClickExitButton()
    {
        exitPopup.SetActive(true);
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
        exitPopup.SetActive(false);
    }
}
