using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;

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
    private int heldItemCount = 0;
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

    public void OnSlotClick(int index, Vector3 slotPosition, bool rightClick)
    {
        var stack = container[index];
        if (stack.IsEmpty) return;

        SwitchState(InteractionState.HoldingItem);
        heldItemIndex = index;
        heldItemCount = rightClick ? stack.Count / 2 : stack.Count;

        draggedImage = helpers.BorrowUIItem();
        draggedImage.transform.position = slotPosition;

        draggedImage.Display(new ItemStack() { Item = stack.Item, Count = heldItemCount });
        UpdateContainerView();

        helpers.AwaitClick(ClickedWhileDragging);
    }

    public bool IsIdle()
    {
        return currentState == InteractionState.None;
    }

    public void AbortInteraction()
    {
        SwitchState(InteractionState.None);
        helpers.AwaitClickAbort();
        UpdateContainerView();
    }

    private void TryMoveItem(int sourceIndex, ItemContainerView targetContainerView, int targetIndex)
    {
        var targetContainer = targetContainerView.container;
        var otherStack = targetContainer[targetIndex];
        var myStack = container[sourceIndex];

        if (container == targetContainer && sourceIndex == targetIndex) return;

        if (otherStack.IsEmpty)
        {
            targetContainer[targetIndex] = myStack.WithCount(heldItemCount);
            container[sourceIndex] = myStack.Subtract(heldItemCount);
        }
        else
        {
            if (otherStack.Item == myStack.Item)
            {
                targetContainer[targetIndex] = otherStack.CombineWith(myStack.WithCount(heldItemCount));
                container[sourceIndex] = myStack.Subtract(heldItemCount);
            }
            else
            {
                (container[sourceIndex], targetContainer[targetIndex]) = (otherStack, myStack);
            }
        }
    }

    private bool TryMoveSingleItem(int sourceIndex, ItemContainerView targetContainerView, int targetIndex)
    {
        var targetContainer = targetContainerView.container;
        var otherStack = targetContainer[targetIndex];
        var myStack = container[sourceIndex];

        if (container == targetContainer && sourceIndex == targetIndex) return false;

        if (otherStack.IsEmpty)
        {
            targetContainer[targetIndex] = myStack.WithCount(1);
            container[sourceIndex] = myStack.Subtract(1);
            return true;
        }
        else
        {
            if (otherStack.Item == myStack.Item)
            {
                targetContainer[targetIndex] = otherStack.Add(1);
                container[sourceIndex] = myStack.Subtract(1);
                return true;
            }
        }
        return false;
    }

    private void ClickedWhileDragging(RectTransform clickedTarget, PointerEventData pointerData)
    {
        var otherSlot = clickedTarget == null
            ? null
            : clickedTarget.GetComponentInParent<ItemSlotView>();

        if (pointerData.button == PointerEventData.InputButton.Left)
        {
            if (otherSlot != null)
            {
                TryMoveItem(heldItemIndex, otherSlot.Owner, otherSlot.Index);
            }

            SwitchState(InteractionState.None);
        }
        else
        {
            if (otherSlot != null)
            {
                bool isTheSameSlot = otherSlot.Owner.container == container && otherSlot.Index == heldItemIndex;
                bool movedItem = isTheSameSlot
                    ? true
                    : TryMoveSingleItem(heldItemIndex, otherSlot.Owner, otherSlot.Index);

                if (movedItem)
                {
                    heldItemCount -= 1;
                    draggedImage.Display(container[heldItemIndex].WithCount(heldItemCount));
                }
            }

            if (container[heldItemIndex].IsEmpty)
            {
                SwitchState(InteractionState.None);
            }
            else
            {
                helpers.AwaitClick(ClickedWhileDragging);
            }
        }

        UpdateContainerView();
    }

    private void UpdateContainerView()
    {
        for (int i = 0; i < container.SlotCount; i++)
        {
            var stack = container[i];

            if (heldItemIndex == i)
            {
                slotViews[i].Display(new ItemStack()
                {
                    Item = stack.Item,
                    Count = stack.Count - heldItemCount,
                });
                continue;
            }

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

        if (newState == InteractionState.None && oldState == InteractionState.HoldingItem)
        {
            helpers.ReturnUIItem(draggedImage);
            draggedImage = null;
        }
    }
}
