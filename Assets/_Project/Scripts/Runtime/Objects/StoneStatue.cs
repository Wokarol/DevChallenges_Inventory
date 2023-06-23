using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class StoneStatue : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private StoneStatueView stoneStatueView = null;
    [Space]
    [SerializeField] private List<ItemStack> expectedItems = new();
    [SerializeField] private List<ItemStack> resultItems = new();
    [SerializeField] private string displayName;
    [SerializeField, TextArea(3, 5)] private string messageBefore;
    [SerializeField, TextArea(3, 5)] private string messageAfter;

    public StoneStatueView View => stoneStatueView;
    public BasicContainer InputContainer { get; private set; }
    public BasicContainer OutputContainer { get; private set; }

    public string DisplayName => displayName;
    public string MessageBefore => messageBefore;
    public string MessageAfter => messageAfter;

    public bool SacrificeDone { get; private set; } = false;

    private void Awake()
    {
        InputContainer = new(expectedItems.Count);
        OutputContainer = new(resultItems.Count);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        FindObjectOfType<Player>().OpenStatue(this);
    }

    public bool TrySacrifice()
    {
        var inputItemIDs = InputContainer.AllItems
            .Select(stack => stack.Item)
            .OrderBy(item => item != null ? item.GetInstanceID() : 0);
        var expectedItemIDs = expectedItems
            .Select(stack => stack.Item)
            .OrderBy(item => item.GetInstanceID());

        var hasAllItems = inputItemIDs.Zip(expectedItemIDs, (a, b) => a == b).All(b => b);

        if (!hasAllItems)
            return false;

        for (int i = 0; i < InputContainer.SlotCount; i++)
        {
            InputContainer[i] = ItemStack.Empty;
        }

        for (int i = 0; i < OutputContainer.SlotCount; i++)
        {
            OutputContainer[i] = resultItems[i];
        }

        SacrificeDone = true;
        return true;
    }
}
