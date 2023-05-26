using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlotView : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private ItemStackView stackView;

    private int index = -1;
    private bool isHeld = false;
    private InventoryHelpers helpers;

    private ItemStackView draggedImage = null;

    private void Awake()
    {
        helpers = GetComponentInParent<InventoryHelpers>();
    }

    private void Update()
    {
        if (isHeld)
        {
            draggedImage.transform.position = Vector2.Lerp(draggedImage.transform.position, Input.mousePosition, 0.2f);
        }
    }

    public void Init(int index)
    {
        this.index = index;
    }

    public void Display(ItemStack? stack)
    {
        stackView.Display(stack);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        isHeld = true;
        stackView.gameObject.SetActive(false);

        draggedImage = helpers.BorrowUIItem();
        draggedImage.transform.position = stackView.transform.position;
        draggedImage.Display(stackView.ShownStack);

        helpers.AwaitClick(ClickedWhileDragging);
    }

    private void ClickedWhileDragging()
    {
        helpers.ReturnUIItem(draggedImage);
        draggedImage = null;

        isHeld = false;
        stackView.gameObject.SetActive(true);
    }
}
