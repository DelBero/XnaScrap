using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XnaScrapREST.Commands.Actions
{
    public abstract class AbstractAction
    {
        private String[] m_data;

        public String[] Data
        {
          get { return m_data; }
          set { m_data = value; }
        }

        public abstract String execute(Game game);
    }
}
