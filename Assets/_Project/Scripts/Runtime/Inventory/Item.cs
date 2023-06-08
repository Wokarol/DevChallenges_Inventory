using NaughtyAttributes;
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
    [SerializeField] private float burningDuration = -1;
    [SerializeField] private Item cookedVersion = null;
    [SerializeField] private float cookingDuration = 0;

    public string DisplayName => string.IsNullOrWhiteSpace(nameOverride) ? name : nameOverride;
    public string Description => description;
    public Sprite Sprite => sprite;
    public int MaxStackSize => maxStackSize;
    public bool CanBeBurned => burningDuration > 0;
    public Item CookedVersion => cookedVersion;
    public float CookingDuration => cookingDuration;

    public float BurningDuration => burningDuration;
}
