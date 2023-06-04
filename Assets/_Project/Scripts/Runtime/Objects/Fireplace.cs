using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Fireplace : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private int cocktopSlotCount = 1;
    [SerializeField] private int fuelSlotCount = 1;
    [SerializeField] private FireplaceView view;

    public FireplaceView View => view;

    private BasicContainer cocktopInputContainer;
    private BasicContainer cocktopOutputContainer;
    private BasicContainer fuelContainer;

    private void Awake()
    {
        cocktopInputContainer = new(cocktopSlotCount);
        cocktopOutputContainer = new(cocktopSlotCount);
        fuelContainer = new(fuelSlotCount);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        FindObjectOfType<Player>().OpenFireplace(this);
    }

    public void BindToView(FireplaceView view)
    {
        view.BindTo(cocktopInputContainer, cocktopOutputContainer, fuelContainer);
    }
}
