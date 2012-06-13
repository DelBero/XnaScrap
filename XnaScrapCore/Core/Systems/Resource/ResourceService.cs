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
using XnaScrapCore.Core.Interfaces.Other;
using XnaScrapCore.Core.Interfaces;
using System.IO;
using XnaScrapCore.Core.Exceptions;


namespace XnaScrapCore.Core.Systems.Resource
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class ResourceService : Microsoft.Xna.Framework.GameComponent, IResourceService, IComponent
    {
        #region member
        private static Guid m_componentId  = new Guid("F7A74DB7-F3D6-4AF5-861A-1DDAC1AEF381");
        public Guid Component_ID
        {
            get { return m_componentId; }
        }

        private Dictionary<String, ModelMesh> m_models = new Dictionary<String, ModelMesh>();
        private Dictionary<String, IScript> m_scripts = new Dictionary<String, IScript>();
        private Dictionary<String, Texture2D> m_textures = new Dictionary<String, Texture2D>();
        private Dictionary<String, Material> m_materials = new Dictionary<String, Material>();
        private Dictionary<String, Material.ParameterMapping> m_materialParameterMappings = new Dictionary<String, Material.ParameterMapping>();
        private Dictionary<String, SpriteFont> m_fonts = new Dictionary<String, SpriteFont>();
        private Dictionary<String, Effect> m_effects = new Dictionary<String, Effect>();

        public Dictionary<String, SpriteFont> Fonts
        {
            get { return m_fonts; }
        }

        public Dictionary<String, IScript> Scripts
        {
            get { return m_scripts; }
        }

        public Dictionary<String, Texture2D> Textures
        {
            get { return m_textures; }
        }

        public Dictionary<String, ModelMesh> Models
        {
            get { return m_models; }
        }

        public Dictionary<String, Material> Materials
        {
            get { return m_materials; }
        }

        public Dictionary<String, Material.ParameterMapping> ParameterMappings
        {
            get { return m_materialParameterMappings; }
        }

        public Dictionary<String, Effect> Effects
        {
            get { return m_effects; }
        }
        #endregion


        public ResourceService(Game game)
            : base(game)
        {
            game.Components.Add(this);
            game.Services.AddService(typeof(IResourceService),this);
            game.Services.AddService(typeof(ResourceService), this);
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

        /// <summary>
        /// Uses the ContentManager to load a Model.
        /// </summary>
        /// <param name="name"></param>
        public void loadModel(String name) 
        {
            Model m = Game.Content.Load<Model>(name);
            if (m != null) 
            {
                foreach (ModelMesh mm in m.Meshes)
                {
                    if (m.Meshes.Count == 1 && m.Tag != null)
                    {
                        // SkinningData
                        mm.Tag = m.Tag;
                        // save animation
                        IAnimationService animationService = Game.Services.GetService(typeof(IAnimationService)) as IAnimationService;
                        if (animationService != null)
                        {
                            animationService.addAnimation(new XnaScrapId(mm.Name),animationService.DefaultSkinnedAnimationFactory(mm.Tag));
                        }
                        else
                        {
                            //throw new ServiceNotFoundException("No Animation service found.", typeof(IAnimationService));
                        }
                    }
                    if (!m_models.Keys.Contains(mm.Name))
                    {
                        m_models.Add(mm.Name, mm);
                    }
                    else
                    {
                        // TODO error handling
                    }
                }
                // Animation
            }
        }

        /// <summary>
        /// Loads a Script.
        /// </summary>
        /// <param name="name">The script's filename.</param>
        public void loadScript(String name)
        {
            IScript script = Game.Content.Load<IScript>(name);
            if (script != null)
            {
                m_scripts.Add(name, script);
            }
        }

        /// <summary>
        /// Loads a texture.
        /// </summary>
        /// <param name="name">The texture.</param>
        public void loadTexture2D(String name)
        {
            Texture2D texture = Game.Content.Load<Texture2D>(name);
            if (texture != null)
            {
                m_textures.Add(name,texture);
            }
        }

        /// <summary>
        /// Loads a material.
        /// </summary>
        /// <param name="name"></param>
        public void loadMaterial(String name)
        {
            Material material = Game.Content.Load<Material>(name);
            if (material != null)
            {
                processMaterial(material);
                m_materials.Add(name, material);
                //m_materials.Add(name, loadMaterial(material));
            }
            //Effect effect = Game.Content.Load<Effect>(name);
            //if (effect != null)
            //{
            //    m_materials.Add(name, effect);
            //}
        }

        private void processMaterial(Material mat)
        {
            for (int i = 0; i < mat.TextureNames.Length; ++i)
            {
                if (!string.IsNullOrEmpty(mat.TextureNames[i]))
                {
                    if (!m_textures.TryGetValue(mat.TextureNames[i], out mat.Textures[i]))
                    {
                        // try to load the texture now
                        loadTexture2D(mat.TextureNames[i]);

                        m_textures.TryGetValue(mat.TextureNames[i], out mat.Textures[i]);
                    }
                }
                else
                {
                    break;
                }
            }

            Effect effect;
            if (m_effects.TryGetValue(mat.EffectName, out effect))
            {
                mat.Effect = effect;
                mat.Effect.CurrentTechnique = mat.Effect.Techniques[mat.Technique];
            }

        }

        /// <summary>
        /// Load a parameter mapping used to fill shaderparameters
        /// </summary>
        /// <param name="name"></param>
        public void loadParameterMapping(String name)
        {
            Material.ParameterMapping mapping = Game.Content.Load<Material.ParameterMapping>(name);
            if (mapping != null)
            {
                m_materialParameterMappings.Add(name, mapping);
            }
        }

        private void processMapping(Material.ParameterMapping mapping)
        {

        }

        /// <summary>
        /// Loads a font.
        /// </summary>
        /// <param name="name"></param>
        public void loadFont(String name)
        {
            SpriteFont font = Game.Content.Load<SpriteFont>(name);
            if (font != null)
            {
                m_fonts.Add(name, font);
            }
        }

        /// <summary>
        /// Load an Effect.
        /// </summary>
        /// <param name="name"></param>
        public void loadEffect(String name)
        {
            Effect effect = Game.Content.Load<Effect>(name);
            if (effect != null)
            {
                m_effects.Add(name, effect);
            }
        }

        /// <summary>
        /// Unloads all resources.
        /// </summary>
        public void unload()
        {
            m_models.Clear();
            m_scripts.Clear();
            m_textures.Clear();
            m_materials.Clear();
            m_effects.Clear();
            Game.Content.Unload();
        }

        /// <summary>
        /// Receive a message
        /// </summary>
        /// <param name="msg">The message to read</param>
        public void doHandleMessage(IDataReader msg)
        {

        }

    }
}
