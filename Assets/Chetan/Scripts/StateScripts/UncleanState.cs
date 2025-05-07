using System.Collections;
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
            throw new System.NotImplementedException();
        }
        public void OnStateExit()
        {
            throw new System.NotImplementedException();
        }
        public void Update()
        {
            throw new System.NotImplementedException();
        }
    }
}
