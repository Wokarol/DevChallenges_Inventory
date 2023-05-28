using System;
using TMPro;
using UnityEngine;

public class ChestView : MonoBehaviour, IInteractibleView
{
    [SerializeField] private TextMeshProUGUI chestLabel = null;
    [SerializeField] private ItemContainerView containerView = null;

    public void AbortInteraction()
    {
        containerView.AbortInteraction();
    }

    public bool IsIdle()
    {
        return containerView.IsIdle();
    }

    internal void BindTo(Chest chest)
    {
        chestLabel.text = chest.Label;
        containerView.BindTo(chest.Inventory);
    }
}