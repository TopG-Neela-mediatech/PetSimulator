namespace TMKOC.PetSimulator
{
    public interface IState
    {
        void OnStateEnter();
        void Update();
        void OnStateExit();
    }
}