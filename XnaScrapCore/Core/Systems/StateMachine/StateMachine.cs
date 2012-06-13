using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using XnaScrapCore.Core.Elements;
using XnaScrapCore.Core.Interfaces.Services;
using XnaScrapCore.Core.Parameter;

namespace XnaScrapCore.Core.Systems.StateMachine
{
    public class StateChangedEventArgs : EventArgs
    {
        public static StateChangedEventArgs Empty;
    }

    public class StateMachine : AbstractElement
    {
        #region transition temp class
        private struct Transition
        {
            public XnaScrapId Msg;
            public XnaScrapId From;
            public XnaScrapId To;

            public void deserialize(IDataReader state)
            {

            }

            public void serialize(IDataWriter state)
            {

            }
        }
        #endregion

        #region member
        private XnaScrapId m_initState;

        private List<IState> m_states = new List<IState>();

        private IState m_activeState = null;

        public IState ActiveState
        {
            get { return m_activeState; }
        }

        private IState State
        {
            set
            {
                if (value != m_activeState)
                {
                    m_activeState = value;
                    OnChanged(StateChangedEventArgs.Empty);
                }
            }
        }
        #endregion

        #region events & delegates
        public delegate void OnChangedState(object sender, StateChangedEventArgs args);

        public event OnChangedState StateChanged;

        protected void OnChanged(StateChangedEventArgs args)
        {
            if (StateChanged != null)
            {
                StateChanged(this,args);
            }
        }
        #endregion

        #region registration
        #region Factory
        /// <summary>
        /// Factory class
        /// </summary>
        public class Constructor : IConstructor
        {
            public AbstractElement getInstance(IDataReader state)
            {
                return new StateMachine(state);
            }
        }
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectBuilder"></param>
        public static void registerElement(IObjectBuilder objectBuilder)
        {
            ParameterSequenceBuilder sequenceBuilder = new ParameterSequenceBuilder();
            sequenceBuilder.createSequence();

            CompoundParameter transition = new CompoundParameter("transition");
            transition.addParameter(new IdParameter("msg", ""));
            transition.addParameter(new IdParameter("from",""));
            transition.addParameter(new IdParameter("to", ""));

            sequenceBuilder.addParameter(new IdParameter("initState", ""));
            sequenceBuilder.addParameter(new SequenceParameter("transitions", transition));

            objectBuilder.registerElement(new Constructor(), sequenceBuilder.CurrentSequence, typeof(StateMachine), "StateMachine", null);
        }
        #endregion

        #region CDtors
        public StateMachine(IDataReader state) 
            : base(state)
        {
            m_initState.deserialize(state);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="initState">The initial state of the StateMachine.</param>
        public void init(IState initState)
        {
            m_activeState = initState;
        }
        #endregion

        #region Interface AbstractElement
        public override void doInitialize()
        {
            base.doInitialize();
            foreach (IState state in Owner.getAll(typeof(IState)))
            {
                m_states.Add(state);
                if (state.Id == m_initState)
                {
                    init(state);
                }
            }
        }

        public override void doDestroy()
        {

            base.doDestroy();
        }

        public override void doSerialize(IDataWriter state)
        {
            base.doSerialize(state);
            m_activeState.Id.serialize(state);
        }
        #endregion

        #region methods
        public void OnReceiveMessage(XnaScrapId msgId)
        {
            if (ActiveState != null)
            {
                State = ActiveState.transition(msgId);
            }
        }

        public void addState(IState state)
        {
            m_states.Add(state);
        }
        #endregion
    }
}
