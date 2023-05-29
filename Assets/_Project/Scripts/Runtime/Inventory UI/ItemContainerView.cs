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

    private bool shouldUpdateView = false;
    private IItemContainer otherContainer;

    private void Awake()
    {
        helpers = GetComponentInParent<InventoryHelpers>();

        slotViews = GetComponentsInChildren<ItemSlotView>();
        for (int i = 0; i < slotViews.Length; i++)
        {
            var slotView = slotViews[i];
            slotView.Display(null, false);
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

    private void LateUpdate()
    {
        if (shouldUpdateView) UpdateContainerView();
    }

    private void OnDestroy()
    {
        if (container != null)
        {
            container.InventoryUpdated -= MarkContainerViewDirty;
        }
    }

    public void BindTo(IItemContainer container)
    {
        if (this.container != null)
        {
            container.InventoryUpdated -= MarkContainerViewDirty;
        }

        this.container = container;

        MarkContainerViewDirty();

        container.InventoryUpdated += MarkContainerViewDirty;
    }

    public void OnSlotClick(int index, Vector3 slotPosition, bool rightClick)
    {
        var stack = container[index];
        if (stack.IsEmpty) return;

        if (!rightClick && Input.GetKey(KeyCode.LeftShift))
        {
            TryMoveStackToOtherContainer(index);
            MarkContainerViewDirty();
            return;
        }

        SwitchState(InteractionState.HoldingItem);
        heldItemIndex = index;
        heldItemCount = rightClick ? stack.Count / 2 : stack.Count;

        draggedImage = helpers.BorrowUIItem();
        draggedImage.transform.position = slotPosition;

        draggedImage.Display(new ItemStack() { Item = stack.Item, Count = heldItemCount });
        MarkContainerViewDirty();

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
        MarkContainerViewDirty();
    }

    private void TryMoveStackToOtherContainer(int sourceIndex)
    {
        // TODO: Implement
        // Get hold of the other container... somehow. Probably by adding needed methods in the view interface
    }

    private bool TryMoveItem(int sourceIndex, ItemContainerView targetContainerView, int targetIndex)
    {
        var targetContainer = targetContainerView.container;
        var otherStack = targetContainer[targetIndex];
        var myStack = container[sourceIndex];

        if (container == targetContainer && sourceIndex == targetIndex) return true;

        if (otherStack.IsEmpty)
        {
            targetContainer[targetIndex] = myStack.WithCount(heldItemCount);
            container[sourceIndex] = myStack.Subtract(heldItemCount);
            return true;
        }
        else
        {
            if (otherStack.Item == myStack.Item)
            {
                targetContainer[targetIndex] = otherStack.CombineWith(myStack.WithCount(heldItemCount));
                container[sourceIndex] = myStack.Subtract(heldItemCount);
                return true;
            }
            else if (heldItemCount == myStack.Count)
            {
                (container[sourceIndex], targetContainer[targetIndex]) = (otherStack, myStack);
                return true;
            }
        }

        return false;
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

        if (otherSlot == null)
        {
            helpers.AwaitClick(ClickedWhileDragging);
            return;
        }

        if (pointerData.button == PointerEventData.InputButton.Left)
        {
            bool movedItem = false;
            if (otherSlot != null)
            {
                movedItem = TryMoveItem(heldItemIndex, otherSlot.Owner, otherSlot.Index);
            }

            if (movedItem)
                SwitchState(InteractionState.None);
            else
                helpers.AwaitClick(ClickedWhileDragging);
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

            if (container[heldItemIndex].IsEmpty || heldItemCount == 0)
            {
                SwitchState(InteractionState.None);
            }
            else
            {
                helpers.AwaitClick(ClickedWhileDragging);
            }
        }

        MarkContainerViewDirty();
    }

    private void MarkContainerViewDirty()
    {
        shouldUpdateView = true;
    }
    private void UpdateContainerView()
    {
        shouldUpdateView = false;
        for (int i = 0; i < container.SlotCount; i++)
        {
            var stack = container[i];

            if (heldItemIndex == i)
            {
                slotViews[i].Display(new ItemStack()
                {
                    Item = stack.Item,
                    Count = stack.Count - heldItemCount,
                }, true);
                continue;
            }

            slotViews[i].Display(stack, false);
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
