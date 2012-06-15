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
using CBero.Service.Elements.Interfaces;
using CBero.Service.Interface;
using XnaScrapCore.Core.Interfaces.Services;
using CBero.Service.Elements;
using CBero.Service.Interfaces.Elements;
using XnaScrapCore.Core.Interfaces;
using System.IO;
using XnaScrapCore.Core.Systems.Resource;
using XnaScrapCore.Core;
using XnaScrapCore.Core.Systems.Performance;
using CBero.Service.CBeroEffects;
using XnaScrapREST.REST;
using XnaScrapREST.Services;
using System.Xml;
using XnaScrapREST.REST.Nodes;
using CBero.Service.GraphicDeviceManagement;
#if WINDOWS
using System.Diagnostics;
#endif


namespace CBero.Service
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class RenderManager : Microsoft.Xna.Framework.DrawableGameComponent, IRenderManager, IComponent
    {
        public const int MAX_TEXTURES = 4;
        public const int MAX_LAYERS = 32;

        public struct TargetWindow
        {
            public int ProcessId;
            public string ChildWndCaption;
            public string ControlName;

            public bool TopLvl()
            {
                return ProcessId > 0;
            }
            public bool SubLvl()
            {
                return ( ChildWndCaption != null && ChildWndCaption != string.Empty);
            }

            public bool Control()
            {
                return (ControlName != null && ControlName != string.Empty);
            }
        }

        private static RenderManager m_current;
        public static RenderManager Current
        {
            get { return m_current; }
        }

        #region member
        #region IComponent
        private static Guid m_componentId = new Guid("D3F71433-1503-42C7-B838-D1FFD6F4C27F");
        public Guid Component_ID
        {
            get { return m_componentId; }
        }
        #endregion
        #region devicemanager
        GraphicsDeviceManager m_deviceManager = null;
        #endregion
        #region rendering stuff
        private List<RenderLayer> m_layers = new List<RenderLayer>();

        public List<RenderLayer> Layers
        {
            get { return m_layers; }
        }
        private List<IRenderable2D> m_overlays = new List<IRenderable2D>();
        private List<ITextRenderable> m_texts = new List<ITextRenderable>();
        private Dictionary<XnaScrapId, IRenderTarget> m_renderTargets = new Dictionary<XnaScrapId, IRenderTarget>();

        public Dictionary<XnaScrapId, IRenderTarget> RenderTargets
        {
            get { return m_renderTargets; }
        }

        private Dictionary<XnaScrapId, ICamera> m_cameras = new Dictionary<XnaScrapId, ICamera>();

        public Dictionary<XnaScrapId, ICamera> Cameras
        {
            get { return m_cameras; }
        }

        static private Material m_basicEffectMaterial = new Material();

        static public Material BasicEffectMaterial
        {
            get { return m_basicEffectMaterial; }
        }
        static private Material m_skinnedEffectMaterial = new Material();

        static public Material SkinnedEffectMaterial
        {
            get { return m_skinnedEffectMaterial; }
        }

        private Stack<RenderState> m_renderState = new Stack<RenderState>();

        public RenderState RenderState
        {
            get { return m_renderState.Peek(); }
        }

        public ICamera ActiveCamera
        {
            get { return RenderState.Camera; }
            set { RenderState.Camera = value; }
        }

        public Matrix View
        {
            get { return RenderState.m_currentCamera.Peek().View; }
        }
        public Matrix Projection
        {
            get { return RenderState.m_currentCamera.Peek().Projection; }
        }

        SpriteBatch m_spriteBatch;

        RenderTargetCollection m_defaultCollection;
        #endregion
        #region performance
        PerformanceSegment m_mainTimer = null;
        PerformanceSegment m_sceneTimer = null;
        PerformanceSegment m_overlayTimer = null;
        #endregion
        #region effects
        private CBeroEffect m_defaultEffect = null;
        #endregion
        #endregion

        public RenderManager(Game game, GraphicsDeviceManager deviceManager)
            : base(game)
        {
            _construct(game, deviceManager);
            m_current = this;
        }

        /// <summary>
        /// Render to a window of another process. Not possible  if deviceManager is not null or there is already a device manager installed.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="deviceManager"></param>
        /// <param name="target"></param>
        public RenderManager(Game game, GraphicsDeviceManager deviceManager, TargetWindow target)
            : base(game)
        {
            if (deviceManager == null)
            {
                CBeroGraphicsDeviceManager devMgr = new CBeroGraphicsDeviceManager(Game);
                if (target.TopLvl())
                {
                    if (target.SubLvl())
                    {
                        if (target.Control())
                        {
                            devMgr.SetToSubCtrlWindow(target.ProcessId, target.ChildWndCaption, target.ControlName);
                        }
                        else
                        {
                            devMgr.SetToSubWindow(target.ProcessId, target.ChildWndCaption);
                        }
                    }
                    else
                    {
                        devMgr.SetToTopWindow(target.ProcessId);
                    }
                }
                deviceManager = devMgr;
            }
            _construct(game, deviceManager);
        }

        private void _construct(Game game, GraphicsDeviceManager deviceManager)
        {
            Game.Components.Add(this);
            Game.Services.AddService(typeof(IRenderManager), this);
            Game.Services.AddService(typeof(RenderManager), this);

            m_deviceManager = deviceManager;
            if (m_deviceManager == null)
            {
                m_deviceManager = new CBeroGraphicsDeviceManager(Game);
                m_deviceManager.PreferredDepthStencilFormat = DepthFormat.Depth24;
                m_deviceManager.ApplyChanges();
            }

            m_renderState.Push(new RenderState(this));

            m_layers.Add(new RenderLayer(0));
            m_layers.Sort(new RenderLayerComparer());
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // register elements
            IObjectBuilder objectBuilder = Game.Services.GetService(typeof(IObjectBuilder)) as IObjectBuilder;
            if (objectBuilder != null)
            {
                registerElements(objectBuilder);
            }

            // performance
            PerformanceMonitor perfMon = Game.Services.GetService(typeof(PerformanceMonitor)) as PerformanceMonitor;
            if (perfMon != null)
            {
                m_mainTimer = perfMon.addPerformanceMeter(new XnaScrapId("CBeroRenderManager"));
                m_sceneTimer = m_mainTimer.addSubTimer("Scene");
                m_overlayTimer = m_mainTimer.addSubTimer("Overlays");
            }

#if WINDOWS
            // register REST stuff
            NetCtrlService netCtrlService = Game.Services.GetService(typeof(NetCtrlService)) as NetCtrlService;
            if (netCtrlService != null)
            {
                netCtrlService.AddServiceNode(new RenderManagerNode(this), this);
            }
#endif
            base.Initialize();
            m_spriteBatch = new SpriteBatch(GraphicsDevice);
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            #region materials
            BasicEffect basicEffect = new BasicEffect(GraphicsDevice);
            basicEffect.EnableDefaultLighting();
            basicEffect.PreferPerPixelLighting = true;
            m_basicEffectMaterial.Effect = basicEffect;
            m_basicEffectMaterial.Technique = basicEffect.Techniques.First().Name;
            Material.ParameterMapping worldviewprojection = new Material.ParameterMapping();
            worldviewprojection.name = "WorldViewProj";
            worldviewprojection.perInstance = true;
            worldviewprojection.semantic = Material.ShaderParameterSemantic.MODEL_VIEW_PROJECTION_MATRIX;
            m_basicEffectMaterial.ParameterMappings.Add(worldviewprojection);

            SkinnedEffect skinnedEffect = new SkinnedEffect(GraphicsDevice);
            skinnedEffect.EnableDefaultLighting();
            m_skinnedEffectMaterial.Effect = skinnedEffect;
            m_skinnedEffectMaterial.Technique = skinnedEffect.Techniques.First().Name;

            RenderState.PushMaterial(m_basicEffectMaterial);
            #endregion

            #region effects
            m_defaultEffect = new CBeroEffect(this);
            m_defaultCollection = new RenderTargetCollection(DefaultRenderTarget.GetInstance().Id, new IRenderTarget[] { DefaultRenderTarget.GetInstance() });
            m_defaultCollection.Effect = m_defaultEffect;
            m_defaultEffect.AddPass(new RenderSceneWithMaterialsPass(this, m_defaultCollection));
            m_defaultEffect.AddPass(new RenderOverlaysPass(this, m_defaultCollection));

            // add DefaultRenderTarget
            m_renderTargets.Add(DefaultRenderTarget.GetInstance().Id, m_defaultCollection);
            #endregion
        }

        protected override void Dispose(bool disposing)
        {
#if WINDOWS
            // register REST stuff
            NetCtrlService netCtrlService = Game.Services.GetService(typeof(NetCtrlService)) as NetCtrlService;
            if (netCtrlService != null)
            {
                netCtrlService.RemoveServiceNode(this);
            }
#endif
            base.Dispose(disposing);
        }

        private void registerElements(IObjectBuilder objectBuilder)
        {
            Renderable3D.registerElement(objectBuilder);
            Renderable2D.registerElement(objectBuilder);
            SkinnedRenderable3D.registerElement(objectBuilder);
            Camera.registerElement(objectBuilder, this);
            TextOverlay.registerElement(objectBuilder);
            Text3DBillboard.registerElement(objectBuilder);
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            foreach (RenderLayer layer in m_layers)
            {
                foreach (IRenderable3D r in layer.Renderables)
                {

                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            // perofrmance
            if (m_mainTimer != null)
                m_mainTimer.Watch.Restart();

            if (m_sceneTimer != null)
                m_sceneTimer.Watch.Reset();

            if (m_overlayTimer != null)
                m_overlayTimer.Watch.Reset();

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.Clear(ClearOptions.DepthBuffer | ClearOptions.Target, RenderState.GetClearColor(), 1.0f, 0);

            foreach (IRenderTarget renderTarget in m_renderTargets.Values)
            {
                if (!renderTarget.Active)
                    continue;

                RenderState.PushRenderTarget(renderTarget);
                foreach (ICamera camera in renderTarget.Cameras)
                {
                    RenderState.PushCamera(camera);
                    // TODO setViewport
                    if (renderTarget.Effect != null)
                    {
                        renderTarget.Effect.Draw();
                    }
                    else
                    {
                        throw new InvalidOperationException("The RenderTarget" + renderTarget.Id + "has no Effect assigend");
                    }
                    RenderState.PopCamera();
                }
                renderTarget.Present();
                RenderState.PopRenderTarget();
            }

            // performance
            if (m_mainTimer != null)
                m_mainTimer.Watch.Stop();

            //base.Draw(gameTime);
        }

        #region rendering
        /// <summary>
        /// 
        /// </summary>
        public void renderSceneWithMaterials()
        {
            if (m_sceneTimer != null)
                m_sceneTimer.Watch.Start();

            List<SkinnedRenderable3D> skinned = new List<SkinnedRenderable3D>();
            foreach (RenderLayer layer in m_layers)
            {
                GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.CornflowerBlue, 1.0f, 0);
                skinned.Clear();
                foreach (IRenderable3D renderable in layer.Renderables)
                {
                    if (renderable.Visible)
                    {
                        RenderState.PushRenderable(renderable);
                        //Renderable3D r = renderable as Renderable3D;
                        RenderState.PushMaterial(renderable.Material);
                        if (renderable.Parts.Count > 0)
                        {
                            RenderState.ApplyEffectParameterPerInstance();
                            RenderState.ApplyEffectPass();
                            foreach (IRenderablePart3D mmp in renderable.Parts)
                            {
                                mmp.bindBuffers(GraphicsDevice);
                                mmp.drawIndexed(GraphicsDevice);
                            }
                        }
                        RenderState.PopMaterial();
                    }
                    //else if (renderable is SkinnedRenderable3D)
                    //{
                    //    skinned.Add(renderable as SkinnedRenderable3D);
                    //}
                }
                //// Skinned
                //foreach (SkinnedRenderable3D r in skinned)
                //{
                //    foreach (SkinnedEffect effect in r.Model.Effects)
                //    {
                //        ApplyPerInstanceSkinned(r, effect);
                //        effect.EnableDefaultLighting();
                //        effect.SpecularColor = new Vector3(0.85f);
                //        effect.SpecularPower = 16;
                //        r.Model.Draw();
                //    }
                //}
            }
            m_spriteBatch.Begin();
            foreach (ITextRenderable text in m_texts)
            {
                text.draw(m_spriteBatch);
            }
            m_spriteBatch.End();

            if (m_sceneTimer != null)
                m_sceneTimer.Watch.Stop();
        }

        /// <summary>
        /// 
        /// </summary>
        public void renderOverlays()
        {
            if (m_overlayTimer != null)
                m_overlayTimer.Watch.Start();

            m_spriteBatch.Begin();
            foreach (IRenderable2D r in m_overlays)
            {
                if (r.Visible)
                {
                    if (r.Material != null)
                    {
                        if (r.Material.Textures[0] != null && r.Material.Textures[0] is Texture2D)
                        {
                            int x = (int)(r.Position.X * GraphicsDevice.Viewport.Width);
                            int y = (int)(r.Position.Y * GraphicsDevice.Viewport.Height);
                            int width = (int)(r.Scale.X * GraphicsDevice.Viewport.Width);
                            int height = (int)(r.Scale.Y * GraphicsDevice.Viewport.Height);
                            Rectangle rect = new Rectangle(x, y, width, height);
                            m_spriteBatch.Draw(r.Material.Textures[0] as Texture2D, rect, Color.White);
                        }
                    }
                    // Text
                    else if (r is TextOverlay)
                    {
                        TextOverlay text = r as TextOverlay;
                        m_spriteBatch.DrawString(text.Font, text.Text, text.Position, Color.Red);
                    }
                }
            }
            m_spriteBatch.End();

            if (m_overlayTimer != null)
                m_overlayTimer.Watch.Stop();
        }

        private void ApplyPerInstanceSkinned(SkinnedRenderable3D r, SkinnedEffect effect)
        {
            if (r.GetSkinTransforms() != null)
            {
                Matrix[] bones = r.GetSkinTransforms();
                effect.SetBoneTransforms(bones);
            }
            effect.View = RenderState.m_currentView.Peek();
            effect.Projection = RenderState.m_currentProj.Peek();
        }
        /// <summary>
        /// Set a shader parameter to some data provided by CBero
        /// </summary>
        /// <param name="effectParameter">the shader parameter</param>
        /// <param name="semantic">the type of data requested</param>
        public void setBuiltInShaderParameter(EffectParameter effectParameter, Material.ShaderParameterSemantic semantic)
        {
            if (effectParameter == null)
                return;
            switch (semantic)
            {
                case Material.ShaderParameterSemantic.MODEL_MATRIX:
                    {
                        effectParameter.SetValue(RenderState.m_currentWorld.Peek());
                        break;
                    }
                case Material.ShaderParameterSemantic.MODEL_INV_MATRIX:
                    {
                        Matrix inv = Matrix.Invert(RenderState.m_currentWorld.Peek());
                        effectParameter.SetValue(inv);
                        break;
                    }
                case Material.ShaderParameterSemantic.MODEL_INV_TRANS_MATRIX:
                    {
                        Matrix invTrans = Matrix.Transpose(Matrix.Invert(RenderState.m_currentWorld.Peek()));
                        effectParameter.SetValue(invTrans);
                        break;
                    }
                case Material.ShaderParameterSemantic.MODEL_TRANS_MATRIX:
                    {
                        Matrix trans = Matrix.Transpose(RenderState.m_currentWorld.Peek());
                        effectParameter.SetValue(trans);
                        break;
                    }
                case Material.ShaderParameterSemantic.VIEW_MATRIX:
                    {
                        effectParameter.SetValue(RenderState.m_currentView.Peek());
                        break;
                    }
                case Material.ShaderParameterSemantic.VIEW_INV_MATRIX:
                    {
                        Matrix inv = Matrix.Invert(RenderState.m_currentView.Peek());
                        effectParameter.SetValue(inv);
                        break;
                    }
                case Material.ShaderParameterSemantic.VIEW_TRANS_MATRIX:
                    {
                        Matrix trans = Matrix.Transpose(RenderState.m_currentView.Peek());
                        effectParameter.SetValue(trans);
                        break;
                    }
                case Material.ShaderParameterSemantic.VIEW_INV_TRANS_MATRIX:
                    {
                        Matrix invTrans = Matrix.Transpose(Matrix.Invert(RenderState.m_currentView.Peek()));
                        effectParameter.SetValue(invTrans);
                        break;
                    }
                case Material.ShaderParameterSemantic.PROJECTION_MATRIX:
                    {
                        effectParameter.SetValue(RenderState.m_currentProj.Peek());
                        break;
                    }
                case Material.ShaderParameterSemantic.VIEW_PROJECTION_MATRIX:
                    {
                        Matrix viewProj = Matrix.Multiply(RenderState.m_currentView.Peek(), RenderState.m_currentProj.Peek());
                        effectParameter.SetValue(viewProj);
                        break;
                    }
                case Material.ShaderParameterSemantic.MODEL_VIEW_PROJECTION_MATRIX:
                    {
                        Matrix mvp = Matrix.Multiply(Matrix.Multiply(RenderState.m_currentWorld.Peek(), RenderState.m_currentView.Peek()), RenderState.m_currentProj.Peek());
                        effectParameter.SetValue(mvp);
                        break;
                    }
                case Material.ShaderParameterSemantic.BONE_TRANSFORMS:
                    {
                        Matrix[] bone;
                        IRenderable3D renderable = RenderState.m_currentRenderable.Peek();
                        if (renderable is SkinnedRenderable3D)
                        {
                            SkinnedRenderable3D skinned = renderable as SkinnedRenderable3D;
                            bone = skinned.GetSkinTransforms();
                        }
                        else
                        {
                            bone = new Matrix[1] { Matrix.Identity };
                        }
                        effectParameter.SetValue(bone);
                        break;
                    }
                case Material.ShaderParameterSemantic.TEXTURE2D0:
                    {
                        effectParameter.SetValue(RenderState.m_currentTexture[0].Peek());
                        break;
                    }
                case Material.ShaderParameterSemantic.TEXTURE2D1:
                    {
                        effectParameter.SetValue(RenderState.m_currentTexture[1].Peek());
                        break;
                    }
                case Material.ShaderParameterSemantic.TEXTURE2D2:
                    {
                        effectParameter.SetValue(RenderState.m_currentTexture[2].Peek());
                        break;
                    }
                case Material.ShaderParameterSemantic.TEXTURE2D3:
                    {
                        effectParameter.SetValue(RenderState.m_currentTexture[3].Peek());
                        break;
                    }
                case Material.ShaderParameterSemantic.AMBIENTLIGHTING:
                    {
                        effectParameter.SetValue(RenderState.GetAmbientLighting().ToVector3());
                        break;
                    }
                case Material.ShaderParameterSemantic.BACKGROUNDCOLOR:
                    {
                        effectParameter.SetValue(RenderState.GetClearColor().ToVector3());
                        break;
                    }
                case Material.ShaderParameterSemantic.FOGCOLOR:
                    {
                        effectParameter.SetValue(RenderState.GetFogColor().ToVector3());
                        break;
                    }
                // TODO implement USER_DEFINED_*
            }
        }
        #endregion

        #region element management
        /// <summary>
        /// Adds a renderable to the list of drawable renderables.
        /// </summary>
        /// <param name="renderable"></param>
        public void addRenderable(IRenderable3D renderable, int layer)
        {
            if (layer >= MAX_LAYERS)
                return;
            foreach (RenderLayer renderLayer in m_layers)
            {
                if (renderLayer.Layer == layer)
                {
                    renderLayer.Renderables.Add(renderable);
                    return;
                }
                else if (renderLayer.Layer > layer)
                {
                    // TODO log!!
                    return;
                }
            }
        }

        public void moveToOtherLayer(IRenderable3D renderable, int oldLayer, int newLayer)
        {
            // remove from old layer
            foreach (RenderLayer renderLayer in m_layers)
            {
                if (renderLayer.Layer == oldLayer)
                {
                    if (!renderLayer.Renderables.Contains(renderable))
                        return;
                    else
                        renderLayer.Renderables.Remove(renderable);
                }
            }

            foreach (RenderLayer renderLayer in m_layers)
            {
                if (renderLayer.Layer == newLayer)
                {
                    renderLayer.Renderables.Add(renderable);
                    return;
                }
            }
        }

        /// <summary>
        /// Add an overlay
        /// </summary>
        /// <param name="renderable"></param>
        public void addRenderable(IRenderable2D renderable)
        {
            m_overlays.Add(renderable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public void addText(ITextRenderable text)
        {
            m_texts.Add(text);
        }


        /// <summary>
        /// Removes a renderable from the list of drawable renderables.
        /// </summary>
        /// <param name="renderable"></param>
        public void removeRenderable(IRenderable3D renderable, int layer)
        {
            if (layer >= MAX_LAYERS)
                return;
            foreach (RenderLayer renderLayer in m_layers)
            {
                if (renderLayer.Layer == layer)
                {
                    renderLayer.Renderables.Remove(renderable);
                }
            }
        }

        /// <summary>
        /// Removes a renderable from the list of drawable renderables.
        /// </summary>
        /// <param name="renderable"></param>
        public void removeRenderable(IRenderable2D renderable)
        {
            m_overlays.Remove(renderable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public void removeText(ITextRenderable text)
        {
            m_texts.Remove(text);
        }
        #endregion

        public IRenderTarget getRenderTarget(XnaScrapId id)
        {
            IRenderTarget target;
            if (m_renderTargets.TryGetValue(id, out target))
                return target;
            else
                return null;
        }

        public void pushRenderTargets(RenderTargetCollection collection)
        {

        }

        public void setRenderTarget(IRenderTarget target)
        {
            if (target is RenderTarget2D)
                GraphicsDevice.SetRenderTarget(target as RenderTarget2D);
            else if (target is RenderTargetCollection)
                setRenderTargets(target as RenderTargetCollection);
            else if (target == null)
                GraphicsDevice.SetRenderTarget(null);
        }

        public void setRenderTargets(RenderTargetCollection collection)
        {
            RenderTargetBinding[] renderTargets = new RenderTargetBinding[collection.Targets.Length]; // collection.Targets.Length
            bool none = true;
            for (int i = 0; i < collection.Targets.Length; ++i)
            {
                if (collection.Targets[i] is RenderTarget2D)
                {
                    renderTargets[i] = collection.Targets[i] as RenderTarget2D;
                    none = false;
                }
            }
            if (none)
                GraphicsDevice.SetRenderTargets(null);
            else
                GraphicsDevice.SetRenderTargets(renderTargets);
        }

        /// <summary>
        /// Receive a message
        /// </summary>
        /// <param name="msg">The message to read</param>
        public void doHandleMessage(IDataReader msg)
        {

        }

        #region renderlayer
        public void addRenderLayer(int layerId)
        {
            if (m_layers.Count >= MAX_LAYERS)
                throw new InvalidOperationException("Already to many Layers defined. Maximum number of layers is " + MAX_LAYERS);
            m_layers.Add(new RenderLayer(layerId));
            m_layers.Sort(new RenderLayerComparer());
        }

        public int getMaxLayerCount()
        {
            return MAX_LAYERS;
        }

        public int getCurrentLayerCount()
        {
            return m_layers.Count;
        }

        public int getAvailableLayerCount()
        {
            return getMaxLayerCount() - getCurrentLayerCount();
        }

        #endregion

        #region effects
        public void setRenderTargetEffect(XnaScrapId id, CBeroEffect effect)
        {
            IRenderTarget renderTarget;
            if (m_renderTargets.TryGetValue(id, out renderTarget))
            {
                renderTarget.Effect = effect;
            }
        }
        #endregion

        public bool addRemoteRenderTarget(int processId,
                                            Nullable<Rectangle> sourceRectangle = null,
                                            Nullable<Rectangle> destinationRectangle = null)
        {
            if (processId <= 0)
                return false;
            try
            {
                Process p = Process.GetProcessById(processId);
                if (p != null)
                {
                    return addRemoteRenderTarget(p.MainWindowHandle, sourceRectangle, destinationRectangle);
                }
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }

        public bool addRemoteRenderTarget(  IntPtr overrideWindowHandle,
                                            Nullable<Rectangle> sourceRectangle = null,
                                            Nullable<Rectangle> destinationRectangle = null
                                            )
        {
            RemoteWindowRenderTarget remoteTarget = new RemoteWindowRenderTarget(sourceRectangle, destinationRectangle, overrideWindowHandle, GraphicsDevice);
            CBeroEffect effect = new CBeroEffect(this);
            RenderTargetCollection remoteCollection = new RenderTargetCollection(remoteTarget.Id, new IRenderTarget[] { remoteTarget });
            remoteCollection.Effect = effect;
            effect.AddPass(new RenderSceneWithMaterialsPass(this, remoteCollection));
            effect.AddPass(new RenderOverlaysPass(this, remoteCollection));

            remoteTarget.Cameras.Add(m_defaultCollection.Cameras.First());

            // add DefaultRenderTarget
            m_renderTargets.Add(remoteTarget.Id, remoteCollection);
            return true;
        }
    }
#if WINDOWS
    public class RenderManagerNode : IRestNode
    {
        #region member
        private RenderManager m_renderManager;
        private NodeList m_subs = new NodeList("Data");
        #endregion
        #region CDtors
        public RenderManagerNode(RenderManager renderManager)
        {
            m_renderManager = renderManager;

            // build the resourcetree
            m_subs.addNode(new ColorNode(m_renderManager.Game
                                        , "AmbientLighting"
                                        , m_renderManager.RenderState.SetAmbientLighting
                                        , m_renderManager.RenderState.GetAmbientLighting));
            m_subs.addNode(new ColorNode(m_renderManager.Game
                                        , "BackgroundColor"
                                        , m_renderManager.RenderState.SetClearColor
                                        , m_renderManager.RenderState.GetClearColor));
            m_subs.addNode(new ColorNode(m_renderManager.Game
                                        , "FogColor"
                                        , m_renderManager.RenderState.SetFogColor
                                        , m_renderManager.RenderState.GetFogColor));

            m_subs.addNode(new RenderTargetEnumeratorNode(m_renderManager));

            m_subs.addNode(new CameraEnumeratorNode(m_renderManager));

            m_subs.addNode(new EffectEnumeratorNode(m_renderManager));
        }
        #endregion
        #region IRestNode Members
        public string Name
        {
            get { return "RenderManager"; }
        }

        private object m_data = null;
        public object Data
        {
            set { m_data = value; }
        }

        public XmlNodeList Get(string resourcePath)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement elements = doc.CreateElement("CBero.RenderManager");

            foreach(IRestNode node in m_subs)
            {
                XmlElement property = doc.CreateElement("Property");
                property.SetAttribute("Name", node.Name);
                elements.AppendChild(property);
            }
            return elements.ChildNodes;
        }

        public string Post(string data)
        {
            return "ok";
        }

        public string Put(string data)
        {
            return "ok";
        }

        public string Delete(string data)
        {
            return "ok";
        }

        public bool HasSub()
        {
            return true;
        }

        public IRestNode OnElement(string elementName)
        {
            if (m_subs != null)
            {
                m_subs.Data = m_renderManager;
                return m_subs.OnElement(elementName);
            }
            return null;
        }

        #endregion
    }

    public class CameraEnumeratorNode : IRestNode
    {
        #region member
        private RenderManager m_renderManager;
        private NodeList m_subs = new NodeList("Cameras");
        #endregion
        #region CDtors
        public CameraEnumeratorNode(RenderManager renderManager)
        {
            m_renderManager = renderManager;
        }
        #endregion
        #region IRestNode Members
        public string Name
        {
            get { return "Cameras"; }
        }

        private object m_data = null;
        public object Data
        {
            set { m_data = value; }
        }

        public XmlNodeList Get(string resourcePath)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement elements = doc.CreateElement("Cameras");

            if (m_data is IRenderTarget)
            {
                IRenderTarget target = m_data as IRenderTarget;
                foreach (ICamera camera in target.Cameras)
                {
                    XmlElement property = doc.CreateElement("Camera");
                    property.SetAttribute("Name", camera.ToString());
                    elements.AppendChild(property);
                }
            }
            else if (m_data is RenderManager)
            {
                return elements.ChildNodes;
            }

            return elements.ChildNodes;
        }

        public string Post(string data)
        {
            return "ok";
        }

        public string Put(string data)
        {
            return "ok";
        }

        public string Delete(string data)
        {
            return "ok";
        }

        public bool HasSub()
        {
            return true;
        }

        public IRestNode OnElement(string elementName)
        {
            if (m_subs != null)
                m_subs.Data = m_renderManager.getRenderTarget(new XnaScrapId(elementName));
            return m_subs;
        }

        #endregion
    }

    public class EffectEnumeratorNode : IRestNode
    {
        #region member
        private RenderManager m_renderManager;
        private NodeList m_subs = new NodeList("Effects");
        #endregion
        #region CDtors
        public EffectEnumeratorNode(RenderManager renderManager)
        {
            m_renderManager = renderManager;
        }
        #endregion
        #region IRestNode Members
        public string Name
        {
            get { return "Effects"; }
        }

        private object m_data = null;
        public object Data
        {
            set { m_data = value; }
        }

        public XmlNodeList Get(string resourcePath)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement elements = doc.CreateElement("Effects");

            if (m_data is IRenderTarget)
            {
                IRenderTarget target = m_data as IRenderTarget;
                CBeroEffect effect = target.Effect;
                {
                    XmlElement property = doc.CreateElement("Effect");
                    property.SetAttribute("Name", effect.ToString());
                    elements.AppendChild(property);
                }
            }
            else if (m_data is RenderManager)
            {
                return elements.ChildNodes;
            }

            return elements.ChildNodes;
        }

        public string Post(string data)
        {
            return "ok";
        }

        public string Put(string data)
        {
            return "ok";
        }

        public string Delete(string data)
        {
            return "ok";
        }

        public bool HasSub()
        {
            return true;
        }

        public IRestNode OnElement(string elementName)
        {
            if (m_subs != null)
            {
                m_subs.Data = m_renderManager.getRenderTarget(new XnaScrapId(elementName));
                return m_subs.OnElement(elementName);
            }
            return m_subs;
        }

        #endregion
    }

    public class RenderTargetEnumeratorNode : IRestNode
    {
        #region member
        private RenderManager m_renderManager;
        private NodeList m_subs = new NodeList("RenderTargets");
        #endregion
        #region CDtors
        public RenderTargetEnumeratorNode(RenderManager renderManager)
        {
            m_renderManager = renderManager;

            m_subs.addNode(new CameraEnumeratorNode(m_renderManager));

            m_subs.addNode(new EffectEnumeratorNode(m_renderManager));
        }
        #endregion
        #region IRestNode Members
        public string Name
        {
            get { return "RenderTargets"; }
        }

        private object m_data = null;
        public object Data
        {
            set { m_data = value; }
        }

        public XmlNodeList Get(string resourcePath)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement elements = doc.CreateElement("RenderTargets");

            foreach (XnaScrapId target in m_renderManager.RenderTargets.Keys)
            {
                XmlElement property = doc.CreateElement("RenderTarget");
                property.SetAttribute("Name", target.ToString());
                elements.AppendChild(property);
            }
            return elements.ChildNodes;
        }

        public string Post(string data)
        {
            return "ok";
        }

        public string Put(string data)
        {
            return "ok";
        }

        public string Delete(string data)
        {
            return "ok";
        }

        public bool HasSub()
        {
            return true;
        }

        public IRestNode OnElement(string elementName)
        {
            if (m_subs != null)
            {
                m_subs.Data = m_renderManager.getRenderTarget(new XnaScrapId(elementName));
                return m_subs;
            }
            return null;
        }

        #endregion
    }
#endif
}
