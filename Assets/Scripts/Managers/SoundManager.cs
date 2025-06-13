using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;


// 2025.06.02 Refactoring Final Version
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("Mixer")]
    public AudioMixer audioMixer;

    private Slider bgmSlider;
    private Slider sfxSlider;

    public AudioClip buttonClip;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (bgmSource == null || sfxSource == null || audioMixer == null)
            {
                Logger.LogError("SoundManager: AudioSource �Ǵ� AudioMixer�� �Ҵ���� �ʾҽ��ϴ�!");
                enabled = false;
                return;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (GameManager.Instance == null || GameManager.Instance.setting == null)
        {
            Logger.LogError("GameManager �ʱ�ȭ���� ����!");
            return;
        }

        SetBgmVolume(GameManager.Instance.setting.bgmVolume);
        SetSfxVolume(GameManager.Instance.setting.sfxVolume);
    }

    public void RegisterBgmSlider(Slider slider)
    {
        if (bgmSlider != null)
            bgmSlider.onValueChanged.RemoveListener(SetBgmVolume);

        bgmSlider = slider;
        bgmSlider.onValueChanged.AddListener(SetBgmVolume);
        bgmSlider.value = GameManager.Instance.setting.bgmVolume;
        SetBgmVolume(bgmSlider.value);
    }

    public void RegisterSfxSlider(Slider slider)
    {
        if (sfxSlider != null)
            sfxSlider.onValueChanged.RemoveListener(SetSfxVolume);

        sfxSlider = slider;
        sfxSlider.onValueChanged.AddListener(SetSfxVolume);
        sfxSlider.value = GameManager.Instance.setting.sfxVolume;
        SetSfxVolume(sfxSlider.value);
    }

    public void UnregisterBgmSlider()
    {
        if (bgmSlider != null)
            bgmSlider.onValueChanged.RemoveListener(SetBgmVolume);
        bgmSlider = null;
    }

    public void UnregisterSfxSlider()
    {
        if (sfxSlider != null)
            sfxSlider.onValueChanged.RemoveListener(SetSfxVolume);
        sfxSlider = null;
    }
    public void SetBgmVolume(float value)
    {
        audioMixer.SetFloat("BGM", Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f);
        GameManager.Instance.setting.bgmVolume = value;
    }

    public void SetSfxVolume(float value)
    {
        audioMixer.SetFloat("SFX", Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f);
        GameManager.Instance.setting.sfxVolume = value;
    }

    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource.clip == clip && bgmSource.isPlaying)
            return;
        bgmSource.clip = clip;
        bgmSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
}
