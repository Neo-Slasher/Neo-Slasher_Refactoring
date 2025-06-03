using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 2025.06.03 Refactoring Final Version
// Preparation Manager를 수정할 때 씬에서의 연결이 끊어지지 않도록 주의할 것
// Tolelom: 인스펙터와 관련된 코드가 많아 리펙토링에서 어느 정도 합의를 봄
public class PreparationManager : MonoBehaviour
{
    [Header("Trait")]
    [SerializeField] private PreparationTraitManager traitManager;
    [SerializeField] private TextMeshProUGUI traitName;
    [SerializeField] private TextMeshProUGUI traitLv;
    [SerializeField] private TextMeshProUGUI traitScript;
    [SerializeField] private Image traitImage;

    [Header("Player Stats")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI expText;
    [SerializeField] private TextMeshProUGUI maxHpText;
    [SerializeField] private TextMeshProUGUI addMaxHpText;
    [SerializeField] private TextMeshProUGUI moveSpeedText;
    [SerializeField] private TextMeshProUGUI addMoveSpeedText;
    [SerializeField] private TextMeshProUGUI attackPowerText;
    [SerializeField] private TextMeshProUGUI addAttackPowerText;
    [SerializeField] private TextMeshProUGUI attackSpeedText;
    [SerializeField] private TextMeshProUGUI addAttackSpeedText;
    [SerializeField] private TextMeshProUGUI attackRangeText;
    [SerializeField] private TextMeshProUGUI addAttackRangeText;
    [SerializeField] private TextMeshProUGUI startMoneyText;
    [SerializeField] private TextMeshProUGUI getMoneyText;

    [Header("Difficulty")]
    [SerializeField] private Button difficultyLeftButton;
    [SerializeField] private Button difficultyRightButton;
    [SerializeField] private TextMeshProUGUI difficultyLevelText;
    [SerializeField] private TextMeshProUGUI recommendLvText;
    [SerializeField] private TextMeshProUGUI rewardExpText;
    [SerializeField] private TextMeshProUGUI goalMoneyText;
    [SerializeField] private TextMeshProUGUI enemyStatusText;
    [SerializeField] private TextMeshProUGUI normalEnhanceText;
    [SerializeField] private TextMeshProUGUI eliteEnhanceText;
    [SerializeField] private TextMeshProUGUI dropRankText;
    [SerializeField] private TextMeshProUGUI enemyRespawnText;

    [Header("Etc")]
    [SerializeField] private Button startButton;
    [SerializeField] private GameObject[] traitBoards;
    [SerializeField] private Image[] levelSelects;
    [SerializeField] private Sprite[] levelBackgrounds;
    [SerializeField] private Sprite[] levelBackgroundSelects;
    [SerializeField] private Button[] traitButtons;
    [SerializeField] private Sprite[] traitImages;

    private const int MIN_DIFFICULTY = 1;
    private const int MAX_DIFFICULTY = 8;

    private static readonly (int start, int end)[] TraitLevelRanges = new (int, int)[]
    {
        (1, 2),    // Level 1
        (3, 5),    // Level 2
        (6, 8),    // Level 3
        (9, 10),   // Level 4
        (11, 13),  // Level 5
        (14, 15),  // Level 6
        (16, 19),  // Level 7
        (20, 22),  // Level 8
        (23, 25),  // Level 9
        (26, 29),  // Level 10
        (30, 32),  // Level 11
        (33, 35),  // Level 12
        (36, 38),  // Level 13
        (39, 41),  // Level 14
        (42, 45),  // Level 15
        (46, 48),  // Level 16
        (49, 52),  // Level 17
        (53, 55),  // Level 18
        (56, 58),  // Level 19
        (59, 62)   // Level 20
    };

    void Start()
    {
        LoadStat();
        LoadStatDifferenceByTrait();

        LoadDifficulty();
        UpdateDifficultyButton();

        LoadTraitButtonImage();
        LoadSelectedTraitUI(1);
        DeactivateTraitButton();
        SetTraitBoard();

        CheckStartButton();
    }

    private void LoadStat()
    {
        Player player = GameManager.Instance.player;

        levelText.text = player.level.ToString();
        expText.text = $"Exp {player.curExp}/{player.reqExp}";
        maxHpText.text = player.maxHp.ToString("N0");
        moveSpeedText.text = player.moveSpeed.ToString();
        attackPowerText.text = player.attackPower.ToString("F0");
        attackSpeedText.text = player.attackSpeed.ToString("F0");
        attackRangeText.text = player.attackRange.ToString("F0");
        startMoneyText.text = player.money.ToString("N0");
    }

    private void LoadStatDifferenceByTrait()
    {
        Player player = GameManager.Instance.player;

        addMaxHpText.text = FormatSignedValue(traitManager.HpByTrait);
        addMoveSpeedText.text = FormatSignedValue(traitManager.MoveSpeedByTrait);
        addAttackPowerText.text = FormatSignedValue(traitManager.AttackPowerByTrait);
        addAttackSpeedText.text = FormatSignedValue(traitManager.AttackSpeedByTrait);
        addAttackRangeText.text = FormatSignedValue(traitManager.AttackRangeByTrait);
        startMoneyText.text = player.startMoney.ToString("N0");
        getMoneyText.text = player.earnMoney.ToString("N0");
    }

    private string FormatSignedValue(double value)
    {
        string sign = value > 0 ? "+" : "";
        return sign + value.ToString("F0");
    }

    private void LoadDifficulty()
    {
        int difficulty = GameManager.Instance.player.difficulty;
        var difficultyData = DataManager.Instance.difficultyList.difficulty[difficulty - 1];

        difficultyLevelText.text = difficulty.ToString();
        recommendLvText.text = $"권장 Lv.{difficultyData.recommandLv}";
        rewardExpText.text = $"보상 EXP {difficultyData.rewardExp:N0}";
        goalMoneyText.text = $"- 목표금액 {difficultyData.goalMoney:N0}";
        enemyStatusText.text = $"- 적 체력, 이동속도, 공격력 {difficultyData.enemyStatus * 100:F0}%";
        normalEnhanceText.text = $"- 일반 적이 {difficultyData.normalEnhance * 100:F0}% 확률로 강화";
        eliteEnhanceText.text = $"- 정예 적이 {difficultyData.eliteEnhance * 100:F0}% 확률로 강화";
        dropRankText.text = $"- 아이템 드롭률 {difficultyData.dropRank * 100:F0}%";
        enemyRespawnText.text = $"- 적 개체수 {difficultyData.enemyRespawn * 100:F0}%";
    }

    private void UpdateDifficultyButton()
    {
        int difficulty = GameManager.Instance.player.difficulty;
        difficultyLeftButton.interactable = (difficulty > MIN_DIFFICULTY);
        difficultyRightButton.interactable = (difficulty < MAX_DIFFICULTY);
    }

    public void OnClickDifficultyLeftButton()
    {
        GameManager.Instance.player.difficulty = Mathf.Clamp(
            GameManager.Instance.player.difficulty - 1,
            MIN_DIFFICULTY,
            MAX_DIFFICULTY
        );
        LoadDifficulty();
        UpdateDifficultyButton();
    }

    public void OnClickDifficultyRightButton()
    {
        GameManager.Instance.player.difficulty = Mathf.Clamp(
            GameManager.Instance.player.difficulty + 1,
            MIN_DIFFICULTY,
            MAX_DIFFICULTY
        );
        LoadDifficulty();
        UpdateDifficultyButton();
    }

    private void LoadTraitButtonImage()
    {
        for (int index = 1; index < traitButtons.Length; ++index)
        {
            int imageIndex = DataManager.Instance.traitList.trait[index - 1].imageIndex;

            TraitButton traitButton = traitButtons[index].GetComponent<TraitButton>();
            if (traitButton == null)
            {
                Logger.LogError($"TraitButton 컴포넌트를 찾을 수 없습니다. index: {index}");
                continue;
            }
            traitButton.SetTraitSprite(traitImages[imageIndex]);
        }
    }

    private void LoadSelectedTraitUI(int n)
    {
        Trait trait = DataManager.Instance.traitList.trait[n - 1];
        traitName.text = trait.name;
        traitLv.text = $"{trait.requireLv} / {(trait.rank == 0 ? "일반" : "핵심")}";
        traitScript.text = trait.script;
        traitImage.sprite = traitButtons[n].GetComponent<Image>().sprite; // 인덱스는 1부터 시작
        traitImage.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = traitButtons[n].transform.GetChild(1).gameObject.GetComponent<Image>().sprite;
    }

    private int GetMaxTraitCount()
    {
        int[] maxTraitCounts = {
            0,    // 인덱스 0 (사용 안 함)
            2,    // 레벨 1
            5,    // 레벨 2
            8,    // 레벨 3
            10,   // 레벨 4
            13,   // 레벨 5
            15,   // 레벨 6
            19,   // 레벨 7
            22,   // 레벨 8
            25,   // 레벨 9
            29,   // 레벨 10
            32,   // 레벨 11
            35,   // 레벨 12
            38,   // 레벨 13
            41,   // 레벨 14
            45,   // 레벨 15
            48,   // 레벨 16
            52,   // 레벨 17
            55,   // 레벨 18
            58,   // 레벨 19
            62    // 레벨 20
        };

        int level = GameManager.Instance.player.level;
        return (level >= 1 && level <= 20) ? maxTraitCounts[level] : 0;
    }

    // 본인 레벨 이하의 특성만 선택 가능하도록 버튼 off
    // 선택된 특성은 상호작용 X 및 selected 이미지 활성화
    private void DeactivateTraitButton()
    {
        int maxTraitButton = GetMaxTraitCount();
        int buttonCount = traitButtons.Length;
        int traitCount = GameManager.Instance.player.trait.Length;

        for (int i = 1; i < buttonCount && i < traitCount; ++i)
        {
            Button btn = traitButtons[i].GetComponent<Button>();
            Transform selected = traitButtons[i].transform.Find("Selected");

            if (i <= maxTraitButton)
            {
                if (GameManager.Instance.player.trait[i])
                {
                    btn.interactable = false;
                    if (selected != null)
                        selected.gameObject.SetActive(true);
                }
                else
                {
                    btn.interactable = true;
                    if (selected != null)
                        selected.gameObject.SetActive(false);
                }
            }

            else
            {
                btn.interactable = false;
                if (selected != null)
                    selected.gameObject.SetActive(false);
            }
        }
    }

    private void SetTraitBoard()
    {
        traitBoards[0].SetActive(true);
        traitBoards[1].SetActive(false);
        traitBoards[2].SetActive(false);
        traitBoards[3].SetActive(false);
    }

    public void OnClickTraitBoardButton(int n)
    {
        for (int i = 0; i < traitBoards.Length; ++i)
        {
            traitBoards[i].SetActive(n == i);
            levelSelects[i].sprite = (n == i) ? levelBackgroundSelects[i] : levelBackgrounds[i];
        }
    }

    public void OnClickStartButton()
    {
        Player.Save(GameManager.Instance.player);
        SceneManager.LoadScene("DayScene");
    }

    public void OnClickCancelButton()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void OnClickTraitButton()
    {
        GameObject button = EventSystem.current.currentSelectedGameObject;
        if (button == null)
        {
            Logger.LogError("No button selected.");
            return;
        }

        if (!int.TryParse(button.name, out int traitNumber))
        {
            Logger.LogError($"Invalid trait button name: {button.name}");
            return;
        }

        DisableTraitInSameLevel(traitNumber);
        traitManager.ActivateTrait(traitNumber);

        var selectedObj = button.transform.Find("Selected");
        if (selectedObj != null)
            selectedObj.gameObject.SetActive(true);
        else
            Logger.LogError($"'Selected' object not found in {button.name}");

        var btnComp = button.GetComponent<Button>();
        if (btnComp != null)
            btnComp.interactable = false;
        else
            Logger.LogError($"Button component not found in {button.name}");

        LoadSelectedTraitUI(traitNumber);
        LoadStat();
        LoadStatDifferenceByTrait();
        CheckStartButton();
    }

    public void CheckStartButton()
    {
        startButton.interactable = (GameManager.Instance.player.traitPoint == 0 && GameManager.Instance.player.difficulty != 8);
    }

    private void DisableTrait(int traitNumber)
    {
        traitManager.DeactivateTrait(traitNumber);
        traitButtons[traitNumber].interactable = true;
        traitButtons[traitNumber].transform.Find("Selected").gameObject.SetActive(false);
    }

    private void DisableTraitInSameLevel(int traitNumber)
    {
        foreach (var (start, end) in TraitLevelRanges)
        {
            if (traitNumber >= start && traitNumber <= end)
            {
                for (int i = start; i <= end; i++)
                {
                    if (GameManager.Instance.player.trait[i])
                        DisableTrait(i);
                }
                break;
            }
        }
    }
}