using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

namespace TMKOC.PetSimulator
{
    [Serializable]
    public class StateMachine
    {
        private PlayerController playerController; 
        // reference to the state objects
        public NormalState normalState;
        public UncleanState uncleanState;
        public HungryState hungryState;
        public InactiveState inactiveState;
        public UnrestedState unrestedState;
        public CriticalState criticalState;


        public IState CurrentState { get; private set; }  // event to notify other objects of the state change
        public event Action<IState> stateChanged;

                
        public StateMachine(PlayerController player)
        {
            normalState = new NormalState(player);
            uncleanState = new UncleanState(player);
            inactiveState = new InactiveState(player);
            hungryState = new HungryState(player);
            criticalState = new CriticalState(player);
            unrestedState = new UnrestedState(player);
        }
        // set the starting state
        public void Initialize(IState state)
        {
            CurrentState = state;
            state.OnStateEnter();           
            stateChanged?.Invoke(state);
        }      
        public void TransitionTo(PetState petState)
        {
            CurrentState.OnStateExit();

            SelectState(petState);

            CurrentState.OnStateEnter();            
            stateChanged?.Invoke(CurrentState);
        }
       
        public void Update()
        {
            if (CurrentState != null)
            {
                CurrentState.Update();
            }
        }

        public void SelectState(PetState petState)
        {
            switch (petState)
            {
                case PetState.Normal:
                    {
                        CurrentState = normalState;
                        break;
                    }
                case PetState.Sleepy:
                    {
                        CurrentState = unrestedState;
                        break;
                    }
                case PetState.Dirty:
                    {
                        CurrentState = uncleanState;
                        break;
                    }
                case PetState.Hungry:
                    {
                        CurrentState = hungryState;
                        break;
                    }

                case PetState.Bored:
                    {
                        CurrentState = unrestedState;
                        break;
                    }
                case PetState.Critical:
                    {
                        CurrentState = criticalState;
                        break;
                    }
            }
        }

    }
}

