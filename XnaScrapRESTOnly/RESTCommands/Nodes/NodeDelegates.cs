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
    //public delegate void SetColor(float r, float g, float b, float a);
    //public delegate void SetColor(float r, float g, float b);
    public delegate void SetColor(Color color);
    public delegate Color GetColor();
}
