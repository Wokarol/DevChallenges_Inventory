using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class BasicContainer : IItemContainer
{
    private ItemStack[] items;
    private Predicate<Item> acceptedItemPredicate;
    private bool ignorePredicateOnDirectInsert;

    public BasicContainer(int slotCount, List<ItemStack> startingItems = null)
    {
        items = new ItemStack[slotCount];

        if (startingItems != null)
        {
            int startingItemsIndex = 0;
            foreach (int i in Enumerable.Range(0, items.Length).OrderBy(v => UnityEngine.Random.value).Take(startingItems.Count))
            {
                items[i] = startingItems[startingItemsIndex];
                startingItemsIndex++;
            } 
        }
    }

    public BasicContainer AcceptsOnly(Predicate<Item> predicate, bool ignoreWhenDirect = false)
    {
        acceptedItemPredicate = predicate;
        ignorePredicateOnDirectInsert = ignoreWhenDirect;
        return this;
    }

    public ItemStack this[int index]
    {
        get => items[index];
        set
        {
            if (value.Item != null && (!AcceptsItem(value.Item) && !ignorePredicateOnDirectInsert)) UnityEngine.Debug.LogError("Container were given an illegal item!");
            if (items[index] == value) return;

            items[index] = value;
            InventoryUpdated?.Invoke();
        }
    }

    public event Action InventoryUpdated;
    public int SlotCount => items.Length;

    public bool CanTakeStack(ItemStack stack, bool onlyFully = false)
    {
        if (!AcceptsItem(stack.Item)) return false;

        if (items.Any(s => s.IsEmpty))
            return true;

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].Item != stack.Item) continue;
            if (items[i].IsFull) continue;

            bool stackFitWhenCombined = (items[i].Count + stack.Count) <= stack.Item.MaxStackSize;

            if (stackFitWhenCombined || !onlyFully) return true;
        }

        return false;
    }

    public void TakeStack(ItemStack stack, out ItemStack remainingStack)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (this[i].Item != stack.Item) continue;
            if (this[i].IsFull) continue;

            this[i] = this[i].CombineWith(stack, out var remainingStackAfterCombine);
            if (remainingStackAfterCombine.IsEmpty)
            {
                remainingStack = ItemStack.Empty;
                return;
            }
            else
            {
                stack = remainingStackAfterCombine;
            }
        }

        // At this point, we should saturate all non-full stacks and have remaining items in hand

        for (int i = 0; i < items.Length; i++)
        {
            if (this[i].IsEmpty)
            {
                this[i] = stack;
                remainingStack = ItemStack.Empty;
                return;
            }
        }

        // At this point we still have items left and no stacks to put them in

        remainingStack = stack;
    }

    public int MoveAllSimilarItemsToSlot(int index)
    {
        var myStack = this[index];

        // We check all non-full stacks first
        for (int i = 0; i < items.Length; i++)
        {
            if (i == index || items[i].IsFull) continue;

            if (myStack.Item == items[i].Item)
            {
                this[index] = this[index].CombineWith(items[i], out var remainingStack);
                items[i] = remainingStack;

                if (!remainingStack.IsEmpty) break;
            }
        }

        // We do... the exact same again, but for full stacks
        for (int i = 0; i < items.Length; i++)
        {
            if (i == index || !items[i].IsFull) continue;

            if (myStack.Item == items[i].Item)
            {
                this[index] = this[index].CombineWith(items[i], out var remainingStack);
                items[i] = remainingStack;

                if (!remainingStack.IsEmpty) break;
            }
        }

        return this[index].Count;
    }

    public void SortItems()
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].IsEmpty || items[i].IsFull) continue;
            MoveAllSimilarItemsToSlot(i);
        }

        items = items
            .OrderBy(s => s.Item == null)
            .ThenBy(s => s.Item == null ? "" : s.Item.DisplayName)
            .ThenByDescending(s => s.Count)
            .ToArray();
        InventoryUpdated?.Invoke();
    }

    public bool AcceptsItem(Item item)
    {
        if (acceptedItemPredicate != null) return acceptedItemPredicate(item);
        return true;
    }

    public bool Contains(ItemStack stack)
    {
        return items.Where(s => s.Item == stack.Item).Sum(s => s.Count) >= stack.Count;
    }

    public int ContainsMany(ItemStack stack)
    {
        return items.Where(s => s.Item == stack.Item).Sum(s => s.Count) / stack.Count;
    }

    public void Remove(ItemStack stack)
    {
        var itemsToTake = stack.Count;
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].Item != stack.Item) continue;

            var takenItems = Mathf.Min(itemsToTake, items[i].Count);
            itemsToTake -= takenItems;

            this[i] = items[i].Subtract(takenItems);

            if (takenItems == 0) break;
        }
    }
}
