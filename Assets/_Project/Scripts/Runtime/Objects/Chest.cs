using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Chest : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private string label;
    [Space]
    [SerializeField] private Inventory inventory;
    [SerializeField] private ChestView chestView;

    public ChestView View => chestView;
    public Inventory Inventory => inventory;
    public string Label => label;

    public void OnPointerClick(PointerEventData eventData)
    {
        FindObjectOfType<Player>().OpenChest(this);
    }
}
