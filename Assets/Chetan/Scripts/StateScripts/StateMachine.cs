using System;
using System.Collections.Generic;
using UnityEngine;

namespace TMKOC.PetSimulator
{
    public struct Transition
    {
        public Func<bool> Condition;
        public PetState NextState;
    }

    [Serializable]
    public class StateMachine
    {
        private readonly PlayerController playerController;

        // concrete state instances
        public NormalState normalState;
        public UnrestedState unrestedState;
        public UncleanState uncleanState;
        public HungryState hungryState;
        public InactiveState inactiveState;
        public CriticalState criticalState;

        // ordered list of “global” transitions
        private readonly List<Transition> globalTransitions;

        public IState CurrentState { get; private set; }
        public event Action<IState> stateChanged;
        public string CurrentStateName => CurrentState.GetType().Name;

        public StateMachine(PlayerController player)
        {
            playerController = player;

            // instantiate all states
            normalState = new NormalState(player);
            unrestedState = new UnrestedState(player);
            uncleanState = new UncleanState(player);
            hungryState = new HungryState(player);
            inactiveState = new InactiveState(player);
            criticalState = new CriticalState(player);

            // build your priority list once
            globalTransitions = new List<Transition>
            {
                // 1) Sleepy
                new Transition {
                    Condition = () => playerController.SleepMeter      < 30f,
                    NextState = PetState.Sleepy
                },
                // 2) Dirty
                new Transition {
                    Condition = () => playerController.HygieneMeter    < 30f,
                    NextState = PetState.Dirty
                },
                // 3) Hungry
                new Transition {
                    Condition = () => playerController.HungerMeter     < 30f,
                    NextState = PetState.Hungry
                },
                // 4) Inactive
                new Transition {
                    Condition = () => playerController.HappinessMeter  < 30f,
                    NextState = PetState.Inactive
                },
                // 5) Recover to Normal once all are healthy
                new Transition {
                    Condition = () =>
                        playerController.SleepMeter      >= 30f &&
                        playerController.HygieneMeter    >= 30f &&
                        playerController.HungerMeter     >= 30f &&
                        playerController.HappinessMeter  >= 30f,
                    NextState = PetState.Normal
                }
            };
        }

        public void Initialize(PetState startState)
        {
            SelectState(startState);
            CurrentState.OnStateEnter();
            stateChanged?.Invoke(CurrentState);
            playerController.PlayerView.ChangeState(CurrentStateName);
        }

        public void Update()
        {
            // 1) Run global transitions in priority order
            foreach (var t in globalTransitions)
            {
                if (t.Condition())
                {
                    if (GetStateInstance(t.NextState) != CurrentState)
                        TransitionTo(t.NextState);
                    return;
                }
            }

            // 2) If none fired, let the current state do its own per-frame work
            CurrentState.Update();
        }

        private void TransitionTo(PetState target)
        {
            CurrentState.OnStateExit();
            SelectState(target);
            CurrentState.OnStateEnter();
            stateChanged?.Invoke(CurrentState);
            playerController.PlayerView.ChangeState(CurrentStateName);
        }

        private void SelectState(PetState petState)
        {
            switch (petState)
            {
                case PetState.Normal: CurrentState = normalState; break;
                case PetState.Sleepy: CurrentState = unrestedState; break;
                case PetState.Dirty: CurrentState = uncleanState; break;
                case PetState.Hungry: CurrentState = hungryState; break;
                case PetState.Inactive: CurrentState = inactiveState; break;
                case PetState.Critical: CurrentState = criticalState; break;
                default: CurrentState = normalState; break;
            }
        }

        private IState GetStateInstance(PetState petState)
        {
            switch (petState)
            {
                case PetState.Normal: return normalState;
                case PetState.Sleepy: return unrestedState;
                case PetState.Dirty: return uncleanState;
                case PetState.Hungry: return hungryState;
                case PetState.Inactive: return inactiveState;
                case PetState.Critical: return criticalState;
                default: return normalState;
            }
        }
    }
}
