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
using XnaScrapGui.Elements;
using XnaScrapCore.Core;
using XnaInput.Elements;
using XnaScrapCore.Core.Interfaces;
using System.IO;


namespace XnaScrapGui.Services
{
    #region helper
    class Quadtree
    {
        class QuadtreeNode
        {
            Vector2 Min;
            Vector2 Max;
            short Depth;
            QuadtreeNode[] Children;
            List<IGuiElement> m_elements = new List<IGuiElement>();

            public QuadtreeNode(Vector2 min, Vector2 max)
            {
                Min = min;
                Max = max;
            }

            public void add(IGuiElement guiElement)
            {
                if (!m_elements.Contains(guiElement))
                {
                    m_elements.Add(guiElement);
                }
            }

            public void remove(IGuiElement guiElement)
            {
                if (!m_elements.Contains(guiElement))
                {
                    m_elements.Remove(guiElement);
                }
            }

            public void click(int x, int y, ref List<IGuiElement> returns)
            {
                foreach (IGuiElement guiElement in m_elements)
                {
                    if (guiElement.Min.X <= x && guiElement.Max.X >= x
                        && guiElement.Min.Y <= y && guiElement.Max.Y >= y)
                        returns.Add(guiElement);
                }
            }

            // TODO implement
            private void split()
            {
                Children = new QuadtreeNode[4];
                Children[0] = new QuadtreeNode(Min, Max);
                Children[1] = new QuadtreeNode(Min, Max);
                Children[2] = new QuadtreeNode(Min, Max);
                Children[3] = new QuadtreeNode(Min, Max);

            }
        }
        #region member
        QuadtreeNode m_root = new QuadtreeNode(new Vector2(-1000, -1000), new Vector2(1000, 1000));
        #endregion

        #region CDtors
        public Quadtree()
        {

        }

        public void add(IGuiElement guiElement)
        {
            m_root.add(guiElement);
        }

        public void remove(IGuiElement guiElement)
        {
            m_root.remove(guiElement);
        }

        public List<IGuiElement> Click(int x, int y)
        {
            List<IGuiElement> returns = new List<IGuiElement>();
            m_root.click(x, y, ref returns);
            return returns;
        }
        #endregion
    }
    #endregion
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class GuiService : Microsoft.Xna.Framework.GameComponent, IComponent, IMouseListener
    {
        #region member
        private Quadtree m_guiElements = new Quadtree();
        private bool m_leftDown = false;
        #endregion

        #region events
        public delegate void ButtonEventHandler(XnaScrapId buttonId, int x, int y);

        public event ButtonEventHandler ButtonClicked;
        public event ButtonEventHandler ButtonDown;
        public event ButtonEventHandler ButtonUp;
        #endregion

        public GuiService(Game game)
            : base(game)
        {
            Game.Components.Add(this); ;
            Game.Services.AddService(typeof(GuiService), this);
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

        private void registerElements(IObjectBuilder objectBuilder)
        {

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

        public void doHandleMessage(BinaryReader msg)
        {
            
        }


        #region IMouseListener Members

        public bool setState(MouseState state)
        {
            // TODO use quadtree or something else
            if (state.XButton1 == ButtonState.Pressed && !m_leftDown)
            {
                List<IGuiElement> elements = m_guiElements.Click(state.X, state.Y);
                foreach (IGuiElement guiElement in elements)
                {
                    ButtonClicked(guiElement.Id, state.X, state.Y);
                    guiElement.OnClick(state.X, state.Y);
                }
                m_leftDown = true;

                // nothing should happen after gui hit
                return true;
            }
            else if (state.XButton1 == ButtonState.Released)
            {
                m_leftDown = false;
            }
            return false;
        }

        #endregion
    }
}
