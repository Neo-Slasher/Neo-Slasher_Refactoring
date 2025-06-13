using UnityEngine;
using UnityEngine.UI;
using TMPro;


// 2025.06.03 Refactoring Final Version
// 상점에서 비교 버튼을 눌렀을 때 뜨는 팝업을 컨트롤 하는 스크립트
public class ComparisonPopupController : MonoBehaviour
{
    [SerializeField] private ShopManager shopManager;
    [SerializeField] private FightingPower fightingPower;

    [SerializeField] private GameObject PurchaseButton;
    [SerializeField] private GameObject comparisonPopup;

    [SerializeField] private Image[] playerComsumableImages;

    [SerializeField] private GameObject equipmentPanel;
    [SerializeField] private GameObject consumablePanel;
    [SerializeField] private GameObject consumableSelectedPlayerItemPanel;
    [SerializeField] private GameObject selectedItemInfoPanel;

    private Item itemToBuy;

    // 플레이어가 버리고 싶어하는 아이템 인덱스
    public int playerConsumableIndex = -1;


    public void OnClickComparisonButton()
    {
        SoundManager.Instance.PlaySFX(SoundManager.Instance.buttonClip);
        if (shopManager.selectedItem == null)
        {
            Logger.Log("비교할 아이템이 없습니다.");
            return;
        }

        comparisonPopup.SetActive(true);

        itemToBuy = shopManager.selectedItem;

        switch (itemToBuy.GetItemType())
        {
            case ItemType.Equipment:
                equipmentPanel.SetActive(true);
                PrintPlayerEquipment();
                break;
            case ItemType.Consumable:

                consumablePanel.SetActive(true);
                PrintPlayerConsumable();
                break;
            default:
                Logger.LogWarning($"처리되지 않은 아이템 타입: {itemToBuy.GetItemType()}");
                break;
        }

        PrintSelectedItem();

        if (GameManager.Instance.player.money < itemToBuy.GetPrice())
            PurchaseButton.GetComponent<Button>().interactable = false;

        playerConsumableIndex = -1;
    }

    private void PrintPlayerEquipment() //장비창 출력
    {
        Player player = GameManager.Instance.player;
        int equipmentPart = ((Equipment)itemToBuy).part;

        if (player.equipment[equipmentPart] == null || player.equipment[equipmentPart].index == 0)
            return;
        
        
        Equipment playerEquipment = player.equipment[equipmentPart];

        Image grid = equipmentPanel.transform.Find("background").Find("grid").GetComponent<Image>();
        Image image = equipmentPanel.transform.Find("background").Find("image").GetComponent<Image>();
        TMP_Text name = equipmentPanel.transform.Find("name").GetComponent<TMP_Text>();
        TMP_Text rank = equipmentPanel.transform.Find("rank").GetComponent<TMP_Text>();
        TMP_Text part = equipmentPanel.transform.Find("part").GetComponent<TMP_Text>();
        TMP_Text info = equipmentPanel.transform.Find("info").GetComponent<TMP_Text>();

        grid.sprite = DataManager.Instance.itemGrids[playerEquipment.rank];
        image.sprite = DataManager.Instance.equipmentIcons[playerEquipment.index];
        name.text = playerEquipment.name;
        rank.text = playerEquipment.GetRank() + "등급";
        part.text = "/ " + playerEquipment.GetCategory();
        info.text = playerEquipment.script;
        
    }

    private void PrintPlayerConsumable() //아이템 창 출력
    {
        Player player = GameManager.Instance.player;

        for (int i = 0; i < player.item.Length; i++)
        {
            if (i < player.itemSlot && player.item[i] != null && player.item[i].itemIdx != 0)
            {
                Consumable playerConsumable = player.item[i];
                playerComsumableImages[i].sprite = DataManager.Instance.consumableIcons[playerConsumable.itemIdx];
            }
            else
            {
                playerComsumableImages[i].GetComponent<Button>().interactable = false;
            }
        }
    }

    private void PrintSelectedItem()
    {
        Image grid = selectedItemInfoPanel.transform.Find("background").Find("grid").GetComponent<Image>();
        Image image = selectedItemInfoPanel.transform.Find("background").Find("image").GetComponent<Image>();
        TMP_Text name = selectedItemInfoPanel.transform.Find("name").GetComponent<TMP_Text>();
        TMP_Text rank = selectedItemInfoPanel.transform.Find("rank").GetComponent<TMP_Text>();
        TMP_Text part = selectedItemInfoPanel.transform.Find("part").GetComponent<TMP_Text>();
        TMP_Text info = selectedItemInfoPanel.transform.Find("info").GetComponent<TMP_Text>();

        if (itemToBuy.GetItemType() == ItemType.Equipment)
        {
            Equipment equipment = (Equipment)itemToBuy;

            grid.sprite = DataManager.Instance.itemGrids[equipment.rank];
            image.sprite = DataManager.Instance.equipmentIcons[equipment.index]; 
            name.text = equipment.name;
            rank.text = equipment.GetRank() + "등급";
            part.text = " / " + equipment.GetCategory();
            info.text = equipment.script;
        }
        else if (itemToBuy.GetItemType() == ItemType.Consumable)
        {
            Consumable consumable = (Consumable)itemToBuy;

            grid.sprite = DataManager.Instance.itemGrids[consumable.rank];
            image.sprite = DataManager.Instance.consumableIcons[consumable.itemIdx];
            name.text = consumable.name;
            rank.text = consumable.GetRank() + "등급";
            part.text = " / " + consumable.GetCategory();
            info.text = consumable.GetConvertedScript(GameManager.Instance.player);
        }
    }

