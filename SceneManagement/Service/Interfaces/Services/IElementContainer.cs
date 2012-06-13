using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SceneManagement.Service.Elements;
using Microsoft.Xna.Framework;
using XnaScrapCore.Core;

namespace SceneManagement.Service.Interfaces.Services
{
    public interface IElementContainer
    {
        SceneElement insertElement(BoundingBox bb, GameObject gameObject);
        void removeElement(SceneElement se);
    }
}
