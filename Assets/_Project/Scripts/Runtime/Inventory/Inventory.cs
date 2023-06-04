using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour, IItemContainer
{
    [SerializeField] private int slotCount = 10;
    [SerializeField] private List<ItemStack> startingItems;

    private ItemStack[] items;

    public ItemStack this[int index]
    {
        get => items[index];
        set
        {
            if (items[index] == value) return;

            items[index] = value;
            InventoryUpdated?.Invoke();
        }
    }

    public event Action InventoryUpdated;
    public int SlotCount => items.Length;

    private void Awake()
    {
        items = new ItemStack[slotCount];

        int startingItemsIndex = 0;
        foreach (int i in Enumerable.Range(0, items.Length).OrderBy(v => UnityEngine.Random.value).Take(startingItems.Count))
        {
            items[i] = startingItems[startingItemsIndex];
            startingItemsIndex++;
        }
    }

    public bool CanTakeStack(ItemStack stack, bool onlyFully = false)
    {
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
}
