using TMKOC.PetSimulator;
using TMPro;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [Header("Drain Rates (per minute)")]
    [SerializeField] float sleepDecreaseRate = 1f;
    [SerializeField] float hungerDecreaseRate = 1.2f;
    [SerializeField] float happinessDecreaseRate = 0.8f;

    [Header("Hygiene Sub-Rates")]
    [SerializeField] float bathDecreaseRate = 0.5f;
    [SerializeField] float pottyDecreaseRate = 0.7f;
    [SerializeField] float brushDecreaseRate = 0.3f;

    [Header("Meter Refresh Interval (seconds)")]
    [SerializeField] float meterRefreshInterval = 1f;

    [SerializeField] Animator m_animator;
    [SerializeField] TextMeshProUGUI m_currentState;

    public PlayerController PlayerController { get; private set; }
    public Animator Animator => m_animator;

    void Start()
    {
        if (m_animator == null)
            m_animator = GetComponentInChildren<Animator>();

        // pass all your inspectable values into the controller
        PlayerController = new PlayerController(
            this,
            sleepDecreaseRate,
            hungerDecreaseRate,
            happinessDecreaseRate,
            bathDecreaseRate,
            pottyDecreaseRate,
            brushDecreaseRate,
            meterRefreshInterval
        );
    }

    void Update()
    {
        // now just a plain Update() call
        PlayerController.Update();
    }

    // existing UI callbacks…
     
    public void TakeBath() => PlayerController.WashPet();
    public void DoToilet() => PlayerController.DoToiletPet();
    public void BrushTeeth() => PlayerController.BrushPetTeeth();
    
    public void FeedPet(float v) => PlayerController.FeedPet(v);
    public void RestPet() => PlayerController.PetRested();
    public void PlayWithPet() => PlayerController.PlayWithPet();
    public void ChangeState(string s) => m_currentState.SetText(s);
}
