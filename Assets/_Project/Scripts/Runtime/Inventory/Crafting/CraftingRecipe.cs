using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CraftingRecipe : ScriptableObject
{
    [SerializeField] private List<ItemStack> ingredients = new();
    [SerializeField] private ItemStack result;

    public IReadOnlyList<ItemStack> Ingredients => ingredients;
    public ItemStack Result => result;
}
