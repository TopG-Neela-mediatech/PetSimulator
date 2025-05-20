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
    [SerializeField] private TeethBrushPainter m_brushPainter;
    [SerializeField] private FoamSpawner m_foamPainter;

    public PlayerController PlayerController { get; private set; }
    public Animator Animator => m_animator;

    public float BrushMeter;

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
        BrushMeter = PlayerController.BrushMeter;
        // now just a plain Update() call
        PlayerController.Update();
    }

    // Events

    public static event Action OnReadyToBrush;
    public static event Action OnBrushingCompleted;

    public void TakeBath() => PlayerController.WashPet();
    public void DoToilet() => PlayerController.DoToiletPet();


    // Initialize brush sequence 
    public void BrushTeeth()
    {
        if (PlayerController.BrushMeter <= 50f)
        {
            m_brushPainter.ResetRT();
            m_brushPainter.StopBrushing = false;
            m_foamPainter.StopBrushing = false;

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
    }

    public void EndBrushing()
    {
        m_brushPainter.StopBrushing = true;
        m_foamPainter.StopBrushing = true;

        var originPoint = GameManager.Instance.GetTransformPoint(TFLocation.FacingCameraAfterBrushing);

        if (originPoint == null)
        {
            Debug.LogError("Cannot find transform point for brushing");
        }

        transform.DORotate(originPoint.rotation.eulerAngles, 1.75f, RotateMode.Fast).SetDelay(1f);

        transform.DOMove(originPoint.position, 2f).SetDelay(1f).OnPlay(() =>
        {
            m_brushingStuff.SetActive(false);
            OnBrushingCompleted?.Invoke();
        });
    }

    public void FeedPet(float v) => PlayerController.FeedPet(v);
    public void RestPet() => PlayerController.PetRested();
    public void PlayWithPet() => PlayerController.PlayWithPet();
    public void ChangeState(string s) => m_currentState.SetText(s);
}
