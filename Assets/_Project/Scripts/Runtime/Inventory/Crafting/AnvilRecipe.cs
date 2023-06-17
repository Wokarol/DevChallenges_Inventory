using UnityEngine;

[CreateAssetMenu]
public class AnvilRecipe : ScriptableObject
{
    [SerializeField] private Sprite formSprite;
    [SerializeField] private Item createdItem;
    [SerializeField] private int metalCost;

    public Sprite FormSprite => formSprite;
    public Item CreatedItem => createdItem;
    public int MetalCost => metalCost;
}
