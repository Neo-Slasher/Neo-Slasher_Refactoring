using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] NightManager nightManager;

    [Header("Timer UI")]
    [SerializeField] GameObject timerParent; // 사용 안하는 중
    [SerializeField] Image timerImage; // 사용 안하는 중
    [SerializeField] TextMeshProUGUI timerText;

    [Header("Timer Settings")]
    [SerializeField] private int startTime = 60;
    public int timerCount { get; private set; }

    private Coroutine timerCoroutine;

    private void Start()
    {
        StartTimer();
    }

    void StartTimer()
    {
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);

        timerCount = startTime;
        UpdateTimerUI();
        timerCoroutine = StartCoroutine(PlayTimerCoroutine());
    }

    IEnumerator PlayTimerCoroutine()
    {
        while (timerCount >= 0)
        {
            if (nightManager.isStageEnd) break;

            UpdateTimerUI();

            if (timerCount == 0)
            {
                nightManager.SetStageEnd();
                break;
            }

            yield return new WaitForSeconds(1f);
            timerCount--;
        }
    }

    public void StopTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
            timerText.text = timerCount.ToString();
    }
}
