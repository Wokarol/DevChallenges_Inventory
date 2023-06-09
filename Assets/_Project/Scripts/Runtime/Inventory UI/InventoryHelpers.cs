﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;

public class InventoryHelpers : MonoBehaviour
{
    [SerializeField] private ItemStackView uiItem = null;
    [SerializeField] private ClickGrabber clickGrabber = null;
    [SerializeField] private ItemDetailsView tooltip = null;

    private ObjectPool<ItemStackView> pool;

    private void Awake()
    {
        uiItem.gameObject.SetActive(false);
        tooltip.gameObject.SetActive(false);
        pool = new ObjectPool<ItemStackView>(
            createFunc: () => Instantiate(uiItem, transform),
            actionOnGet: obj => obj.gameObject.SetActive(true),
            actionOnRelease: obj => obj.gameObject.SetActive(false),
            actionOnDestroy: obj => Destroy(obj.gameObject),
            defaultCapacity: 2);
    }

    private void Update()
    {
        if (tooltip.isActiveAndEnabled)
        {
            tooltip.transform.position = Input.mousePosition;
        }
    }

    public ItemStackView BorrowUIItem()
    {
        return pool.Get();
    }

    public void ReturnUIItem(ItemStackView item)
    {
        pool.Release(item);
    }

    public void AwaitClick(Action<RectTransform, PointerEventData> clickCallback)
    {
        clickGrabber.Show(clickCallback);
    }

    public void AwaitClickAbort()
    {
        clickGrabber.Abort();
    }

    public void ActivateTooltip(Item item)
    {
        tooltip.gameObject.SetActive(true);
        tooltip.BindTo(item);
    }

    public void DeactivateTooltip()
    {
        tooltip.gameObject.SetActive(false);
    }
}