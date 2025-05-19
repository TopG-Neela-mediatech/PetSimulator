using TMKOC.PetSimulator;
using TMPro;
using DG.Tweening;
using UnityEngine;
using System;

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

    [Header("Brush wala Stuff")]
    [SerializeField] private GameObject m_brushingStuff;

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

    public static event Action OnReadyToBrush;

    public void TakeBath() => PlayerController.WashPet();
    public void DoToilet() => PlayerController.DoToiletPet();
    public void BrushTeeth()
    {
        // Get transform point for brushing from game manager
        Transform tfPoint = GameManager.Instance.GetTransformPoint(TFLocation.Brush);

        if (tfPoint == null)
        {
            Debug.LogError("Cannot find transform point for brushing");
        }

        transform.DORotate(tfPoint.rotation.eulerAngles, 1.75f, RotateMode.Fast);
        transform.DOMove(tfPoint.position, 2f).OnComplete(() =>
        {
            //PlayerController.BrushPetTeeth();
            m_brushingStuff.SetActive(true);
            OnReadyToBrush?.Invoke();   
        });
    }

    public void FeedPet(float v) => PlayerController.FeedPet(v);
    public void RestPet() => PlayerController.PetRested();
    public void PlayWithPet() => PlayerController.PlayWithPet();
    public void ChangeState(string s) => m_currentState.SetText(s);
}
