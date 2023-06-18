using UnityEngine;
using UnityEngine.EventSystems;

public class Grindstone : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GrindstoneView grindstoneView;

    public GrindstoneView View => grindstoneView;

    public void OnPointerClick(PointerEventData eventData)
    {
        FindObjectOfType<Player>().OpenGrindstone(this);
    }
}
