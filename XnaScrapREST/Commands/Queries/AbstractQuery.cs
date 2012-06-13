using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml;

namespace XnaScrapREST.Commands.Queries
{
    public abstract class AbstractQuery
    {
        private String[] m_data;

        public String[] Data
        {
          get { return m_data; }
          set { m_data = value; }
        }

        protected AbstractQuery m_subQuery;

        public AbstractQuery SubQuery
        {
            get { return m_subQuery; }
        }

        public abstract String execute(Game game);

        public abstract XmlNodeList executeRest(Game game);
    }
}
