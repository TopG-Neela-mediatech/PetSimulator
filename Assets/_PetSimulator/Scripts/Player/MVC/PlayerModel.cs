
using UnityEngine;

namespace TMKOC.PetSimulator
{
    public class PlayerModel : ScriptableObject
    {
        public float RestedValue;
        public float HygieneValue;
        public float HungerValue;
        public float HappinessValue;


        public PlayerModel(PlayerData playerData)
        {
            RestedValue = playerData.HowRested;
            HygieneValue = playerData.HowHygienic;
            HungerValue = playerData.HowHungry;
            HappinessValue = playerData.HowHappy;
        }
    }

    [System.Serializable]
    public class PlayerData
    {
        public float HowRested;
        public float HowHygienic;
        public float HowHungry;
        public float HowHappy;
    }
}