using UnityEngine;
using UnityEngine.EventSystems;

public class ItemGiver : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Item item;

    public void OnPointerClick(PointerEventData eventData)
    {
        FindObjectOfType<Player>().GiveItem(item, 1);
    }
}
