using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GrindstoneView : MonoBehaviour, IInventoryMenuView
{
    [SerializeField] private ItemContainerView weaponContainerView = null;
    [SerializeField] private Button grindButton = null;
    [Header("Grinding minigame")]
    [SerializeField] private InputUIButton leftButton = null;
    [SerializeField] private InputUIButton rightButton = null;
    [SerializeField] private GameObject grindingMinigamePanel = null;
    [SerializeField] private Transform sliderArrow = null;
    [SerializeField] private RectTransform progressBar = null;

    private Grindstone grindstone;
    private BasicContainer weaponContainer;

    private void Awake()
    {
        grindButton.onClick.AddListener(OnGrindClick);
    }

    private void Update()
    {
        bool isHoldingKeyboardLeft = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
        bool isHoldingKeyboardRight = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);

        bool isHoldingLeft = isHoldingKeyboardLeft || leftButton.Down;
        bool isHoldingRight = isHoldingKeyboardRight || rightButton.Down;
        float input = (isHoldingLeft ? 1 : 0) + (isHoldingRight ? -1 : 0);

        leftButton.FakeDown = isHoldingKeyboardLeft;
        rightButton.FakeDown = isHoldingKeyboardRight;

        grindstone.GrindInput = input;
    }

    private void LateUpdate()
    {
        if (grindstone == null) return;

        grindingMinigamePanel.SetActive(grindstone.IsGrinding);
        sliderArrow.localRotation = Quaternion.Euler(0, 0, grindstone.GrindAngle);

        progressBar.anchorMin = new Vector2(0.5f - grindstone.GrindProgress / 2f, 0);
        progressBar.anchorMax = new Vector2(0.5f + grindstone.GrindProgress / 2f, 1);
    }

    public void BindTo(Grindstone grindstone)
    {
        this.grindstone = grindstone;

        weaponContainer = grindstone.WeaponContainer;
        weaponContainerView.BindTo(weaponContainer);
    }

    public void AbortInteraction()
    {
        weaponContainerView.AbortInteraction();
        grindstone.AbordGrind();
    }

    public IItemContainer GetBestContainerFor(ItemStack stack)
    {
        if (grindstone.IsGrinding) return null;
        return weaponContainer;
    }

    public void Inject(Func<ItemStack, IItemContainer> bestOtherContainerFinder)
    {
        weaponContainerView.OtherContainerFindStrategy = bestOtherContainerFinder;
    }

    public bool IsIdle()
    {
        return weaponContainerView.IsIdle() && !grindstone.IsGrinding;
    }

    private void OnGrindClick()
    {
        grindstone.StartGrind();
    }
}
