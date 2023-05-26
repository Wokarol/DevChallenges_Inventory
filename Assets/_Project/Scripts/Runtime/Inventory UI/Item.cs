using UnityEngine;

[CreateAssetMenu]
public class Item : ScriptableObject
{
    [SerializeField] private string nameOverride;
    [SerializeField, TextArea(3, 10)] private string description;
    [SerializeField] private Sprite sprite;

    public string DisplayName => string.IsNullOrWhiteSpace(nameOverride) ? name : nameOverride;
    public string Description => description;
    public Sprite Sprite => sprite;
}
