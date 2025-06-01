using UnityEngine;
using UnityEngine.UI;
using TMPro;


// 상점에서 비교 버튼을 눌렀을 때 뜨는 팝업을 컨트롤 하는 스크립트
public class ComparisonPopup : MonoBehaviour
{
    public ShopManager shopManager;
    public FightingPower FightingPower;

    public GameObject PurchaseButton;
    public GameObject comparisonPopup;

    public Image[] playerComsumableImages;

    public GameObject equipmentPanel;
    public GameObject consumablePanel;
    public GameObject consumableSelectedPlayerItemPanel;
    public GameObject selectedItemInfoPanel;

    Item itemToBuy;

    // 플레이어가 버리고 싶어하는 아이템 인덱스
    public int playerConsumableIndex = -1;


    public void OnClickComparisonButton()
    {
        if (shopManager.selectedItem == null)
            return;

        comparisonPopup.SetActive(true);

        itemToBuy = shopManager.selectedItem;
        if (itemToBuy.GetItemType() == ItemType.Equipment)
        {
            equipmentPanel.SetActive(true);
            PrintPlayerEquipment();
        }
        else if (itemToBuy.GetItemType() == ItemType.Consumable)
        {
            consumablePanel.SetActive(true);
            PrintPlayerConsumable();
        }

        PrintSelectedItem();

        if (GameManager.Instance.player.money < itemToBuy.GetPrice())
            PurchaseButton.GetComponent<Button>().interactable = false;

        playerConsumableIndex = -1;
    }

    void PrintPlayerEquipment() //장비창 출력
    {
        Player player = GameManager.Instance.player;
        int equipmentPart = ((Equipment)itemToBuy).part;

        if (player.equipment[equipmentPart].index != 0)
        {
            Equipment playerEquipment = player.equipment[equipmentPart];

            Image image = equipmentPanel.transform.Find("background").Find("image").GetComponent<Image>();
            TMP_Text name = equipmentPanel.transform.Find("name").GetComponent<TMP_Text>();
            TMP_Text rank = equipmentPanel.transform.Find("rank").GetComponent<TMP_Text>();
            TMP_Text part = equipmentPanel.transform.Find("part").GetComponent<TMP_Text>();
            TMP_Text info = equipmentPanel.transform.Find("info").GetComponent<TMP_Text>();

            image.sprite = Resources.Load<Sprite>("Equip/" + playerEquipment.name);
            name.text = playerEquipment.name;
            rank.text = playerEquipment.GetRank() + "등급";
            part.text = "/ " + playerEquipment.GetCategory();
            info.text = playerEquipment.script;
        }
    }

    void PrintPlayerConsumable() //아이템 창 출력
    {
        Player player = GameManager.Instance.player;

        for (int i = 0; i < player.item.Length; i++)
        {
            if (i < player.itemSlot && player.item[i].itemIdx != 0)
            {
                Consumable playerConsumable = player.item[i];
                playerComsumableImages[i].sprite = Resources.Load<Sprite>("Item/" + playerConsumable.name);
            }
            else
            {
                playerComsumableImages[i].GetComponent<Button>().interactable = false;
            }
        }
    }

    void PrintSelectedItem()
    {
        Image image = selectedItemInfoPanel.transform.Find("background").Find("image").GetComponent<Image>();
        TMP_Text name = selectedItemInfoPanel.transform.Find("name").GetComponent<TMP_Text>();
        TMP_Text rank = selectedItemInfoPanel.transform.Find("rank").GetComponent<TMP_Text>();
        TMP_Text part = selectedItemInfoPanel.transform.Find("part").GetComponent<TMP_Text>();
        TMP_Text info = selectedItemInfoPanel.transform.Find("info").GetComponent<TMP_Text>();

        if (itemToBuy.GetItemType() == ItemType.Equipment)
        {
            Equipment equipment = (Equipment)itemToBuy;

            image.sprite = Resources.Load<Sprite>("Equip/" + equipment.name);
            name.text = equipment.name;
            rank.text = equipment.GetRank() + "등급";
            part.text = " / " + equipment.GetCategory();
            info.text = equipment.script;
        }
        else if (itemToBuy.GetItemType() == ItemType.Consumable)
        {
            Consumable consumable = (Consumable)itemToBuy;

            image.sprite = Resources.Load<Sprite>("item/" + consumable.name);
            name.text = consumable.name;
            rank.text = consumable.GetRank() + "등급";
            part.text = " / " + consumable.GetCategory();
            info.text = "<size=20>" + consumable.GetConvertedScript(GameManager.Instance.player) + "</size>";
        }
    }







    public void OnClickPurchaseButton()
    {
        Player player = GameManager.Instance.player;

        if (player.money < itemToBuy.GetPrice())
        {
            Debug.LogWarning("돈이 부족합니다.");
            return;
        }


        if (itemToBuy.GetItemType() == ItemType.Equipment)
        {
            Equipment equipment = (Equipment)itemToBuy;

            FightingPower.RemoveEquipment(equipment.part);
            FightingPower.EquipEquipment(equipment);
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
                    Debug.LogWarning("아이템 슬롯이 가득 찼습니다. 버릴 아이템을 선택하세요.");
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

        FightingPower.UpdateFightPower();

        OnClickCancelButton();
    }







    public void OnClickConsumablePanelItemButton(int buttonNumber)
    {
        playerConsumableIndex = buttonNumber;

        Player player = GameManager.Instance.player;
        Consumable consumable = player.item[buttonNumber];

        Image image = consumableSelectedPlayerItemPanel.transform.Find("background").Find("image").GetComponent<Image>();
        TMP_Text name = consumableSelectedPlayerItemPanel.transform.Find("name").GetComponent<TMP_Text>();
        TMP_Text rank = consumableSelectedPlayerItemPanel.transform.Find("rank").GetComponent<TMP_Text>();
        TMP_Text part = consumableSelectedPlayerItemPanel.transform.Find("part").GetComponent<TMP_Text>();
        TMP_Text info = consumableSelectedPlayerItemPanel.transform.Find("info").GetComponent<TMP_Text>();

        image.sprite = Resources.Load<Sprite>("item/" + consumable.name);
        name.text = consumable.name;
        rank.text = consumable.GetRank() + "등급";
        part.text = " / " + consumable.GetCategory();
        info.text = "<size=20>" + consumable.GetConvertedScript(player) + "</size>";

        Debug.Log("chosen" + playerConsumableIndex.ToString());
    }

    public void OnClickCancelButton()
    {
        consumablePanel.SetActive(false);
        equipmentPanel.SetActive(false);

        comparisonPopup.SetActive(false);

        ResetSelectedItemInfoPanel();
    }


    public void ResetSelectedItemInfoPanel()
    {
        Image image = consumableSelectedPlayerItemPanel.transform.Find("background").Find("image").GetComponent<Image>();
        TMP_Text name = consumableSelectedPlayerItemPanel.transform.Find("name").GetComponent<TMP_Text>();
        TMP_Text rank = consumableSelectedPlayerItemPanel.transform.Find("rank").GetComponent<TMP_Text>();
        TMP_Text part = consumableSelectedPlayerItemPanel.transform.Find("part").GetComponent<TMP_Text>();
        TMP_Text info = consumableSelectedPlayerItemPanel.transform.Find("info").GetComponent<TMP_Text>();

        image.sprite = Resources.Load<Sprite>("slotBack");
        name.text = "";
        rank.text = "";
        part.text = "";
        info.text = "";
    }

}




