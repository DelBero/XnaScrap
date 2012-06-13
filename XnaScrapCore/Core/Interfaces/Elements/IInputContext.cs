using System;
namespace XnaScrapCore.Core.Interfaces.Elements
{
    public interface IInputContext
    {
        bool setGesture(Microsoft.Xna.Framework.Input.Touch.GestureSample sample, Microsoft.Xna.Framework.Input.Touch.GestureType type);
        bool setState(Microsoft.Xna.Framework.Input.KeyboardState state);
        bool setState(Microsoft.Xna.Framework.Input.MouseState state);
        bool setTouch(System.Collections.Generic.IList<Microsoft.Xna.Framework.Input.Touch.TouchLocation> location);
        bool setState(Microsoft.Xna.Framework.Input.GamePadState state);

        Microsoft.Xna.Framework.PlayerIndex PlayerIndex
        {
            get;
        }
    }
}
