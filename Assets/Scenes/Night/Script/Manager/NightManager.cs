using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NightManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] ItemManager itemManager;
    [SerializeField] TimerManager timerManager;
    [SerializeField] NightSFXManager nightSFXManager;


    [Header("UI")]
    [SerializeField] SpriteRenderer backGround;
    [SerializeField] GameObject endPopup;
    [SerializeField] GameObject itemChangePopup;
    [SerializeField] Transform GetThing; // itemChangePopup 내부
    //230904 추가된 팝업 (아이템을 획득했을 때 나타나는 창)
    [SerializeField] Image[] characterItemImageArr;
    //환경설정
    [SerializeField] Button settingButton;
    [SerializeField] GameObject setting;


    [Header("GameObjects")]
    [SerializeField] GameObject character;
    [SerializeField] private Transform enemies;


    [Header("Resources")]
    [SerializeField] Sprite[] backGroundSpriteArr;
    [SerializeField] GameObject[] normalEnemyPrefabs;
    [SerializeField] GameObject[] eliteEnemyPrefabs;


    [SerializeField] int selectItemIdx;

    private const int MAX_NORMAL_ENEMY_INDEX = 2;
    private const int MAX_ELITE_ENEMY_INDEX = 2;


    public bool isStageEnd = false; 
    public int killCount = 0;
    public int killNormal = 0;
    public int killElite = 0;




    private void Start()
    {
        SetBackGround();
        InstantiateEnemy();
    }

    void SetBackGround()
    {
        switch (GameManager.instance.player.assassinationCount)
        {
            case 0:
                backGround.sprite = backGroundSpriteArr[0];
                break;
            case 1:
                backGround.sprite = backGroundSpriteArr[1];
                break;
            case 2:
                backGround.sprite = backGroundSpriteArr[2];
                break;
            case 3:
                backGround.sprite = backGroundSpriteArr[3];
                break;
        }
    }


    void InstantiateEnemy()
    {
        for (int normalEnemyDataIndex = 0; normalEnemyDataIndex <= MAX_NORMAL_ENEMY_INDEX; normalEnemyDataIndex++)
        {
            StartCoroutine(InstantiateEnemyCoroutine(normalEnemyPrefabs, false, normalEnemyDataIndex));

        }
        for (int eliteEnemyDataIndex = 0; eliteEnemyDataIndex <= MAX_ELITE_ENEMY_INDEX; eliteEnemyDataIndex++)
        {
            StartCoroutine(InstantiateEnemyCoroutine(eliteEnemyPrefabs, true, eliteEnemyDataIndex));
        }
    }

    IEnumerator InstantiateEnemyCoroutine(GameObject[] prefabs, bool isElite, int index)
    {
        double spawnRate = GetSpawnRate(isElite, index);
        if (spawnRate == 0.0) yield break;

        float spawnTime = (float)(1 / spawnRate);
        yield return new WaitForSeconds(spawnTime);

        while (!isStageEnd && timerManager.timerCount > 1)
        {
            GameObject enemy = Instantiate(prefabs[index], SetEnemyPosition(), Quaternion.identity);
            enemy.GetComponent<Enemy>().SetEnforceData(GameManager.instance.player.level, false);
            enemy.transform.SetParent(enemies);
            yield return new WaitForSeconds(spawnTime);
        }
    }


    double GetSpawnRate(bool isElite, int getIndex)
    {
        int nowAssassination = GameManager.instance.player.assassinationCount;
        switch (getIndex)
        {
            case 0:
                if (isElite)
                    return DataManager.instance.assassinationStageList.assassinationStage[nowAssassination].elite1Spawn;
                else
                    return DataManager.instance.assassinationStageList.assassinationStage[nowAssassination].normal1Spawn;
            case 1:
                if (isElite)
                    return DataManager.instance.assassinationStageList.assassinationStage[nowAssassination].elite2Spawn;
                else
                    return DataManager.instance.assassinationStageList.assassinationStage[nowAssassination].normal2Spawn;
            case 2:
                if (isElite)
                    return DataManager.instance.assassinationStageList.assassinationStage[nowAssassination].elite3Spawn;
                else
                    return DataManager.instance.assassinationStageList.assassinationStage[nowAssassination].normal3Spawn;
            default:
                return -1;
        }
    }

    Vector3 SetEnemyPosition()
    {
        const float MIN_DISTANCE = 10; // 플레이어와 적이 스폰될 때 최소 거리

        Vector3 instantiatePos = new Vector3(Random.Range(-15f, 15f), Random.Range(-30f, 30f), 0);
        while (Vector3.Distance(instantiatePos, character.transform.position) < MIN_DISTANCE)
        {
            // tolelom: 아래 두 범위 중 무엇이 옳은 지는 아직 모름
            //instantiatePos = new Vector3(Random.Range(-10f, 10f), Random.Range(-20f, 20f), 0);
            instantiatePos = new Vector3(Random.Range(-15f, 15f), Random.Range(-30f, 30f), 0);
        }

        return instantiatePos;
    }


    // 게임 종료 함수
    // 플레이어 체력이 0, 시간이 0일 때 발동되는 함수
    public void SetStageEnd()
    {
        isStageEnd = true;

        //플레이어 공격 정지
        Character characterComp = character.GetComponent<Character>();
        characterComp.StopAttack();
        characterComp.EndMove();

        //조이스틱 사용은 nightManager.isStageEnd를 매번 받기 때문에 알아서 꺼짐

        // 적 삭제 (안전하게 복사 후 삭제)
        Transform[] enemyList = new Transform[enemies.childCount];

        for (int i = 0; i < enemyList.Length; i++)
            enemyList[i] = enemies.GetChild(i);

        foreach (Transform child in enemies)
        {
            if (child != null)
                Destroy(child.gameObject);
        }


        if (!endPopup.activeSelf)
        {
            SetEndPopUp();
            endPopup.SetActive(true);
        }
    }








    //팝업창 데이터 설정
    void SetEndPopUp()
    {
        Transform enemytTexts = endPopup.transform.Find("PopupBoard").Find("EnemyTexts");
        Transform UIs = endPopup.transform.Find("PopupBoard").Find("UIs");

        Player player = GameManager.instance.player;

        TMP_Text aliveOrDie = UIs.Find("AliveOrDieText").GetComponent<TMP_Text>();
        aliveOrDie.text = (timerManager.timerCount == 0) ? "생존" : "사망";

        TMP_Text day = UIs.Find("DayText").GetComponent<TMP_Text>();
        day.text = player.day + "일차 D-" + (7 - player.day);

        TMP_Text normalCount = enemytTexts.Find("NormalKill").Find("NormalKillCount").GetComponent<TMP_Text>();
        TMP_Text eliteCount = enemytTexts.Find("EliteKill").Find("EliteKillCount").GetComponent<TMP_Text>();

        normalCount.text = killNormal.ToString();
        eliteCount.text = killElite.ToString();

        CalculateItemReward();
        CalculateMoneyReward();

        character.GetComponent<Character>().NightEnd();

        // 아이템이 마음에 들지 않아 게임 강제 종료 가능
        // SavePlayerData의 위치가 옳지 않을 수 있음
        // 실질적인 day가 증가하지 않기 때문에 여러 번 강제 종료해서 돈, 아이템 수급 가능
        Player.Save(player);
    }

    //밤이 끝나고 아이템 획득하는 함수
    void CalculateItemReward()
    {
        Player player = GameManager.instance.player;

        // (처치한 일반 몬스터 수 × 해당 단계에서의 일반 드랍 확률) +
        // (처치한 정예 몬스터 수 × 해당 단계에서의 정예 드랍 확률) = (장비 및 아이템을 획득할 확률)
        int difficulty = player.difficulty;
        int assassination = player.assassinationCount;

        // Tolelom: 이 식도 맞는 지 확인해볼것
        int rank = player.dropRank + DataManager.instance.difficultyList.difficulty[difficulty].dropRank
                        + DataManager.instance.assassinationStageList.assassinationStage[assassination].stageDropRank;

        int itemRank = SetItemRank(rank);

        var stageData = DataManager.instance.assassinationStageList.assassinationStage[assassination];
        double normalRate = stageData.normalDropRate * killNormal;
        double eliteRate = stageData.eliteDropRate * killElite;

        float rate = (float)(normalRate + eliteRate);

        float nowProb = Random.value;

        const int ITEMS_PER_RANK = 15;
        const int MAX_ITEM_INDEX = 14;

        Transform dropItemUI = endPopup.transform.Find("PopupBoard").Find("DropItemUI");
        Image itemImage = dropItemUI.Find("DropItemImage").GetComponent<Image>();
        TMP_Text itemName = dropItemUI.Find("DropItemNameText").GetComponent<TMP_Text>();

        if (nowProb < rate) // 아이템 획득
        {
            int getItemIdx = Random.Range(itemRank * ITEMS_PER_RANK, itemRank * ITEMS_PER_RANK + MAX_ITEM_INDEX);

            //아이템 중복 확인
            while (IsItemDuplication(getItemIdx))
            {
                getItemIdx = Random.Range(itemRank * ITEMS_PER_RANK, itemRank * ITEMS_PER_RANK + MAX_ITEM_INDEX);
            }

            Consumable getItem = DataManager.instance.consumableList.item[getItemIdx];

            itemImage.sprite = Resources.Load<Sprite>("Item/" + getItem.name); 
            itemName.text = getItem.name;

            int itemSlotIndex = -1;
            for (int i = 0; i < player.itemSlot; ++i)
            {
                if (player.item[i] != null)
                {
                    itemSlotIndex = i;
                    break;
                }
            }

            if (itemSlotIndex != -1)
                player.item[itemSlotIndex] = getItem;
            else
            {
                // 슬롯이 부족한 경우
                // 미구현 상태
            }
        }
        else // 획득 x
        {
            itemImage.gameObject.SetActive(false);
            itemName.text = "없음";
        }
    }

    void CalculateMoneyReward()
    {
        Player player = GameManager.instance.player;

        int assassination = player.assassinationCount;

        var stageData = DataManager.instance.assassinationStageList.assassinationStage[assassination];
        int normalMoney = stageData.normalReward * killNormal;
        int eliteMoney = stageData.eliteReward * killElite;


        Transform enemytTexts = endPopup.transform.Find("PopupBoard").Find("EnemyTexts");
        TMP_Text norlmalAlpha = enemytTexts.Find("NormalAlpha").Find("NormalAlpha").GetComponent<TMP_Text>();
        TMP_Text eliteAlpha = enemytTexts.Find("EliteAlpha").Find("EliteAlpha").GetComponent <TMP_Text>();

        norlmalAlpha.text = $"{normalMoney}a";
        eliteAlpha.text = $"{eliteMoney}a";


        //(처치한 일반 적 수 * 일반 적 현상금 + 정예 적 수 * 적 현상금) * 획득 재화값
        int resultMoney = (int)((normalMoney + eliteMoney) * (1 + player.earnMoney));
        player.money += resultMoney;
    }



    // 끝난 날짜 및 데이터 조정 및 저장
    public void OnTouchEndBtn()
    {
        Player player = GameManager.instance.player;

        // 7일차가 아니면 저장하고 낮씬으로 가고 아니면 엔딩으로 갑니다.
        if (player.day < 7)
        {
            player.day++;

            //아이템을 비교해야하면 팝업창이 뜨게 작업
            // TODO: 남은 아이템 개수를 파악하고 있도록 해야 함
            if (player.item.Length <= player.itemSlot)
                UnityEngine.SceneManagement.SceneManager.LoadScene("DayScene");
            else
                SetItemChangePopup();
        }
        else
        {
            //재화 정리하고 엔딩으로
            int nowDifficulty = player.difficulty;
            if (nowDifficulty != 7)
            {
                if (player.money >= DataManager.instance.difficultyList.difficulty[nowDifficulty].goalMoney)
                {
                    player.curExp += DataManager.instance.difficultyList.difficulty[nowDifficulty].rewardExp;
                }
                else
                {
                    player.difficulty = 8;
                }
                player.day = 1;
                player.playingGame = false;

                while (player.level < 20 && player.reqExp < player.curExp)
                {
                    player.curExp -= player.reqExp;
                    player.level++;
                    player.reqExp = DataManager.instance.expList.exp[player.level - 1].reqExp;
                }

            }

            //아이템을 비교해야하면 팝업창이 뜨게 작업
            // TODO: 남은 아이템 개수를 파악하고 있도록 해야 함
            if (player.item.Length <= player.itemSlot)
                UnityEngine.SceneManagement.SceneManager.LoadScene("CutScene");
            else
                SetItemChangePopup();
        }
    }

    public void UpdateKillCount()
    {
        killCount++;
        itemManager.ChargingReaperGauge();  //차징 리퍼 쓰면 동작
    }



    bool IsItemDuplication(int getNowItemIdx)
    {
        Player player = GameManager.instance.player;
        for (int i = 0; i < player.itemSlot; i++)
        {
            if (player.item[i] != null && player.item[i].itemIdx == getNowItemIdx)
                return true;
        }
        return false;
    }

    int SetItemRank(int nowRank)
    {
        if (nowRank < 8)
            return 0;
        else if (nowRank < 17)
            return 1;
        else if (nowRank < 31)
            return 2;
        else
            return 3;
    }



    //환경설정 여는 함수
    public void OnClickSettingBtn()
    {
        setting.SetActive(true);
        Time.timeScale = 0;
    }

    public void OnClickContinueBtn()
    {
        setting.SetActive(false);
        Time.timeScale = 1;
    }

    public void OnClickExitBtn()
    {
        //씬 이동  
        setting.SetActive(false);
        Time.timeScale = 1;
        SetEndPopUp();
        endPopup.SetActive(true);
    }

    //마지막 추가된 팝업 함수
    void SetItemChangePopup()
    {
        Player player = GameManager.instance.player;
        int itemSlotCount = player.itemSlot;


        for (int i = 0; i < itemSlotCount; i++)
        {
            characterItemImageArr[i].sprite = itemManager.itemIconArr[player.item[i].imgIdx];
            characterItemImageArr[i].gameObject.SetActive(true);
        }

        Image image = GetThing.Find("Image").GetComponent<Image>();
        TMP_Text name = GetThing.Find("Name").GetComponent<TMP_Text>();
        TMP_Text rank = GetThing.Find("Rank").GetComponent<TMP_Text>();
        TMP_Text part = GetThing.Find("Part").GetComponent<TMP_Text>();
        TMP_Text info = GetThing.Find("Info").GetComponent<TMP_Text>();

        image.sprite = itemManager.itemIconArr[player.item[itemSlotCount].imgIdx];
        name.text = player.item[itemSlotCount].name;
        rank.text = player.item[itemSlotCount].GetRank() + "등급";
        part.text = player.item[itemSlotCount].GetCategory();
        info.text = player.item[itemSlotCount].GetConvertedScript(player);

        itemChangePopup.SetActive(true);
    }

    public void OnClickItemChangePopupBtn(int itemIdx)
    {
        Player player = GameManager.instance.player;
        if (itemIdx >= player.itemSlot)
            return;

        selectItemIdx = itemIdx;

        Transform haveItem = itemChangePopup.transform.Find("haveItem");
        Image itemImage = haveItem.Find("Image").GetComponent<Image>();
        TMP_Text itemName = haveItem.Find("Iname").GetComponent<TMP_Text>();
        TMP_Text itemRank = haveItem.Find("Irank").GetComponent<TMP_Text>();
        TMP_Text itemPart = haveItem.Find("Ipart").GetComponent<TMP_Text>();
        TMP_Text itemInfo = haveItem.Find("Iinfo").GetComponent <TMP_Text>();

        itemImage.sprite = itemManager.itemIconArr[player.item[itemIdx].imgIdx];
        itemImage.gameObject.SetActive(true);

        itemName.text = player.item[itemIdx].name;
        itemRank.text = player.item[itemIdx].GetRank() + "등급";
        itemPart.text = player.item[itemIdx].GetCategory();
        itemInfo.text = player.item[itemIdx].GetConvertedScript(player);
    }

    public void OnClickItemThrowBtn()
    {
        // 어떤 아이템인지 받아와서 null 처리 할 것
        //GameManager.instance.player.item.RemoveAt(GameManager.instance.player.itemSlot);

        Player.Save(GameManager.instance.player);

        GoNextScene();
    }

    public void OnClickItemChangeBtn()
    {
        GameManager.instance.player.item[selectItemIdx] = null;

        Player.Save(GameManager.instance.player);

        GoNextScene();
    }

    public void GoNextScene()
    {
        //7일차가 아니면 저장하고 낮씬으로 가고 아니면 엔딩으로 갑니다.
        if (GameManager.instance.player.day <= 7)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("DayScene");
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("CutScene");
        }
    }

}
