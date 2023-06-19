using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Grindstone : MonoBehaviour, IPointerClickHandler
{
    enum State { Idle, Grinding }

    [SerializeField] private GrindstoneView grindstoneView;
    [Space]
    [SerializeField] private float grindingSwayForce = 30;
    [SerializeField] private AnimationCurve angleToSwayInputCurve = AnimationCurve.Linear(0, 0.2f, 5f, 1f);
    [Space]
    [SerializeField] private float grindingNoiseSwayForce = 20;
    [SerializeField] private float grindingNoiseSpeed = 2;
    [Space]
    [SerializeField] private float grindingInputForce = 60;
    [SerializeField] private float grindingAngleAcceleration = 40;
    [SerializeField] private float grindingAngleDeceleration = 40;
    [Space]
    [SerializeField] private AnimationCurve grindingAngleToProgressSpeedCurve = AnimationCurve.Linear(0f, 1f, 15f, 0f);
    [SerializeField] private float grindingSpeedMultiplier = 0.06f;

    public BasicContainer WeaponContainer;

    public GrindstoneView View => grindstoneView;
    
    public bool IsGrinding => state != State.Idle;
    public float GrindAngle { get; internal set; }
    public float GrindProgress { get; internal set; }
    public float GrindInput { get; internal set; }

    private State state;

    private float grindAngleVelocity = 0;

    private void Awake()
    {
        WeaponContainer = new(1);
        WeaponContainer.AcceptsOnly(i => i.CanBeGrinded, ignoreWhenDirect: true);
    }

    private void Update()
    {
        if (state == State.Grinding)
        {
            float input = GrindInput;
            float swayInput = angleToSwayInputCurve.Evaluate(Mathf.Abs(GrindAngle));
            float swayNoise = Mathf.PerlinNoise(Time.time * grindingNoiseSpeed, 127.514f) * 2f - 1f;

            GrindAngle += grindingNoiseSwayForce * Time.deltaTime * swayNoise;
            GrindAngle += grindingSwayForce * Time.deltaTime * Mathf.Sign(GrindAngle) * swayInput;

            grindAngleVelocity += Time.deltaTime * input * grindingAngleAcceleration;
            if (input == 0 || Mathf.Sign(input) != Mathf.Sign(grindAngleVelocity)) 
                grindAngleVelocity -= grindingAngleDeceleration * Time.deltaTime * Mathf.Sign(grindAngleVelocity);

            grindAngleVelocity = Mathf.Clamp(grindAngleVelocity, -grindingInputForce, grindingInputForce);

            GrindAngle += Time.deltaTime * grindAngleVelocity;
            GrindAngle = Mathf.Clamp(GrindAngle, -90f, 90f);

            float angleProgressMultiplier = grindingAngleToProgressSpeedCurve.Evaluate(Mathf.Abs(GrindAngle));
            GrindProgress += angleProgressMultiplier * grindingSpeedMultiplier * Time.deltaTime;
            GrindProgress = Mathf.Clamp01(GrindProgress);

            if (GrindProgress >= 1)
            {
                FinishGrind();
            }

            if (GrindProgress <= 0)
            {
                FailGrind();
            }
        }
    }

    public void StartGrind()
    {
        if (WeaponContainer[0].IsEmpty) return;
        if (!WeaponContainer[0].Item.CanBeGrinded) return;

        SwitchStateTo(State.Grinding);
    }

    public void AbordGrind()
    {
        if (IsGrinding)
            FailGrind();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        FindObjectOfType<Player>().OpenGrindstone(this);
    }

    private void FinishGrind()
    {
        SwitchStateTo(State.Idle);
        var currentStack = WeaponContainer[0];
        WeaponContainer[0] = new (currentStack.Item.GrindedVersion, currentStack.Count);
    }

    private void FailGrind()
    {
        SwitchStateTo(State.Idle);
        WeaponContainer[0] = ItemStack.Empty;
    }

    private void SwitchStateTo(State newState)
    {
        state = newState;

        if (newState == State.Grinding)
        {
            GrindAngle = 0;
            GrindProgress = 0.2f;
        }
    }
}
