using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMKOC.PetSimulator
{
    public interface IState 
    {
        public void OnStateEnter();
        public void Update();
        public void OnStateExit();
    }
}
