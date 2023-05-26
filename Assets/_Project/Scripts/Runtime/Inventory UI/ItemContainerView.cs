using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainerView : MonoBehaviour
{
    private IItemContainer container;
    private ItemSlotView[] slotViews = null;

    private void Awake()
    {
        slotViews = GetComponentsInChildren<ItemSlotView>();
        for (int i = 0; i < slotViews.Length; i++)
        {
            var slotView = slotViews[i];
            slotView.Display(null);
            slotView.Init(i);
        }
    }

    public void BindTo(IItemContainer container)
    {
        this.container = container;

        for (int i = 0; i < container.SlotCount; i++)
        {
            var stack = container[i];
            slotViews[i].Display(stack);
        }
    }
}
