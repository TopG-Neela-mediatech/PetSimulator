using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Meter : MonoBehaviour
{
    [SerializeField] private MeterType meterType; // <-- NEW
    [SerializeField] private Image cooldown;
    [SerializeField] private TextMeshProUGUI m_percent;

    private PetStatusManager m_petStatusManager; // <-- NEW

    private void Start()
    {
        m_petStatusManager = PetStatusManager.Instance;
        if (m_percent == null)
        {
            Debug.LogError("Reference Missing for percentage text");
        }
    }

    private void Update()
    {
        if (m_petStatusManager == null) return;

        float currentMeterValue = GetMeterValue();
        cooldown.fillAmount = currentMeterValue / 100f; // Assuming 0-100 range
        m_percent.SetText(currentMeterValue.ToString("F2") + "%");
    }

    public string FormatToTwoDecimals(float value)
    {
        return value.ToString("F2");
    }

    private float GetMeterValue()
    {
        switch (meterType)
        {
            case MeterType.Sleep:
                return m_petStatusManager.sleepMeter;
            case MeterType.Hygiene:
                return m_petStatusManager.hygieneMeter;
            case MeterType.Hunger:
                return m_petStatusManager.hungerMeter;
            case MeterType.Happiness:
                return m_petStatusManager.happinessMeter;
            default:
                return 0f;
        }
    }
}
