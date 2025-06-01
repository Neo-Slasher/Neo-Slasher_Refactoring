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

    private float velocity = 0.0f;
    private float shieldMinVelocity = 0f;
    private float shieldMaxVelocity = 0f;


    // �ӽ÷� Update �Լ����� UpdateBar�� ȣ�� �� 
    // �����丵 �� UpdateBar�� ������ �κ��� Ȯ�εǸ� �߰� �� ������ ��
    // �̹�Ʈ �������� �����ص� ����
    private void Update()
    {
        UpdateBar();
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
            hpBar.fillAmount = Mathf.SmoothDamp(hpBar.fillAmount, hpFill, ref velocity, 0.3f);

            float shieldRatio = (float)(curShield / maxHp);
            float targetMinX = hpBar.fillAmount;
            float targetMaxX = hpBar.fillAmount + shieldRatio;

            float currentMinX = shieldBarRect.anchorMin.x;
            float currentMaxX = shieldBarRect.anchorMax.x;

            shieldBarRect.anchorMin = new Vector2(
                Mathf.SmoothDamp(currentMinX, targetMinX, ref shieldMinVelocity, 0.3f), 0f);
            shieldBarRect.anchorMax = new Vector2(
                Mathf.SmoothDamp(currentMaxX, targetMaxX, ref shieldMaxVelocity, 0.3f), 1f);
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
