using UnityEngine;


// 2025.06.02 Refactoring Final Version
public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;

    public Player player;
    public Setting setting;

    public bool has_save_data;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else 
        {
            Destroy(gameObject);
            return;
        }


        player = Player.Load();
        has_save_data = player != null;
        if (player == null)
        {
            player = new Player();
            Player.Save(player);
        }


        setting = Setting.Load();
        if (setting == null)
        {
            setting = new Setting();
            Setting.Save(setting);
        }
    }
}
