using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Fireplace : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private FireplaceView view;

    public FireplaceView View => view;

    public void OnPointerClick(PointerEventData eventData)
    {
        FindObjectOfType<Player>().OpenFireplace(this);
    }

    public void BindToView(FireplaceView view)
    {
        Debug.LogError("NO BOUND LOGIC IS DONE YET");
    }
}
