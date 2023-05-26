﻿using System;
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
            slotView.Init(i, this);
        }
    }

    private void OnDestroy()
    {
        if (container == null)
        {
            container.InventoryUpdated -= UpdateContainerView;
        }
    }

    public void BindTo(IItemContainer container)
    {
        if (this.container != null)
        {
            container.InventoryUpdated -= UpdateContainerView;
        }

        this.container = container;

        UpdateContainerView();

        container.InventoryUpdated += UpdateContainerView;
    }

    private void UpdateContainerView()
    {
        for (int i = 0; i < container.SlotCount; i++)
        {
            var stack = container[i];
            slotViews[i].Display(stack);
        }
    }

    public void TryMoveItem(int sourceIndex, ItemContainerView targetContainerView, int targetIndex)
    {
        var targetContainer = targetContainerView.container;
        var otherStack = targetContainer[targetIndex];
        var myStack = container[sourceIndex];

        if (otherStack.IsEmpty)
        {
            targetContainer[targetIndex] = myStack;
            container[sourceIndex] = ItemStack.Empty;
        }
        else
        {
            if (otherStack.Item == myStack.Item)
            {
                container[sourceIndex] = ItemStack.Empty;
                targetContainer[targetIndex] = myStack.CombineWith(otherStack);
            }
            else
            {
                container[sourceIndex] = otherStack;
                targetContainer[targetIndex] = myStack;
            }
        }
    }
}
