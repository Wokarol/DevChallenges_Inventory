using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ItemSlotView : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private ItemStackView stackView;
    [SerializeField] private CanvasGroup grabbedSlotBackgroundIcon;

    private int index = -1;
    private ItemContainerView owner;

    public int Index => index;
    public ItemContainerView Owner => owner;

    private bool wasGrabbed = false;

    private void Awake()
    {
        if (grabbedSlotBackgroundIcon != null)
            grabbedSlotBackgroundIcon.alpha = 0;
    }

    public void Init(int index, ItemContainerView owner)
    {
        this.index = index;
        this.owner = owner;
    }

    public void Display(ItemStack? stack, bool grabbed)
    {
        if (wasGrabbed != grabbed && grabbedSlotBackgroundIcon != null)
        {
            if (grabbed)
            {
                grabbedSlotBackgroundIcon.DOKill();
                grabbedSlotBackgroundIcon.DOFade(1, 0.2f);
            }
            else
            {
                grabbedSlotBackgroundIcon.DOKill();
                grabbedSlotBackgroundIcon.DOFade(0, 0.2f);
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        owner.OnSlotEnter(index);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        owner.OnSlotExit(index);
    }
}
