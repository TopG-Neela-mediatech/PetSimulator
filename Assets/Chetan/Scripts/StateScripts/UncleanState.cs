using System.Collections.Generic;
using UnityEngine;

namespace TMKOC.PetSimulator
{
    public class UncleanState : IState
    {
        private PlayerController playerController;
        public UncleanState(PlayerController playerController) { this.playerController = playerController; }


        public void OnStateEnter()
        {
            Debug.Log("Entering Unclean State");
            playerController.PlayerView.Animator.SetTrigger("Unclean");
        }
        public void OnStateExit()
        {
            Debug.Log("Exiting Unclean State");

        }
        public void Update()
        {
        }

    }
}
