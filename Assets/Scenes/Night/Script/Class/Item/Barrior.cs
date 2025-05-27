using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrior : MonoBehaviour
{
    [SerializeField]
    Transform barriorImageTransform;
    [SerializeField]
    float createSpeed;
    float startSize = 0;
    float nowSize = 0;
    float maxSize = 1;


    public void CreateBarrior()
    {
        SetBarriorActive(true);
        StartCoroutine(CreateBarriorCoroutine());
    }
    
    //isActive가 참이면 켜짐
    public void SetBarriorActive(bool isActive)
    {
        this.gameObject.SetActive(isActive);
    }

    IEnumerator CreateBarriorCoroutine()
    {
        Vector3 nowScale = Vector3.zero;
        barriorImageTransform.localScale = nowScale;
        nowSize = startSize;
        while (nowSize <= maxSize)
        {
            nowSize += Time.deltaTime * createSpeed;
            nowScale.x = nowSize;
            nowScale.y = nowSize;
            barriorImageTransform.localScale = nowScale;

            yield return null;
        }
    }
}
