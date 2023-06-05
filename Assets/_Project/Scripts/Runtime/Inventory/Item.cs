using UnityEngine;

[CreateAssetMenu]
public class Item : ScriptableObject
{
    [SerializeField] private string nameOverride;
    [SerializeField, TextArea(3, 10)] private string description;
    [Space]
    [SerializeField] private Sprite sprite;
    [SerializeField] private int maxStackSize = 10;
    [Header("Props")]
    [SerializeField] private bool canBeBurned = false;
    [SerializeField] private Item cookedVersion = null;

    public string DisplayName => string.IsNullOrWhiteSpace(nameOverride) ? name : nameOverride;
    public string Description => description;
    public Sprite Sprite => sprite;
    public int MaxStackSize => maxStackSize;
    public bool CanBeBurned => canBeBurned;
    public Item CookedVersion => cookedVersion;
}
