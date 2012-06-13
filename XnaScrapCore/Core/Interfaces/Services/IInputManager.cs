using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaInput.Elements;
using XnaScrapCore.Core.Interfaces.Elements;

namespace XnaScrapCore.Core.Interfaces.Services
{
    public interface IInputManager
    {
        List<IKeyboardListener> KeyboardListener
        {
            get;
        }

        List<IMouseListener> MouseListener
        {
            get;
        }

        List<ITouchListener> TouchListener
        {
            get;
        }

        void setInputContext(IInputContext context);
        void removeInputContext(IInputContext context);
    }
}
