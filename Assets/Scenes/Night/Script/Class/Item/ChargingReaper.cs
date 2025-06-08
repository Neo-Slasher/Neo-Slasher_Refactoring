using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargingReaper : MonoBehaviour
{
    [SerializeField]
    GameObject reaperAfterImage;
    public bool isAttack = false;
    public int chargingGauge = 100;
    public float reaperAttackDamaege; //임시
    int itemRank;

    public void SetItemRank(int getRank)
    {
        itemRank = getRank;
        SetChargingReaperData();
    }

    void SetChargingReaperData()
    {
        switch (itemRank)
        {
            case 0:
                reaperAttackDamaege = 13;
                break;
            case 1:
                reaperAttackDamaege = 18;
                break;
            case 2:
                reaperAttackDamaege = 27;
                break;
            case 3:
                reaperAttackDamaege = 42;
                break;
        }
    }

    public bool IsChargingGaugeFull()
    {
        if (chargingGauge < 100)
        {
            return false;
        }
        else
        {
            //StartCoroutine(ReaperAfterImageCoroutine());
            this.gameObject.SetActive(true);
            return true;
        }
    }

    public void ReaperUse()
    {
        this.gameObject.SetActive(false);
        //리퍼 사용
    }

    public void ChargingGauge()
    {
        chargingGauge += 5;
    }

    IEnumerator ReaperAfterImageCoroutine()
    {
        reaperAfterImage.SetActive(false) ;
        yield return new WaitForSeconds(0.1f);
        reaperAfterImage.SetActive(true);
    }

    public IEnumerator SetCoolTime(Image coolTimeImage)
    {
        yield return new WaitUntil(() => !this.gameObject.activeSelf);
        coolTimeImage.gameObject.SetActive(true);

        while (coolTimeImage.fillAmount >= 0)
        {
            coolTimeImage.fillAmount = 1 - ((float)chargingGauge / 100);
            yield return null;
        }
    }
}
