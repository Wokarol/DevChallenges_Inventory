using System;
using UnityEngine;

public class FireplaceView : MonoBehaviour, IInventoryMenuView
{
    [SerializeField] private ItemContainerView cooktopInputContainerView;
    [SerializeField] private ItemContainerView cooktopOutputContainerView;
    [SerializeField] private ItemContainerView fuelContainerView;

    public void BindTo(BasicContainer cocktopInputContainer, BasicContainer cocktopOutputContainer, BasicContainer fuelContainer)
    {
        cooktopInputContainerView.BindTo(cocktopInputContainer);
        cooktopOutputContainerView.BindTo(cocktopOutputContainer);
        fuelContainerView.BindTo(fuelContainer);
    }

    public void AbortInteraction()
    {
        cooktopInputContainerView.AbortInteraction();
        cooktopOutputContainerView.AbortInteraction();
        fuelContainerView.AbortInteraction();
    }

    public IItemContainer GetBestContainerFor(ItemStack stack)
    {
        throw new NotImplementedException();
    }

    public void Inject(Func<ItemStack, IItemContainer> bestOtherContainerFinder)
    {
        cooktopInputContainerView.OtherContainerFindStrategy = bestOtherContainerFinder;
        fuelContainerView.OtherContainerFindStrategy = bestOtherContainerFinder;
    }

    public bool IsIdle()
    {
        return cooktopInputContainerView.IsIdle() &&
            cooktopOutputContainerView.IsIdle() &&
            fuelContainerView.IsIdle();
    }
}
