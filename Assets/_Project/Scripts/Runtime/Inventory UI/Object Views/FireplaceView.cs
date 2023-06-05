using System;
using UnityEngine;

public class FireplaceView : MonoBehaviour, IInventoryMenuView
{
    [SerializeField] private ItemContainerView cooktopInputContainerView;
    [SerializeField] private ItemContainerView cooktopOutputContainerView;
    [SerializeField] private ItemContainerView fuelContainerView;

    private BasicContainer cooktopInputContainer;
    private BasicContainer fuelContainer;

    public void BindTo(Fireplace fireplace)
    {
        cooktopInputContainer = fireplace.CooktopInputContainer;
        fuelContainer = fireplace.FuelContainer;

        cooktopInputContainerView.BindTo(fireplace.CooktopInputContainer);
        cooktopOutputContainerView.BindTo(fireplace.CooktopOutputContainer);
        fuelContainerView.BindTo(fuelContainer);

        cooktopOutputContainerView.OutputOnly = true;
    }

    public void AbortInteraction()
    {
        cooktopInputContainerView.AbortInteraction();
        cooktopOutputContainerView.AbortInteraction();
        fuelContainerView.AbortInteraction();
    }

    public IItemContainer GetBestContainerFor(ItemStack stack)
    {
        if (cooktopInputContainer.CanTakeStack(stack)) return cooktopInputContainer;
        if (fuelContainer.CanTakeStack(stack)) return fuelContainer;

        return null;
    }

    public void Inject(Func<ItemStack, IItemContainer> bestOtherContainerFinder)
    {
        cooktopInputContainerView.OtherContainerFindStrategy = bestOtherContainerFinder;
        cooktopOutputContainerView.OtherContainerFindStrategy = bestOtherContainerFinder;
        fuelContainerView.OtherContainerFindStrategy = bestOtherContainerFinder;
    }

    public bool IsIdle()
    {
        return cooktopInputContainerView.IsIdle() &&
            cooktopOutputContainerView.IsIdle() &&
            fuelContainerView.IsIdle();
    }
}
