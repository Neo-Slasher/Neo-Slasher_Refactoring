using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBackImage : MonoBehaviour
{

    public Transform character;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyThisObject());
    }

    private void Update()
    {
        this.transform.localPosition = character.localPosition;
    }

    IEnumerator DestroyThisObject()
    {
        yield return new WaitForSeconds(0.2f);
        Destroy(this.gameObject);
    }
}
