using System.Collections.Generic;
using UnityEngine;

namespace TMKOC.PetSimulator
{
    public class HungryState : IState
    {
        private PlayerController playerController;
        public HungryState(PlayerController playerController)
        {
            this.playerController = playerController;
        }

        public void OnStateEnter()
        {
            Debug.Log("Entering Hungry State");

            playerController.PlayerView.Animator.SetTrigger("Hungry");

        }
        public void OnStateExit()
        {
            Debug.Log("Exiting Hungry State");
        }
        public void Update()
        {

        }
    }
}
