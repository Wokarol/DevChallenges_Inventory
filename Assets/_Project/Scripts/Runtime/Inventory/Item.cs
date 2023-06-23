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
    [SerializeField] private bool indestructible = false;
    [Header("Burning")]
    [SerializeField] private float burningDuration = -1;
    [Header("Cooking")]
    [SerializeField] private Item cookedVersion = null;
    [SerializeField] private float cookingDuration = 0;
    [Header("Grinding")]
    [SerializeField] private Item grindedVersion = null;

    public string DisplayName => string.IsNullOrWhiteSpace(nameOverride) ? name : nameOverride;
    public string Description => description;
    public Sprite Sprite => sprite;
    public int MaxStackSize => maxStackSize;

    public bool CanBeCooked => cookedVersion != null;
    public Item CookedVersion => cookedVersion;
    public float CookingDuration => cookingDuration;

    public bool CanBeBurned => burningDuration > 0;
    public float BurningDuration => burningDuration;

    public bool CanBeGrinded => grindedVersion != null;
    public Item GrindedVersion => grindedVersion;

    public bool Indestructible => indestructible;
}
