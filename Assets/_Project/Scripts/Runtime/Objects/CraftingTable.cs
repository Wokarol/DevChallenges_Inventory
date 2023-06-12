using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftingTable : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private int craftingInputSlots = 3;
    [SerializeField] private int craftingOutputSlots = 1;
    [SerializeField] private CraftingRecipeList recipeList = null;
    [Space]
    [SerializeField] private CraftingTableView craftingTableView;

    public CraftingTableView View => craftingTableView;

    public BasicContainer InputContainer { get; private set; }
    public BasicContainer OutputContainer { get; private set; }

    private void Awake()
    {
        InputContainer = new(craftingInputSlots);
        OutputContainer = new(craftingOutputSlots);
    }

    public void Craft(int count = 1)
    {
        CraftingRecipe recipe = null;
        int maxCraftedItems = 0;

        var list = recipeList.List;
        for (int r = 0; r < list.Count; r++)
        {
            if (RecipeFits(list[r], out int maxCraftedItemsForRecipe))
            {
                if (recipe != null)
                {
                    Debug.LogError("Found two fitting recipes");
                    return;
                }
                recipe = list[r];
                maxCraftedItems = maxCraftedItemsForRecipe;
            }
        }

        if (recipe == null) return;

        int itemsToCraft = Mathf.Min(count, maxCraftedItems);
        for (int craftItemI = 0; craftItemI < itemsToCraft; craftItemI++)
        {
            if (OutputContainer[0].SpaceLeft >= 1)
            {
                for (int i = 0; i < recipe.Ingredients.Count; i++)
                {
                    InputContainer.Remove(recipe.Ingredients[i]);
                }
                OutputContainer[0] = OutputContainer[0].CombineWith(recipe.Result, out var remainingStack);
                if (!remainingStack.IsEmpty)
                {
                    Debug.LogError("Oh no");
                }
            }
            else
            {
                return;
            }
        }
    }

    private bool RecipeFits(CraftingRecipe recipe, out int craftCount)
    {
        craftCount = 0;
        int maxCraftItemsCount = int.MaxValue;
        for (int i = 0; i < recipe.Ingredients.Count; i++)
        {
            int howManyCanBeCrafted = InputContainer.ContainsMany(recipe.Ingredients[i]);

            if (howManyCanBeCrafted <= 0)
                return false;
            else
                maxCraftItemsCount = Mathf.Min(maxCraftItemsCount, howManyCanBeCrafted);
        }

        for (int i = 0; i < InputContainer.SlotCount; i++)
        {
            if (InputContainer[i].IsEmpty) continue;

            bool ingredientIsUsed = false;
            for (int j = 0; j < recipe.Ingredients.Count; j++)
            {
                if (recipe.Ingredients[j].Item == InputContainer[i].Item)
                    ingredientIsUsed = true;
            }

            if (!ingredientIsUsed)
                return false;
        }

        craftCount = maxCraftItemsCount;
        return true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        FindObjectOfType<Player>().OpenCraftingTable(this);
    }
}
