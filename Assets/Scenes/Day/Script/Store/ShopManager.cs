using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


// 2025.06.05 Refactoring Final Version
// 상점의 아이템 슬롯 3개를 세팅하는 역할
public class ShopManager : MonoBehaviour
{
    [SerializeField] private Button[] slots;
    [SerializeField] private List<Item> selectedItems = new();
    [SerializeField] private GameObject selected;

    [SerializeField] private Button comparisonButton;
    [SerializeField] private GameObject comparisonPopup;

    [SerializeField] private int selectedNumber;

    public Item selectedItem;


    void Start()
    {
        SetShopItems();
    }

    private void SetShopItems()
    {
        Player player = GameManager.Instance.player;

        // 판매할 수 있는 후보 아이템 뽑기
        int shopMinRank = player.shopMinRank;
        int shopMaxRank = player.shopMaxRank;

        List<Equipment> equipments = DataManager.Instance.equipmentList.equipment
            .Where(e => shopMinRank <= e.rank && e.rank <= shopMaxRank)
            .ToList();

        List<Consumable> consumables = DataManager.Instance.consumableList.item
            .Where(c => shopMinRank <= c.rank && c.rank <= shopMaxRank)
            .ToList();

        List<Equipment> shuffledEquipments = equipments.OrderBy(x => Random.value).ToList();
        List<Consumable> shuffledConsumables = consumables.OrderBy(x => Random.value).ToList();

        int equipmentIndex = 0;
        int consumableIndex = 0;

        // 후보 중 판매할 아이템 선택
        for (int i = 0; i < player.shopSlot; i++)
        {
            int equipOrItem = Random.Range(0, 10);

            if (equipOrItem < 6 && equipmentIndex < shuffledEquipments.Count) // 60%
            {
                selectedItems.Add(shuffledEquipments[equipmentIndex++]);
            }
            else if (equipOrItem >= 6 && equipOrItem <= 9 && consumableIndex < shuffledConsumables.Count) // 40%
            {
                selectedItems.Add(shuffledConsumables[consumableIndex++]);
            }
            // 후보가 소진된 경우는 건너뜀
        }


        // 선택된 아이템 UI에 출력
        for (int i = 0; i < 3; i++)
        {
            if (i < player.shopSlot)
            {
                var slot = slots[i];
                var item = selectedItems[i];

                Image itemImage = slot.GetComponent<Image>();
                Image grid = slot.transform.Find("grid").GetComponent<Image>();
                TMP_Text itemPrice = slot.transform.Find("alpha").GetComponent<TMP_Text>();
                TMP_Text itemRank = slot.transform.Find("rank").GetComponent<TMP_Text>();

                if (item.GetItemType() == ItemType.Equipment)
                {
                    Equipment equipment = (Equipment)selectedItems[i];

                    itemImage.sprite = DataManager.Instance.equipmentIcons[equipment.index];
                    grid.sprite = DataManager.Instance.itemGrids[equipment.rank];
                    itemPrice.text = equipment.price + "α";
                    itemRank.text = equipment.GetRank();
                }
                else if (item.GetItemType() == ItemType.Consumable)
                {
                    Consumable consumable = (Consumable)item;

                    itemImage.sprite = DataManager.Instance.consumableIcons[consumable.itemIdx];
                    grid.sprite = DataManager.Instance.itemGrids[consumable.rank];
                    itemPrice.text = consumable.price + "α";
                    itemRank.text = consumable.GetRank();
                }
            }
            else
            {
                Transform locked = slots[i].transform.Find("locked");
                if (locked != null)
                {
                    locked.gameObject.SetActive(true);
                    slots[i].GetComponent<Button>().interactable = false;
                }
            }
        }
    }


    public void OnClickSlotButton(int slot)
    {

        Image image = selected.transform.Find("background").Find("image").GetComponent<Image>();
        Image grid = selected.transform.Find("background").Find("grid").GetComponent<Image>();
        TMP_Text name = selected.transform.Find("name").GetComponent<TMP_Text>();
        TMP_Text rank = selected.transform.Find("rank").GetComponent<TMP_Text>();
        TMP_Text part = selected.transform.Find("part").GetComponent<TMP_Text>();
        TMP_Text info = selected.transform.Find("info").GetComponent<TMP_Text>();


        Item item = selectedItems[slot];
        selectedNumber = slot;

        if (item == null)
        {
            Logger.LogWarning("슬롯 버튼에 해당된 아이템이 존재하지 않습니다.");
            return;
        }

        ActiveChecked(slot);


        switch (item.GetItemType())
        {
            case ItemType.Consumable:
                Consumable consumable = (Consumable)item;
                selectedItem = consumable;

                image.sprite = DataManager.Instance.consumableIcons[consumable.itemIdx];
                grid.sprite = DataManager.Instance.itemGrids[consumable.rank];
                name.text = consumable.name;
                rank.text = consumable.GetRank() + "등급";
                part.text = "/ " + consumable.GetCategory();
                info.text = consumable.GetConvertedScript(GameManager.Instance.player);
                break;
            case ItemType.Equipment:
                Equipment equipment = (Equipment)item;
                selectedItem = equipment;

                image.sprite = DataManager.Instance.equipmentIcons[equipment.index];
                grid.sprite = DataManager.Instance.itemGrids[equipment.rank];
                name.text = equipment.name;
                rank.text = equipment.GetRank() + "등급";
                part.text = "/ " + equipment.GetCategory();
                info.text = equipment.script;
                break;
            default:
                Logger.LogWarning("알 수 없는 아이템 타입(무기, 소비)");
                break;
        }
    }


    private void ActiveChecked(int index)
    {
        for (int i = 0; i < 3; ++i)
        {
            slots[i].transform.Find("checked").gameObject.SetActive(index == i);
        }
    }

    // ComparisonPopupController에서 호출 중
    public void ResetSlotAfterPurchase()
    {
        slots[selectedNumber].transform.Find("checked").gameObject.SetActive(false);
        slots[selectedNumber].GetComponent<Image>().sprite = Resources.Load<Sprite>("slotBack");
        slots[selectedNumber].GetComponent<Button>().interactable = false;
        slots[selectedNumber].transform.Find("alpha").GetComponent<TMP_Text>().text = "";
        slots[selectedNumber].transform.Find("rank").GetComponent<TMP_Text>().text = "";
    }
}
