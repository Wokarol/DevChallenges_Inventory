using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryView : MonoBehaviour
{
    [SerializeField] private RectTransform fullPanel = null;
    [SerializeField] private ItemContainerView playerInventoryView = null;
    [Space]
    [SerializeField] private RectTransform playerInventorySection = null;
    [SerializeField] private RectTransform secondarySection = null;

    public bool IsOpen { get; private set; }

    private GameObject mountedPanel = null;

    private void Awake()
    {
        PutPanelBelow();
    }

    public void OpenAlone()
    {
        playerInventorySection.anchorMin = new(0.25f, 0);
        playerInventorySection.anchorMax = new(0.75f, 1);

        UnmountCurrentSecondaryView();

        SlidePanelIn();
        IsOpen = true;
    }

    public void OpenWithSecondMenu<T>(T view, Action<T> onBind) where T : Component
    {
        playerInventorySection.anchorMin = new(0, 0);
        playerInventorySection.anchorMax = new(0.5f, 1);

        UnmountCurrentSecondaryView();
        var panel = MountSecondaryView(view);
        onBind(panel);

        SlidePanelIn();
        IsOpen = true;
    }

    public void HideInventory()
    {
        SlidePanelOut();
        IsOpen = false;
    }

    public void BindPlayerInventoryTo(Inventory inventory)
    {
        playerInventoryView.BindTo(inventory);
    }

    private void SlidePanelIn()
    {
        fullPanel.DOAnchorMin(Vector2.zero, 0.3f);
        fullPanel.DOAnchorMax(Vector2.one, 0.3f);
    }

    private void SlidePanelOut()
    {
        fullPanel.DOAnchorMin(new(0, -1f), 0.3f);
        fullPanel.DOAnchorMax(new(1, 0), 0.3f);
    }

    private void PutPanelBelow()
    {
        fullPanel.anchorMin = new(0, -1f);
        fullPanel.anchorMax = new(1, 0);
    }

    private T MountSecondaryView<T>(T view) where T : Component
    {
        var createdView = Instantiate(view, secondarySection);
        mountedPanel = createdView.gameObject;
        return createdView;
    }

    private void UnmountCurrentSecondaryView()
    {
        if (mountedPanel == null) return;

        Destroy(mountedPanel);
        mountedPanel = null;
    }
}
