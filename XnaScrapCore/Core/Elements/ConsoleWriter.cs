using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaScrapCore.Core.Elements;
using XnaScrapCore.Core.Interfaces.Services;
using XnaScrapCore.Core.Parameter;
using System.IO;

namespace XnaScrapCore.Core.Systems.Core.Elements
{
    public class ConsoleWriter : AbstractElement
    {
        #region member
        #region AbstractLogic
        private static XnaScrapId m_id = new XnaScrapId("{C99216F7-3CE6-4FE3-A2A2-92924F7E4625}");

        public static XnaScrapId Id
        {
            get { return ConsoleWriter.m_id; }
        }
        #endregion

        public static XnaScrapId PrintId = new XnaScrapId("print");

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
                return new ConsoleWriter(state);
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

            objectBuilder.registerElement(new Constructor(), sequenceBuilder.CurrentSequence, typeof(ConsoleWriter), "ConsoleWriter", null);
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        public ConsoleWriter(IDataReader state)
            : base(state)
        {
        }


        public override void doInitialize()
        {
            base.doInitialize();
        }

        public override void doDestroy()
        {
            base.doDestroy();
        }

        public override void doHandleMessage(IDataReader msg)
        {
            XnaScrapId msgId = new XnaScrapId(msg);
            if (PrintId == msgId)
            {
                String message = msg.ReadString();
                System.Console.WriteLine(message);
            }
            base.doHandleMessage(msg);
        }

    }
}
