using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private InventoryView inventoryView;

    private void Start()
    {
        inventoryView.BindPlayerInventoryTo(inventory);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (inventoryView.IsOpen)
                inventoryView.HideInventory();
            else
                inventoryView.OpenAlone();
        }
    }
    public void OpenChest(Chest chest)
    {
        inventoryView.OpenWithSecondMenu(chest.View, v => v.BindTo(chest), chest.CloseChest);
    }

    public void OpenFireplace(Fireplace fireplace)
    {
        inventoryView.OpenWithSecondMenu(fireplace.View, view => view.BindTo(fireplace));
    }

    public void OpenCraftingTable(CraftingTable table)
    {
        inventoryView.OpenWithSecondMenu(table.View, view => view.BindTo(table));
    }

    public void OpenAnvil(Anvil anvil)
    {
        inventoryView.OpenWithSecondMenu(anvil.View, view => view.BindTo(anvil));
    }

    public void OpenGrindstone(Grindstone grindstone)
    {
        inventoryView.OpenWithSecondMenu(grindstone.View, view => view.BindTo(grindstone));
    }

    public void GiveItem(Item item, int count)
    {
        inventory.Container.TakeStack(new(item, count), out var remaining);
        if (!remaining.IsEmpty) Debug.LogError("Could not store all the taken items!");

        FindObjectOfType<NotificationPanel>().ItemAdded(item, count); // Ugly but quick to write
    }
}
