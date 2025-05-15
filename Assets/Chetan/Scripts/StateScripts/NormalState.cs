using System.Collections.Generic;
using TMKOC.PetSimulator;
using UnityEngine;

public class NormalState : IState
{
    private PlayerController playerController;

    public NormalState(PlayerController playerController)
    {
        this.playerController = playerController;
    }

    public void OnStateEnter()
    {
        Debug.Log("Entered Normal State");
        playerController.PlayerView.Animator.SetTrigger("Normal");
    }

    public void Update()
    {
        // nothing else every frame for Normal
    }

    public void OnStateExit()
    {
        Debug.Log("Exiting Normal State");
    }
}
