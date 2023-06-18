using System;
using UnityEngine;

public class GrindstoneView : MonoBehaviour, IInventoryMenuView
{
    public void BindTo(Grindstone grindstone)
    {
        Debug.LogError("NOT IMPLEMENTED");
    }

    public void AbortInteraction()
    {
        Debug.LogError("NOT IMPLEMENTED");
    }

    public IItemContainer GetBestContainerFor(ItemStack stack)
    {
        throw new NotImplementedException();
    }

    public void Inject(Func<ItemStack, IItemContainer> bestOtherContainerFinder)
    {
        Debug.LogError("NOT IMPLEMENTED");
    }

    public bool IsIdle()
    {
        throw new NotImplementedException();
    }
}
