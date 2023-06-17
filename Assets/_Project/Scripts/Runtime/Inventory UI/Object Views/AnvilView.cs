using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class AnvilView : MonoBehaviour, IInventoryMenuView
{
    [SerializeField] private ItemContainerView oreInputContainerView;
    [SerializeField] private ItemContainerView fuelInputContainerView;
    [SerializeField] private ItemContainerView outputContainerView;
    [Space]
    [SerializeField] private RectTransform metalLiquid;
    [SerializeField] private Image heatBar;
    [SerializeField] private Image currentRecipeInset;
    [Space]
    [SerializeField] private Button previousRecipeButton;
    [SerializeField] private Button nextRecipeButton;
    [SerializeField] private Button smithButton;

    private Anvil anvil;
    private BasicContainer oreInputContainer;
    private BasicContainer fuelInputContainer;

    private int lastMetalFill = -1;

    private void Awake()
    {
        previousRecipeButton.onClick.AddListener(OnPreviousRecipe);
        nextRecipeButton.onClick.AddListener(OnNextRecipe);
        smithButton.onClick.AddListener(OnSmithPressed);
    }

    private void LateUpdate()
    {
        if (anvil == null) return;

        heatBar.fillAmount = anvil.Heat;
        currentRecipeInset.sprite = anvil.CurrentRecipe.FormSprite;

        if (lastMetalFill != anvil.MetalFill)
        {
            Vector2 newLiquidAnchor = new(1, anvil.MetalFill / (float)anvil.MaxMetal);
            if (newLiquidAnchor.y < 0.01) newLiquidAnchor = new(1, -0.1f);

            metalLiquid.DOKill();
            metalLiquid.DOAnchorMax(newLiquidAnchor, 0.2f);
        }
        lastMetalFill = anvil.MetalFill;
    }

    public void BindTo(Anvil anvil)
    {
        this.anvil = anvil;
        oreInputContainer = anvil.OreInputContainer;
        fuelInputContainer = anvil.FuelInputContainer;

        oreInputContainerView.BindTo(oreInputContainer);
        fuelInputContainerView.BindTo(fuelInputContainer);
        outputContainerView.BindTo(anvil.OutputContainer);

        outputContainerView.OutputOnly = true;
    }

    public void AbortInteraction()
    {
        oreInputContainerView.AbortInteraction();
        fuelInputContainerView.AbortInteraction();
        outputContainerView.AbortInteraction();
    }

    public IItemContainer GetBestContainerFor(ItemStack stack)
    {
        if (oreInputContainer.CanTakeStack(stack)) return oreInputContainer;
        return fuelInputContainer;
    }

    public void Inject(Func<ItemStack, IItemContainer> bestOtherContainerFinder)
    {
        oreInputContainerView.OtherContainerFindStrategy = bestOtherContainerFinder;
        fuelInputContainerView.OtherContainerFindStrategy = bestOtherContainerFinder;
        outputContainerView.OtherContainerFindStrategy = bestOtherContainerFinder;
    }

    public bool IsIdle()
    {
        return oreInputContainerView.IsIdle() &&
            fuelInputContainerView.IsIdle() &&
            outputContainerView.IsIdle();
    }

    private void OnPreviousRecipe()
    {
        anvil.PreviousRecipe();
    }

    private void OnNextRecipe()
    {
        anvil.NextRecipe();
    }

    private void OnSmithPressed()
    {
        anvil.Smith();
    }
}
