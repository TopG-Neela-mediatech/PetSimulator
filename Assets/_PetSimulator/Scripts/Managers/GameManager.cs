using UnityEngine;

namespace TMKOC.PetSimulator
{
    public enum TFLocation
    {
        None = 0,
        Bath,
        Brush,
    }

    [System.Serializable]
    public struct TransformLocations
    {
        public TFLocation LocationType;
        public Transform TransformPoint;
    }

    public class GameManager : GenericSingleton<GameManager>
    {
        /*
        // Game Loop

        // On Game Start - trigger point invoked at start of the game 
           
            //  Get Time - Realtime? for day or night lighting Cycle
            // Spawn and initialize player
            // Get Status values from player model
        
        // On Status Change - 


        // Player Spawner Service -> MVC -> No model ->  PlayerView is Spawner

        // Pet Status Manager - decrement values of player model and get the values for displaying using UI_Meter 

        */

        [SerializeField] private PlayerView m_playerView;

        [Header("Transform Points for Specific Actions")]
        [SerializeField] private TransformLocations[] m_locationPoints;

        public Transform GetTransformPoint(TFLocation location)
        {
            foreach (var locationPoint in m_locationPoints)
            {
                if (location == locationPoint.LocationType)
                {
                    return locationPoint.TransformPoint;
                }
            }
            return null;
        }


        public PlayerView PlayerView
        {
            get { return m_playerView; }

        }
    }
}