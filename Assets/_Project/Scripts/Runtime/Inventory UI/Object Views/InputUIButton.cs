using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputUIButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Image image = null;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color pressedColor = Color.grey;

    private bool fakeDown;

    public bool Down { get; private set; } = false;
    public bool FakeDown
    {
        get => fakeDown; set
        {
            fakeDown = value;
            UpdateSpriteColor();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Down = true;
        UpdateSpriteColor();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Down = false;
        UpdateSpriteColor();
    }

    private void UpdateSpriteColor()
    {
        image.color = (Down || fakeDown) ? pressedColor : normalColor;
    }
}
