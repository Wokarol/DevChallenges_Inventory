using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Fireplace : MonoBehaviour, IPointerClickHandler
{
    enum State { Idle, Cooking, Suspended }

    [SerializeField] private FireplaceView view;

    public FireplaceView View => view;

    public BasicContainer CooktopInputContainer { get; private set; }
    public BasicContainer CooktopOutputContainer { get; private set; }
    public BasicContainer FuelContainer { get; private set; }


    private State state = State.Idle;
    private float cookingTime;

    public float CookingTime => cookingTime;
    public Item CookedItem => CooktopInputContainer[0].Item;

    public bool HasFuel => !FuelContainer[0].IsEmpty;

    public float ItemBurningTime { get; private set; } = 0;
    public float BurnTimeLeft { get; private set; } = 0;

    private void Awake()
    {
        CooktopInputContainer = new(1);
        CooktopOutputContainer = new(1);
        FuelContainer = new(1);

        CooktopInputContainer.AcceptsOnly(i => i.CookedVersion != null);
        FuelContainer.AcceptsOnly(i => i.CanBeBurned);

        CooktopInputContainer.InventoryUpdated += CooktopInputContainer_InventoryUpdated;
    }

    private void Update()
    {
        ProcessCooking();

        BurnTimeLeft -= Time.deltaTime;
        if (BurnTimeLeft < 0) BurnTimeLeft = 0;
    }

    private void ProcessCooking()
    {
        if (state is State.Idle)
            return;

        var stackToBeCooked = CooktopInputContainer[0];
        ItemStack cookedItemStack = new(stackToBeCooked.Item.CookedVersion, 1);
        bool outputCanAcceptCookedItem = CooktopOutputContainer.CanTakeStack(cookedItemStack, true);

        if (!outputCanAcceptCookedItem)
        {
            SwitchStateTo(State.Suspended);
            return;
        }

        if (state is State.Suspended)
        {
            if (HasFuel)
            {
                BurnFuel();
                SwitchStateTo(State.Cooking);
            }
            else return;
        }

        if (BurnTimeLeft <= 0)
        {
            if (HasFuel)
            {
                BurnFuel();
            }
            else
            {
                SwitchStateTo(State.Suspended);
                return;
            }
        }

        cookingTime += Time.deltaTime;
        if (cookingTime >= stackToBeCooked.Item.CookingDuration)
        {
            cookingTime -= stackToBeCooked.Item.CookingDuration;
            CooktopOutputContainer.TakeStack(cookedItemStack, out var remainingStack);
            CooktopInputContainer[0] = stackToBeCooked.Subtract(1);

            if (!remainingStack.IsEmpty)
                throw new Exception("This should not happen!");
        }
    }

    private void BurnFuel()
    {
        ItemBurningTime = FuelContainer[0].Item.BurningDuration;
        BurnTimeLeft += ItemBurningTime;
        FuelContainer[0] = FuelContainer[0].Subtract(1);
    }

    private void CooktopInputContainer_InventoryUpdated()
    {
        if (state is State.Cooking or State.Suspended && CooktopInputContainer[0].IsEmpty)
        {
            SwitchStateTo(State.Idle);
        }

        if (state is State.Idle && !CooktopInputContainer[0].IsEmpty)
        {
            SwitchStateTo(State.Cooking);
        }
    }

    private void SwitchStateTo(State newState)
    {
        var oldState = state;
        state = newState;

        if (newState is State.Idle)
        {
            cookingTime = 0;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        FindObjectOfType<Player>().OpenFireplace(this);
    }
}
