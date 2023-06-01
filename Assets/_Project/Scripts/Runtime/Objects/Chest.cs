using DG.Tweening;
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
    [Space]
    [SerializeField] private Transform chestLid;

    public ChestView View => chestView;
    public Inventory Inventory => inventory;
    public string Label => label;

    public void OnPointerClick(PointerEventData eventData)
    {
        FindObjectOfType<Player>().OpenChest(this);

        chestLid.DOKill();
        chestLid.DOLocalRotate(Vector3.right * 60f, 0.5f);
    }

    public void CloseChest()
    {
        chestLid.DOKill();
        chestLid.DOLocalRotate(Vector3.zero, 0.5f);
    }
}
