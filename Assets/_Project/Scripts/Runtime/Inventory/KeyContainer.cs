using System;

public class KeyContainer : IItemContainer
{
    private Item expectedKey;

    private ItemStack storedKey;

    public ItemStack this[int index] {
        get {
            if (index == 0) return storedKey;
            else throw new ArgumentOutOfRangeException("index");
        }
        set
        {
            if (index == 0)
            {
                if (storedKey == value) return;

                storedKey = value;
                InventoryUpdated();
            }
            else throw new ArgumentOutOfRangeException("index");
        }
    }

    public int SlotCount => 1;

    public event Action InventoryUpdated;

    public KeyContainer(Item expectedKey)
    {
        this.expectedKey = expectedKey;
    }

    public bool AcceptsItem(Item item)
    {
        return item == expectedKey;
    }

    public bool CanTakeStack(ItemStack stack, bool onlyFully = false)
    {
        return AcceptsItem(stack.Item);
    }

    public int MoveAllSimilarItemsToSlot(int heldItemIndex)
    {
        return storedKey.Count;
    }

    public void SortItems()
    {
    }

    public void TakeStack(ItemStack myStack, out ItemStack remainingStack)
    {
        if (storedKey.IsEmpty)
        {
            this[0] = myStack;
            remainingStack = ItemStack.Empty;
            return;
        }

        storedKey = storedKey.CombineWith(myStack, out remainingStack);
    }
}
