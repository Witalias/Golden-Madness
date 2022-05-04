using System.Collections;
using UnityEngine;

public class Lamp : MonoBehaviour
{
    [SerializeField] private bool enableFuelDecrease = false;
    [SerializeField] [Range(0, 100)] private float fuelCount = 100f;
    [SerializeField] private float minLightRange = 8f;
    [SerializeField] private float maxLightRange = 25f;
    [SerializeField] private float fuelDecreaseValue = 0.5f;
    [SerializeField] private float timeFuelDecrease = 1f;

    private readonly float checkFuelCountBetweenTime = 2f;

    private readonly float frontier1 = 50f;
    private readonly float frointer2 = 25f;
    private readonly float frointer3 = 0;

    private readonly float decreasingSanity1 = 0.083f;
    private readonly float decreasingSanity2 = 0.167f;
    private readonly float decreasingSanity3 = 0.333f;

    private float currentDecreasingSanity = 0;

    private Light light_;
    private Consumables consumables;
    private SanityController sanity;

    private Coroutine decreaseFuel;
    private Coroutine checkFuelCount;

    private float FuelCount
    {
        get => fuelCount;
        set
        {
            if (value < 0)
                fuelCount = 0;
            else if (value > 100)
                fuelCount = 100;
            else
                fuelCount = value;
            light_.range = Mathf.Lerp(minLightRange, maxLightRange, fuelCount / 100);
        }
    }

    private void Awake()
    {
        light_ = GetComponent<Light>();
        consumables = transform.parent.GetComponent<Consumables>();
        sanity = transform.parent.GetComponent<SanityController>();
    }

    private void Start()
    {
        FuelCount = fuelCount;
        decreaseFuel = StartCoroutine(DecreaseLampFuel());
        checkFuelCount = StartCoroutine(CheckFuelCount());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && consumables.FuelTanksCount > 0)
        {
            FuelCount += consumables.FuelTankRecovery;
            --consumables.FuelTanksCount;
        }
    }

    private void ChangeDecreasingSanity(float value)
    {
        sanity.DecreasingSanity += value - currentDecreasingSanity;
        currentDecreasingSanity = value;
    }

    private IEnumerator DecreaseLampFuel()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeFuelDecrease);
            if (enableFuelDecrease)
                FuelCount -= fuelDecreaseValue;
        }
    }

    private IEnumerator CheckFuelCount()
    {
        while (true)
        {
            yield return new WaitForSeconds(checkFuelCountBetweenTime);
            if (fuelCount > frontier1)
                ChangeDecreasingSanity(0);
            else if (fuelCount <= frontier1 && fuelCount > frointer2)
                ChangeDecreasingSanity(decreasingSanity1);
            else if (fuelCount <= frointer2 && fuelCount > frointer3)
                ChangeDecreasingSanity(decreasingSanity2);
            else if (fuelCount <= frointer3)
                ChangeDecreasingSanity(decreasingSanity3);
        }
    }
}