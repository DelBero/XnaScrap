using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;
using XnaScrapCore.Core.Interfaces.Services;
using XnaScrapCore.Core;
using XnaScrapCore.Core.Systems.Macro;
using XnaScrapCore.Core.Interfaces.Other;

namespace XnaScrapREST.REST.Nodes
{
    public class ColorNode : IRestNode
    {
        #region member
        private Game m_game;
        private String m_name;

        public String Name
        {
            get { return m_name; }
            set { m_name = value; }
        }
        public object Data
        {
            set { }
        }

        private SetColor m_setColorCallback;
        private GetColor m_getColorCallback;
        #endregion
        public ColorNode(Game game, String name, SetColor setColor, GetColor getColor)
        {
            m_game = game;
            m_name = name;
            m_setColorCallback = setColor;
            m_getColorCallback = getColor;
        }

        #region interface
        public bool HasSub() { return false; }

        public XmlNodeList Get(String resourcePath)
        {          
            XmlDocument doc = new XmlDocument();
            XmlElement color = doc.CreateElement("ColorNode");

            XmlElement colorValue = doc.CreateElement("Color");
            colorValue.SetAttribute("Name", Name);
            colorValue.SetAttribute("Value", m_getColorCallback().ToString());
            color.AppendChild(colorValue);

            return color.ChildNodes;
        }

        public String Post(String data)
        {
            data = data.Trim(new char[] { '{', '}' });
            string[] values = data.Split(' ');
            if (values.Length < 3 || values.Length > 4)
                return "error - wrong number of values supplied";

            int[] iValues = new int[values.Length];
            bool ok = true;
            for (int i = 0; i < values.Length; ++i)
                ok |= int.TryParse(values[i].Substring(2), out iValues[i]);

            if (values.Length == 3)
                m_setColorCallback(new Color(iValues[0],iValues[1],iValues[2]));
            else if (values.Length == 4)
                m_setColorCallback(new Color(iValues[0], iValues[1], iValues[2], iValues[3]));

            return "ok";
        }

        public String Put(String data)
        {
            return "invalid";
        }

        public String Delete(String data)
        {
            return "invalid";
        }

        public IRestNode OnElement(String elementName)
        {
            return null; // for put and delete commands
        }
        #endregion

        private String parseParameter(String resource)
        {
            String trimmed = resource.TrimStart('/');
            if (trimmed.EndsWith(m_name))
            {
                trimmed = trimmed.Substring(0, trimmed.Length - m_name.Length - 1 /* remove '/' */);
            }
            return trimmed.Substring(trimmed.LastIndexOf('/')).TrimStart('/');
        }
    }
}
