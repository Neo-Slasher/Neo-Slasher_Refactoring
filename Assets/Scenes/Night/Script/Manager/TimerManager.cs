using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerManager : MonoBehaviour
{
    [SerializeField] NightManager nightManager;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] int startTime = 60;

    private WaitForSeconds waitOneSecond = new WaitForSeconds(1f);
    private Coroutine timerCoroutine;
    private int _timerCount;

    public int timerCount
    {
        get => _timerCount;
        private set
        {
            _timerCount = value;
            UpdateTimerUI();
        }
    }




    private void Start()
    {
        Debug.Assert(nightManager != null, "NightManager 참조 필요");
        Debug.Assert(timerText != null, "Timer Text 참조 필요");
        StartTimer();
    }

    void StartTimer()
    {
        StopTimer();
        timerCount = startTime;
        timerCoroutine = StartCoroutine(TimerCoroutine());
    }

    IEnumerator TimerCoroutine()
    {
        while (timerCount > 0)
        {
            if (nightManager.isStageEnd) break;

            if (timerCount == 0)
            {
                nightManager.SetStageEnd();
                break;
            }

            yield return waitOneSecond;
            timerCount--;
        }
    }
    private void UpdateTimerUI()
    {
        if (timerText != null)
            timerText.text = timerCount.ToString();
    }

    public void StopTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }
    }


}
