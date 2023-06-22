using DG.Tweening;
using FMODUnity;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Chest : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private string label;
    [Space]
    [SerializeField] private Inventory inventory;
    [SerializeField] private ChestView chestView;
    [Space]
    [SerializeField] private Transform chestLid;
    [Space]
    [SerializeField] private bool requiresKey;
    [ShowIf(nameof(requiresKey))]
    [SerializeField] private Item key;
    [Header("Audio")]
    [SerializeField] private StudioEventEmitter openCloseSound;
    [SerializeField] private StudioEventEmitter lockSound;

    public ChestView View => chestView;
    public Inventory Inventory => inventory;
    public string Label => label;

    public KeyContainer KeyContainer { get; private set; }
    public bool RequiresKey => requiresKey;
    public bool IsLocked { get; private set; } = false;
    public event Action<bool> LockedStateChanged;

    private void Awake()
    {
        if (requiresKey)
        {
            KeyContainer = new(key);
            IsLocked = true;
            KeyContainer.InventoryUpdated += () =>
            {
                IsLocked = KeyContainer[0].IsEmpty;
                LockedStateChanged?.Invoke(IsLocked);
                lockSound.Play();
            };
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        FindObjectOfType<Player>().OpenChest(this);

        chestLid.DOKill();
        chestLid.DOLocalRotate(Vector3.right * 60f, 0.5f);

        openCloseSound.Play();
    }

    public void CloseChest()
    {
        chestLid.DOKill();
        chestLid.DOLocalRotate(Vector3.zero, 0.5f);

        openCloseSound.Play();
    }
}
