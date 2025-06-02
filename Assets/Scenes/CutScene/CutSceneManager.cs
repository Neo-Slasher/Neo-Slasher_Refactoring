using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* Cut Scene�� ���ۿ� ���� ���
 * �ƾ��� ũ�� 3���� ���丮�� �����ִ� ������ �Ѵ�.
 * 1. ������ ó�� �����ϴ� ���: ��Ʈ�� ���� �����ش�.
 *     �� �� �÷��̾��� difficulty�� 0�̴�.
 * 2. 7���� ���� ���ϴ� ���� ���� ���� ���: ��� ������ �����ش�.
 *     �� �� �÷��̾��� difficulty�� 9�̴�.
 * 3. 7���� ���� ���ϴ� ���� ���� ���: �� ���̵��� �ش��ϴ� ������ �����ش�.
 *     �� �� �÷��̾��� difficulty�� 1~8�̴�. (8�� ��� ���� ����)
 */

// 2025.06.02 Refactoring Final Version
// ��� �����丵 ���� Intro �̿��� �ٸ� ������ ���� �۵����� �ʴ´ٸ� �ٽ� Ȯ���غ� ��
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
            Logger.LogError("[�ƾ�] ���丮 �����Ͱ� �������� �ʽ��ϴ�!");
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

        // ��Ʈ��(difficulty=0)�� ��쿡�� ��� ����
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
            Logger.Log("[�ƾ�] Ÿ���� ����: ���丮 ������ ��� �ֽ��ϴ�.");
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
        Logger.Log("[�ƾ�] �غ� ȭ������ �̵��մϴ�.");
        GameManager.Instance.player.difficulty = 1;
        Player.Save(GameManager.Instance.player);
        SceneManager.LoadScene("PreparationScene");
    }
}

