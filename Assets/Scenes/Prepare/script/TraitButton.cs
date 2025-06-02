using UnityEngine;
using UnityEngine.UI;

// 2025.06.03 Refactoring Final Version
public class TraitButton : MonoBehaviour
{
    [SerializeField] private Image traitImage;

    public void SetTraitSprite(Sprite sprite)
    {
        traitImage.sprite = sprite;
    }
}
