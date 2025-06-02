using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Preparation Manager를 수정할 때 씬에서의 연결이 끊어지지 않도록 주의할 것
// 인스펙터와 관련된 코드가 많아 리펙토링에서 어느 정도 합의를 봄
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

    void Start()
    {
        LoadStat();
        LoadStatDifferenceByTrait();

        LoadDifficulty();
        UpdateDifficultyButton();

        LoadTraitButtonImage();
        LoadSelectedTraitUI(1);
        UnactiveTraitButton();
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
    private void UnactiveTraitButton()
    {
        int maxTraitButton = GetMaxTraitCount();
        for (int i = 1; i <= maxTraitButton; ++i)
        {
            if (GameManager.Instance.player.trait[i])
            {
                traitButtons[i].GetComponent<Button>().interactable = false;
                Transform selected = traitButtons[i].transform.Find("Selected");
                selected.gameObject.SetActive(true);
            }
        }
        for (int i = maxTraitButton + 1; i <= 62; ++i)
        {
            traitButtons[i].GetComponent<Button>().interactable = false;
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
        for (int i = 0; i < 4; ++i)
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
        int traitNumber = int.Parse(button.name);

        DisableTraitInSameLevel(traitNumber);

        // activae trait
        traitManager.ActivateTrait(traitNumber);

        button.transform.GetChild(2).gameObject.SetActive(true); // selected object
        button.GetComponent<Button>().interactable = false;


        LoadSelectedTraitUI(traitNumber);

        LoadStat();
        LoadStatDifferenceByTrait();

        CheckStartButton();
    }

    public void CheckStartButton()
    {
        startButton.interactable = (GameManager.Instance.player.traitPoint == 0 && GameManager.Instance.player.difficulty != 8);
    }


    void DisableTrait(int traitNumber)
    {
        Debug.Log($"disable {traitNumber}");
        traitManager.DeactivateTrait(traitNumber);
        traitButtons[traitNumber].interactable = true;
        traitButtons[traitNumber].transform.GetChild(2).gameObject.SetActive(false);
    }

    public void DisableTraitInSameLevel(int traitNumber)
    {
        switch (traitNumber)
        {
            case 1:
            case 2:
                if (GameManager.Instance.player.trait[1])
                    DisableTrait(1);
                if (GameManager.Instance.player.trait[2])
                    DisableTrait(2);
                break;
            case 3:
            case 4:
            case 5:
                if (GameManager.Instance.player.trait[3])
                    DisableTrait(3);
                if (GameManager.Instance.player.trait[4])
                    DisableTrait(4);
                if (GameManager.Instance.player.trait[5])
                    DisableTrait(5);
                break;
            case 6:
            case 7:
            case 8:
                if (GameManager.Instance.player.trait[6])
                    DisableTrait(6);
                if (GameManager.Instance.player.trait[7])
                    DisableTrait(7);
                if (GameManager.Instance.player.trait[8])
                    DisableTrait(8);
                break;
            case 9:
            case 10:
                if (GameManager.Instance.player.trait[9])
                    DisableTrait(9);
                if (GameManager.Instance.player.trait[10])
                    DisableTrait(10);
                break;
            case 11:
            case 12:
            case 13:
                if (GameManager.Instance.player.trait[11])
                    DisableTrait(11);
                if (GameManager.Instance.player.trait[12])
                    DisableTrait(12);
                if (GameManager.Instance.player.trait[13])
                    DisableTrait(13);
                break;
            case 14:
            case 15:
                if (GameManager.Instance.player.trait[14])
                    DisableTrait(14);
                if (GameManager.Instance.player.trait[15])
                    DisableTrait(15);
                break;
            case 16:
            case 17:
            case 18:
            case 19:
                if (GameManager.Instance.player.trait[16])
                    DisableTrait(16);
                if (GameManager.Instance.player.trait[17])
                    DisableTrait(17);
                if (GameManager.Instance.player.trait[18])
                    DisableTrait(18);
                if (GameManager.Instance.player.trait[19])
                    DisableTrait(19);
                break;
            case 20:
            case 21:
            case 22:
                if (GameManager.Instance.player.trait[20])
                    DisableTrait(20);
                if (GameManager.Instance.player.trait[21])
                    DisableTrait(21);
                if (GameManager.Instance.player.trait[22])
                    DisableTrait(22);
                break;
            case 23:
            case 24:
            case 25:
                if (GameManager.Instance.player.trait[23])
                    DisableTrait(23);
                if (GameManager.Instance.player.trait[24])
                    DisableTrait(24);
                if (GameManager.Instance.player.trait[25])
                    DisableTrait(25);
                break;
            case 26:
            case 27:
            case 28:
            case 29:
                if (GameManager.Instance.player.trait[26])
                    DisableTrait(26);
                if (GameManager.Instance.player.trait[27])
                    DisableTrait(27);
                if (GameManager.Instance.player.trait[28])
                    DisableTrait(28);
                if (GameManager.Instance.player.trait[29])
                    DisableTrait(29);
                break;
            case 30:
            case 31:
            case 32:
                if (GameManager.Instance.player.trait[30])
                    DisableTrait(30);
                if (GameManager.Instance.player.trait[31])
                    DisableTrait(31);
                if (GameManager.Instance.player.trait[32])
                    DisableTrait(32);
                break;
            case 33:
            case 34:
            case 35:
                if (GameManager.Instance.player.trait[33])
                    DisableTrait(33);
                if (GameManager.Instance.player.trait[34])
                    DisableTrait(34);
                if (GameManager.Instance.player.trait[35])
                    DisableTrait(35);
                break;
            case 36:
            case 37:
            case 38:
                if (GameManager.Instance.player.trait[36])
                    DisableTrait(36);
                if (GameManager.Instance.player.trait[37])
                    DisableTrait(37);
                if (GameManager.Instance.player.trait[38])
                    DisableTrait(38);
                break;
            case 39:
            case 40:
            case 41:
                if (GameManager.Instance.player.trait[39])
                    DisableTrait(39);
                if (GameManager.Instance.player.trait[40])
                    DisableTrait(40);
                if (GameManager.Instance.player.trait[41])
                    DisableTrait(41);
                break;
            case 42:
            case 43:
            case 44:
            case 45:
                if (GameManager.Instance.player.trait[42])
                    DisableTrait(42);
                if (GameManager.Instance.player.trait[43])
                    DisableTrait(43);
                if (GameManager.Instance.player.trait[44])
                    DisableTrait(44);
                if (GameManager.Instance.player.trait[45])
                    DisableTrait(45);
                break;
            case 46:
            case 47:
            case 48:
                if (GameManager.Instance.player.trait[46])
                    DisableTrait(46);
                if (GameManager.Instance.player.trait[47])
                    DisableTrait(47);
                if (GameManager.Instance.player.trait[48])
                    DisableTrait(48);
                break;
            case 49:
            case 50:
            case 51:
            case 52:
                if (GameManager.Instance.player.trait[49])
                    DisableTrait(49);
                if (GameManager.Instance.player.trait[50])
                    DisableTrait(50);
                if (GameManager.Instance.player.trait[51])
                    DisableTrait(51);
                if (GameManager.Instance.player.trait[52])
                    DisableTrait(52);
                break;
            case 53:
            case 54:
            case 55:
                if (GameManager.Instance.player.trait[53])
                    DisableTrait(53);
                if (GameManager.Instance.player.trait[54])
                    DisableTrait(54);
                if (GameManager.Instance.player.trait[55])
                    DisableTrait(55);
                break;
            case 56:
            case 57:
            case 58:
                if (GameManager.Instance.player.trait[56])
                    DisableTrait(56);
                if (GameManager.Instance.player.trait[57])
                    DisableTrait(57);
                if (GameManager.Instance.player.trait[58])
                    DisableTrait(58);
                break;
            case 59:
            case 60:
            case 61:
            case 62:
                if (GameManager.Instance.player.trait[59])
                    DisableTrait(59);
                if (GameManager.Instance.player.trait[60])
                    DisableTrait(60);
                if (GameManager.Instance.player.trait[61])
                    DisableTrait(61);
                if (GameManager.Instance.player.trait[62])
                    DisableTrait(62);
                break;

        }
    }
}