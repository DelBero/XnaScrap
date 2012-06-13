using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaScrapCore.Core;
using XnaScrapCore.Core.Interfaces.Services;
using XnaScrapCore.Core.Parameter;
using System.IO;
using XnaScrapCore.Core.Elements;
using XnaScrapGui.Services;
using Microsoft.Xna.Framework;

namespace XnaScrapGui.Elements
{
    public interface IGuiElement
    {
        Vector2 Min
        {
            get;
        }

        Vector2 Max
        {
            get;
        }

        XnaScrapId Id
        {
            get;
        }

        void OnClick(int x, int y);

        void OnMouseOver(int x, int y);

        void OnMouseLeft(int x, int y);

        void OnButtonDown(int x, int y);

        void OnButtonUp(int x, int y);
    }
}
