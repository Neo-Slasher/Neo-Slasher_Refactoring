using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* Cut Scene의 동작에 대해 기술
 * 컷씬은 크게 3개의 스토리를 보여주는 역할을 한다.
 * 1. 게임을 처음 실행하는 경우: 인트로 씬을 보여준다.
 *     이 때 플레이어의 difficulty는 0이다.
 * 2. 7일차 동안 원하는 돈을 얻지 못한 경우: 배드 엔딩을 보여준다.
 *     이 때 플레이어의 difficulty는 9이다.
 * 3. 7일차 동안 원하는 돈을 얻은 경우: 각 난이도에 해당하는 엔딩을 보여준다.
 *     이 때 플레이어의 difficulty는 1~8이다. (8인 경우 최종 엔딩)
 */

// 2025.06.02 Refactoring Final Version
// 밤씬 리펙토링 이후 Intro 이외의 다른 엔딩이 정상 작동하지 않는다면 다시 확인해볼 것
public class CutSceneManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI typingText;
    [SerializeField] private Image background;
    [SerializeField] private List<Sprite> backgrounds;

    private List<string> stories;
    private int currentStoryIndex;
    private Coroutine typingCoroutine;

    private const float TYPING_SPEED = 0.05f;
    private const float AUTO_NEXT_DELAY = 1.5f;

    private bool isTyping = false;
    private bool autoStory = false;
    private bool touchScreen = false;



    void Start()
    {
        SetBackground();
        LoadStory();
        StartStory();
    }

    public void OnClickSkipButton()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        LoadPreparationScene();
    }

    public void OnClickAutoButton()
    {
        autoStory = !autoStory;
        if (autoStory && !isTyping)
            touchScreen = true;
    }

    public void OnClickScreen()
    {
        touchScreen = true;
    }

    private void SetBackground()
    {
        int difficulty = GameManager.Instance.player.difficulty;
        switch (difficulty)
        {
            case 0:
                background.sprite = backgrounds[1];
                break;
            case 9:
                background.sprite = backgrounds[9];
                break;
            default:
                background.sprite = backgrounds[8];
                break;
        }
    }

    private void LoadStory()
    {
        int difficulty = GameManager.Instance.player.difficulty;

        if (DataManager.Instance?.storyList?.stories == null)
        {
            Logger.LogError("[컷씬] 스토리 데이터가 존재하지 않습니다!");
            LoadPreparationScene();
            return;
        }

        stories = DataManager.Instance.storyList.stories[difficulty].story;
    }

    private void StartStory()
    {
        currentStoryIndex = 0;
        ShowNextStory();
    }

    private void ShowNextStory()
    {
        if (currentStoryIndex >= stories.Count)
        {
            LoadPreparationScene();
            return;
        }

        // 인트로(difficulty=0)인 경우에만 배경 변경
        if (GameManager.Instance.player.difficulty == 0 && currentStoryIndex < backgrounds.Count)
            background.sprite = backgrounds[currentStoryIndex];


        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(stories[currentStoryIndex]));
    }

    private IEnumerator TypeText(string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            Logger.Log("[컷씬] 타이핑 오류: 스토리 내용이 비어 있습니다.");
            yield break;
        }

        isTyping = true;
        touchScreen = false; 
        typingText.text = "";

        for (int i = 0; i < message.Length; i++)
        {
            typingText.text = message[..(i + 1)];
            if (touchScreen)
            {
                typingText.text = message;
                touchScreen = false;
                break;
            }
            yield return new WaitForSeconds(TYPING_SPEED);
        }

        isTyping = false;

        if (autoStory)
            yield return new WaitForSeconds(AUTO_NEXT_DELAY);
        else
            yield return new WaitUntil(() => touchScreen);

        currentStoryIndex++;
        ShowNextStory();
    }

    private void LoadPreparationScene()
    {
        Logger.Log("[컷씬] 준비 화면으로 이동합니다.");
        GameManager.Instance.player.difficulty = 1;
        Player.Save(GameManager.Instance.player);
        SceneManager.LoadScene("PreparationScene");
    }
}

