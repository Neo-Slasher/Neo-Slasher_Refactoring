using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;


// 낮 씬에서의 Footer에 있는 Toggle들을 control하는 기능
public class Category : MonoBehaviour
{
    public GameObject[] Scenes;
    public TMP_Text SName;
    public GameObject inform;
    public Image background;
    public List<Sprite> backgrounds;


    public void Start()
    {
        ChangeScene("상점");
    }

    public void ChangeScene(string sceneName)
    {
        if (sceneName == "상점")
        {
            Scenes[0].SetActive(true);
            Scenes[1].SetActive(false);
            Scenes[2].SetActive(false);
            inform.SetActive(true);
            background.sprite = backgrounds[0];
            SName.text = sceneName;
        }
        if (sceneName == "암살")
        {
            Scenes[0].SetActive(false);
            Scenes[1].SetActive(true);
            Scenes[2].SetActive(false);
            inform.SetActive(false);
            background.sprite = backgrounds[1];
            SName.text = sceneName;
        }
        if (sceneName == "정비")
        {
            Scenes[0].SetActive(false);
            Scenes[1].SetActive(false);
            Scenes[2].SetActive(true);
            inform.SetActive(true);
            background.sprite = backgrounds[2];
            SName.text = sceneName;
        }
    }
}
