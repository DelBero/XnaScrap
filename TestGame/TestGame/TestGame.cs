using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

using XnaScrapCore.Core.Systems.ObjectBuilder;
using XnaScrapCore.Core.Systems.Logic.Elements;
using XnaScrapCore.Core.Systems.Core;
using XnaScrapCore.Core.Interfaces.Services;
using CBero.Service;
using CBero.Service.Elements;
using XmlScripting.Service;
using System.IO;
using XnaScrapCore.Core;
using XnaScrapCore.Core.Parameter;
using CollisionSystem.Service;
using SceneManagement.Service;
using XnaInput.Service;
using XnaInput.Elements;
using XnaScrapCore.Core.Interfaces.Other;
#if WINDOWS
using XnaScrapREST.Services;
using CollisionSystem.Data_Structures;
using CollisionSystem.Elements;
using XnaScrapCore.Core.Systems.Performance;
using CollisionSystem.DataStructures;
using XnaScrapAnimation.Service;
using System.Reflection;

#endif


namespace TestGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class TestGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics = null;
        SpriteBatch spriteBatch;
        SpriteFont font1;
        SpriteFont font2;
        Core m_core;
        RenderManager m_renderManager;
        XmlScriptExecutor m_scriptExecutor;
        XmlScriptCompilerService m_scriptCompiler;
        CollisionManager m_collisionManager;
        SceneManager m_sceneManager;
        InputManager m_inputManager;
        AnimationService m_animationService;
#if WINDOWS
        NetCtrlService m_netCtrl;
#endif

        public TestGame(int processId = 0, string window = "")
        {
            //graphics = new GraphicsDeviceManager(this);
            //graphics.PreferredDepthStencilFormat = DepthFormat.Depth24;
            //graphics.ApplyChanges();
            Content.RootDirectory = "Content";

            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);

            m_core = new Core(this);
            m_scriptExecutor = XmlScriptExecutor.GetInstance(this);
            m_scriptCompiler = new XmlScriptCompilerService(this);
            m_collisionManager = new CollisionManager(this);
            m_sceneManager = new SceneManager(this);
            m_inputManager = new InputManager(this);
            m_animationService = new AnimationService(this);
            // graphics should be inserted last, as they should be updated at the end of a frame
            //RenderManager.TargetWindow target = new RenderManager.TargetWindow();
            //target.ProcessId = processId;
            //target.ChildWndCaption = window;
            //m_renderManager = new RenderManager(this, graphics, target);
            m_renderManager = new RenderManager(this, graphics);
#if WINDOWS
            m_netCtrl = new NetCtrlService(this);
#endif
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
#if WINDOWS
            //m_netCtrl.start(38796);
            m_netCtrl.startREST();
#endif
            System.Console.WriteLine(Assembly.GetExecutingAssembly().GetName().Version);
            base.Initialize();

            //m_renderManager.addRemoteRenderTarget(996);
        }

        protected override void Dispose(bool disposing)
        {
#if WINDOWS
            //m_netCtrl.stop();
            //m_netCtrl.startREST();
#endif
            base.Dispose(disposing);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font1 = Content.Load<SpriteFont>("Font1");
            font2 = Content.Load<SpriteFont>("Font2");
            IResourceService resService = Services.GetService(typeof(IResourceService)) as IResourceService;
            if (resService != null) {
                resService.loadModel("models/primitives");
                resService.loadModel("models/Character_run");
                resService.loadTexture2D("textures/sw");
                resService.loadEffect("effects/Blinn");
                resService.loadMaterial("materials/Material");
                resService.loadFont("Font1");
            }

            IObjectBuilder objectBuilder = Services.GetService(typeof(IObjectBuilder)) as IObjectBuilder;

            // test scripting
            XmlScriptCompiled script1 = Content.Load<XmlScriptCompiled>("scripts/Script1");
            //XmlScriptCompiled script2 = Content.Load<XmlScriptCompiled>("scripts/AnimationTest");
            DateTime start = DateTime.Now;
            m_scriptExecutor.execute(script1, new String[] { "0,0,0", "Sphere01", "ScriptSphere" });
            //m_scriptExecutor.execute(script2);

            DateTime end = DateTime.Now;
            TimeSpan executionTime = end - start;

            // camera
            objectBuilder.beginGameObject(new XnaScrapCore.Core.XnaScrapId("Camera"));
            objectBuilder.beginElement(typeof(Camera));
            objectBuilder.endElement();
            objectBuilder.beginElement(typeof(StaticPosition3D));
            objectBuilder.beginParameter("position");
            objectBuilder.setValues("0,0,5");
            objectBuilder.endParameter();
            objectBuilder.endElement();
#if WINDOWS_PHONE
            //objectBuilder.beginElement(typeof(TouchRotation3D));
#endif
#if WINDOWS
            //objectBuilder.beginElement(typeof(MouseRotation3D));
#endif
#if XBOX360
#endif
            //objectBuilder.endElement();
            objectBuilder.endGameObject();

            //graphics.SupportedOrientations = DisplayOrientation.Portrait |
            //                                 DisplayOrientation.LandscapeLeft |
            //                                 DisplayOrientation.LandscapeRight;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            Content.Unload();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            PerformanceMonitor perfMon = Services.GetService(typeof(PerformanceMonitor)) as PerformanceMonitor;
            String perf = "";
            KeyboardState keyboard =  Keyboard.GetState();
            if (perfMon != null)
            {
                perf = perfMon.ToString();
                //System.Console.WriteLine(perfMon);
            }

            base.Draw(gameTime);
            String fps = "Fps(CPU): " + (float)1.0f / (float)gameTime.ElapsedGameTime.Milliseconds * 1000.0f; // +" MaxFps: " + (float)1.0f / (perfMon.getOverallMilliseconds() + 1) * 1000.0f;
            spriteBatch.Begin();
            if (keyboard.IsKeyDown(Keys.P))
                spriteBatch.DrawString(font2, perf, new Vector2(10, 10), Color.Blue);
            else
                spriteBatch.DrawString(font1, fps , new Vector2(20,20), Color.Red);
            spriteBatch.End();
        }
    }
}
