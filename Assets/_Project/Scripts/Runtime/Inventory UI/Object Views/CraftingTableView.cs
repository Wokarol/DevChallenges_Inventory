using System;
using UnityEngine;

public class CraftingTableView : MonoBehaviour, IInventoryMenuView
{
    [SerializeField] private ItemContainerView tableInputContainerView;
    [SerializeField] private ItemContainerView tableOutputContainerView;

    private CraftingTable table;
    private BasicContainer tableInputContainer;
    private BasicContainer tableOutputContainer;

    public void BindTo(CraftingTable table)
    {
        this.table = table;

        tableInputContainer = table.InputContainer;
        tableOutputContainer = table.OutputContainer;

        tableInputContainerView.BindTo(tableInputContainer);
        tableOutputContainerView.BindTo(tableOutputContainer);

        tableOutputContainerView.OutputOnly = true;
    }

    public void AbortInteraction()
    {
        tableInputContainerView.AbortInteraction();
        tableOutputContainerView.AbortInteraction();
    }

    public IItemContainer GetBestContainerFor(ItemStack stack)
    {
        return tableInputContainer;
    }

    public void Inject(Func<ItemStack, IItemContainer> bestOtherContainerFinder)
    {
        tableInputContainerView.OtherContainerFindStrategy = bestOtherContainerFinder;
        tableOutputContainerView.OtherContainerFindStrategy = bestOtherContainerFinder;
    }

    public bool IsIdle()
    {
        return tableInputContainerView.IsIdle() &&
            tableOutputContainerView.IsIdle();
    }
}
