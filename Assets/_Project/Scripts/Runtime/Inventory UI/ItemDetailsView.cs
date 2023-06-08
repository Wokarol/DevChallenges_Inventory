using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ItemDetailsView : MonoBehaviour
{
    [SerializeField] private TMP_Text itemNameLabel = null;
    [SerializeField] private TMP_Text itemDescriptionLabel = null;

    public void BindTo(Item item)
    {
        itemNameLabel.text = item.DisplayName;
        itemDescriptionLabel.text = item.Description;

        transform.localScale = Vector3.zero;

        transform.DOKill();
        transform.DOScale(1f, 0.2f)
            .SetDelay(0.6f);
    }

    private void OnDisable()
    {
        transform.DOKill();
    }
}
