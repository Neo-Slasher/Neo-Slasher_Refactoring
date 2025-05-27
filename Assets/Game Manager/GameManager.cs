using System.IO;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public bool has_save_data;
    public Player player;
    public Setting setting;

    private string save_file_path;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        save_file_path = Path.Combine(Application.persistentDataPath, "/UserData.json");
        if (!File.Exists(save_file_path))
        {
            has_save_data = false;
        }
        else
        {
            has_save_data = true;
            player = Player.Load();
        }

        setting = Setting.Load();
        player = Player.Load();
    }


}
