using UnityEngine;
using System;

// 2025.06.02 Refactoring Final Version
public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    public DifficultyList difficultyList;
    public TraitList traitList;
    public EquipmentList equipmentList;
    public AssassinationStageList assassinationStageList;
    public ConsumableList consumableList;
    public MonsterList monsterList;
    public StoryList storyList;
    public ExpList expList;

    public Sprite[] consumableIcons = Array.Empty<Sprite>();
    public Sprite[] equipmentIcons = Array.Empty<Sprite>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
    }

    void Start()
    {
        LoadAllData();
        LoadAllIcons();
    }

    private void LoadAllData()
    {
        difficultyList = LoadJson<DifficultyList>("Difficulty");
        equipmentList = LoadJson<EquipmentList>("Equipment");
        traitList = LoadJson<TraitList>("Trait");
        assassinationStageList = LoadJson<AssassinationStageList>("AssassinationStage");
        consumableList = LoadJson<ConsumableList>("Item");
        monsterList = LoadJson<MonsterList>("Monster");
        storyList = LoadJson<StoryList>("Story");
        expList = LoadJson<ExpList>("Exp");
    }

    private void LoadAllIcons()
    {
        SetConsumableIcons();
        SetEquipmentIcons();
    }

    private T LoadJson<T>(string fileName) where T : new()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>($"Json/{fileName}");
        if (jsonFile == null)
        {
            Logger.LogError($"{fileName}.json 파일 누락");
            return new T();
        }

        try
        {
            return JsonUtility.FromJson<T>(jsonFile.text);
        }
        catch (Exception e)
        {
            Logger.LogError($"{fileName} 파싱 실패: {e.Message}");
            return new T();
        }
    }

    private void SetConsumableIcons()
    {
        if (consumableList?.item == null)
        {
            Logger.LogWarning("소비 아이템 데이터가 비어있습니다.");
            return;
        }

        int maxIdx = 0;
        foreach (var c in consumableList.item)
            if (c.itemIdx > maxIdx) maxIdx = c.itemIdx;
        consumableIcons = new Sprite[maxIdx + 1];

        foreach (var consumable in consumableList.item)
        {
            Sprite icon = Resources.Load<Sprite>("Item/" + consumable.name);
            consumableIcons[consumable.itemIdx] = icon;
            if (consumableIcons[consumable.itemIdx] == null)
            {
                Logger.LogWarning($"아이템 아이콘 누락: {consumable.name}");
            }
        }
    }

    private void SetEquipmentIcons()
    {
        if (equipmentList?.equipment == null)
        {
            Logger.LogWarning("장비 아이템 데이터가 비어있습니다.");
            return;
        }

        int maxIdx = 0;
        foreach (var c in equipmentList.equipment)
            if (c.index > maxIdx) maxIdx = c.index;
        equipmentIcons = new Sprite[maxIdx + 1];

        foreach (var equipment in equipmentList.equipment)
        {
            Sprite icon = Resources.Load<Sprite>("Equip/" + equipment.name);
            equipmentIcons[equipment.index] = icon;
            if (equipmentIcons[equipment.index] == null)
            {
                Logger.LogWarning($"아이템 아이콘 누락: {equipment.name}");
            }
        }
    }
}
