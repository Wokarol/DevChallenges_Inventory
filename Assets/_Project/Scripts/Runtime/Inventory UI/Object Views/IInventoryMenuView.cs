using System;

public interface IInventoryMenuView
{
    void AbortInteraction();
    bool IsIdle();

    void Inject(Func<ItemStack, IItemContainer> bestOtherContainerFinder);
    IItemContainer GetBestContainerFor(ItemStack stack);
}