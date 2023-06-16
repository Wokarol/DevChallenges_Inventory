using System;
using UnityEngine;

public class AnvilView : MonoBehaviour, IInventoryMenuView
{


    public void BindTo(Anvil anvil)
    {

    }

    public void AbortInteraction()
    {
        throw new NotImplementedException();
    }

    public IItemContainer GetBestContainerFor(ItemStack stack)
    {
        throw new NotImplementedException();
    }

    public void Inject(Func<ItemStack, IItemContainer> bestOtherContainerFinder)
    {
        throw new NotImplementedException();
    }

    public bool IsIdle()
    {
        throw new NotImplementedException();
    }
}
