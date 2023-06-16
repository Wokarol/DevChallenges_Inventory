using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class CraftingRecipeList : ScriptableObject
{
    [SerializeField] private List<CraftingRecipe> list;

    public IReadOnlyList<CraftingRecipe> List => list;
}
