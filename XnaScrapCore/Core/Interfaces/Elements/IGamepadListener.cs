using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace XnaInput.Elements
{
    public interface IGamepadListener
    {
        /// <summary>
        /// transmit state
        /// </summary>
        /// <param name="state"></param>
        /// <returns>Is the state completly handleded</returns>
        bool setState(GamePadState state);

        /// <summary>
        /// Player Gamepad Index
        /// </summary>
        PlayerIndex PlayerIndex
        {
            get;
        }
    }
}
