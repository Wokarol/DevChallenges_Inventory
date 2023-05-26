using System;
using System.Collections.Generic;

[Serializable]
public struct ItemStack : IEquatable<ItemStack>
{
    public Item Item;
    public int Count;

    public override bool Equals(object obj)
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

    public static bool operator ==(ItemStack left, ItemStack right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ItemStack left, ItemStack right)
    {
        return !(left == right);
    }
}