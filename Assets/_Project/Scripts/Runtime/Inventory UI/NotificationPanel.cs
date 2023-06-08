using System;
using System.Collections.Generic;
using UnityEngine;

public class NotificationPanel : MonoBehaviour
{
    [SerializeField] private NotificationView notificationViewTemplate;
    [Space]
    [SerializeField] private float offset = 120;

    private List<NotificationView> notifications = new();

    private void Awake()
    {
        notificationViewTemplate.gameObject.SetActive(false);
    }

    internal void ItemAdded(Item item, int count)
    {
        var notification = Instantiate(notificationViewTemplate, notificationViewTemplate.transform.parent);
        notification.gameObject.SetActive(true);
        notification.Fill(item, count);

        notification.SlideFromLeft(notifications.Count * offset, () =>
        {
            notifications.Remove(notification);
            RefreshPositions();
        });

        notifications.Add(notification);
    }

    private void RefreshPositions()
    {
        for (int i = 0; i < notifications.Count; i++)
        {
            float expectedPosition = offset * i;
            var notification = notifications[i];
            if (Mathf.Approximately(expectedPosition, notification.transform.position.y))
                continue;

            notification.SlideTo(expectedPosition);
        }
    }
}
