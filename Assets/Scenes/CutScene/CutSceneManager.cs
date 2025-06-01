using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class CutSceneManager : MonoBehaviour
{
    public TextMeshProUGUI typing_text;
    public List<string> stories;
    public Image background;
    public List<Sprite> backgrounds;

    private int story_number;
    private float story_speed = 0.05f;
    // 아래 두 변수는 디버깅을 위해 public으로 변경할 수 있음
    private bool auto_story = false;
    private bool touch_screen = false;


    void Start()
    {
        stories = DataManager.Instance.storyList.stories[GameManager.Instance.player.difficulty + 1].story;

        Debug.Log("start cut씬");
        Debug.Log(stories);

        if (GameManager.Instance.player.difficulty == 0) // intro
            background.sprite = backgrounds[1];
        else if (GameManager.Instance.player.difficulty == 9) // bad ending
            background.sprite = backgrounds[9];
        else
            background.sprite = backgrounds[8]; // good ending

        StartStory();
    }

    public void OnClickSkipButton()
    {
        ChangeScene();
    }
    public void OnClickAutoButton()
    {
        auto_story = !auto_story;
    }

    public void OnClickScreen()
    {
        touch_screen = true;
    }

    private void StartStory()
    {
        story_number = 0;
        NextStory();
    }
    private void NextStory()
    {
        Debug.Log("Next Story");
        Debug.Log(story_number);
        if (story_number == stories.Count)
        {
            ChangeScene();
            return;
        }
        if (GameManager.Instance.player.difficulty == 1) // 배경은 인트로인 경우에만 바꿈
        {
            background.sprite = backgrounds[story_number];
        }
        StartCoroutine(Typing(typing_text, stories[story_number], story_speed));
    }
    private void ChangeScene()
    {
        SceneManager.LoadScene("PreparationScene");
    }

    private IEnumerator Typing(TextMeshProUGUI typingText, string message, float speed)
    {
        if (string.IsNullOrEmpty(message))
        {
            Debug.Log("Typing Function Error, story is empty");
            yield break;
        }

        for (int i = 0; i < message.Length; i++)
        {
            typingText.text = message[..(i + 1)]; 
            yield return new WaitForSeconds(speed);
        }

        touch_screen = false;

        if (auto_story)
            yield return new WaitForSeconds(1.5f);
        else
            yield return new WaitUntil(() => touch_screen);

        story_number++;
        NextStory();
    }

}

