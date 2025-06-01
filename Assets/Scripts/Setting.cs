using System;
using System.IO;
using UnityEditor;
using UnityEngine;

[Serializable]
public class Setting
{
    public const string file_path = "Setting.json";

    public float bgm_volume;
    public float sfx_volume;
    public float joy_stick_size;

    public Setting()
    {
        bgm_volume = 100f;
        sfx_volume = 100f;
        joy_stick_size = 100f;
    }

    public static void Save(Setting instance)
    {
        string setting_file_path = Path.Combine(Application.persistentDataPath, file_path);
        string json = JsonUtility.ToJson(instance);
        File.WriteAllText(setting_file_path, json);
    }


    // Load the setting from persistent data path, if does not exist, create a new setting
    public static Setting Load()
    {
        string setting_file_path = Path.Combine(Application.persistentDataPath, file_path);
        if (!File.Exists(setting_file_path))
        {
            return null;
        }
        string savedData = File.ReadAllText(setting_file_path);
        return JsonUtility.FromJson<Setting>(savedData);
    }
}
