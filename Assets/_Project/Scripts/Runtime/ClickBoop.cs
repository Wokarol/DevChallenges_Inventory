using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickBoop : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        DOTween.Sequence()
            .Append(transform.DOBlendableScaleBy(Vector3.one * -0.1f, 0.1f))
            .Append(transform.DOBlendableScaleBy(Vector3.one * 0.1f, 0.1f))
            .SetLink(gameObject);
    }
}
