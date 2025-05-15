using System.Collections.Generic;
using UnityEngine;

namespace TMKOC.PetSimulator
{
    public class UnrestedState : IState
    {
        private PlayerController playerController;
        public UnrestedState(PlayerController playerController) { this.playerController = playerController; }

        public void OnStateEnter()
        {
            Debug.Log("Entering Unrested State");
            playerController.PlayerView.Animator.SetTrigger("Sleepy");
        }
        public void OnStateExit()
        {
            Debug.Log("Exiting Unrested State");
        }
        public void Update()
        {
            
        }
    }
}
