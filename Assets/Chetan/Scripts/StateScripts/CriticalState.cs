using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMKOC.PetSimulator
{
    public class CriticalState : IState
    {
        private PlayerController playerController;
        public CriticalState(PlayerController playerController) { this.playerController = playerController; }


        public void OnStateEnter()
        {
            Debug.Log("Entering Critical State");
            playerController.PlayerView.Animator.SetTrigger("Critical");
        }
        public void OnStateExit()
        {
            Debug.Log("Exiting Critical State");
        }
        public void Update()
        {

        }

    }
}
