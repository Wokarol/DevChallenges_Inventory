using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private int slotCount = 10;
    [SerializeField] private List<ItemStack> startingItems;

    public BasicContainer Container { get; private set; }

    private void Awake()
    {
        Container = new(slotCount, startingItems);
    }
}
