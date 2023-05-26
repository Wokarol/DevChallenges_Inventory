using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotView : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI stackCount;

    private int index = -1;

    public void Init(int index)
    {
        this.index = index;
    }

    public void Display(Item item, int count = 0)
    {
        if (item == null)
        {
            itemIcon.enabled = false;
            itemIcon.sprite = null;
            stackCount.text = "";
        }
        else
        {
            itemIcon.enabled = true;
            itemIcon.sprite = item.Sprite;
            stackCount.text = count == 0 ? "" : count.ToString();
        }
    }
}
