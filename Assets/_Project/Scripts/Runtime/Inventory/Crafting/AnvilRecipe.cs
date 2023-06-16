using UnityEngine;

[CreateAssetMenu]
public class AnvilRecipe : ScriptableObject
{
    [SerializeField] private Sprite formSprite;
    [SerializeField] private Item createdItem;
    [SerializeField] private int metalCost;
}
