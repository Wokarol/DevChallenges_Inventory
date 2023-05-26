using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemStackView : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI stackCount;

    public ItemStack? ShownStack { get; private set; }

    public void Display(ItemStack? stack)
    {
        ShownStack = stack;
        if (stack == null || stack.Value.Item == null)
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
            stackCount.text = count == 0 ? "" : count.ToString();
        }
    }
}