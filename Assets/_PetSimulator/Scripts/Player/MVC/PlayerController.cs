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
        Inactive, // Bored
        Critical,
    }
    public class PlayerController
    {
        private float sleepMeter = 100f;
        private float hungerMeter = 100f;
        private float happinessMeter = 100f;

        private float sleepDecreaseRate;
        private float hungerDecreaseRate;
        private float happinessDecreaseRate;
        private float bathDecreaseRate;
        private float pottyDecreaseRate;
        private float brushDecreaseRate;

        private float bathMeter = 100f;
        private float bathroomMeter = 100f;
        private float brushMeter = 100f;

        private float hygieneMeter = 100f;

        private float meterTimer = 0f;
        private float m_meterRefreshTimer;

        public float SleepMeter { get { return sleepMeter; } set { sleepMeter = value; } }
        public float HygieneMeter { get { return hygieneMeter; } set { hygieneMeter = value; } }
        public float HungerMeter { get { return hungerMeter; } set { hungerMeter = value; } }
        public float HappinessMeter { get { return happinessMeter; } set { happinessMeter = value; } }

        public PlayerView PlayerView { get; set; }

        public StateMachine StateManager { get; set; }

        public void Update()
        {
            DecreaseMetersOverTime();
            StateManager.Update();
        }

        public IEnumerator DecreaseMetersOverTime(float meterDecreaseRate, float meterRefreshTimer)
        {
            while (true)
            {
                sleepMeter = Mathf.Max(0, sleepMeter - meterDecreaseRate / 60f);
                hygieneMeter = Mathf.Max(0, hygieneMeter - meterDecreaseRate / 60f);
                hungerMeter = Mathf.Max(0, hungerMeter - meterDecreaseRate / 60f);
                happinessMeter = Mathf.Max(0, happinessMeter - meterDecreaseRate / 60f);

                yield return new WaitForSeconds(meterRefreshTimer);
            }
        }

        private void DecreaseMetersOverTime()
        {
            meterTimer += Time.deltaTime;
            if (meterTimer < m_meterRefreshTimer) return;
            meterTimer = 0f;

            sleepMeter = Mathf.Max(0, sleepMeter - sleepDecreaseRate);
            hungerMeter = Mathf.Max(0, hungerMeter - hungerDecreaseRate);
            happinessMeter = Mathf.Max(0, happinessMeter - happinessDecreaseRate);

            bathMeter = Mathf.Max(0, bathMeter - bathDecreaseRate);
            bathroomMeter = Mathf.Max(0, bathroomMeter - pottyDecreaseRate);
            brushMeter = Mathf.Max(0, brushMeter - brushDecreaseRate);

            hygieneMeter = (bathMeter + bathroomMeter + brushMeter) / 3f;
        }

        public void FeedPet(float foodAmount)
        {
            hungerMeter += foodAmount;
            if (hungerMeter > 100f) hungerMeter = 100f;
        }

        public void BrushPetTeeth()
        {
            brushMeter = 100f;
            hygieneMeter = (brushMeter + bathMeter + bathroomMeter) * 0.3f;
        }
        public void WashPet()
        {
            bathMeter = 100f;
            hygieneMeter = (brushMeter + bathMeter + bathroomMeter) * 0.3f;
        }
        public void DoToiletPet()
        {
            bathroomMeter = 100f;
            hygieneMeter = (brushMeter + bathMeter + bathroomMeter) * 0.3f;
        }

        public void PlayWithPet()
        {
            happinessMeter = 100f;
        }

        public void PetRested()
        {
            sleepMeter = 100f;
        }

        public PlayerController(
            PlayerView view,
            float sleepRate,
            float hungerRate,
            float happinessRate,
            float bathRate,
            float pottyRate,
            float brushRate,
            float refreshInterval)
        {
            PlayerView = view;
            sleepDecreaseRate = sleepRate;
            hungerDecreaseRate = hungerRate;
            happinessDecreaseRate = happinessRate;
            bathDecreaseRate = bathRate;
            pottyDecreaseRate = pottyRate;
            brushDecreaseRate = brushRate;
            m_meterRefreshTimer = refreshInterval;

            StateManager = new StateMachine(this);
            StateManager.Initialize(PetState.Normal);
        }
    }
}