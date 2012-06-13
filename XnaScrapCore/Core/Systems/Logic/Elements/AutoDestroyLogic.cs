using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaScrapCore.Core.Elements;
using XnaScrapCore.Core.Interfaces.Services;
using XnaScrapCore.Core.Parameter;
using System.IO;

namespace XnaScrapCore.Core.Systems.Logic.Elements
{
    public class AutoDestroyLogic : AbstractLogic
    {
        #region member
        #region AbstractLogic
        private static XnaScrapId m_id = new XnaScrapId("{A24943B7-F67F-43DB-92BB-D4F6B7C3541E}");

        public static XnaScrapId Id
        {
            get { return AutoDestroyLogic.m_id; }
        }
        #endregion

        private int m_timeout;

        public int Timeout
        {
            get { return m_timeout; }
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
                return new AutoDestroyLogic(state);
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
            sequenceBuilder.addParameter(new IntParameter("timeout", "1000")); // miliseconds

            objectBuilder.registerElement(new Constructor(), sequenceBuilder.CurrentSequence, typeof(AutoDestroyLogic),"AutoDestroyLogic", null);
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        public AutoDestroyLogic(IDataReader state)
            : base(state)
        {
            m_timeout = state.ReadInt32();
        }

        

        public override void doInitialize()
        {
            base.doInitialize();
        }

        public override void doDestroy()
        {
            base.doDestroy();
        }

        public override void doUpdate(Microsoft.Xna.Framework.GameTime time)
        {
            m_timeout -= time.ElapsedGameTime.Milliseconds;
            if (m_timeout <= 0)
            {
                IObjectBuilder objectBuilder = Owner.Game.Services.GetService(typeof(IObjectBuilder)) as IObjectBuilder;
                objectBuilder.deleteGameObject(Owner.Id);
            }
            base.doUpdate(time);
        }
    }
}
