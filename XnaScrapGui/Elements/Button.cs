using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaScrapCore.Core;
using XnaScrapCore.Core.Elements;
using System.IO;
using XnaScrapCore.Core.Interfaces.Services;
using XnaScrapCore.Core.Parameter;
using XnaScrapGui.Services;
using Microsoft.Xna.Framework;

namespace XnaScrapGui.Elements
{
    public class Button : AbstractElement, IGuiElement
    {
        #region registration
        #region Factory
        /// <summary>
        /// Factory class
        /// </summary>
        public class Constructor : IConstructor
        {
            GuiService m_guiService;
            public Constructor(GuiService guiService)
            {
                m_guiService = guiService;
            }
            public AbstractElement getInstance(BinaryReader state)
            {
                return new Button(state, m_guiService);
            }
        }
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectBuilder"></param>
        public static void registerElement(IObjectBuilder objectBuilder, GuiService guiService)
        {
            ParameterSequenceBuilder sequenceBuilder = new ParameterSequenceBuilder();
            sequenceBuilder.createSequence();

            objectBuilder.registerElement(new Constructor(guiService), sequenceBuilder.CurrentSequence, typeof(Button), "Button", null);
        }
        #endregion

        #region member
        private Vector2 m_min = new Vector2();
        private Vector2 m_max = new Vector2();
        private XnaScrapId m_id = new XnaScrapId();
        #endregion

        public Button(BinaryReader state, GuiService guiService)
            : base(state)
        {

        }

        #region IGuiElement
        public Vector2 Min
        {
            get { return m_min; }
        }

        public Vector2 Max
        {
            get { return m_min; }
        }

        public XnaScrapId Id
        {
            get { return m_id; }
        }

        public void OnClick(int x, int y) { }

        public void OnMouseOver(int x, int y) { }

        public void OnMouseLeft(int x, int y) { }

        public void OnButtonDown(int x, int y) { }

        public void OnButtonUp(int x, int y) { }
        #endregion
    }
}
