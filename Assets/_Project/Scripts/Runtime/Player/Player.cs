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

    internal void OpenFireplace(Fireplace fireplace)
    {
        inventoryView.OpenWithSecondMenu(fireplace.View, fireplace.BindToView);
    }
}
