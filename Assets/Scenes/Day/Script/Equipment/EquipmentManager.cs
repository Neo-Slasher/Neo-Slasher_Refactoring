using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Unity.VisualScripting;

public class EquipmentManager : MonoBehaviour
{
    public Button[] equipment_buttons;
    public Button[] item_buttons;

    // Selected 관련
    public GameObject selected;


    public void Start()
    {
        LoadPlayerItems();
    }

    public void OnEnable()
    {
        LoadPlayerItems();
    }

    public void LoadPlayerItems()
    {
        Player player = GameManager.instance.player;

        for (int i = 0; i < player.equipment.Length; i++)
        {
            if (player.equipment[i].index == 0)
                continue;

            Equipment equipment = player.equipment[i];


            Sprite sprite = Resources.Load<Sprite>("Equip/" + equipment.name);
            if (sprite != null)
                equipment_buttons[i].image.sprite = sprite;
            else
                Debug.LogWarning($"스프라이트를 찾을 수 없습니다: Equip/{equipment.name}");


            Transform rankTransform = equipment_buttons[i].transform.Find("rank");
            if (rankTransform != null)
            {
                TMP_Text rankText = rankTransform.GetComponent<TMP_Text>();
                if (rankText != null)
                {
                    string rank = equipment.GetRank();
                    rankText.text = rank ?? "";
                }
                else
                {
                    Debug.LogWarning("rank 오브젝트에 TMP_Text 컴포넌트가 없습니다.");
                }
            }
            else
            {
                Debug.LogWarning("rank 오브젝트를 찾을 수 없습니다.");
            }
        }


        int max_consumable_count = 3;
        for (int i = 0; i < max_consumable_count; i++)
        {
            if (i < player.itemSlot)
            {
                if (player.item[i].itemIdx == 0)
                {
                    item_buttons[i].interactable = false;
                    continue;
                }

                item_buttons[i].interactable = true;
                Consumable consumable = player.item[i];

                // null 체크
                if (consumable != null)
                {
                    Sprite sprite = Resources.Load<Sprite>("Item/" + consumable.name);
                    if (sprite != null)
                        item_buttons[i].image.sprite = sprite;
                    else
                        Debug.LogWarning($"스프라이트 없음: Item/{consumable.name}");

                    Transform rankTransform = item_buttons[i].transform.Find("rank");
                    if (rankTransform != null)
                    {
                        TMP_Text rankText = rankTransform.GetComponent<TMP_Text>();
                        if (rankText != null)
                            rankText.text = consumable.GetRank();
                    }
                }

                Transform lockedTransform = item_buttons[i].transform.Find("locked");
                if (lockedTransform != null)
                    lockedTransform.gameObject.SetActive(false);

            }
            else
            {
                item_buttons[i].interactable = false;
                Transform lockedTransform = item_buttons[i].transform.Find("locked");
                if (lockedTransform != null)
                    lockedTransform.gameObject.SetActive(true);
                else
                    Debug.LogWarning("아이템 잠금 표시를 찾을 수 없습니다.");
            }
        }
    }

    public void OnClickConsumableSlot(int slotNumber)
    {
        ActiveConsumableButtonSelected(slotNumber);

        Image image = selected.transform.Find("background").Find("image").GetComponent<Image>();
        TMP_Text name = selected.transform.Find("name").GetComponent<TMP_Text>();
        TMP_Text rank = selected.transform.Find("rank").GetComponent<TMP_Text>();
        TMP_Text part = selected.transform.Find("part").GetComponent<TMP_Text>();
        TMP_Text info = selected.transform.Find("info").GetComponent<TMP_Text>();

        Consumable consumable = GameManager.instance.player.item[slotNumber];
        image.sprite = Resources.Load<Sprite>("Item/" + consumable.name);
        name.text = consumable.name;
        rank.text = consumable.GetRank() + "등급";
        part.text = consumable.GetCategory();
        info.text = consumable.GetConvertedScript(GameManager.instance.player);
    }

    public void OnClickEquipmentSlot(int slotNumber)
    {
        ActiveEquipmentButtonSelected(slotNumber);

        Image image = selected.transform.Find("background").Find("image").GetComponent<Image>();
        TMP_Text name = selected.transform.Find("name").GetComponent<TMP_Text>();
        TMP_Text rank = selected.transform.Find("rank").GetComponent<TMP_Text>();
        TMP_Text part = selected.transform.Find("part").GetComponent<TMP_Text>();
        TMP_Text info = selected.transform.Find("info").GetComponent<TMP_Text>();

        Equipment equipment = GameManager.instance.player.equipment[slotNumber];
        image.sprite = Resources.Load<Sprite>("Equip/" + equipment.name);
        name.text = equipment.name;
        rank.text = equipment.GetRank() + "등급";
        part.text = equipment.GetCategory();
        info.text = equipment.script;
    }


    private void ActiveEquipmentButtonSelected(int slotNumber)
    {
        for (int i = 0; i < equipment_buttons.Length; i++)
        {
            if (i == slotNumber)
            {
                equipment_buttons[i].transform.Find("selected").gameObject.SetActive(true);
            }
            else
            {
                equipment_buttons[i].transform.Find("selected").gameObject.SetActive(false);
            }
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
            if (i == slotNumber)
            {
                item_buttons[i].transform.Find("selected").gameObject.SetActive(true);
            }
            else
            {
                item_buttons[i].transform.Find("selected").gameObject.SetActive(false);
            }
        }
    }

}
