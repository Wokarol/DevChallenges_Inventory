using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemGiver : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private List<Option> items;

    public void OnPointerClick(PointerEventData eventData)
    {

        Item item = GetRandomItem();

        FindObjectOfType<Player>().GiveItem(item, 1);
    }

    private Item GetRandomItem()
    {
        float totalWeight = 0f;
        foreach (Option option in items)
        {
            totalWeight += option.Weigth;
        }

        float randomValue = UnityEngine.Random.Range(0f, totalWeight);

        foreach (Option option in items)
        {
            if (randomValue < option.Weigth)
            {
                return option.Item;
            }

            randomValue -= option.Weigth;
        }

        return null;
    }

    [System.Serializable]
    public struct Option
    {
        public Item Item;
        public float Weigth;
    }
}
