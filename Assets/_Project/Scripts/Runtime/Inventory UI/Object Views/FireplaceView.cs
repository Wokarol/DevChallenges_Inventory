using System;
using UnityEngine;

public class FireplaceView : MonoBehaviour, IInventoryMenuView
{
    [SerializeField] private ItemContainerView cooktopContainerView;
    [SerializeField] private ItemContainerView fuelContainerView;

    public void AbortInteraction()
    {
        cooktopContainerView.AbortInteraction();
        fuelContainerView.AbortInteraction();
    }

    public IItemContainer GetBestContainerFor(ItemStack stack)
    {
        throw new NotImplementedException();
    }

    public void Inject(Func<ItemStack, IItemContainer> bestOtherContainerFinder)
    {
        cooktopContainerView.OtherContainerFindStrategy = bestOtherContainerFinder;
        fuelContainerView.OtherContainerFindStrategy = bestOtherContainerFinder;
    }

    public bool IsIdle()
    {
        return cooktopContainerView.IsIdle() && fuelContainerView.IsIdle();
    }
}
