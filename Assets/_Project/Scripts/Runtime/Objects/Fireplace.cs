using DG.Tweening;
using FMODUnity;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Fireplace : MonoBehaviour, IPointerClickHandler
{
    enum State { Idle, Cooking, Suspended }

    [SerializeField] private FireplaceView view;
    [Space]
    [SerializeField] private StudioEventEmitter fireBurningEvent;
    [SerializeField] private string isCookingParameterName = "Is Cooking";
    [SerializeField] private string isOpenParameterName = "Is Open";
    [SerializeField] private StudioEventEmitter woodAddEvent;
    [Space]
    [SerializeField] private Transform flameRoot = null;

    public FireplaceView View => view;

    public BasicContainer CooktopInputContainer { get; private set; }
    public BasicContainer CooktopOutputContainer { get; private set; }
    public BasicContainer FuelContainer { get; private set; }


    private State state = State.Idle;
    private float cookingTime;
    private bool isOpen;

    public float CookingTime => cookingTime;
    public Item CookedItem => CooktopInputContainer[0].Item;

    public bool HasFuel => !FuelContainer[0].IsEmpty && FuelContainer[0].Count > ItemsInHandFromFuelContainer;

    public float ItemBurningTime { get; private set; } = 0;
    public float BurnTimeLeft { get; private set; } = 0;
    
    // This is a hack caused by lack of "hand slot"
    public int ItemsInHandFromFuelContainer { get; set; } = 0; // We use this hack to fix an issue with fuel being consumed while the player holds the stack
    public int ItemsInHandFromInputContainer { get; set; } = 0; // We use this hack to fix an issue with input being consumed while the player holds the stack


    private void Awake()
    {
        CooktopInputContainer = new(1);
        CooktopOutputContainer = new(1);
        FuelContainer = new(1);

        CooktopInputContainer.AcceptsOnly(i => i.CanBeCooked);
        FuelContainer.AcceptsOnly(i => i.CanBeBurned);

        CooktopInputContainer.InventoryUpdated += CooktopInputContainer_InventoryUpdated;

        flameRoot.localScale = Vector3.zero;
    }

    private void Update()
    {
        ProcessCooking();
        if (fireBurningEvent != null && fireBurningEvent.IsPlaying())
        {
            if (BurnTimeLeft <= 0)
            {
                fireBurningEvent.Stop();
                flameRoot.DOScale(0, 0.25f);
            }
            fireBurningEvent.SetParameter(isCookingParameterName, state is State.Cooking ? 1 : 0);
        }

        if (BurnTimeLeft > 0)
        {
            BurnTimeLeft -= Time.deltaTime;
            if (BurnTimeLeft <= 0)
            {
                BurnTimeLeft = 0;
            }
        }
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

        if (isOpen && woodAddEvent != null) woodAddEvent.Play();
    }

    private void CooktopInputContainer_InventoryUpdated()
    {
        bool hasInput = !CooktopInputContainer[0].IsEmpty && CooktopInputContainer[0].Count > ItemsInHandFromInputContainer;

        if (state is State.Cooking or State.Suspended && !hasInput)
        {
            SwitchStateTo(State.Idle);
        }

        if (state is State.Idle && hasInput)
        {
            if (HasFuel || BurnTimeLeft > 0)
            {
                SwitchStateTo(State.Cooking);
            }
            else
            {
                SwitchStateTo(State.Suspended);
            }
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

        if (newState is State.Cooking)
        {
            if (fireBurningEvent != null)
            {
                fireBurningEvent.Play();
                flameRoot.DOScale(1, 0.25f);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        FindObjectOfType<Player>().OpenFireplace(this);
        isOpen = true;
        if (fireBurningEvent != null) fireBurningEvent.SetParameter(isOpenParameterName, 1);
    }

    public void Close()
    {
        isOpen = false;
        if (fireBurningEvent != null) fireBurningEvent.SetParameter(isOpenParameterName, 0);
    }

    public void ForceInventoryUpdate()
    {
        CooktopInputContainer_InventoryUpdated();
    }
}
