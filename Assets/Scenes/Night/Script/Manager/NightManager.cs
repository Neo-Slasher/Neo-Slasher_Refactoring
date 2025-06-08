using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NightManager : MonoBehaviour
{
    public static NightManager Instance { get; private set; }


    [Header("UI")]
    [SerializeField] SpriteRenderer backGround;
    [SerializeField] GameObject endPopup;
    [SerializeField] GameObject itemChangePopup;
    [SerializeField] Transform GetThing; // itemChangePopup 내부
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


    [SerializeField] Consumable getItem; // 밤 보상으로 획득한 아이템
    [SerializeField] int selectItemIdx;

    private const int MAX_NORMAL_ENEMY_INDEX = 2;
    private const int MAX_ELITE_ENEMY_INDEX = 2;


    public bool isStageEnd = false;
    public int killCount = 0;
    public int killNormal = 0;
    public int killElite = 0;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SetBackGround();
        InstantiateEnemy();
    }

    void SetBackGround()
    {
        switch (GameManager.Instance.player.assassinationCount)
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

        while (!isStageEnd && TimerManager.Instance.timerCount > 1)
        {
            GameObject enemy = Instantiate(prefabs[index], SetEnemyPosition(), Quaternion.identity);
            enemy.GetComponent<Enemy>().SetEnforceData();
            enemy.transform.SetParent(enemies);
            yield return new WaitForSeconds(spawnTime);
        }
    }


    double GetSpawnRate(bool isElite, int getIndex)
    {
        int nowAssassination = GameManager.Instance.player.assassinationCount;
        switch (getIndex)
        {
            case 0:
                if (isElite)
                    return DataManager.Instance.assassinationStageList.assassinationStage[nowAssassination].elite1Spawn;
                else
                    return DataManager.Instance.assassinationStageList.assassinationStage[nowAssassination].normal1Spawn;
            case 1:
                if (isElite)
                    return DataManager.Instance.assassinationStageList.assassinationStage[nowAssassination].elite2Spawn;
                else
                    return DataManager.Instance.assassinationStageList.assassinationStage[nowAssassination].normal2Spawn;
            case 2:
                if (isElite)
                    return DataManager.Instance.assassinationStageList.assassinationStage[nowAssassination].elite3Spawn;
                else
                    return DataManager.Instance.assassinationStageList.assassinationStage[nowAssassination].normal3Spawn;
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

        Character characterComp = character.GetComponent<Character>();
        characterComp.NightEnd();

        //조이스틱 사용은 nightManager.isStageEnd를 매번 받기 때문에 알아서 꺼짐

        DeleteEnemies();

        if (!endPopup.activeSelf)
        {
            SetEndPopUp();
            endPopup.SetActive(true);
        }
    }

    private void DeleteEnemies()
    {
        // 적 삭제 (안전하게 복사 후 삭제)
        Transform[] enemyList = new Transform[enemies.childCount];

        for (int i = 0; i < enemyList.Length; i++)
            enemyList[i] = enemies.GetChild(i);

        foreach (Transform child in enemies)
        {
            if (child != null)
                Destroy(child.gameObject);
        }
    }













    //팝업창 데이터 설정
    void SetEndPopUp()
    {
        Player player = GameManager.Instance.player;
        Transform enemytTexts = endPopup.transform.Find("PopupBoard").Find("EnemyTexts");
        Transform UIs = endPopup.transform.Find("PopupBoard").Find("UIs");

        TMP_Text aliveOrDie = UIs.Find("AliveOrDieText").GetComponent<TMP_Text>();
        TMP_Text day = UIs.Find("DayText").GetComponent<TMP_Text>();

        aliveOrDie.text = (TimerManager.Instance.timerCount == 0) ? "생존" : "사망";
        day.text = player.day + "일차 D-" + (7 - player.day);

        TMP_Text normalCount = enemytTexts.Find("NormalKill").Find("NormalKillCount").GetComponent<TMP_Text>();
        TMP_Text eliteCount = enemytTexts.Find("EliteKill").Find("EliteKillCount").GetComponent<TMP_Text>();

        normalCount.text = killNormal.ToString();
        eliteCount.text = killElite.ToString();

        character.GetComponent<Character>().Pause();


        // 얻은 돈은 이 함수에서 직접 처리 (적절한 처리는 아님)
        int money = CalculateMoneyReward();
        player.money += money;

        // 얻은 아이템은 다음 함수(OnTouchEndBtn)에서 처리
        getItem = CalculateItemReward();
    }

    //밤이 끝나고 아이템 획득하는 함수
    private Consumable CalculateItemReward()
    {
        Player player = GameManager.Instance.player;
        var stageData = DataManager.Instance.assassinationStageList.assassinationStage[player.assassinationCount];

        const int ITEMS_PER_RANK = 15;
        const int MAX_ITEM_INDEX = 14;

        float dropRate = (float)((stageData.normalDropRate * killNormal + stageData.eliteDropRate * killElite) * player.dropRate);
        float randomValue = Random.value; // 0~1 사이 값

        Transform dropItemUI = endPopup.transform.Find("PopupBoard").Find("DropItemUI");
        Image itemImage = dropItemUI.Find("DropItemImage").GetComponent<Image>();
        TMP_Text itemName = dropItemUI.Find("DropItemNameText").GetComponent<TMP_Text>();

        if (randomValue < dropRate) // 아이템 획득
        //if (true) // 임시로 디버깅 위해 항상 획득
        {
            int rank = player.dropRank + DataManager.Instance.difficultyList.difficulty[player.difficulty].dropRank
                        + DataManager.Instance.assassinationStageList.assassinationStage[player.assassinationCount].stageDropRank;

            int itemRank = SetItemRank(rank);

            int getItemIdx = Random.Range(itemRank * ITEMS_PER_RANK, itemRank * ITEMS_PER_RANK + MAX_ITEM_INDEX);

            while (player.haveConsumable(getItemIdx))
            {
                getItemIdx = Random.Range(itemRank * ITEMS_PER_RANK, itemRank * ITEMS_PER_RANK + MAX_ITEM_INDEX);
            }

            Consumable getItem = DataManager.Instance.consumableList.item[getItemIdx];

            itemImage.sprite = Resources.Load<Sprite>("Item/" + getItem.name);
            itemName.text = getItem.name;

            return getItem;
        }
        else // 획득 x
        {
            itemImage.gameObject.SetActive(false);
            itemName.text = "없음";

            return null;
        }
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


    private int CalculateMoneyReward()
    {
        Player player = GameManager.Instance.player;


        var stageData = DataManager.Instance.assassinationStageList.assassinationStage[player.assassinationCount];
        int normalMoney = stageData.normalReward * killNormal;
        int eliteMoney = stageData.eliteReward * killElite;


        Transform enemytTexts = endPopup.transform.Find("PopupBoard").Find("EnemyTexts");
        TMP_Text norlmalAlpha = enemytTexts.Find("NormalAlpha").Find("NormalAlpha").GetComponent<TMP_Text>();
        TMP_Text eliteAlpha = enemytTexts.Find("EliteAlpha").Find("EliteAlpha").GetComponent<TMP_Text>();

        norlmalAlpha.text = $"{normalMoney}a";
        eliteAlpha.text = $"{eliteMoney}a";

        //(처치한 일반 적 수 * 일반 적 현상금 + 정예 적 수 * 적 현상금) * 획득 재화 배율
        int resultMoney = (int)((normalMoney + eliteMoney) * player.earnMoney);

        return resultMoney;
    }








    // 끝난 날짜 및 데이터 조정 및 저장
    public void OnTouchEndBtn()
    {
        Player player = GameManager.Instance.player;

        // 여기서 아이템 보상이 있다면 처리해야 함
        if (getItem != null)
        { // 아이템이 드롭된 경우
            int playerItemSlot = player.GetEmptyComsumableSlot();
            Debug.Log($"비어있는 player item slot {playerItemSlot}");
            if (playerItemSlot != -1)
            {
                player.item[playerItemSlot] = getItem;
                GoNextScene();
            }
            else
            {
                SetItemChangePopup();
                itemChangePopup.SetActive(true);
            }
        }
        else
        {
            GoNextScene();
        }
    }

    void SetItemChangePopup()
    {
        Player player = GameManager.Instance.player;

        for (int i = 0; i < player.itemSlot; i++)
        {
            characterItemImageArr[i].sprite = Resources.Load<Sprite>("Item/" + player.item[i].name);
            characterItemImageArr[i].gameObject.SetActive(true);
        }

        Image image = GetThing.Find("Image").GetComponent<Image>();
        TMP_Text name = GetThing.Find("Name").GetComponent<TMP_Text>();
        TMP_Text rank = GetThing.Find("Rank").GetComponent<TMP_Text>();
        TMP_Text part = GetThing.Find("Part").GetComponent<TMP_Text>();
        TMP_Text info = GetThing.Find("Info").GetComponent<TMP_Text>();

        Consumable consumable = getItem;

        image.sprite = Resources.Load<Sprite>("Item/" + consumable.name);
        name.text = consumable.name;
        rank.text = consumable.GetRank() + "등급";
        part.text = consumable.GetCategory();
        info.text = consumable.GetConvertedScript(player);
    }

    // '버리기' 버튼 눌렀을 때 
    public void OnClickItemThrowBtn()
    {
        GoNextScene();
    }

    // '교체' 버튼 눌렀을 때
    public void OnClickItemChangeBtn()
    {
        GameManager.Instance.player.item[selectItemIdx] = getItem;
        Player.Save(GameManager.Instance.player);

        GoNextScene();
    }

    public void OnClickItemChangePopupBtn(int itemIdx)
    {
        Player player = GameManager.Instance.player;
        if (itemIdx >= player.itemSlot)
            return;

        selectItemIdx = itemIdx;

        Transform haveItem = itemChangePopup.transform.Find("haveItem");
        Image itemImage = haveItem.Find("Image").GetComponent<Image>();
        TMP_Text itemName = haveItem.Find("Iname").GetComponent<TMP_Text>();
        TMP_Text itemRank = haveItem.Find("Irank").GetComponent<TMP_Text>();
        TMP_Text itemPart = haveItem.Find("Ipart").GetComponent<TMP_Text>();
        TMP_Text itemInfo = haveItem.Find("Iinfo").GetComponent<TMP_Text>();

        itemImage.sprite = ItemManager.Instance.itemIconArr[player.item[itemIdx].imgIdx];
        itemImage.gameObject.SetActive(true);

        itemName.text = player.item[itemIdx].name;
        itemRank.text = player.item[itemIdx].GetRank() + "등급";
        itemPart.text = player.item[itemIdx].GetCategory();
        itemInfo.text = player.item[itemIdx].GetConvertedScript(player);
    }


    public void GoNextScene()
    {
        Player player = GameManager.Instance.player;
        //7일차가 아니면 저장하고 낮씬으로 가고 아니면 엔딩으로 갑니다.
        if (GameManager.Instance.player.day < 7)
        {
            GameManager.Instance.player.day++;
            Player.Save(GameManager.Instance.player);
            UnityEngine.SceneManagement.SceneManager.LoadScene("DayScene");
        }
        else // day == 7 인 경우
        {

            if (player.money >= DataManager.Instance.difficultyList.difficulty[player.difficulty].goalMoney)
            {
                // good ending
                player.curExp += DataManager.Instance.difficultyList.difficulty[player.difficulty].rewardExp;
                while (player.level < 20 && player.reqExp < player.curExp)
                {
                    player.curExp -= player.reqExp;
                    player.level++;
                    player.reqExp = DataManager.Instance.expList.exp[player.level - 1].reqExp;
                }
            }
            else
            {
                // 플레이어가 선택한 난이도의 목표 재화를 7일 동안 얻지 못한 경우 
                // 7일차 종료 후 bad ending을 진행하게 됩니다.
                // cut scene에서 스토리를 difficulty를 기준으로 출력 중
                player.difficulty = 9;
            }


            player.day = 1;
            Player.Save(GameManager.Instance.player);
            UnityEngine.SceneManagement.SceneManager.LoadScene("CutScene");
        }
    }





    public void UpdateKillCount()
    {
        killCount++;
        ItemManager.Instance.ChargingReaperGauge();  //차징 리퍼 쓰면 동작
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

        // Tolelom: 왜 시간을 다시 카운트 하는 지?
        Time.timeScale = 1;

        SetEndPopUp();
        endPopup.SetActive(true);
    }
}
