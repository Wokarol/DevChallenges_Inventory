using DG.Tweening;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickBoop : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private StudioEventEmitter eventEmitter;

    public void OnPointerClick(PointerEventData eventData)
    {
        DOTween.Sequence()
            .Append(transform.DOBlendableScaleBy(Vector3.one * -0.1f, 0.1f))
            .Append(transform.DOBlendableScaleBy(Vector3.one * 0.1f, 0.1f))
            .SetLink(gameObject);

        if (eventEmitter != null) eventEmitter.Play();
    }
}
