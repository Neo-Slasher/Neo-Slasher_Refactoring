using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using System.Text;
using TMPro;

public class DataManager : MonoBehaviour {
    public static DataManager instance = null;

    public DifficultyList difficultyList;
    public TraitList traitList;
    public EquipmentList equipmentList;
    public AssassinationStageList assassinationStageList;
    public ConsumableList consumableList;
    public MonsterList monsterList;
    public StoryList storyList;
    public ExpList expList;

    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);

    }


    void Start() {
        // init difficulty data
        string DifficultyData = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "Difficulty.json"));
        difficultyList = JsonUtility.FromJson<DifficultyList>(DifficultyData);

        // init Equipment data
        string EquipmentData = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "Equipment.json"));
        equipmentList = JsonUtility.FromJson<EquipmentList>(EquipmentData);

        // init trait data
        string TraitData = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "Trait.json"));
        traitList = JsonUtility.FromJson<TraitList>(TraitData);

        // assassination stage data
        string AssassinationStageData = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "AssassinationStage.json"));
        assassinationStageList = JsonUtility.FromJson<AssassinationStageList>(AssassinationStageData);

        // item data 
        string ItemData = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "Item.json"));
        consumableList = JsonUtility.FromJson<ConsumableList>(ItemData);

        // monster data
        string MonsterData = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "Monster.json"));
        monsterList = JsonUtility.FromJson<MonsterList>(MonsterData);

        // stroy data
        string StoryData = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "Story.json"));
        storyList = JsonUtility.FromJson<StoryList>(StoryData);

        // exp data
        string ExpData = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "Exp.json"));
        expList = JsonUtility.FromJson<ExpList>(ExpData);
    }

    public T ResourceDataLoad<T>(string name) {
        T gameData;

        string directory = "Json/";
        string appender1 = name;

        StringBuilder builder = new StringBuilder(directory);
        builder.Append(appender1);

        TextAsset jsonString = Resources.Load<TextAsset>(builder.ToString());

        gameData = JsonUtility.FromJson<T>(jsonString.ToString());


        return gameData;
    }
}