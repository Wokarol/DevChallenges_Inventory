using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AnvilRecipeList : ScriptableObject
{
    [SerializeField] private List<AnvilRecipe> list;

    public IReadOnlyList<AnvilRecipe> List => list;
}
