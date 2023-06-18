using DG.Tweening;
using System;
using UnityEngine;

public class InventoryView : MonoBehaviour
{
    [SerializeField] private RectTransform fullPanel = null;
    [SerializeField] private GameObject clickBlockerPane = null;
    [SerializeField] private ItemContainerView playerInventoryView = null;
    [SerializeField] private ItemContainerView trashView = null;
    [Space]
    [SerializeField] private RectTransform playerInventorySection = null;
    [SerializeField] private RectTransform secondarySection = null;

    public bool IsOpen { get; private set; }

    private IInventoryMenuView mountedPanel = null;
    private Action closeCallback = null;
    private Inventory playerInventory;

    private BasicContainer trash;

    private void Awake()
    {
        PutPanelBelow();

        playerInventoryView.OtherContainerFindStrategy = FindOtherContainerForPlayerPanelStrategy;
        trash = new(1);
        trashView.BindTo(trash);

        trash.InventoryUpdated += Trash_InventoryUpdated;
    }

    private void Trash_InventoryUpdated()
    {
        if (trash[0].IsEmpty) return;

        trash[0] = ItemStack.Empty;
    }

    public void OpenAlone(Action onClose = null)
    {
        playerInventorySection.anchorMin = new(0.25f, 0);
        playerInventorySection.anchorMax = new(0.75f, 1);

        UnmountCurrentSecondaryView();

        SlidePanelIn();
        IsOpen = true;
    }

    public void OpenWithSecondMenu<T>(T view, Action<T> onBind, Action onClose = null) where T : Component, IInventoryMenuView
    {
        playerInventorySection.anchorMin = new(0, 0);
        playerInventorySection.anchorMax = new(0.5f, 1);

        UnmountCurrentSecondaryView();
        var panel = MountSecondaryView(view);

        onBind(panel);

        SlidePanelIn();
        IsOpen = true;

        closeCallback = onClose;
    }

    public void HideInventory()
    {
        closeCallback?.Invoke();
        closeCallback = null;

        SlidePanelOut();
        IsOpen = false;

        playerInventoryView.AbortInteraction();
        mountedPanel?.AbortInteraction();
    }

    public void HideInventoryIfIdle()
    {
        if (!playerInventoryView.IsIdle()) return;
        if (mountedPanel != null && !mountedPanel.IsIdle()) return;

        HideInventory();
    }

    public void BindPlayerInventoryTo(Inventory inventory)
    {
        playerInventory = inventory;
        playerInventoryView.BindTo(inventory.Container);
    }

    private void SlidePanelIn()
    {
        clickBlockerPane.SetActive(true);
        fullPanel.DOAnchorMin(Vector2.zero, 0.3f);
        fullPanel.DOAnchorMax(Vector2.one, 0.3f);
    }

    private void SlidePanelOut()
    {
        fullPanel.DOAnchorMin(new(0, -1f), 0.3f);
        fullPanel.DOAnchorMax(new(1, 0), 0.3f)
            .OnComplete(() => clickBlockerPane.SetActive(false));
    }

    private void PutPanelBelow()
    {
        clickBlockerPane.SetActive(false);
        fullPanel.anchorMin = new(0, -1f);
        fullPanel.anchorMax = new(1, 0);
    }

    private T MountSecondaryView<T>(T view) where T : Component, IInventoryMenuView
    {
        var createdView = Instantiate(view, secondarySection);
        createdView.Inject(FindOtherContainerForSecondaryPanelStrategy);
        mountedPanel = createdView;
        return createdView;
    }

    private void UnmountCurrentSecondaryView()
    {
        if (mountedPanel == null) return;

        Destroy(((Component)mountedPanel).gameObject);
        mountedPanel = null;
    }

    private IItemContainer FindOtherContainerForSecondaryPanelStrategy(ItemStack stack)
    {
        return playerInventory.Container;
    }

    private IItemContainer FindOtherContainerForPlayerPanelStrategy(ItemStack stack)
    {
        return mountedPanel.GetBestContainerFor(stack);
    }
}
