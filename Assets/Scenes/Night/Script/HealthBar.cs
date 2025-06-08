using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("HP Settings")]
    public Image hpBar;
    public double maxHp = 100;
    public double curHp = 100;


    [Header("Shield Settings")]
    public RectTransform shieldBarRect;
    public Image shieldBar;
    public double curShield = 50;

    [SerializeField] private CharacterHealth CharacterHealth;

    private void OnEnable()
    {
        if (CharacterHealth != null)
        {
            CharacterHealth.OnHpChanged += UpdateBar;
        }
    }

    private void OnDisable()
    {
        if (CharacterHealth != null)
        {
            CharacterHealth.OnHpChanged -= UpdateBar;
        }
    }


    public void UpdateBar()
    {
        maxHp = GameManager.Instance.player.maxHp;
        curHp = GameManager.Instance.player.curHp;
        curShield = GameManager.Instance.player.shieldPoint;

        double totalAmount = curHp + curShield;

        if (totalAmount <= maxHp)
        {
            float hpFill = (float)(curHp / maxHp);
            hpBar.fillAmount = hpFill; 

            float shieldRatio = (float)(curShield / maxHp);
            float targetMinX = hpBar.fillAmount;
            float targetMaxX = hpBar.fillAmount + shieldRatio;

            float currentMinX = shieldBarRect.anchorMin.x;
            float currentMaxX = shieldBarRect.anchorMax.x;

            shieldBarRect.anchorMin = new Vector2(hpFill, 0f);
            shieldBarRect.anchorMax = new Vector2(targetMaxX, 1f);
        }
        else // totalAmount > maxHp (curHp <= maxHp)
        {
            float hpFill = (float)(curHp / totalAmount);
            hpBar.fillAmount = hpFill;

            shieldBarRect.anchorMin = new Vector2(hpFill, 0f);
            shieldBarRect.anchorMax = new Vector2(1f, 1f);
        }

        shieldBar.fillAmount = 1f;
    }
}
