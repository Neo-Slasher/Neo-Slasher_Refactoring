using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// 2025.06.05 Refactoring Final Version
public class AssassinationManager : MonoBehaviour
{
    [SerializeField] private FightingPower FightingPower;

    [SerializeField] private GameObject[] StepButtons;

    [SerializeField] private TMP_Text[] StageRecCps;
    [SerializeField] private TMP_Text[] CPGaps;
    [SerializeField] private GameObject[] CheckMarks;
    [SerializeField] private TMP_Text[] assassinationInfos;

    private void Start()
    {
        InitStepData();
    }

    private void InitStepData()
    {
        for (int i = 0; i < StepButtons.Length; i++)
        {
            int stageRecCP = DataManager.Instance.assassinationStageList.assassinationStage[i].stageRecCP;

            int currentCP = FightingPower.currentCP;


            int diffCP = currentCP - stageRecCP;
            double cpGap = ((double)(currentCP - stageRecCP) / stageRecCP) * 100;


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


    private void InitAssassinationStageInfomation(int i)
    {
        var stageData = DataManager.Instance.assassinationStageList.assassinationStage[i];

        int normalReward = stageData.normalReward;
        int eliteReward = stageData.eliteReward;
        int stageDropRank = stageData.stageDropRank;

        string dropRank = stageData.stageDropRank switch
        {
            <= 7 => "C",
            >= 8 and <= 16 => "B",
            >= 17 and <= 30 => "A",
            >= 31 => "S",
        };

        assassinationInfos[i].text = $"-일반 적 현상금 : {normalReward}α\n" + $"-정예 적 현상금 : {eliteReward}α\n-" + dropRank + "등급 아이템 드롭";
    }

    public void OnClickGoNightButton()
    {
        SoundManager.Instance.PlaySFX(SoundManager.Instance.buttonClip);
        Player.Save(GameManager.Instance.player);
        SceneManager.LoadScene("NightScene");
    }


    public void OnClickStepButton(int step)
    {
        SoundManager.Instance.PlaySFX(SoundManager.Instance.buttonClip);
        GameManager.Instance.player.assassinationCount = step;
        for (int i = 0; i < StepButtons.Length; i++)
        {
            CheckMarks[i].SetActive(i == step);
        }
    }
}
