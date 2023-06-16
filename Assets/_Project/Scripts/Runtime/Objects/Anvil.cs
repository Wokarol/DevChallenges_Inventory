using UnityEngine;
using UnityEngine.EventSystems;

public class Anvil : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private AnvilView anvilView;
    [SerializeField] private AnvilRecipeList recipeList;

    public AnvilView View => anvilView;

    public BasicContainer OreInputContainer { get; private set; }
    public BasicContainer FuelInputContainer { get; private set; }
    public BasicContainer OutputContainer { get; private set; }

    public void OnPointerClick(PointerEventData eventData)
    {
        FindObjectOfType<Player>().OpenAnvil(this);
    }
}
