using UnityEngine;

[CreateAssetMenu]
public class CommonUISettings : ScriptableObject
{
    [SerializeField] private float doubleClickInterval = 0.1f;

    public float DoubleClickInterval => doubleClickInterval;
}
