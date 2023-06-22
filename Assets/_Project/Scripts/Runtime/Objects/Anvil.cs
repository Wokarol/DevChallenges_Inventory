using FMODUnity;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Anvil : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Item acceptedOre;
    [SerializeField] private AnvilView anvilView;
    [SerializeField] private AnvilRecipeList recipeList;
    [SerializeField] private int maxMetal = 5;
    [Space]
    [SerializeField] private float cooldownBetweenSmithinAndAddingOre = 0.5f;
    [SerializeField] private float cooldownBetweenOreAdding = 0.5f;
    [Space]
    [SerializeField] private float heatPerFuelUnit = 0.05f;
    [SerializeField] private float heatDropPerSecond = 0.1f;
    [SerializeField] private float cooldownBetweenFuelAdding = 0.5f;
    [Space]
    [SerializeField] private StudioEventEmitter craftItemSoundEmitter = null;
    [SerializeField] private StudioEventEmitter dropletSoundEmitter = null;
    [SerializeField] private StudioEventEmitter fireSoundEmitter = null;
    [SerializeField] private string fireSoundIsOpenParameterName = "Is Open";
    [SerializeField] private string fireSoundStrengthParameterName = "Strength";

    private float nextOreAddTime;
    private float nextFuelAddTime;

    public AnvilView View => anvilView;
    public int MaxMetal => maxMetal;

    public BasicContainer OreInputContainer { get; private set; }
    public BasicContainer FuelInputContainer { get; private set; }
    public BasicContainer OutputContainer { get; private set; }
    public float Heat { get; private set; }
    public int MetalFill { get; private set; }
    public AnvilRecipe CurrentRecipe { get; private set; }
    public bool IsHeatedUp => Heat > 0.8;

    // This is a hack caused by lack of "hand slot"
    public int ItemsInHandFromFuelContainer { get; set; } = 0; // We use this hack to fix an issue with fuel being consumed while the player holds the stack
    public int ItemsInHandFromOreContainer { get; set; } = 0; // We use this hack to fix an issue with ore being consumed while the player holds the stack

    public event Action ConsumedMetal;
    public event Action ConsumedFuel;

    private void Awake()
    {
        OreInputContainer = new(1);
        FuelInputContainer = new(1);
        OutputContainer = new(1);

        OreInputContainer.AcceptsOnly(i => i == acceptedOre);
        FuelInputContainer.AcceptsOnly(i => i.CanBeBurned);

        CurrentRecipe = recipeList.List[0];
    }

    private void Update()
    {
        ProcessBurning();
        ProcessOreMelting();
    }

    private void ProcessBurning()
    {
        bool fuelAddCooldownGone = nextFuelAddTime < Time.time;
        bool hasFuel = !FuelInputContainer[0].IsEmpty && FuelInputContainer[0].Count > ItemsInHandFromFuelContainer;

        if (fuelAddCooldownGone && !IsHeatedUp && hasFuel)
        {
            Heat += FuelInputContainer[0].Item.BurningDuration * heatPerFuelUnit;
            FuelInputContainer[0] = FuelInputContainer[0].Subtract(1);
            ConsumedFuel?.Invoke();

            nextFuelAddTime = Time.time + cooldownBetweenFuelAdding;
        }

        Heat -= heatDropPerSecond * Time.deltaTime;
        Heat = Mathf.Clamp(Heat, 0, 100);

        if (fireSoundEmitter != null)
        {
            if (fireSoundEmitter.IsPlaying() && Heat <= 0)
            {
                fireSoundEmitter.Stop();
            }
            if (!fireSoundEmitter.IsPlaying() && Heat > 0)
            {
                fireSoundEmitter.Play();
                fireSoundEmitter.SetParameter(fireSoundIsOpenParameterName, 1);
            }

            fireSoundEmitter.SetParameter(fireSoundStrengthParameterName, Heat);
        }
    }

    private void ProcessOreMelting()
    {
        bool oreAddCooldownGone = nextOreAddTime < Time.time;
        bool isMetalFull = MetalFill >= MaxMetal;
        bool hasMetalOre = !OreInputContainer[0].IsEmpty && OreInputContainer[0].Count > ItemsInHandFromOreContainer;

        if (oreAddCooldownGone && !isMetalFull && hasMetalOre && IsHeatedUp)
        {
            OreInputContainer[0] = OreInputContainer[0].Subtract(1);
            MetalFill += 1;
            ConsumedMetal?.Invoke();

            if (dropletSoundEmitter != null) dropletSoundEmitter.Play();

            nextOreAddTime = Mathf.Max(nextOreAddTime, Time.time + cooldownBetweenOreAdding);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        FindObjectOfType<Player>().OpenAnvil(this);
        if (fireSoundEmitter != null) fireSoundEmitter.SetParameter(fireSoundIsOpenParameterName, 1);
    }

    public void Close()
    {
        if (fireSoundEmitter != null) fireSoundEmitter.SetParameter(fireSoundIsOpenParameterName, 0);
    }

    public void PreviousRecipe()
    {
        if (!OutputContainer[0].IsEmpty) return;

        int current = recipeList.List.Count;
        for (int r = 0; r < recipeList.List.Count; r++)
        {
            if (recipeList.List[r] == CurrentRecipe)
            {
                current = r;
                break;
            }
        }

        current = ((current - 1) + recipeList.List.Count) % recipeList.List.Count;
        CurrentRecipe = recipeList.List[current];
    }

    public void NextRecipe()
    {
        if (!OutputContainer[0].IsEmpty) return;

        int current = -1;
        for (int r = 0; r < recipeList.List.Count; r++)
        {
            if (recipeList.List[r] == CurrentRecipe)
            {
                current = r;
                break;
            }
        }

        current = (current + 1) % recipeList.List.Count;
        CurrentRecipe = recipeList.List[current];
    }

    public void Smith()
    {
        if (!IsHeatedUp) return;
        if (OutputContainer[0].SpaceLeft == 0) return;
        nextOreAddTime = Mathf.Max(nextOreAddTime, Time.time + cooldownBetweenSmithinAndAddingOre);

        MetalFill -= CurrentRecipe.MetalCost;
        OutputContainer[0] = OutputContainer[0].CombineWith(new(CurrentRecipe.CreatedItem, 1), out var remainingStack);

        if (craftItemSoundEmitter != null) craftItemSoundEmitter.Play();

        if (!remainingStack.IsEmpty)
            Debug.LogError("Oh no, the stack was not left empty!");
    }
}
