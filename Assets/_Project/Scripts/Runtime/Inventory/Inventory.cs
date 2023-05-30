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

    public bool CanTakeStack(ItemStack stack)
    {
        bool hasEmptySlot = items.Any(s => s.IsEmpty);

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].Item != stack.Item) continue;

            bool stackFitWhenCombined = (items[i].Count + stack.Count) < stack.Item.MaxStackSize;
            if (stackFitWhenCombined) return true;
        }

        return hasEmptySlot;
    }

    public void TakeStack(ItemStack stack)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (this[i].Item == stack.Item)
            {
                this[i] = this[i].CombineWith(stack, out var remainingStack);
                if (!remainingStack.IsEmpty)
                {
                    stack = remainingStack;
                    break;
                }
                return;
            }
        }

        for (int i = 0; i < items.Length; i++)
        {
            if (this[i].IsEmpty)
            {
                this[i] = stack;
                return;
            }
        }
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
        items = items
            .OrderBy(s => s.Item == null)
            .ThenBy(s => s.Item == null ? "" : s.Item.DisplayName)
            .ThenByDescending(s => s.Count)
            .ToArray();
        InventoryUpdated?.Invoke();
    }
}
