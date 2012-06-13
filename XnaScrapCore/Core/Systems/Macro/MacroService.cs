using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using XnaScrapCore.Core.Interfaces.Services;
using XnaScrapCore.Core.Interfaces;
using System.IO;


namespace XnaScrapCore.Core.Systems.Macro
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class MacroService : Microsoft.Xna.Framework.GameComponent, IMacroService, IComponent
    {
        #region member
        private static Guid m_componentId = new Guid("D8C1D74B-6524-4D47-A6BE-DBDCE850080B");
        public Guid Component_ID
        {
            get { return m_componentId; }
        }

        private Dictionary<XnaScrapId, IMacro> m_registeredMacros = new Dictionary<XnaScrapId, IMacro>();

        public Dictionary<XnaScrapId, IMacro> RegisteredMacros
        {
            get { return m_registeredMacros; }
            set { m_registeredMacros = value; }
        }
        #endregion
        public MacroService(Game game)
            : base(game)
        {
            Game.Components.Add(this);
            Game.Services.AddService(typeof(IMacroService), this);
            Game.Services.AddService(typeof(MacroService), this);
            // TODO: Construct any child components here
        }

        public IMacro getMacro(XnaScrapId name)
        {
            IMacro macro = null;
            m_registeredMacros.TryGetValue(name, out macro);
            return macro;
        }

        public bool registerMacro(XnaScrapId name, IMacro macro)
        {
            if (!m_registeredMacros.Keys.Contains(name))
            {
                m_registeredMacros.Add(name,macro);
                return true;
            }
            return false;
        }

        public String getMacroNames(char seperator)
        {
            String s = "";
            foreach (XnaScrapId id in m_registeredMacros.Keys)
            {
                s += id.ToString();
                s += seperator;
            }
            return s;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }


        /// <summary>
        /// Receive a message
        /// </summary>
        /// <param name="msg">The message to read</param>
        public void doHandleMessage(IDataReader msg)
        {

        }

    }
}
