using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


// 2025.06.03 Refactoring Final Version
public class Category : MonoBehaviour
{
    [SerializeField] private GameObject[] scenes;
    [SerializeField] private TMP_Text sceneNameText;
    [SerializeField] private GameObject informationPopup;
    [SerializeField] private Image background;
    [SerializeField] private List<Sprite> backgrounds;

    

    private void Start() 
    {
        ChangeScene("상점");
    }

    // 낮 씬에서의 Footer에 있는 Toggle들을 눌렀을 때 창을 변경하는 기능
    public void ChangeScene(string targetSceneName)
    {
        SoundManager.Instance.PlaySFX(SoundManager.Instance.buttonClip);
        foreach (var scene in scenes)
        {
            scene.SetActive(false);
        }

        switch (targetSceneName)
        {
            case "상점":
                scenes[0].SetActive(true);
                informationPopup.SetActive(true);
                background.sprite = backgrounds[0];
                break;

            case "암살":
                scenes[1].SetActive(true);
                informationPopup.SetActive(false);
                background.sprite = backgrounds[1];
                break;
            case "정비":
                scenes[2].SetActive(true);
                informationPopup.SetActive(true);
                background.sprite = backgrounds[2];
                break;
        }
        sceneNameText.text = targetSceneName;
    }
}