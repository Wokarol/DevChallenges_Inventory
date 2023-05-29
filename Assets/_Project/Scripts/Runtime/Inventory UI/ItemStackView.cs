using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemStackView : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI stackCount;
    [Space]
    [SerializeField] private bool hasAnimations = true;

    public ItemStack? ShownStack { get; private set; }

    public bool IsEmpty => ShownStack?.IsEmpty ?? true;

    public void Display(ItemStack? stack)
    {
        bool itemsIncreased = ShownStack == null || ShownStack.Value.Count < stack.Value.Count;
        bool sameItem = ShownStack != null && ShownStack.Value.Item == stack.Value.Item;
        if ((itemsIncreased || !sameItem) && hasAnimations)
        {
            DOTween.Sequence()
                .Append(transform.DOBlendableScaleBy(Vector3.up * -0.2f, 0.1f))
                .Join(transform.DOBlendableScaleBy(Vector3.right * 0.2f, 0.1f))
                .Append(transform.DOBlendableScaleBy(Vector3.up * 0.2f, 0.05f))
                .Join(transform.DOBlendableScaleBy(Vector3.right * -0.2f, 0.05f));
        }

        ShownStack = stack;
        if (stack == null || stack.Value.Item == null || stack.Value.Count == 0)
        {
            itemIcon.enabled = false;
            itemIcon.sprite = null;
            stackCount.text = "";
        }
        else
        {
            var item = stack.Value.Item;
            var count = stack.Value.Count;
            itemIcon.enabled = true;
            itemIcon.sprite = item.Sprite;
            stackCount.text = count <= 1 && count >= 0 ? "" : count.ToString();
        }
    }
}