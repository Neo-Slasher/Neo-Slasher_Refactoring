using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TraitButton : MonoBehaviour
{
    public PreparationTraitManager traitManager;
    public PreparationManager preparationManager;
    [SerializeField] private Image traitImage;

    public void SetTraitSprite(Sprite sprite)
    {
        traitImage.sprite = sprite;
    }
}
