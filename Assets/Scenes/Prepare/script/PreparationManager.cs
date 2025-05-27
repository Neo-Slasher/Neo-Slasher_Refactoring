using System.IO;
using System.Collections;
using System.Collections.Generic;
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
        levelText.text = GameManager.instance.player.level.ToString();
        expText.text = $"Exp {GameManager.instance.player.curExp}/{GameManager.instance.player.reqExp}";
        maxHpText.text = GameManager.instance.player.maxHp.ToString();
        moveSpeedText.text = GameManager.instance.player.moveSpeed.ToString();
        attackPowerText.text = GameManager.instance.player.attackPower.ToString();
        attackSpeedText.text = GameManager.instance.player.attackSpeed.ToString();
        attackRangeText.text = GameManager.instance.player.attackRange.ToString();
        startMoneyText.text = GameManager.instance.player.money.ToString();
    }

    public void LoadStatDifferenceByTrait()
    {
        addMaxHpText.text = (traitManager.hp_by_trait > 0 ? "+" : "") + traitManager.hp_by_trait;
        addMoveSpeedText.text = (traitManager.move_speed_by_trait > 0 ? "+" : "") + traitManager.move_speed_by_trait;
        addAttackPowerText.text = (traitManager.attack_power_by_trait > 0 ? "+" : "") + traitManager.attack_power_by_trait;
        addAttackSpeedText.text = (traitManager.attack_speed_by_trait > 0 ? "+" : "") + traitManager.attack_speed_by_trait;
        addAttackRangeText.text = (traitManager.attack_range_by_trait > 0 ? "+" : "") + traitManager.attack_range_by_trait;
        startMoneyText.text = GameManager.instance.player.startMoney.ToString();
        getMoneyText.text = GameManager.instance.player.earnMoney.ToString();
    }

    private void LoadDifficulty()
    {
        int difficulty = GameManager.instance.player.difficulty;
        difficultyLevelText.text = difficulty.ToString();
        recommandLvText.text = $"권장 Lv.{DataManager.instance.difficultyList.difficulty[difficulty].recommandLv}";
        rewardExpText.text = $"보상 EXP {DataManager.instance.difficultyList.difficulty[difficulty].rewardExp}";
        goalMoneyText.text = $"- 목표금액 {DataManager.instance.difficultyList.difficulty[difficulty].goalMoney}";
        enemyStatusText.text = "- 적 체력, 이동속도, 공격력 " + (DataManager.instance.difficultyList.difficulty[difficulty].enemyStatus * 100f).ToString() + "%";
        normalEnhanceText.text = "- 일반 적이 " + (DataManager.instance.difficultyList.difficulty[difficulty].normalEnhance * 100f).ToString() + "% 확률로 강화";
        eliteEnhanceText.text = "- 정예 적이 " + (DataManager.instance.difficultyList.difficulty[difficulty].eliteEnhance * 100f).ToString() + "% 확률로 강화";
        dropRankText.text = "- 아이템 드롭률 " + (DataManager.instance.difficultyList.difficulty[difficulty].dropRank * 100f).ToString() + "%";
        enemyRespawnText.text = "- 적 개체수 " + (DataManager.instance.difficultyList.difficulty[difficulty].enemyRespawn * 100f).ToString() + "%";
    }

    // TODO: Difficulty를 따로 관리하는 스크립트로 분리
    public void UpdateDifficultyButton()
    {
        difficultyLeftButton.interactable = (GameManager.instance.player.difficulty == 1) ? false : true;
        difficultyRightButton.interactable = (GameManager.instance.player.difficulty == 8) ? false : true;
    }

    public void OnClickDifficultyLeftButton()
    {
        if (GameManager.instance.player.difficulty == 1)
        {
            return;
        }
        GameManager.instance.player.difficulty -= 1;
        LoadDifficulty();
        UpdateDifficultyButton();
    }

    public void OnClickDifficultyRightButton()
    {
        if (GameManager.instance.player.difficulty == 8)
        {
            return;
        }
        GameManager.instance.player.difficulty += 1;
        LoadDifficulty();
        UpdateDifficultyButton();
    }


    private void LoadTraitButtonImage()
    {
        for (int index = 1; index < traitButtons.Length; ++index)
        {
            int imageIndex = DataManager.instance.traitList.trait[index - 1].imageIndex;
            Sprite traitSprite = traitImages[imageIndex];

            traitButtons[index].GetComponent<TraitButton>().SetTraitSprite(traitSprite);
        }
    }

    // 처음 씬이 시작될 때 기본적으로 보여줄 특성을 세팅
    public void LoadSelectedTraitUI(int n)
    {
        Trait defaultTrait = DataManager.instance.traitList.trait[n - 1];
        traitName.text = defaultTrait.name;
        traitLv.text = defaultTrait.requireLv.ToString() + " / " + (defaultTrait.rank == 0 ? "일반" : "핵심");
        traitScript.text = defaultTrait.script;
        traitImage.sprite = traitButtons[n].GetComponent<Image>().sprite; // 인덱스는 1부터 시작
        traitImage.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = traitButtons[n].transform.GetChild(1).gameObject.GetComponent<Image>().sprite;
    }

    private int GetMaxTraitCount()
    {
        int level = GameManager.instance.player.level;
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
            if (GameManager.instance.player.trait[i])
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
        Player.Save(GameManager.instance.player);
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
        traitManager.activeTrait(traitNumber);

        button.transform.GetChild(2).gameObject.SetActive(true); // selected object
        button.GetComponent<Button>().interactable = false;


        LoadSelectedTraitUI(traitNumber);

        LoadStat();
        LoadStatDifferenceByTrait();

        CheckStartButton();
    }

    public void CheckStartButton()
    {
        startButton.interactable = (GameManager.instance.player.traitPoint == 0 && GameManager.instance.player.difficulty != 8);
    }


    void DisableTrait(int traitNumber)
    {
        Debug.Log($"disable {traitNumber}");
        traitManager.unactiveTrait(traitNumber);
        traitButtons[traitNumber].interactable = true;
        traitButtons[traitNumber].transform.GetChild(2).gameObject.SetActive(false);
    }

    public void DisableTraitInSameLevel(int traitNumber)
    {
        switch (traitNumber)
        {
            case 1:
            case 2:
                if (GameManager.instance.player.trait[1])
                    DisableTrait(1);
                if (GameManager.instance.player.trait[2])
                    DisableTrait(2);
                break;
            case 3:
            case 4:
            case 5:
                if (GameManager.instance.player.trait[3])
                    DisableTrait(3);
                if (GameManager.instance.player.trait[4])
                    DisableTrait(4);
                if (GameManager.instance.player.trait[5])
                    DisableTrait(5);
                break;
            case 6:
            case 7:
            case 8:
                if (GameManager.instance.player.trait[6])
                    DisableTrait(6);
                if (GameManager.instance.player.trait[7])
                    DisableTrait(7);
                if (GameManager.instance.player.trait[8])
                    DisableTrait(8);
                break;
            case 9:
            case 10:
                if (GameManager.instance.player.trait[9])
                    DisableTrait(9);
                if (GameManager.instance.player.trait[10])
                    DisableTrait(10);
                break;
            case 11:
            case 12:
            case 13:
                if (GameManager.instance.player.trait[11])
                    DisableTrait(11);
                if (GameManager.instance.player.trait[12])
                    DisableTrait(12);
                if (GameManager.instance.player.trait[13])
                    DisableTrait(13);
                break;
            case 14:
            case 15:
                if (GameManager.instance.player.trait[14])
                    DisableTrait(14);
                if (GameManager.instance.player.trait[15])
                    DisableTrait(15);
                break;
            case 16:
            case 17:
            case 18:
            case 19:
                if (GameManager.instance.player.trait[16])
                    DisableTrait(16);
                if (GameManager.instance.player.trait[17])
                    DisableTrait(17);
                if (GameManager.instance.player.trait[18])
                    DisableTrait(18);
                if (GameManager.instance.player.trait[19])
                    DisableTrait(19);
                break;
            case 20:
            case 21:
            case 22:
                if (GameManager.instance.player.trait[20])
                    DisableTrait(20);
                if (GameManager.instance.player.trait[21])
                    DisableTrait(21);
                if (GameManager.instance.player.trait[22])
                    DisableTrait(22);
                break;
            case 23:
            case 24:
            case 25:
                if (GameManager.instance.player.trait[23])
                    DisableTrait(23);
                if (GameManager.instance.player.trait[24])
                    DisableTrait(24);
                if (GameManager.instance.player.trait[25])
                    DisableTrait(25);
                break;
            case 26:
            case 27:
            case 28:
            case 29:
                if (GameManager.instance.player.trait[26])
                    DisableTrait(26);
                if (GameManager.instance.player.trait[27])
                    DisableTrait(27);
                if (GameManager.instance.player.trait[28])
                    DisableTrait(28);
                if (GameManager.instance.player.trait[29])
                    DisableTrait(29);
                break;
            case 30:
            case 31:
            case 32:
                if (GameManager.instance.player.trait[30])
                    DisableTrait(30);
                if (GameManager.instance.player.trait[31])
                    DisableTrait(31);
                if (GameManager.instance.player.trait[32])
                    DisableTrait(32);
                break;
            case 33:
            case 34:
            case 35:
                if (GameManager.instance.player.trait[33])
                    DisableTrait(33);
                if (GameManager.instance.player.trait[34])
                    DisableTrait(34);
                if (GameManager.instance.player.trait[35])
                    DisableTrait(35);
                break;
            case 36:
            case 37:
            case 38:
                if (GameManager.instance.player.trait[36])
                    DisableTrait(36);
                if (GameManager.instance.player.trait[37])
                    DisableTrait(37);
                if (GameManager.instance.player.trait[38])
                    DisableTrait(38);
                break;
            case 39:
            case 40:
            case 41:
                if (GameManager.instance.player.trait[39])
                    DisableTrait(39);
                if (GameManager.instance.player.trait[40])
                    DisableTrait(40);
                if (GameManager.instance.player.trait[41])
                    DisableTrait(41);
                break;
            case 42:
            case 43:
            case 44:
            case 45:
                if (GameManager.instance.player.trait[42])
                    DisableTrait(42);
                if (GameManager.instance.player.trait[43])
                    DisableTrait(43);
                if (GameManager.instance.player.trait[44])
                    DisableTrait(44);
                if (GameManager.instance.player.trait[45])
                    DisableTrait(45);
                break;
            case 46:
            case 47:
            case 48:
                if (GameManager.instance.player.trait[46])
                    DisableTrait(46);
                if (GameManager.instance.player.trait[47])
                    DisableTrait(47);
                if (GameManager.instance.player.trait[48])
                    DisableTrait(48);
                break;
            case 49:
            case 50:
            case 51:
            case 52:
                if (GameManager.instance.player.trait[49])
                    DisableTrait(49);
                if (GameManager.instance.player.trait[50])
                    DisableTrait(50);
                if (GameManager.instance.player.trait[51])
                    DisableTrait(51);
                if (GameManager.instance.player.trait[52])
                    DisableTrait(52);
                break;
            case 53:
            case 54:
            case 55:
                if (GameManager.instance.player.trait[53])
                    DisableTrait(53);
                if (GameManager.instance.player.trait[54])
                    DisableTrait(54);
                if (GameManager.instance.player.trait[55])
                    DisableTrait(55);
                break;
            case 56:
            case 57:
            case 58:
                if (GameManager.instance.player.trait[56])
                    DisableTrait(56);
                if (GameManager.instance.player.trait[57])
                    DisableTrait(57);
                if (GameManager.instance.player.trait[58])
                    DisableTrait(58);
                break;
            case 59:
            case 60:
            case 61:
            case 62:
                if (GameManager.instance.player.trait[59])
                    DisableTrait(59);
                if (GameManager.instance.player.trait[60])
                    DisableTrait(60);
                if (GameManager.instance.player.trait[61])
                    DisableTrait(61);
                if (GameManager.instance.player.trait[62])
                    DisableTrait(62);
                break;

        }
    }
}