using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using XnaScrapCore.Core.Systems.Resource;
using XnaScrapCore.Core.Interfaces.Other;

namespace XnaScrapCore.Core.Interfaces.Services
{
    public interface IResourceService
    {
        Dictionary<String, ModelMesh> Models
        {
            get;
        }

        Dictionary<String, Material> Materials
        {
            get;
        }

        Dictionary<String, Texture2D> Textures
        {
            get;
        }

        Dictionary<String, SpriteFont> Fonts
        {
            get;
        }

        Dictionary<String, IScript> Scripts
        {
            get;
        }

        Dictionary<String, Effect> Effects
        {
            get;
        }

        Dictionary<String, Material.ParameterMapping> ParameterMappings
        {
            get;
        }

        void loadModel(String name);
        void loadScript(String name);
        void loadTexture2D(String name);
        void loadEffect(String name);
        void loadMaterial(String name);
        void loadParameterMapping(String name);
        void loadFont(String name);
        void unload();
    }
}
