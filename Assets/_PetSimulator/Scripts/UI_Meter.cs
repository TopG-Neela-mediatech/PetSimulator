using TMKOC.PetSimulator;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Meter : MonoBehaviour
{
    [SerializeField] private MeterType meterType; // <-- NEW
    [SerializeField] private Image cooldown;
    [SerializeField] private TextMeshProUGUI m_percent;


    private GameManager m_GameManager;

    private void Start()
    {
        if (m_percent == null)
        {
            Debug.LogError("Reference Missing for percentage text");
        }

        m_GameManager = GameManager.Instance;
    }

    private void Update()
    {
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
                return m_GameManager.PlayerView.PlayerController.SleepMeter;
            case MeterType.Hygiene:
                return m_GameManager.PlayerView.PlayerController.HygieneMeter;
            case MeterType.Hunger:
                return m_GameManager.PlayerView.PlayerController.HungerMeter;
            case MeterType.Happiness:
                return m_GameManager.PlayerView.PlayerController.HappinessMeter;
            default:
                return 0f;
        }
    }
}
