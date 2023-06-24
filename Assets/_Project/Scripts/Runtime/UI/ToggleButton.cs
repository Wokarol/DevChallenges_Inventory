using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToggleButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private string messageEnabled = "Enabled";
    [SerializeField] private string messageDisabled = "Disabled";
    [SerializeField] private TMP_Text buttonLabel;


    private bool state = false;

    public event Action<bool> OnChanged;
    public bool State
    {
        get => state; 
        private set
        {
            if (state == value) return;

            state = value;
            OnChanged?.Invoke(state);
        }
    }

    private void Start()
    {
        UpdateVisuals();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        State = !State;
        UpdateVisuals();
    }

    public void SetState(bool s)
    {
        State = s;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        buttonLabel.text = State ? messageEnabled : messageDisabled;
    }
}
