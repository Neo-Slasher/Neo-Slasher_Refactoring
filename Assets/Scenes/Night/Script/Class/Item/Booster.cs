using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Booster : MonoBehaviour
{
    public SpriteRenderer character;
    [SerializeField] Vector3 leftPos;
    [SerializeField] Vector3 rightPos;

    void Update()
    {
        if(character.flipX == false)
        {
            this.transform.localScale = new Vector3(1, 1, 1);
            this.transform.localPosition = leftPos;
        }
        else
        {
            this.transform.localScale = new Vector3(-1, 1, 1);
            this.transform.localPosition = rightPos;
        }
    }
}
