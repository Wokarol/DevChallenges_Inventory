using System;
using TMPro;
using UnityEngine;

public class ChestView : MonoBehaviour, IInventoryMenuView
{
    [SerializeField] private TextMeshProUGUI chestLabel = null;
    [SerializeField] private ItemContainerView containerView = null;
    private Inventory chestInventory;

    public void AbortInteraction()
    {
        containerView.AbortInteraction();
    }

    public IItemContainer GetBestContainerFor(ItemStack stack)
    {
        return chestInventory;
    }

    public void Inject(Func<ItemStack, IItemContainer> otherContainerFindStrategy)
    {
        containerView.OtherContainerFindStrategy = otherContainerFindStrategy;
    }

    public bool IsIdle()
    {
        return containerView.IsIdle();
    }

    internal void BindTo(Chest chest)
    {
        chestLabel.text = chest.Label;
        chestInventory = chest.Inventory;
        containerView.BindTo(chestInventory);
    }
}