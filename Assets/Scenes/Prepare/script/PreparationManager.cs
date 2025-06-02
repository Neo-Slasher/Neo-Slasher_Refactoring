using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PreparationManager : MonoBehaviour
{
    public PreparationTraitManager traitManager;

    public TextMeshProUGUI levelText;
    public TextMeshProUGUI expText;
    public TextMeshProUGUI maxHpText;
    public TextMeshProUGUI addMaxHpText;
    public TextMeshProUGUI moveSpeedText;
    public TextMeshProUGUI addMoveSpeedText;
    public TextMeshProUGUI attackPowerText;
    public TextMeshProUGUI addAttackPowerText;
    public TextMeshProUGUI attackSpeedText;
    public TextMeshProUGUI addAttackSpeedText;
    public TextMeshProUGUI attackRangeText;
    public TextMeshProUGUI addAttackRangeText;
    public TextMeshProUGUI startMoneyText;
    public TextMeshProUGUI getMoneyText;

    public Button difficultyLeftButton;
    public Button difficultyRightButton;
    public TextMeshProUGUI difficultyLevelText;
    public TextMeshProUGUI recommandLvText;
    public TextMeshProUGUI rewardExpText;
    public TextMeshProUGUI goalMoneyText;
    public TextMeshProUGUI enemyStatusText;
    public TextMeshProUGUI normalEnhanceText;
    public TextMeshProUGUI eliteEnhanceText;
    public TextMeshProUGUI dropRankText;
    public TextMeshProUGUI enemyRespawnText;


    public TextMeshProUGUI traitName;
    public TextMeshProUGUI traitLv;
    public TextMeshProUGUI traitScript;


    public Image traitImage;

    public GameObject[] traitBoards;
    public Image[] levelSelects;
    public Sprite[] levelBackgrounds;
    public Sprite[] levelBackgroundSelects;
    public Button[] traitButtons;
    public Sprite[] traitImages;

    public Player addValueByTrait;
    public Button startButton;

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

    public void LoadStat()
    {
        levelText.text = GameManager.Instance.player.level.ToString();
        expText.text = $"Exp {GameManager.Instance.player.curExp}/{GameManager.Instance.player.reqExp}";
        maxHpText.text = GameManager.Instance.player.maxHp.ToString();
        moveSpeedText.text = GameManager.Instance.player.moveSpeed.ToString();
        attackPowerText.text = GameManager.Instance.player.attackPower.ToString();
        attackSpeedText.text = GameManager.Instance.player.attackSpeed.ToString();
        attackRangeText.text = GameManager.Instance.player.attackRange.ToString();
        startMoneyText.text = GameManager.Instance.player.money.ToString();
    }

    public void LoadStatDifferenceByTrait()
    {
        addMaxHpText.text = (traitManager.HpByTrait > 0 ? "+" : "") + traitManager.HpByTrait;
        addMoveSpeedText.text = (traitManager.MoveSpeedByTrait > 0 ? "+" : "") + traitManager.MoveSpeedByTrait;
        addAttackPowerText.text = (traitManager.AttackPowerByTrait > 0 ? "+" : "") + traitManager.AttackPowerByTrait;
        addAttackSpeedText.text = (traitManager.AttackSpeedByTrait > 0 ? "+" : "") + traitManager.AttackSpeedByTrait;
        addAttackRangeText.text = (traitManager.AttackRangeByTrait > 0 ? "+" : "") + traitManager.AttackRangeByTrait;
        startMoneyText.text = GameManager.Instance.player.startMoney.ToString();
        getMoneyText.text = GameManager.Instance.player.earnMoney.ToString();
    }

    private void LoadDifficulty()
    {
        int difficulty = GameManager.Instance.player.difficulty;
        difficultyLevelText.text = difficulty.ToString();
        recommandLvText.text = $"권장 Lv.{DataManager.Instance.difficultyList.difficulty[difficulty].recommandLv}";
        rewardExpText.text = $"보상 EXP {DataManager.Instance.difficultyList.difficulty[difficulty].rewardExp}";
        goalMoneyText.text = $"- 목표금액 {DataManager.Instance.difficultyList.difficulty[difficulty].goalMoney}";
        enemyStatusText.text = "- 적 체력, 이동속도, 공격력 " + (DataManager.Instance.difficultyList.difficulty[difficulty].enemyStatus * 100f).ToString() + "%";
        normalEnhanceText.text = "- 일반 적이 " + (DataManager.Instance.difficultyList.difficulty[difficulty].normalEnhance * 100f).ToString() + "% 확률로 강화";
        eliteEnhanceText.text = "- 정예 적이 " + (DataManager.Instance.difficultyList.difficulty[difficulty].eliteEnhance * 100f).ToString() + "% 확률로 강화";
        dropRankText.text = "- 아이템 드롭률 " + (DataManager.Instance.difficultyList.difficulty[difficulty].dropRank * 100f).ToString() + "%";
        enemyRespawnText.text = "- 적 개체수 " + (DataManager.Instance.difficultyList.difficulty[difficulty].enemyRespawn * 100f).ToString() + "%";
    }

    // TODO: Difficulty를 따로 관리하는 스크립트로 분리
    public void UpdateDifficultyButton()
    {
        difficultyLeftButton.interactable = (GameManager.Instance.player.difficulty == 1) ? false : true;
        difficultyRightButton.interactable = (GameManager.Instance.player.difficulty == 8) ? false : true;
    }

    public void OnClickDifficultyLeftButton()
    {
        if (GameManager.Instance.player.difficulty == 1)
        {
            return;
        }
        GameManager.Instance.player.difficulty -= 1;
        LoadDifficulty();
        UpdateDifficultyButton();
    }

    public void OnClickDifficultyRightButton()
    {
        if (GameManager.Instance.player.difficulty == 8)
        {
            return;
        }
        GameManager.Instance.player.difficulty += 1;
        LoadDifficulty();
        UpdateDifficultyButton();
    }


    private void LoadTraitButtonImage()
    {
        for (int index = 1; index < traitButtons.Length; ++index)
        {
            int imageIndex = DataManager.Instance.traitList.trait[index - 1].imageIndex;
            Sprite traitSprite = traitImages[imageIndex];

            traitButtons[index].GetComponent<TraitButton>().SetTraitSprite(traitSprite);
        }
    }

    // 처음 씬이 시작될 때 기본적으로 보여줄 특성을 세팅
    public void LoadSelectedTraitUI(int n)
    {
        Trait defaultTrait = DataManager.Instance.traitList.trait[n - 1];
        traitName.text = defaultTrait.name;
        traitLv.text = defaultTrait.requireLv.ToString() + " / " + (defaultTrait.rank == 0 ? "일반" : "핵심");
        traitScript.text = defaultTrait.script;
        traitImage.sprite = traitButtons[n].GetComponent<Image>().sprite; // 인덱스는 1부터 시작
        traitImage.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = traitButtons[n].transform.GetChild(1).gameObject.GetComponent<Image>().sprite;
    }

    private int GetMaxTraitCount()
    {
        int level = GameManager.Instance.player.level;
        if (level == 1) return 2;
        else if (level == 2) return 5;
        else if (level == 3) return 8;
        else if (level == 4) return 10;
        else if (level == 5) return 13;
        else if (level == 6) return 15;
        else if (level == 7) return 19;
        else if (level == 8) return 22;
        else if (level == 9) return 25;
        else if (level == 10) return 29;
        else if (level == 11) return 32;
        else if (level == 12) return 35;
        else if (level == 13) return 38;
        else if (level == 14) return 41;
        else if (level == 15) return 45;
        else if (level == 16) return 48;
        else if (level == 17) return 52;
        else if (level == 18) return 55;
        else if (level == 19) return 58;
        else if (level == 20) return 62;
        return 0;
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