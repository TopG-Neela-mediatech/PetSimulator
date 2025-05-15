using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMKOC.PetSimulator
{
    public class InactiveState : IState
    {
        private PlayerController playerController;
        public InactiveState(PlayerController playerController) { this.playerController = playerController; }


        public void OnStateEnter()
        {
            Debug.Log("Entering Inactive State");
            playerController.PlayerView.Animator.SetTrigger("Inactive");
        }
        public void OnStateExit()
        {
            Debug.Log("Exiting Inactive State");
        }
        public void Update()
        {

        }
    }
}
