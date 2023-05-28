using System;
using System.Collections.Generic;

[Serializable]
public struct ItemStack : IEquatable<ItemStack>
{
    public Item Item;
    public int Count;

    public static ItemStack Empty => default;

    public readonly bool IsEmpty => Item == null;

    public override readonly bool Equals(object obj)
    {
        return obj is ItemStack stack && Equals(stack);
    }

    public readonly bool Equals(ItemStack other)
    {
        return EqualityComparer<Item>.Default.Equals(Item, other.Item) &&
               Count == other.Count;
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(Item, Count);
    }

    public ItemStack CombineWith(ItemStack otherStack)
    {
        if (otherStack.Item != Item) throw new ArgumentException("The items must match when combining");

        return new ItemStack()
        {
            Item = Item,
            Count = otherStack.Count + Count,
        };
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