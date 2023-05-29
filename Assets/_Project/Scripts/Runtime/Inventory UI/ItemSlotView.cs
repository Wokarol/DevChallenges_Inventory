﻿using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlotView : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private ItemStackView stackView;
    [SerializeField] private CanvasGroup grabbedImage;

    private int index = -1;
    private ItemContainerView owner;

    public int Index => index;
    public ItemContainerView Owner => owner;

    private bool wasGrabbed = false;

    private void Awake()
    {
        if (grabbedImage != null)
            grabbedImage.alpha = 0;
    }

    public void Init(int index, ItemContainerView owner)
    {
        this.index = index;
        this.owner = owner;
    }

    public void Display(ItemStack? stack, bool grabbed)
    {
        if (wasGrabbed != grabbed && grabbedImage != null)
        {
            if (grabbed)
            {
                grabbedImage.DOKill();
                grabbedImage.DOFade(1, 0.2f);
            }
            else
            {
                grabbedImage.DOKill();
                grabbedImage.DOFade(0, 0.2f);
            }
        }
        wasGrabbed = grabbed;

        stackView.Display(stack);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Middle) return;

        bool rightClick = eventData.button == PointerEventData.InputButton.Right;
        owner.OnSlotClick(index, stackView.transform.position, rightClick);
    }
}
