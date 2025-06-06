using UnityEngine;

public class HitBox : MonoBehaviour
{
    public Character character;

    private void Start()
    {
        character = GameObject.FindWithTag("Player").transform.Find("CharacterImage").GetComponent<Character>();
    }
    
    public bool isAttacked;
}
