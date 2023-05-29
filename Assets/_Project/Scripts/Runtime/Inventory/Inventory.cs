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
        return items.Any(s => (s.Item = stack.Item) || s.IsEmpty);
    }

    public void TakeStack(ItemStack stack)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (this[i].Item == stack.Item)
            {
                this[i] = this[i].CombineWith(stack);
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
}
