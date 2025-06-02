using System;
using System.IO;
using UnityEditor;
using UnityEngine;

[Serializable]
public class Setting
{
    public const string filePath = "Setting.json";

    public float bgmVolume;
    public float sfxVolume;
    public float joystickSize;

    public Setting()
    {
        bgmVolume = 100f;
        sfxVolume = 100f;
        joystickSize = 100f;
    }

    public static void Save(Setting instance)
    {
        string settingFilePath = Path.Combine(Application.persistentDataPath, filePath);
        string json = JsonUtility.ToJson(instance);
        File.WriteAllText(settingFilePath, json);
    }

    public static Setting Load()
    {
        string setting_file_path = Path.Combine(Application.persistentDataPath, filePath);
        if (!File.Exists(setting_file_path))
        {
            return null;
        }
        string savedData = File.ReadAllText(setting_file_path);
        return JsonUtility.FromJson<Setting>(savedData);
    }
}
