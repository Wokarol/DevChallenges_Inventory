using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class InventoryHelpers : MonoBehaviour
{
    [SerializeField] private ItemStackView uiItem = null;
    [SerializeField] private ClickGrabber clickGrabber = null;

    private ObjectPool<ItemStackView> pool;

    private void Awake()
    {
        uiItem.gameObject.SetActive(false);
        pool = new ObjectPool<ItemStackView>(
            createFunc: () => Instantiate(uiItem, transform),
            actionOnGet: obj => obj.gameObject.SetActive(true),
            actionOnRelease: obj => obj.gameObject.SetActive(false),
            actionOnDestroy: obj => Destroy(obj.gameObject),
            defaultCapacity: 2);
    }

    public ItemStackView BorrowUIItem()
    {
        return pool.Get();
    }

    public void ReturnUIItem(ItemStackView item)
    {
        pool.Release(item);
    }

    public void AwaitClick(Action<RectTransform> clickCallback)
    {
        clickGrabber.Show(clickCallback);
    }
}