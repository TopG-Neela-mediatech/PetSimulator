
namespace TMKOC.PetSimulator
{
    public class UnrestedState : IState
    {
        private PlayerController playerController;
        public UnrestedState(PlayerController playerController) { this.playerController = playerController; }


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
