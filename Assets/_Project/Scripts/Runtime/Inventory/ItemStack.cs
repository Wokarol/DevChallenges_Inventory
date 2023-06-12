using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ItemStack : IEquatable<ItemStack>
{
    public Item Item;
    [SerializeField] private int count;

    public ItemStack(Item item, int count) : this()
    {
        Item = item;
        Count = count;
    }

    public static ItemStack Empty => default;

    public readonly bool IsEmpty => Item == null;
    public readonly bool IsFull => Item != null && count == Item.MaxStackSize;

    public int Count
    {
        get => count; 
        set
        {
            if (Item != null && value > Item.MaxStackSize)
                Debug.LogError("GOING OVER MAX STACK!");
            count = value;
        }
    }

    public int SpaceLeft => Item != null ? (Item.MaxStackSize - Count) : int.MaxValue;

    public override bool Equals(object obj)
    {
        return obj is ItemStack stack && Equals(stack);
    }

    public bool Equals(ItemStack other)
    {
        return EqualityComparer<Item>.Default.Equals(Item, other.Item) &&
               Count == other.Count;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Item, Count);
    }

    public ItemStack CombineWith(ItemStack otherStack, out ItemStack remainingStack)
    {
        if (IsEmpty)
        {
            remainingStack = Empty;
            return otherStack;
        }

        if (otherStack.Item != Item) throw new ArgumentException("The items must match when combining");

        int total = otherStack.Count + Count;
        if (total > Item.MaxStackSize)
        {
            remainingStack = otherStack.WithCount(total - Item.MaxStackSize);
            return this.WithCount(Item.MaxStackSize);
        }

        remainingStack = ItemStack.Empty;
        return this.WithCount(total);
    }

    public ItemStack Subtract(int count)
    {
        if (Count == count) return Empty;

        return new ItemStack()
        {
            Item = Item,
            Count = Count - count
        };
    }

    public ItemStack Add(int count)
    {
        return new ItemStack()
        {
            Item = Item,
            Count = Count + count
        };
    }

    public ItemStack WithCount(int count)
    {
        return new ItemStack()
        {
            Item = Item,
            Count = count
        };
    }

    public static bool operator ==(ItemStack left, ItemStack right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ItemStack left, ItemStack right)
    {
        return !(left == right);
    }
}