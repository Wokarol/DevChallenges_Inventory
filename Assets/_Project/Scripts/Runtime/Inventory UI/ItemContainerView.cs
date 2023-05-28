using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ItemContainerView : MonoBehaviour
{
    enum InteractionState
    {
        None,
        HoldingItem,
    }

    private InventoryHelpers helpers;

    private IItemContainer container;
    private ItemSlotView[] slotViews = null;

    private InteractionState currentState = InteractionState.None;
    private int heldItemIndex = -1;
    private ItemStackView draggedImage;

    private void Awake()
    {
        helpers = GetComponentInParent<InventoryHelpers>();

        slotViews = GetComponentsInChildren<ItemSlotView>();
        for (int i = 0; i < slotViews.Length; i++)
        {
            var slotView = slotViews[i];
            slotView.Display(null);
            slotView.Init(i, this);
        }
    }

    private void Update()
    {
        if (currentState == InteractionState.HoldingItem)
        {
            draggedImage.transform.position = Vector2.Lerp(draggedImage.transform.position, Input.mousePosition, 0.2f);
        }
    }

    private void OnDestroy()
    {
        if (container != null)
        {
            //Debug.Log($"Unsubscribing {name}[{gameObject.GetInstanceID()}] from {((Component)this.container).name}[{((Component)this.container).gameObject.GetInstanceID()}]");
            container.InventoryUpdated -= UpdateContainerView;
        }
    }

    public void BindTo(IItemContainer container)
    {
        if (this.container != null)
        {
            //Debug.Log($"Unsubscribing {name}[{gameObject.GetInstanceID()}] from {((Component)this.container).name}[{((Component)this.container).gameObject.GetInstanceID()}]");
            container.InventoryUpdated -= UpdateContainerView;
        }

        this.container = container;

        UpdateContainerView();

        //Debug.Log($"Subscribing {name}[{gameObject.GetInstanceID()}] to {((Component)this.container).name}[{((Component)this.container).gameObject.GetInstanceID()}]");
        container.InventoryUpdated += UpdateContainerView;
    }

    public void OnSlotClick(int index, Vector3 slotPosition)
    {
        if (container[index].IsEmpty) return;
        SwitchState(InteractionState.HoldingItem);
        heldItemIndex = index;

        draggedImage = helpers.BorrowUIItem();
        draggedImage.transform.position = slotPosition;

        draggedImage.Display(container[index]);
        UpdateContainerView();

        helpers.AwaitClick(ClickedWhileDragging);
    }

    public void TryMoveItem(int sourceIndex, ItemContainerView targetContainerView, int targetIndex)
    {
        var targetContainer = targetContainerView.container;
        var otherStack = targetContainer[targetIndex];
        var myStack = container[sourceIndex];

        if (container == targetContainer && sourceIndex == targetIndex) return;

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

    private void ClickedWhileDragging(RectTransform clickedTarget)
    {
        var otherSlot = clickedTarget == null 
            ? null 
            : clickedTarget.GetComponentInParent<ItemSlotView>();

        if (otherSlot != null)
        {
            TryMoveItem(heldItemIndex, otherSlot.Owner, otherSlot.Index);
        }

        helpers.ReturnUIItem(draggedImage);
        draggedImage = null;

        SwitchState(InteractionState.None);
        UpdateContainerView();
    }

    private void UpdateContainerView()
    {
        for (int i = 0; i < container.SlotCount; i++)
        {
            if (heldItemIndex == i)
            {
                slotViews[i].Display(ItemStack.Empty);
                continue;
            }

            var stack = container[i];
            slotViews[i].Display(stack);
        }
    }

    private void SwitchState(InteractionState newState)
    {
        var oldState = currentState;
        currentState = newState;

        if (newState == InteractionState.None)
        {
            heldItemIndex = -1;
        }
    }
}