    public void OnClickPurchaseButton()
    {
        SoundManager.Instance.PlaySFX(SoundManager.Instance.buttonClip);
        Player player = GameManager.Instance.player;

        if (player.money < itemToBuy.GetPrice())
        {
            Debug.LogWarning("돈이 부족합니다.");
            return;
        }


        if (itemToBuy.GetItemType() == ItemType.Equipment)
        {
            Equipment equipment = (Equipment)itemToBuy;

            fightingPower.EquipEquipment(equipment);
            player.money -= itemToBuy.GetPrice();
        }
        else // Consumable
        {
            Consumable consumable = (Consumable)itemToBuy;

            int playerItemSlotIndex = -1;
            for (int i = 0; i < player.itemSlot; i++)
            {
                if (player.item[i].itemIdx == 0)
                {
                    playerItemSlotIndex = i;
                    break;
                }
            }

            if (playerItemSlotIndex == -1)
            {

                if (playerConsumableIndex == -1)
                {
                    Logger.LogWarning("아이템 슬롯이 가득 찼습니다. 버릴 아이템을 선택하세요.");
                    return;
                }
                else
                {
                    player.item[playerConsumableIndex] = consumable;
                    player.money -= itemToBuy.GetPrice();
                }
            }
            else
            {
                player.item[playerItemSlotIndex] = consumable;
                player.money -= itemToBuy.GetPrice();
            }
        }

        shopManager.ResetSlotAfterPurchase();
        fightingPower.UpdateFightPower();
        OnClickCancelButton();
    }

    public void OnClickConsumablePanelItemButton(int buttonNumber)
    {
        SoundManager.Instance.PlaySFX(SoundManager.Instance.buttonClip);
        playerConsumableIndex = buttonNumber;

        Player player = GameManager.Instance.player;
        Consumable consumable = player.item[buttonNumber];

        Image grid = consumableSelectedPlayerItemPanel.transform.Find("background").Find("grid").GetComponent<Image>();
        Image image = consumableSelectedPlayerItemPanel.transform.Find("background").Find("image").GetComponent<Image>();
        TMP_Text name = consumableSelectedPlayerItemPanel.transform.Find("name").GetComponent<TMP_Text>();
        TMP_Text rank = consumableSelectedPlayerItemPanel.transform.Find("rank").GetComponent<TMP_Text>();
        TMP_Text part = consumableSelectedPlayerItemPanel.transform.Find("part").GetComponent<TMP_Text>();
        TMP_Text info = consumableSelectedPlayerItemPanel.transform.Find("info").GetComponent<TMP_Text>();

        grid.sprite = DataManager.Instance.itemGrids[consumable.rank];
        image.sprite = DataManager.Instance.consumableIcons[consumable.itemIdx];
        name.text = consumable.name;
        rank.text = $"{consumable.GetRank()}등급";
        part.text = $" / {consumable.GetCategory()}";
        info.text = consumable.GetConvertedScript(player);
    }

    public void OnClickCancelButton()
    {
        SoundManager.Instance.PlaySFX(SoundManager.Instance.buttonClip);
        consumablePanel.SetActive(false);
        equipmentPanel.SetActive(false);
        comparisonPopup.SetActive(false);

        ResetSelectedItemInfoPanel();
    }

    private void ResetSelectedItemInfoPanel()
    {
        Image grid = consumableSelectedPlayerItemPanel.transform.Find("background").Find("grid").GetComponent<Image>();
        Image image = consumableSelectedPlayerItemPanel.transform.Find("background").Find("image").GetComponent<Image>();
        TMP_Text name = consumableSelectedPlayerItemPanel.transform.Find("name").GetComponent<TMP_Text>();
        TMP_Text rank = consumableSelectedPlayerItemPanel.transform.Find("rank").GetComponent<TMP_Text>();
        TMP_Text part = consumableSelectedPlayerItemPanel.transform.Find("part").GetComponent<TMP_Text>();
        TMP_Text info = consumableSelectedPlayerItemPanel.transform.Find("info").GetComponent<TMP_Text>();

        grid.sprite = Resources.Load<Sprite>("slotBack");
        image.sprite = Resources.Load<Sprite>("slotBack");
        name.text = "";
        rank.text = "";
        part.text = "";
        info.text = "";
    }
}




