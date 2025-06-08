using System.Collections;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    private Animator Animator;
    private SpriteRenderer SpriteRenderer;

    private void Awake()
    {
        Animator = GetComponent<Animator>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetBool(string parameterName, bool value)
    {
        Animator.SetBool(parameterName, value);
    }

    public void SetTrigger(string parameterName)
    {
        Animator.SetTrigger(parameterName);
    }

    public void SetFloat(string parameterName, float value)
    {
        Animator.SetFloat(parameterName, value);
    }

    public void PlayDamagedAnimation()
    {
        SetTrigger("knockback");

        ////홀로그램도 같이 움직이도록
        //if (isHologramAnimate)
        //{
        //    for (int i = 0; i < 2; i++)
        //    {
        //        hologramAnimatorArr[i].SetTrigger("knockback");
        //    }
        //}

        StartCoroutine(SetDamagedAnimCoroutine());
    }

    IEnumerator SetDamagedAnimCoroutine()
    {
        for (int i = 0; i < 2; i++)
        {
            Color nowColor = SpriteRenderer.color;
            nowColor.a = 0.7f;
            SpriteRenderer.color = nowColor;
            yield return new WaitForSeconds(0.2f);

            nowColor.a = 1f;
            SpriteRenderer.color = nowColor;
            yield return new WaitForSeconds(0.3f);
        }
    }

    public void LookRight()
    {
        SpriteRenderer.flipX = false;
    }
}
