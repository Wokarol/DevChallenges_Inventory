using Cysharp.Threading.Tasks;
using DG.Tweening;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameWonNotification : MonoBehaviour
{
    [SerializeField] private GameObject notificationPanel;
    [SerializeField] private Button closeNotificationButton;
    [Space]
    [SerializeField] private StudioEventEmitter swipeSoundEvent;
    private RectTransform rectTransform;

    private void Awake()
    {
        notificationPanel.SetActive(false);
        closeNotificationButton.onClick.AddListener(Hide);

        rectTransform = (RectTransform)transform;
    }

    public void QueueShow()
    {
        QueueShowAsync().Forget();
    }

    private async UniTask QueueShowAsync()
    {
        Debug.Log("Ready to show the end notification");

        var inventoryView = FindObjectOfType<InventoryView>();
        await UniTask.WaitUntil(() => !inventoryView.IsOpen);

        notificationPanel.SetActive(true);
        rectTransform.anchorMax = new Vector2(1, 2);
        rectTransform.anchorMin = new Vector2(0, 1);

        _ = rectTransform.DOAnchorMax(new(1, 1), 0.5f);
        _ = rectTransform.DOAnchorMin(new(0, 0), 0.5f);

        if (swipeSoundEvent != null) swipeSoundEvent.Play();
    }

    private void Hide()
    {
        _ = rectTransform.DOAnchorMax(new(1, 2), 0.5f);
        _ = rectTransform.DOAnchorMin(new(0, 1), 0.5f)
            .OnComplete(() =>
            {
                notificationPanel.SetActive(false);
            });
    }
}
