using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 2025.06.05 Refactoring Final Version
public class EquipmentManager : MonoBehaviour
{
    [SerializeField] private Button[] equipment_buttons;
    [SerializeField] private Button[] item_buttons;

    // Selected 관련
    public GameObject selected;

    public void OnEnable()
    {
        LoadPlayerItems();
    }

    private void LoadPlayerItems()
    {
        Player player = GameManager.Instance.player;

        for (int i = 0; i < player.equipment.Length; i++)
        {
            Equipment equipment = player.equipment[i];
            Button currentButton = equipment_buttons[i];

            if (equipment == null || equipment.index == 0)
                continue;


            Sprite sprite = DataManager.Instance.equipmentIcons[equipment.index];
            if (sprite != null)
                currentButton.image.sprite = sprite;
            else
                Logger.LogWarning($"스프라이트를 찾을 수 없습니다: Equip/{equipment.name}");


            TMP_Text rankText = currentButton.transform.Find("rank")?.GetComponent<TMP_Text>();
            string rank = equipment.GetRank();
            rankText.text = rank ?? "";

            Image grid = currentButton.transform.Find("grid").GetComponent<Image>();
            grid.sprite = DataManager.Instance.itemGrids[equipment.rank];
        }


        const int MAX_CONSUMABLE_COUNT = 3;
        for (int i = 0; i < MAX_CONSUMABLE_COUNT; i++)
        {
            if (i < player.itemSlot)
            {
                Consumable consumable = player.item[i];
                Button currentButton = item_buttons[i];

                if (consumable == null || consumable.itemIdx == 0)
                {
                    currentButton.interactable = false;
                    continue;
                }

                currentButton.interactable = true;

                Sprite sprite = DataManager.Instance.consumableIcons[consumable.itemIdx];
                currentButton.image.sprite = sprite;

                TMP_Text rankText = currentButton.transform.Find("rank").GetComponent<TMP_Text>();
                rankText.text = consumable.GetRank();

                Transform lockedTransform = currentButton.transform.Find("locked");
                lockedTransform.gameObject.SetActive(false);

                Image grid = currentButton.transform.Find("grid").GetComponent<Image>();
                grid.sprite = DataManager.Instance.itemGrids[consumable.rank];
            }
            else
            {
                item_buttons[i].interactable = false;
                Transform lockedTransform = item_buttons[i].transform.Find("locked");
                lockedTransform.gameObject.SetActive(true);
            }
        }
    }

    public void OnClickConsumableSlot(int slotNumber)
    {
        SoundManager.Instance.PlaySFX(SoundManager.Instance.buttonClip);
        ActiveConsumableButtonSelected(slotNumber);

        Image image = selected.transform.Find("background").Find("image").GetComponent<Image>();
        Image grid = selected.transform.Find("background").Find("grid").GetComponent<Image>();
        TMP_Text name = selected.transform.Find("name").GetComponent<TMP_Text>();
        TMP_Text rank = selected.transform.Find("rank").GetComponent<TMP_Text>();
        TMP_Text part = selected.transform.Find("part").GetComponent<TMP_Text>();
        TMP_Text info = selected.transform.Find("info").GetComponent<TMP_Text>();

        Consumable consumable = GameManager.Instance.player.item[slotNumber];

        image.sprite = DataManager.Instance.consumableIcons[consumable.itemIdx];
        grid.sprite = DataManager.Instance.itemGrids[consumable.rank];
        name.text = consumable.name;
        rank.text = consumable.GetRank() + "등급";
        part.text = consumable.GetCategory();
        info.text = consumable.GetConvertedScript(GameManager.Instance.player);
    }

    public void OnClickEquipmentSlot(int slotNumber)
    {
        SoundManager.Instance.PlaySFX(SoundManager.Instance.buttonClip);
        ActiveEquipmentButtonSelected(slotNumber);

        Image image = selected.transform.Find("background").Find("image").GetComponent<Image>();
        Image grid = selected.transform.Find("background").Find("grid").GetComponent<Image>();
        TMP_Text name = selected.transform.Find("name").GetComponent<TMP_Text>();
        TMP_Text rank = selected.transform.Find("rank").GetComponent<TMP_Text>();
        TMP_Text part = selected.transform.Find("part").GetComponent<TMP_Text>();
        TMP_Text info = selected.transform.Find("info").GetComponent<TMP_Text>();

        Equipment equipment = GameManager.Instance.player.equipment[slotNumber];
        image.sprite = Resources.Load<Sprite>("Equip/" + equipment.name);
        grid.sprite = DataManager.Instance.itemGrids[equipment.rank];
        name.text = equipment.name;
        rank.text = equipment.GetRank() + "등급";
        part.text = equipment.GetCategory();
        info.text = equipment.script;
    }

    private void ActiveEquipmentButtonSelected(int slotNumber)
    {
        for (int i = 0; i < equipment_buttons.Length; i++)
        {
            equipment_buttons[i].transform.Find("selected").gameObject.SetActive(i == slotNumber);
        }

        for (int i = 0; i < item_buttons.Length; i++)
            item_buttons[i].transform.Find("selected").gameObject.SetActive(false);
    }

    private void ActiveConsumableButtonSelected(int slotNumber)
    {
        for (int i = 0; i < equipment_buttons.Length; i++)
            equipment_buttons[i].transform.Find("selected").gameObject.SetActive(false);

        for (int i = 0; i < item_buttons.Length; i++)
        {
            item_buttons[i].transform.Find("selected").gameObject.SetActive(i == slotNumber);
        }
    }
}
