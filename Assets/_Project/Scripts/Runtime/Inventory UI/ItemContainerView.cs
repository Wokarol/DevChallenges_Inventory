using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemContainerView : MonoBehaviour
{
    enum InteractionState
    {
        None,
        HoldingItem,
    }

    readonly struct MoveResult
    { 
        enum MoveResultType { Failed, Moved, PartiallyMoved }

        private readonly MoveResultType type;
        public readonly int LeftInHand;

        private MoveResult(MoveResultType type, int leftInHand)
        {
            this.type = type;
            LeftInHand = leftInHand;
        }

        public readonly bool HasFailed => type == MoveResultType.Failed;
        public readonly bool WasMoved => type == MoveResultType.Moved;
        public readonly bool WasPartiallyMoved => type == MoveResultType.PartiallyMoved;


        public static readonly MoveResult Failed = new(MoveResultType.Failed, 0);
        public static readonly MoveResult Moved = new(MoveResultType.Moved, 0);
        public static MoveResult PartiallyMoved(int leftInHand) => new(MoveResultType.PartiallyMoved, leftInHand);
    }

    [SerializeField] private CommonUISettings settings = null;
    [SerializeField] private Button sortButton = null;

    private InventoryHelpers helpers;

    private IItemContainer container;
    private ItemSlotView[] slotViews = null;

    private InteractionState currentState = InteractionState.None;
    private int heldItemIndex = -1;
    private int heldItemCount = 0;
    private ItemStackView draggedImage;
    private float slotClockTimestamp = 0;

    private bool shouldUpdateView = false;
    
    public Func<ItemStack, IItemContainer> OtherContainerFindStrategy;

    public bool OutputOnly { get; set; }
    public int HeldItemCount
    {
        get => heldItemCount; set
        {
            if (heldItemCount == value) return;
            heldItemCount = value;
            OnHeldItemsChanged?.Invoke(heldItemCount);
        }
    }

    public event Action<int> OnHeldItemsChanged;

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

        if (sortButton != null)
        {
            sortButton.onClick.AddListener(SortItems);
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
        slotClockTimestamp = Time.time;

        var stack = container[index];
        if (stack.IsEmpty) return;

        if (!rightClick && Input.GetKey(KeyCode.LeftShift))
        {
            TryMoveStackToOtherContainer(index);
            helpers.DeactivateTooltip();
            MarkContainerViewDirty();
            return;
        }

        SwitchState(InteractionState.HoldingItem);
        heldItemIndex = index;
        HeldItemCount = rightClick ? stack.Count / 2 : stack.Count;

        draggedImage = helpers.BorrowUIItem();
        draggedImage.transform.position = slotPosition;

        RerenderStackInHand();
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
        var myStack = container[sourceIndex];
        var otherContainer = OtherContainerFindStrategy(myStack);

        if (otherContainer != null)
        {
            if (!otherContainer.CanTakeStack(myStack)) return;

            otherContainer.TakeStack(myStack, out var remainingStack);
            container[sourceIndex] = remainingStack; 
        }
    }

    private MoveResult TryMoveItem(int sourceIndex, ItemContainerView targetContainerView, int targetIndex)
    {
        var targetContainer = targetContainerView.container;
        var otherStack = targetContainer[targetIndex];
        var myStack = container[sourceIndex];

        if (!targetContainerView.AcceptsItem(myStack.Item)) return MoveResult.Failed;
        if (container == targetContainer && sourceIndex == targetIndex) return MoveResult.Moved;

        if (otherStack.IsEmpty)
        {
            targetContainer[targetIndex] = myStack.WithCount(HeldItemCount);
            container[sourceIndex] = myStack.Subtract(HeldItemCount);
            return MoveResult.Moved;
        }
        else
        {
            if (otherStack.Item == myStack.Item)
            {
                if (otherStack.IsFull)
                {
                    (container[sourceIndex], targetContainer[targetIndex]) = (otherStack, myStack);
                    return MoveResult.PartiallyMoved(otherStack.Count);
                }

                var itemsInHand = myStack.WithCount(HeldItemCount);
                var newOtherStack = otherStack.CombineWith(itemsInHand, out var itemsLeftInHand);
                var newMyStack = myStack.Subtract(HeldItemCount - itemsLeftInHand.Count);

                targetContainer[targetIndex] = newOtherStack;
                container[sourceIndex] = newMyStack;
                
                return itemsLeftInHand.Count == 0
                    ? MoveResult.Moved
                    : MoveResult.PartiallyMoved(itemsLeftInHand.Count);
            }
            else if (HeldItemCount == myStack.Count)
            {
                (container[sourceIndex], targetContainer[targetIndex]) = (otherStack, myStack);
                return MoveResult.Moved;
            }
        }

        return MoveResult.Failed;
    }

    private bool TryMoveSingleItem(int sourceIndex, ItemContainerView targetContainerView, int targetIndex)
    {
        var targetContainer = targetContainerView.container;
        var otherStack = targetContainer[targetIndex];
        var myStack = container[sourceIndex];

        if (!targetContainerView.AcceptsItem(myStack.Item)) return false;
        if (otherStack.IsFull) return false;

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

    private bool AcceptsItem(Item item)
    {
        if (OutputOnly) return false;
        return container.AcceptsItem(item);
    }

    private void ClickedWhileDragging(RectTransform clickedTarget, PointerEventData pointerData)
    {
        bool isDoubleClick = Time.time < (slotClockTimestamp + settings.DoubleClickInterval);
        bool isLeftClick = pointerData.button == PointerEventData.InputButton.Left;

        var otherSlot = clickedTarget == null
            ? null
            : clickedTarget.GetComponentInParent<ItemSlotView>();

        bool? sameSlot = otherSlot == null 
            ? null 
            : otherSlot.Index == heldItemIndex;

        if (isDoubleClick && isLeftClick && sameSlot == true)
        {
            HeldItemCount = container.MoveAllSimilarItemsToSlot(heldItemIndex);
            RerenderStackInHand();
            helpers.AwaitClick(ClickedWhileDragging);
            return;
        }

        if (otherSlot == null)
        {
            helpers.AwaitClick(ClickedWhileDragging);
            return;
        }

        if (isLeftClick)
        {
            bool movedAllItems = false;
            if (otherSlot != null)
            {
                var moveResult = TryMoveItem(heldItemIndex, otherSlot.Owner, otherSlot.Index);

                if (moveResult.WasMoved)
                    movedAllItems = true;
                else if (moveResult.WasPartiallyMoved)
                {
                    HeldItemCount = moveResult.LeftInHand;
                    RerenderStackInHand();
                }
            }

            if (movedAllItems)
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
                    HeldItemCount -= 1;
                    RerenderStackInHand();
                }
            }

            if (container[heldItemIndex].IsEmpty || HeldItemCount == 0)
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

    private void SortItems()
    {
        container.SortItems();
    }

    private void RerenderStackInHand()
    {
        draggedImage.Display(container[heldItemIndex].WithCount(HeldItemCount));
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
                    Count = stack.Count - HeldItemCount,
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
            HeldItemCount = 0;
        }

        if (newState == InteractionState.None && oldState == InteractionState.HoldingItem)
        {
            helpers.ReturnUIItem(draggedImage);
            draggedImage = null;
        }
    }

    internal void OnSlotEnter(int index)
    {
        if (container[index].IsEmpty) return;

        helpers.ActivateTooltip(container[index].Item);
    }

    internal void OnSlotExit(int _)
    {
        helpers.DeactivateTooltip();
    }
}
