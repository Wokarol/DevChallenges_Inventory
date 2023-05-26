using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickGrabber : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image image;

    private event Action<RectTransform> onClick;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void Show(Action<RectTransform> onClick)
    {
        gameObject.SetActive(true);
        this.onClick += onClick;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        gameObject.SetActive(false);

        if (!image.canvas.TryGetComponent(out GraphicRaycaster raycaster))
        {
            Debug.LogError("There has to be a Graphic Ryacaster on the canvas!", image.canvas);
            return;
        }

        List<RaycastResult> results = new List<RaycastResult>();
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = eventData.position
        };

        raycaster.Raycast(pointerData, results);

        var clickedItem = results.Count == 0 
            ? null 
            : results[0].gameObject.transform as RectTransform;

        onClick?.Invoke(clickedItem);
        onClick = null;
    }
}
