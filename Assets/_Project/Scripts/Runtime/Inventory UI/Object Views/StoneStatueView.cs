using Cysharp.Threading.Tasks;
using DG.Tweening;
using FMODUnity;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoneStatueView : MonoBehaviour, IInventoryMenuView
{
    [SerializeField] private TMP_Text titleLabel;
    [SerializeField] private TMP_Text messageLabel;
    [SerializeField] private ItemContainerView inputContainerView;
    [SerializeField] private ItemContainerView outputContainerView;
    [SerializeField] private Button accept;
    [Space]
    [SerializeField] private RectTransform closingSlab;
    [SerializeField] private GameObject inputContainerHolder;
    [SerializeField] private GameObject outputContainerHolder;
    [SerializeField] private float slabCloseDuration = 2;
    [SerializeField] private float slabOpenDuration = 2;
    [SerializeField] private float waitForResult = 1;
    [SerializeField] private float waitAfterResult = 0.5f;
    [SerializeField] private AnimationCurve slabCloseCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private Image[] flashes;
    [SerializeField] private float flashDuration;
    [SerializeField] private AnimationCurve flashScaleCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private AnimationCurve flashRotateCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private Color flashColorGood = Color.white;
    [SerializeField] private Color flashColorBad = Color.white;
    [Space]
    [SerializeField] private StudioEventEmitter closeSoundEvent;
    [SerializeField] private StudioEventEmitter openSoundEvent;
    [SerializeField] private StudioEventEmitter correctItemsSoundEvent;
    [SerializeField] private StudioEventEmitter wrongItemsSoundEvent;

    private StoneStatue statue;

    private void Awake()
    {
        accept.onClick.AddListener(() => OnAccept().Forget());
        closingSlab.anchoredPosition = Vector2.zero;
        closingSlab.anchorMin = new(0, 1);
        closingSlab.anchorMax = new(1, 2);
    }

    public void BindTo(StoneStatue statue)
    {
        this.statue = statue;

        titleLabel.text = statue.DisplayName;

        inputContainerView.BindTo(statue.InputContainer);
        outputContainerView.BindTo(statue.OutputContainer);

        outputContainerView.OutputOnly = true;

        UpdateMessageAndHolders();
        accept.interactable = !statue.SacrificeDone;
    }

    public void AbortInteraction()
    {
        inputContainerView.AbortInteraction();
        outputContainerView.AbortInteraction();
    }

    public IItemContainer GetBestContainerFor(ItemStack stack)
    {
        return statue.InputContainer;
    }

    public void Inject(Func<ItemStack, IItemContainer> bestOtherContainerFinder)
    {
        inputContainerView.OtherContainerFindStrategy = bestOtherContainerFinder;
        outputContainerView.OtherContainerFindStrategy = bestOtherContainerFinder;
    }

    public bool IsIdle()
    {
        return inputContainerView.IsIdle() && outputContainerView.IsIdle();
    }

    private async UniTask OnAccept()
    {
        PrepareFlash(flashes[0].transform);
        PrepareFlash(flashes[1].transform);

        accept.interactable = false;

        if (closeSoundEvent != null) closeSoundEvent.Play();
        await DOTween.Sequence()
            .Append(closingSlab.DOAnchorMin(new(0, 0), slabCloseDuration).SetEase(slabCloseCurve))
            .Join(closingSlab.DOAnchorMax(new(1, 1), slabCloseDuration).SetEase(slabCloseCurve))
            .SetLink(closingSlab.gameObject);

        bool sacrificed = statue.TrySacrifice();
        UpdateMessageAndHolders();

        await UniTask.Delay(TimeSpan.FromSeconds(waitForResult));

        flashes[0].color = sacrificed ? flashColorGood : flashColorBad;
        flashes[1].color = sacrificed ? flashColorGood : flashColorBad;

        if (sacrificed)
        {
            if (correctItemsSoundEvent != null) correctItemsSoundEvent.Play();
        }
        else
        {
            if (wrongItemsSoundEvent != null) wrongItemsSoundEvent.Play();
        }

        _ = AnimateFlash(flashes[0].transform);
        _ = AnimateFlash(flashes[1].transform);

        await UniTask.Delay(TimeSpan.FromSeconds(waitAfterResult));

        if (openSoundEvent != null)
        {
            openSoundEvent.Play();
        }

        await DOTween.Sequence()
            .Append(closingSlab.DOAnchorMin(new(0, 1), slabOpenDuration).SetEase(Ease.OutCubic))
            .Join(closingSlab.DOAnchorMax(new(1, 2), slabOpenDuration).SetEase(Ease.OutCubic))
            .SetLink(closingSlab.gameObject);

        accept.interactable = !statue.SacrificeDone;
    }

    private void UpdateMessageAndHolders()
    {
        messageLabel.text = statue.SacrificeDone ? statue.MessageAfter : statue.MessageBefore;
        inputContainerHolder.SetActive(!statue.SacrificeDone);
        outputContainerHolder.SetActive(statue.SacrificeDone);
    }

    private void PrepareFlash(Transform t)
    {
        t.localScale = Vector3.zero;
        t.rotation = Quaternion.identity;
    }

    private async UniTask AnimateFlash(Transform t)
    {
        _ = t.DOScale(1, flashDuration).SetEase(flashScaleCurve);
        await t.DORotate(new(0, 0, 1), flashDuration).SetEase(flashRotateCurve);
    }
}
