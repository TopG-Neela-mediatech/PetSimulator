using UnityEngine;

namespace TMKOC.PetSimulator
{
    public class GameManager : GenericSingleton<GameManager>
    {
        /*
        // Game Loop

        // On Game Start - trigger point invoked at start of the game 
           
            //  Get Time - Realtime? for day or night lighting Cycle
            // Spawn and initialize player
            // Get Status values from player model
        
        // On Status Change - 


        // Player Spawner Service -> MVC 

        // Pet Status Manager - decrement values of player model and get the values for displaying using UI_Meter 

        */

        [SerializeField] private PlayerView m_playerView;

        public PlayerView PlayerView
        {
            get { return m_playerView; }

        }
    }
}