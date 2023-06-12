using UnityEngine;
using UnityEngine.EventSystems;

public class CraftingTable : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private int craftingInputSlots = 3;
    [SerializeField] private int craftingOutputSlots = 1;
    [Space]
    [SerializeField] private CraftingTableView craftingTableView;

    public CraftingTableView View => craftingTableView;

    public BasicContainer InputContainer { get; private set; }
    public BasicContainer OutputContainer { get; private set; }

    private void Awake()
    {
        InputContainer = new(craftingInputSlots);
        OutputContainer = new(craftingOutputSlots);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        FindObjectOfType<Player>().OpenCraftingTable(this);
    }
}
