using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerManager : MonoBehaviour
{    
    public static TimerManager Instance { get; private set; }

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

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // 시간을 활용하는 특성(초전도 등)이 Start에서 timerCount를 참조하므로 Awake에서 시간을 세팅
        timerCount = startTime;
    }



    private void Start()
    {
        Debug.Assert(NightManager.Instance != null, "NightManager 참조 필요");
        Debug.Assert(timerText != null, "Timer Text 참조 필요");
        StartTimer();
    }

    void StartTimer()
    {
        StopTimer();
        timerCoroutine = StartCoroutine(TimerCoroutine());
    }

    IEnumerator TimerCoroutine()
    {
        while (timerCount >= 0)
        {
            if (NightManager.Instance.isStageEnd) break;

            if (timerCount == 0)
            {
                NightManager.Instance.SetStageEnd();
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
