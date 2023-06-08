using System;
using UnityEngine;
using UnityEngine.UI;

public class FireplaceView : MonoBehaviour, IInventoryMenuView
{
    [SerializeField] private ItemContainerView cooktopInputContainerView;
    [SerializeField] private ItemContainerView cooktopOutputContainerView;
    [SerializeField] private ItemContainerView fuelContainerView;
    [Space]
    [SerializeField] private Image cookingTimeBar;
    [SerializeField] private Image fuelTimeBar;

    private Fireplace fireplace;
    private BasicContainer cooktopInputContainer;
    private BasicContainer fuelContainer;

    private void Update()
    {
        if (fireplace.CookedItem != null)
        {
            cookingTimeBar.fillAmount = fireplace.CookingTime / fireplace.CookedItem.CookingDuration;
        }
        else
        {
            cookingTimeBar.fillAmount = 0;
        }


        if (fireplace.ItemBurningTime > 0)
        {
            fuelTimeBar.fillAmount = fireplace.BurnTimeLeft / fireplace.ItemBurningTime;
        }
        else
        {
            fuelTimeBar.fillAmount = 0;
        }
    }

    public void BindTo(Fireplace fireplace)
    {
        this.fireplace = fireplace;

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
