using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class CraftingRecipe : ScriptableObject
{
    [SerializeField] private List<ItemStack> ingredients = new();
    [SerializeField] private ItemStack result;

    public IReadOnlyList<ItemStack> Ingredients => ingredients;
    public ItemStack Result => result;

    private void Awake()
    {
        var isInvalid = ingredients.GroupBy(s => s.Item).Any(s => s.Count() > 2);
        if (isInvalid)
            Debug.LogError("Two items in the crafting recipee are the same, that should not be done");
    }
}
