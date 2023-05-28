using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlotView : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private ItemStackView stackView;

    private int index = -1;
    private ItemContainerView owner;

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

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Middle) return;

        bool rightClick = eventData.button == PointerEventData.InputButton.Right;
        owner.OnSlotClick(index, stackView.transform.position, rightClick);
    }
}
