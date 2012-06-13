using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaScrapCore.Core.Elements;
using System.IO;
using XnaScrapCore.Core.Parameter;
using XnaScrapCore.Core.Interfaces.Services;
using XnaScrapCore.Core.Interfaces.Other;
using XnaScrapCore.Core.Exceptions;

namespace XnaScrapCore.Core.Systems.StateMachine.Elements
{
    public class ScriptState : AbstractElement, IState
    {
        #region members
        private XnaScrapId m_id;

        public XnaScrapId Id
        {
            get { return m_id; }
        }

        private Dictionary<XnaScrapId, IState> m_transitions = new Dictionary<XnaScrapId, IState>();

        public Dictionary<XnaScrapId, IState> Transitions
        {
            get { return m_transitions; }
        }

        private XnaScrapId m_enterScriptId;
        private XnaScrapId m_exitScriptId;

        private IScript m_enterScript;
        private IScript m_exitScript;

        private IScriptExecutor m_scriptExecutor;
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
                return new ScriptState(state);
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
            sequenceBuilder.addParameter(new IdParameter("id",""));
            sequenceBuilder.addParameter(new IdParameter("enterScript", ""));
            sequenceBuilder.addParameter(new IdParameter("exitScript", ""));
            objectBuilder.registerElement(new Constructor(), sequenceBuilder.CurrentSequence, typeof(ScriptState), "ScriptState", null);
        }
        #endregion

        #region CDtors
        public ScriptState(IDataReader state) : base(state)
        {
            addInterface(typeof(IState));
            m_id.deserialize(state);
            m_enterScriptId = new XnaScrapId(state);
            m_exitScriptId = new XnaScrapId(state);
        }
        #endregion

        #region Interface AbstractElement
        public override void doInitialize()
        {
            base.doInitialize();
            IResourceService resService = Owner.Game.Services.GetService(typeof(IResourceService)) as IResourceService;
            m_scriptExecutor = Owner.Game.Services.GetService(typeof(IScriptExecutor)) as IScriptExecutor;

            if (resService == null)
            {
                throw new ServiceNotFoundException("IResourceService not found", typeof(IResourceService));
            }
            if (m_scriptExecutor == null)
            {
                throw new ServiceNotFoundException("IScriptExecutor not found", typeof(IScriptExecutor));
            }

            m_enterScript = resService.Scripts[m_enterScriptId.ToString()];
            m_exitScript = resService.Scripts[m_exitScriptId.ToString()];
        }

        public override void doDestroy()
        {
            base.doDestroy();
        }

        public override void doSerialize(IDataWriter state)
        {
            base.doSerialize(state);
            m_id.serialize(state);
            m_enterScriptId.serialize(state);
            m_exitScriptId.serialize(state);
        }
        #endregion

        #region Interface IState
        public bool addTransition(XnaScrapId msg, IState toState)
        {
            if (!m_transitions.Keys.Contains(msg))
            {
                m_transitions.Add(msg, toState);
                return true;
            }
            return false;
        }

        public bool canTransition(XnaScrapId msg)
        {
            return m_transitions.Keys.Contains(msg);
        }

        public IState transition(XnaScrapId msg)
        {
            if (m_transitions.Keys.Contains(msg))
            {
                return m_transitions[msg];
            }
            return this;
        }

        public void OnEnter()
        {
            m_scriptExecutor.execute(m_enterScript);
        }

        public void OnExit()
        {
            m_scriptExecutor.execute(m_exitScript);
        }
        #endregion
    }
}
