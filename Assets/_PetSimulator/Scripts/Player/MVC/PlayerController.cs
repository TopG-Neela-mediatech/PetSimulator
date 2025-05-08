using GLTFast.Schema;
using System;
using System.Collections;
using UnityEngine;

namespace TMKOC.PetSimulator
{
    public enum PetState
    {
        None,
        Normal,
        Sleepy,
        Dirty,
        Hungry,
        Bored,
        Critical,
    }

    public class PlayerController
    {
        private float sleepMeter = 100f;
        private float hygieneMeter = 100f;
        private float hungerMeter = 100f;
        private float happinessMeter = 100f;

        private PetState currentMood = PetState.None;

        public float SleepMeter { get { return sleepMeter; } set { sleepMeter = value; } }
        public float HygieneMeter { get { return hygieneMeter; } set { hygieneMeter = value; } }
        public float HungerMeter { get { return hungerMeter; } set { hungerMeter = value; } }
        public float HappinessMeter { get { return happinessMeter; } set { happinessMeter = value; } }

        public PlayerView PlayerView { get; set; }

        public StateMachine StateManager { get; set; }


        public IEnumerator DecreaseMetersOverTime(float meterDecreaseRate, float meterRefreshTimer)
        {
            while (true)
            {
                sleepMeter = Mathf.Max(0, sleepMeter - meterDecreaseRate / 60f);
                hygieneMeter = Mathf.Max(0, hygieneMeter - meterDecreaseRate / 60f);
                hungerMeter = Mathf.Max(0, hungerMeter - meterDecreaseRate / 60f);
                happinessMeter = Mathf.Max(0, happinessMeter - meterDecreaseRate / 60f);

                UpdateMood();
                yield return new WaitForSeconds(meterRefreshTimer); // check every second
            }
        }

        private void UpdateMood()
        {
            // Priority order: Sleep > Hygiene > Hunger > Boredom
            if (sleepMeter < 30f)
            {
                SetState(PetState.Sleepy);
            }
            else if (hygieneMeter < 30f)
            {
                SetState(PetState.Dirty);
            }
            else if (hungerMeter < 30f)
            {
                SetState(PetState.Hungry);
            }
            else if (happinessMeter < 30f)
            {
                SetState(PetState.Bored);
            }
            else
            {
                SetState(PetState.Normal);
            }

            Debug.Log("Current Mood: " + currentMood);
        }

        private void SetState(PetState mood)
        {
            if (currentMood == mood) return; // already in this mood
            currentMood = mood;

            // Play animation or trigger depending on mood
            switch (mood)
            {
                case PetState.Normal:
                    PlayerView.Animator.SetTrigger("Normal");
                    StateManager.TransitionTo(PetState.Normal);
                    break;
                case PetState.Sleepy:
                    PlayerView.Animator.SetTrigger("Sleepy");
                    StateManager.TransitionTo(PetState.Sleepy);
                    break;
                case PetState.Dirty:
                    PlayerView.Animator.SetTrigger("Dirty");
                    StateManager.TransitionTo(PetState.Dirty);
                    break;
                case PetState.Hungry:
                    PlayerView.Animator.SetTrigger("Hungry");
                    StateManager.TransitionTo(PetState.Hungry);
                    break;
                case PetState.Bored:
                    PlayerView.Animator.SetTrigger("Bored");
                    StateManager.TransitionTo(PetState.Bored);
                    break;
            }
        }

        // --- External functions to fill meters when you feed / clean / play ---
        public void FeedPet()
        {
            hungerMeter = 100f;
            UpdateMood();
        }

        public void CleanPet()
        {
            hygieneMeter = 100f;
            UpdateMood();
        }

        public void PlayWithPet()
        {
            happinessMeter = 100f;
            UpdateMood();
        }

        public void PutPetToSleep()
        {
            sleepMeter = 100f;
            UpdateMood();
        }

        public PlayerController(PlayerView playerView)
        {
            PlayerView = playerView;

            StateManager = new StateMachine(this);

        }
    }
}