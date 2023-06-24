using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDetailsView : MonoBehaviour
{
    [SerializeField] private TMP_Text itemNameLabel = null;
    [SerializeField] private TMP_Text itemDescriptionLabel = null;
    [SerializeField] private GameObject fireIcon = null;
    [SerializeField] private GameObject campfireIcon = null;
    [SerializeField] private GameObject craftingIcon = null;
    [Space]
    [SerializeField] private List<CraftingRecipeList> craftingRecipesToLookAt;

    public void BindTo(Item item)
    {
        itemNameLabel.text = item.DisplayName;
        itemDescriptionLabel.text = item.Description;

        fireIcon.SetActive(item.CanBeBurned);
        campfireIcon.SetActive(item.CanBeCooked);
        craftingIcon.SetActive(ItemIsCraftingRecipes(item));

        transform.localScale = Vector3.zero;

        transform.DOKill();
        transform.DOScale(1f, 0.2f)
            .SetDelay(0.6f);
    }

    private void OnDisable()
    {
        transform.DOKill();
    }

    private bool ItemIsCraftingRecipes(Item item)
    {
        foreach (var recipeList in craftingRecipesToLookAt)
        {
            foreach (var recipe in recipeList.List)
            {
                foreach (var ingredient in recipe.Ingredients)
                {
                    if (ingredient.Item == item) return true;
                }
            }
        }

        return false;
    }
}
