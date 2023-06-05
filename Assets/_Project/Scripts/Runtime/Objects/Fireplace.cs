using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Fireplace : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private int cooktopSlotCount = 1;
    [SerializeField] private int fuelSlotCount = 1;
    [SerializeField] private FireplaceView view;

    public FireplaceView View => view;

    public BasicContainer CooktopInputContainer { get; private set; }
    public BasicContainer CooktopOutputContainer { get; private set; }
    public BasicContainer FuelContainer { get; private set; }

    private void Awake()
    {
        CooktopInputContainer = new(cooktopSlotCount);
        CooktopOutputContainer = new(cooktopSlotCount);
        FuelContainer = new(fuelSlotCount);

        CooktopInputContainer.AcceptsOnly(i => i.CookedVersion != null);
        FuelContainer.AcceptsOnly(i => i.CanBeBurned);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        FindObjectOfType<Player>().OpenFireplace(this);
    }
}
