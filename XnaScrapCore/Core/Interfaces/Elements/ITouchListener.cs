using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace XnaInput.Elements
{
    public interface ITouchListener
    {
        /// <summary>
        /// transmit locations
        /// </summary>
        /// <param name="state"></param>
        /// <returns>Is the state completly handleded</returns>
        bool setTouch(IList<TouchLocation> location);
        bool setGesture(GestureSample sample, GestureType type);
    }
}
