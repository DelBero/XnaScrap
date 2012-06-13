using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using XnaScrapCore.Core.Systems;
using XnaScrapCore.Core.Interfaces.Services;
using Microsoft.Xna.Framework;


namespace XnaScrapCore.Core.Message
{
    public class Message
    {
        private MemoryStream m_stream = new MemoryStream();

        public IDataWriter Writer
        {
            get { return m_stream.Writer; }
        }

        public Message(XnaScrapId msgId)
        {
            reset(msgId);
        }

        public void reset(XnaScrapId msgId)
        {
            m_stream.reset();
            msgId.serialize(m_stream.Writer);
        }

        public void send(IObjectBuilder objectBuilder, XnaScrapId target)
        {
            m_stream.Position = 0;
            objectBuilder.sendMessage(m_stream.Reader, target);
        }

        public void send(Game game, XnaScrapId target)
        {
            m_stream.Position = 0;
            IObjectBuilder objectBuilder = game.Services.GetService(typeof(IObjectBuilder)) as IObjectBuilder;
            objectBuilder.sendMessage(m_stream.Reader, target);
        }
    }
}
