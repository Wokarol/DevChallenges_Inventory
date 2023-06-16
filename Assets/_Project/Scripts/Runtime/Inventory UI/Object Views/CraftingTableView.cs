using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class CraftingTableView : MonoBehaviour, IInventoryMenuView
{
    enum CraftButtonState { Invalid, Single, Multiple, All }

    [SerializeField] private ItemContainerView tableInputContainerView;
    [SerializeField] private ItemContainerView tableOutputContainerView;
    [SerializeField] private Button craftButton;
    [SerializeField] private TMP_Text craftButtonText;
    [Space]
    [SerializeField] private string craftText = "Craft";
    [SerializeField] private string craftTextNumber = "Craft ({0})";
    [SerializeField] private string craftTextAll = "Craft (All)";
    [Space]
    [SerializeField] private int craftsOnCtrl = 5;

    private CraftingTable table;
    private BasicContainer tableInputContainer;
    private BasicContainer tableOutputContainer;

    private CraftButtonState craftButtonState = CraftButtonState.Invalid;

    private bool Ctrl => Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
    private bool Shift => Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

    private void Awake()
    {
        craftButton.onClick.AddListener(() =>
        {
            switch (craftButtonState)
            {
                case CraftButtonState.Single:
                    table.Craft();
                    break;

                case CraftButtonState.Multiple:
                    table.Craft(craftsOnCtrl);
                    break;

                case CraftButtonState.All:
                    table.Craft(int.MaxValue);
                    break;
            }
        });
    }

    private void Update()
    {
        var previousButtonState = craftButtonState;
        if (Ctrl)
            craftButtonState = CraftButtonState.Multiple;
        else if (Shift)
            craftButtonState = CraftButtonState.All;
        else
            craftButtonState = CraftButtonState.Single;

        if (previousButtonState != craftButtonState)
        {
            switch (craftButtonState)
            {
                case CraftButtonState.Single:
                    craftButtonText.text = craftText;
                    break;

                case CraftButtonState.Multiple:
                    craftButtonText.text = string.Format(craftTextNumber, craftsOnCtrl);
                    break;

                case CraftButtonState.All:
                    craftButtonText.text = craftTextAll;
                    break;
            }
        }
    }

    public void BindTo(CraftingTable table)
    {
        this.table = table;

        tableInputContainer = table.InputContainer;
        tableOutputContainer = table.OutputContainer;

        tableInputContainerView.BindTo(tableInputContainer);
        tableOutputContainerView.BindTo(tableOutputContainer);

        tableOutputContainerView.OutputOnly = true;
    }

    public void AbortInteraction()
    {
        tableInputContainerView.AbortInteraction();
        tableOutputContainerView.AbortInteraction();
    }

    public IItemContainer GetBestContainerFor(ItemStack stack)
    {
        return tableInputContainer;
    }

    public void Inject(Func<ItemStack, IItemContainer> bestOtherContainerFinder)
    {
        tableInputContainerView.OtherContainerFindStrategy = bestOtherContainerFinder;
        tableOutputContainerView.OtherContainerFindStrategy = bestOtherContainerFinder;
    }

    public bool IsIdle()
    {
        return tableInputContainerView.IsIdle() &&
            tableOutputContainerView.IsIdle();
    }
}
