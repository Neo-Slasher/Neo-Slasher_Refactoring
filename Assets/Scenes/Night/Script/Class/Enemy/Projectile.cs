using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Projectile : MonoBehaviour
{
    public bool isEnemy;
    public float projPower;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isEnemy)
        {
            //적이 쏜 투사체일 경우
            if (collision.name == "BackGround")
            {
                gameObject.SetActive(false);
                if (transform.parent != null)
                    transform.position = transform.parent.position;
            }
            else if(collision.name == "Character")
            {
                gameObject.SetActive(false);
                if (transform.parent != null)
                    transform.position = transform.parent.position;
            }
            
        }
        else
        {
            //내가 쏜 투사체일 경우
            if (collision.name == "BackGround")
            {
                gameObject.SetActive(false);
                if (transform.parent != null)
                    transform.position = transform.parent.parent.GetChild(0).position;
            }
            else if (collision.tag == "Normal" || collision.tag == "Elite")
            {
                gameObject.SetActive(false);
                if (transform.parent != null)
                    transform.position = transform.parent.parent.GetChild(0).position;
            }
        }
    }

    public void SetProjPos()
    {
        if (isEnemy)
        {
            gameObject.SetActive(false);
            if (transform.parent != null)
                transform.position = transform.parent.position;
        }
    }
}
