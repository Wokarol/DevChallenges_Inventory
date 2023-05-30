using UnityEngine;

[CreateAssetMenu]
public class Item : ScriptableObject
{
    [SerializeField] private string nameOverride;
    [SerializeField, TextArea(3, 10)] private string description;
    [Space]
    [SerializeField] private Sprite sprite;
    [SerializeField] private int maxStackSize = 10;

    public string DisplayName => string.IsNullOrWhiteSpace(nameOverride) ? name : nameOverride;
    public string Description => description;
    public Sprite Sprite => sprite;
    public int MaxStackSize => maxStackSize;
}
