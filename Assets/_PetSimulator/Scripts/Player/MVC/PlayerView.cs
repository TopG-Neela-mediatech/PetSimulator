using System.Collections;
using System.Collections.Generic;
using TMKOC.PetSimulator;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [SerializeField] private float meterDecreaseRate = 1f; // per minute

    [SerializeField] private float m_meterRefreshTimer = 1f;

    [SerializeField] private Animator m_animator;

    public PlayerController PlayerController { get; set; }

    public Animator Animator { get { return m_animator; } }

    private void Start()
    {
        if (m_animator == null)
        {
            m_animator = GetComponent<Animator>();
        }

        PlayerController = new PlayerController(this);

        StartCoroutine(PlayerController.DecreaseMetersOverTime(meterDecreaseRate, m_meterRefreshTimer));
    }


}
