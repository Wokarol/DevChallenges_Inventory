using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlotView : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private ItemStackView stackView;

    private int index = -1;
    private ItemContainerView owner;

    private ItemStackView draggedImage = null;

    public int Index => index;
    public ItemContainerView Owner => owner;

    public void Init(int index, ItemContainerView owner)
    {
        this.index = index;
        this.owner = owner;
    }

    public void Display(ItemStack? stack)
    {
        stackView.Display(stack);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        owner.OnSlotClick(index, stackView.transform.position);
    }
}
