using System.Collections;
using UnityEngine;

public enum PetMood
{
    Normal,
    Sleepy,
    Dirty,
    Hungry,
    Bored,
    Critical // or 
}

public enum MeterType
{
    Sleep,
    Hygiene,
    Hunger,
    Happiness
}

public class PetStatusManager : GenericSingleton<PetStatusManager>
{
    public float sleepMeter = 100f;
    public float hygieneMeter = 100f;
    public float hungerMeter = 100f;
    public float happinessMeter = 100f;

    [SerializeField] private float meterDecreaseRate = 1f; // per minute

    [SerializeField] private float m_meterRefreshTimer = 1f;

    private PetMood currentMood = PetMood.Normal;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(DecreaseMetersOverTime());
    }

    private IEnumerator DecreaseMetersOverTime()
    {
        while (true)
        {
            sleepMeter = Mathf.Max(0, sleepMeter - meterDecreaseRate / 60f);
            hygieneMeter = Mathf.Max(0, hygieneMeter - meterDecreaseRate / 60f);
            hungerMeter = Mathf.Max(0, hungerMeter - meterDecreaseRate / 60f);
            happinessMeter = Mathf.Max(0, happinessMeter - meterDecreaseRate / 60f);

            UpdateMood();
            yield return new WaitForSeconds(m_meterRefreshTimer); // check every second
        }
    }

    private void UpdateMood()
    {
        // Priority order: Sleep > Hygiene > Hunger > Boredom
        if (sleepMeter < 30f)
        {
            SetMood(PetMood.Sleepy);
        }
        else if (hygieneMeter < 30f)
        {
            SetMood(PetMood.Dirty);
        }
        else if (hungerMeter < 30f)
        {
            SetMood(PetMood.Hungry);
        }
        else if (happinessMeter < 30f)
        {
            SetMood(PetMood.Bored);
        }
        else
        {
            SetMood(PetMood.Normal);
        }
    }

    private void SetMood(PetMood mood)
    {
        if (currentMood == mood) return; // already in this mood
        currentMood = mood;

        // Play animation or trigger depending on mood
        switch (mood)
        {
            case PetMood.Normal:
                animator.SetTrigger("Normal");
                break;
            case PetMood.Sleepy:
                animator.SetTrigger("Sleepy");
                break;
            case PetMood.Dirty:
                animator.SetTrigger("Dirty");
                break;
            case PetMood.Hungry:
                animator.SetTrigger("Hungry");
                break;
            case PetMood.Bored:
                animator.SetTrigger("Inactive");
                break;
        }
    }

    // --- External functions to fill meters when you feed / clean / play ---
    public void FeedPet()
    {
        hungerMeter = 100f;
        UpdateMood();
    }

    public void CleanPet()
    {
        hygieneMeter = 100f;
        UpdateMood();
    }

    public void PlayWithPet()
    {
        happinessMeter = 100f;
        UpdateMood();
    }

    public void PutPetToSleep()
    {
        sleepMeter = 100f;
        UpdateMood();
    }
}
