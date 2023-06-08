using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationView : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text notificationMessage;

    private float targetYPos = 0;
    private float velocity;

    private void Update()
    {
        var rectTransform = transform as RectTransform;
        var pos = rectTransform.anchoredPosition;

        pos.y = Mathf.SmoothDamp(pos.y, targetYPos, ref velocity, 0.2f);

        rectTransform.anchoredPosition = pos;
    }

    public void Fill(Item item, int count)
    {
        itemIcon.sprite = item.Sprite;
        notificationMessage.text = $"{(count > 0 ? '+' : '-')}{count} {item.DisplayName}";
    }

    public void SlideFromLeft(float positionY, Action callback)
    {
        var rectTransform = transform as RectTransform;

        rectTransform.anchoredPosition = new(0, positionY);
        targetYPos = positionY;

        rectTransform.DOPivotX(rectTransform.pivot.x + 1.5f, 0.5f)
            .From()
            .SetId("Slide in tween");

        rectTransform.DOPivotX(rectTransform.pivot.x, 0.5f)
            .SetDelay(2)
            .OnComplete(() =>
            {
                callback();
                Destroy(gameObject);
            })
            .SetLink(gameObject)
            .SetId("Slide out tween");
    }

    internal void SlideTo(float newPos)
    {
        targetYPos = newPos;
    }
}
