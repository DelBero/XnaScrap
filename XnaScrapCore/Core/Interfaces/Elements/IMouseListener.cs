﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace XnaInput.Elements
{
    public interface IMouseListener
    {
        /// <summary>
        /// transmit state
        /// </summary>
        /// <param name="state"></param>
        /// <returns>Is the state completly handleded</returns>
        bool setState(MouseState state);
    }
}
