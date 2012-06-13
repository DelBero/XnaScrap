using System;
namespace XnaScrapCore.Core.Systems.StateMachine
{
    public interface IState
    {
        XnaScrapId Id
        {
            get;
        }
        bool addTransition(XnaScrapCore.Core.XnaScrapId msg, IState toState);
        bool canTransition(XnaScrapCore.Core.XnaScrapId msg);
        IState transition(XnaScrapCore.Core.XnaScrapId msg);

        void OnEnter();
        void OnExit();
    }
}
