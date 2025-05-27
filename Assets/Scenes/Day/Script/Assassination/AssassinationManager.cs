using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AssassinationManager : MonoBehaviour
{
    public FightingPower FightingPower;

    public GameObject[] StepButtons;


    public TMP_Text[] StageRecCps;
    public TMP_Text[] CPGaps;
    public GameObject[] CheckMarks;
    public TMP_Text[] assassinationInfos;

    private void Start()
    {
        InitStepData();
    }

    public void InitStepData()
    {
        for (int i = 0; i < StepButtons.Length; i++)
        {
            int stageRecCP = DataManager.instance.assassinationStageList.assassinationStage[i].stageRecCP;

            int currentCP = FightingPower.currentCP;
            double cpGap = ((double)(currentCP - stageRecCP) / (double)stageRecCP) * 100;

            int diffCP = currentCP - stageRecCP;

            if (cpGap > 10)
                CPGaps[i].text = $"<color=green>{diffCP}</color>";
            else if (cpGap <= 10 && cpGap >= -10)
                CPGaps[i].text = $"<color=yellow>{diffCP}</color>";
            else if (cpGap < -10)
                CPGaps[i].text = $"<color=red>{diffCP}</color>";

            StageRecCps[i].text = stageRecCP.ToString();


            InitAssassinationStageInfomation(i);
        }
    }

    public void InitAssassinationStageInfomation(int i)
    {
        string dropRank = "";
        int normalReward = DataManager.instance.assassinationStageList.assassinationStage[i].normalReward;
        int eliteReward = DataManager.instance.assassinationStageList.assassinationStage[i].eliteReward;
        int stageDropRank = DataManager.instance.assassinationStageList.assassinationStage[i].stageDropRank;

        if (stageDropRank <= 7)
            dropRank = "C";
        else if (stageDropRank >= 8 && stageDropRank <= 16)
            dropRank = "B";
        else if (stageDropRank >= 17 && stageDropRank <= 30)
            dropRank = "A";
        else if (stageDropRank >= 31)
            dropRank = "S";

        assassinationInfos[i].text = $"-일반 적 현상금 : {normalReward}α\n" + $"-정예 적 현상금 : {eliteReward}α\n-" + dropRank + "등급 아이템 드롭";
    }

    public void OnClickGoNightButton()
    {
        Player.Save(GameManager.instance.player);
        SceneManager.LoadScene("NightScene");
    }


    public void OnClickStepButton(int step)
    {
        GameManager.instance.player.assassinationCount = step;

        for (int i = 0; i < StepButtons.Length; i++)
        {
            if (i == step)
                CheckMarks[i].SetActive(true);
            else
                CheckMarks[i].SetActive(false);
        }
    }
}
