using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


// 상점의 아이템 슬롯 3개를 세팅하는 역할
public class ShopManager : MonoBehaviour
{
    public Button[] slots;
    public List<Item> selectedItems = new();
    public GameObject selected;

    public Button comparisonButton;
    public GameObject comparisonPopup;

    public int selectedNumber;
    public Item selectedItem;


    void Start()
    {
        SetShopItems();
    }

    private void SetShopItems()
    {
        Player player = GameManager.Instance.player;

        // 판매할 수 있는 후보 아이템 뽑기
        int shop_min_rank = player.shopMinRank;
        int shop_max_rank = player.shopMaxRank;

        List<Equipment> equipments = new();
        for (int i = 0; i < DataManager.Instance.equipmentList.equipment.Count; i++)
        {
            Equipment equipment = DataManager.Instance.equipmentList.equipment[i];
            if (shop_min_rank <= equipment.rank && equipment.rank <= shop_max_rank)
                equipments.Add(equipment);
        }

        List<Consumable> consumables = new();
        for (int i = 0; i < DataManager.Instance.consumableList.item.Count; i++)
        {
            Consumable consumable = DataManager.Instance.consumableList.item[i];
            if (shop_min_rank <= consumable.rank && consumable.rank <= shop_max_rank)
                consumables.Add(consumable);
        }



        // 후보 중 판매할 아이템 선택
        for (int i = 0; i < player.shopSlot; i++)
        {
            int equip_or_item = UnityEngine.Random.Range(0, 10);

            if (equip_or_item < 6) // 장비의 확률 60%
            {
                Equipment equip;
                do
                {
                    int equip_num = UnityEngine.Random.Range(0, equipments.Count);
                    equip = equipments[equip_num];
                } while (selectedItems.Contains(equip));

                selectedItems.Add(equip);
            }
            else if (6 <= equip_or_item && equip_or_item <= 9) // 소비 아이템 확률 40%
            {
                Consumable consumable;
                do
                {
                    int consumable_num = UnityEngine.Random.Range(0, consumables.Count);
                    consumable = consumables[consumable_num];
                } while (selectedItems.Contains(consumable));

                selectedItems.Add(consumable);
            }
        }


        // 선택된 아이템 UI에 출력
        for (int i = 0; i < 3; i++)
        {
            if (i < player.shopSlot)
            {
                Image item_image = slots[i].gameObject.GetComponent<Image>();
                TMP_Text item_price = slots[i].transform.Find("alpha").gameObject.GetComponent<TMP_Text>();
                TMP_Text item_rank = slots[i].transform.Find("rank").gameObject.GetComponent<TMP_Text>();

                if (selectedItems[i].GetItemType() == ItemType.Equipment)
                {
                    Equipment equipment = (Equipment)selectedItems[i];

                    item_image.sprite = Resources.Load<Sprite>("Equip/" + equipment.name);
                    item_price.text = equipment.price + "α";
                    item_rank.text = equipment.GetRank();
                }
                else if (selectedItems[i].GetItemType() == ItemType.Consumable)
                {
                    Consumable item = (Consumable)selectedItems[i];

                    item_image.sprite = Resources.Load<Sprite>("Item/" + item.name);
                    item_price.text = item.price + "α";
                    item_rank.text = item.GetRank();
                }
            }
            else
            {
                Transform lockedTransform = slots[i].transform.Find("locked");
                if (lockedTransform != null)
                {
                    lockedTransform.gameObject.SetActive(true);
                    slots[i].GetComponent<Button>().interactable = false;
                }
            }
        }
    }


    public void OnClickSlotButton(int slot)
    {
        Image image = selected.transform.Find("background").Find("image").GetComponent<Image>();
        TMP_Text name = selected.transform.Find("name").GetComponent<TMP_Text>();
        TMP_Text rank = selected.transform.Find("rank").GetComponent<TMP_Text>();
        TMP_Text part = selected.transform.Find("part").GetComponent<TMP_Text>();
        TMP_Text info = selected.transform.Find("info").GetComponent<TMP_Text>();


        Item item = selectedItems[slot];
        selectedNumber = slot;

        if (item == null)
        {
            Debug.LogWarning("슬롯 버튼에 해당된 아이템이 존재하지 않습니다.");
            return;
        }

        ActiveChecked(slot);

        if (item.GetItemType() == ItemType.Consumable)
        {
            Consumable consumable = (Consumable)item;
            selectedItem = consumable;

            image.sprite = Resources.Load<Sprite>("Item/" + consumable.name);
            name.text = consumable.name;
            rank.text = consumable.GetRank() + "등급";
            part.text = "/ " + consumable.GetCategory();
            info.text = consumable.GetConvertedScript(GameManager.Instance.player);
        }
        else if (item.GetItemType() == ItemType.Equipment)
        {
            Equipment equipment = (Equipment)item;
            selectedItem = equipment;

            image.sprite = Resources.Load<Sprite>("Equip/" + equipment.name);
            name.text = equipment.name;
            rank.text = equipment.GetRank() + "등급";
            part.text = "/ " + equipment.GetCategory();
            info.text = equipment.script;
        }
        else
        {
            Debug.LogWarning("알 수 없는 아이템 타입(무기, 소비)");
        }
    }


    public void ActiveChecked(int index)
    {
        for (int i = 0; i < 3; ++i)
        {
            if (index == i)
            {
                slots[i].transform.Find("checked").gameObject.SetActive(true);
            }
            else
            {
                slots[i].transform.Find("checked").gameObject.SetActive(false);
            }
        }
    }

    public void ResetSlotAfterPurchase()
    {
        slots[selectedNumber].transform.Find("checked").gameObject.SetActive(false);
        slots[selectedNumber].GetComponent<Image>().sprite = Resources.Load<Sprite>("slotBack");
        slots[selectedNumber].GetComponent<Button>().interactable = false;
        slots[selectedNumber].transform.Find("alpha").GetComponent<TMP_Text>().text = "";
        slots[selectedNumber].transform.Find("rank").GetComponent<TMP_Text>().text = "";
    }

}
