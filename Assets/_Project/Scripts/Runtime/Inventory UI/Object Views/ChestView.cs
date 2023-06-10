using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChestView : MonoBehaviour, IInventoryMenuView
{
    [SerializeField] private TextMeshProUGUI chestLabel = null;
    [SerializeField] private ItemContainerView containerView = null;
    [Space]
    [SerializeField] private ItemContainerView keyContainerView = null;
    [SerializeField] private CanvasGroup lockOverlay = null;
    [SerializeField] private Button lockButton = null;

    private Inventory chestInventory;
    private Chest chest;
    private KeyContainer keyContainer;
    private Func<ItemStack, IItemContainer> otherContainerFindStrategy;

    private void OnDestroy()
    {
        if (chest != null) chest.LockedStateChanged -= Chest_LockedStateChanged;
    }

    public void BindTo(Chest chest)
    {
        if (chest != null) chest.LockedStateChanged -= Chest_LockedStateChanged;

        this.chest = chest;

        chestLabel.text = chest.Label;
        chestInventory = chest.Inventory;
        containerView.BindTo(chestInventory.Container);

        if (chest.RequiresKey)
        {
            keyContainer = chest.KeyContainer;
            keyContainerView.BindTo(keyContainer);

            chest.LockedStateChanged += Chest_LockedStateChanged;
        }

        lockOverlay.gameObject.SetActive(chest.RequiresKey && chest.IsLocked);

        if (lockButton)
        {
            lockButton.onClick.AddListener(() => { TryToLockChest(); });
            lockButton.gameObject.SetActive(chest.RequiresKey);
        }
    }

    public void AbortInteraction()
    {
        containerView.AbortInteraction();
    }

    public IItemContainer GetBestContainerFor(ItemStack stack)
    {
        if (chest.RequiresKey && chest.IsLocked)
            return keyContainer;

        return chestInventory.Container;
    }

    public void Inject(Func<ItemStack, IItemContainer> otherContainerFindStrategy)
    {
        this.otherContainerFindStrategy = otherContainerFindStrategy;
        containerView.OtherContainerFindStrategy = otherContainerFindStrategy;
    }

    public bool IsIdle()
    {
        return containerView.IsIdle();
    }

    private void Chest_LockedStateChanged(bool locked)
    {
        if (locked)
        {
            lockOverlay.gameObject.SetActive(true);
            lockOverlay.DOFade(1, 0.4f)
                .OnComplete(() =>
                {
                    lockOverlay.interactable = true;
                });
        }
        else
        {
            lockOverlay.interactable = false;
            lockOverlay.DOFade(0, 0.4f)
                .SetDelay(0.1f)
                .OnComplete(() =>
                {
                    lockOverlay.gameObject.SetActive(false);
                });
        }
    }

    private void TryToLockChest()
    {
        var container = otherContainerFindStrategy(keyContainer[0]);
        if (container.CanTakeStack(keyContainer[0], onlyFully: true))
        {
            container.TakeStack(keyContainer[0], out var remainingStack);
            if (!remainingStack.IsEmpty) Debug.Log("Oh no");

            keyContainer[0] = ItemStack.Empty;
        }
    }
}