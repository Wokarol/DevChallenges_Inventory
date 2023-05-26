using System;
using TMPro;
using UnityEngine;

public class ChestView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI chestLabel = null;
    [SerializeField] private ItemContainerView containerView = null;

    internal void BindTo(Chest chest)
    {
        chestLabel.text = chest.Label;
        containerView.BindTo(chest.Inventory);
    }
}