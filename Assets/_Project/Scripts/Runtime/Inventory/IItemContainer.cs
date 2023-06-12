using System;
using System.Collections.Generic;

public interface IItemContainer
{
    ItemStack this[int index] { get; set; }

    int SlotCount { get; }

    event Action InventoryUpdated;

    bool AcceptsItem(Item item);
    bool CanTakeStack(ItemStack stack, bool onlyFully = false);
    int MoveAllSimilarItemsToSlot(int heldItemIndex);
    void SortItems();
    void TakeStack(ItemStack myStack, out ItemStack remainingStack);
    bool Contains(ItemStack stack);
    void Remove(ItemStack stack);
}