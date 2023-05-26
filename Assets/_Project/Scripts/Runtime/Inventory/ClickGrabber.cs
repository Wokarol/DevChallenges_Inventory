using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickGrabber : MonoBehaviour, IPointerClickHandler
{
    private event Action onClick;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void Show(Action onClick)
    {
        gameObject.SetActive(true);
        this.onClick += onClick;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        gameObject.SetActive(false);
        onClick?.Invoke();
        onClick = null;
    }
}
