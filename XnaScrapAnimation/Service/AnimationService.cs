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
using XnaScrapCore.Core;
using XnaScrapCore.Core.Delegates;
using XnaScrapCore.Core.Interfaces;
using System.IO;
using XnaScrapCore.Core.Interfaces.Services;
using XnaScrapAnimation.Elements;
using XnaScrapAnimation.Animations.SkinnedMeshAnimation;
using XnaScrapAnimation.AnimationHandling;
using XnaScrapCore.Core.Systems.Performance;


namespace XnaScrapAnimation.Service
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class AnimationService : Microsoft.Xna.Framework.GameComponent, IComponent, IAnimationService
    {
        #region members
        private static Guid m_componentId = new Guid("F50C83E4-4DB2-400A-A0E2-31AC33446758");
        public Guid Component_ID
        {
            get { return m_componentId; }
        }
        private Dictionary<XnaScrapId, IAnimationFactory> m_animationPlayer = new Dictionary<XnaScrapId, IAnimationFactory>();
        private List<AnimationInstance> m_activeAnimations = new List<AnimationInstance>();
        private List<IAbstractAnimation> m_activePlayers = new List<IAbstractAnimation>();
        
        #region performance
        PerformanceSegment m_mainTimer = null;
        #endregion
        #endregion

        #region MsgIds
        public static XnaScrapId CREATE_ANIMATION_ID;
        #endregion

        public AnimationService(Game game)
            : base(game)
        {
            game.Components.Add(this);
            game.Services.AddService(typeof(AnimationService), this);
            game.Services.AddService(typeof(IAnimationService), this);
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // init elements
            IObjectBuilder objectBuilder = Game.Services.GetService(typeof(IObjectBuilder)) as IObjectBuilder;
            if (objectBuilder != null)
            {
                registerElements(objectBuilder);
            }
            base.Initialize();

            // performance
            PerformanceMonitor perfMon = Game.Services.GetService(typeof(PerformanceMonitor)) as PerformanceMonitor;
            if (perfMon != null)
            {
                m_mainTimer = perfMon.addPerformanceMeter(new XnaScrapId("AnimationService"));
            }
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // performance
            if (m_mainTimer != null)
                m_mainTimer.Watch.Restart();

            foreach (AnimationInstance ai in m_activeAnimations)
            {
                ai.play(gameTime.ElapsedGameTime);
            }
            foreach (IAbstractAnimation ai in m_activePlayers)
            {
                if (ai is AnimationPlayer)
                {
                    AnimationPlayer player = ai as AnimationPlayer;
                    player.update(null,gameTime.ElapsedGameTime);
                }
            }

            // performance
            if (m_mainTimer != null)
                m_mainTimer.Watch.Stop();
            base.Update(gameTime);
        }

        public void addAnimation(XnaScrapId id, IAnimationFactory aa)
        {
            m_animationPlayer.Add(id, aa);
        }

        /// <summary>
        /// Get an animation by name
        /// </summary>
        /// <param name="id">Id of the animation</param>
        /// <returns></returns>
        public IAbstractAnimation getAnimation(XnaScrapId animationId)
        {
            IAnimationFactory animationfactory = null;
            m_animationPlayer.TryGetValue(animationId, out animationfactory);
            IAbstractAnimation player = animationfactory.createAnimation();
            m_activePlayers.Add(player);
            return player;
        }

        public void removeAnimation(XnaScrapId id)
        {
            m_animationPlayer.Remove(id);
        }

        //public void addClip(XnaScrapId id, AbstractAnimationClip clip)
        //{
        //    if (m_clips.Keys.Contains(id))
        //    {
        //        // TODO exception
        //    }
        //    else
        //    {
        //        m_clips.Add(id,clip);
        //    }
        //}

        public IAnimationFactory DefaultSkinnedAnimationFactory(object data)
        {
            return new AnimationPlayer.AnimationPlayerFactory(data as SkinningData);
        }

        /// <summary>
        /// Add an element to the list of active animations
        /// </summary>
        /// <param name="clipId">The animation to use</param>
        /// <param name="aa">The animation player</param>
        /// <param name="startTime">The starting time (offset) within the animation</param>
        /// <param name="eventHandler">Event handler that is called after each frame</param>
        public void animateMe(XnaScrapId clipId,IAbstractAnimation animationPlayer, TimeSpan startTime,bool loop, AnimationChangedEventHandler eventHandler)
        {
            //AnimationInstance newAnimation = new AnimationInstance(animationPlayer as AbstractAnimation, startTime);
            //newAnimation.Start(clipId);
            //newAnimation.Changed += eventHandler;
            //m_activeAnimations.Add(newAnimation);
            if (animationPlayer is AnimationPlayer)
            {
                AnimationPlayer player = animationPlayer as AnimationPlayer;
                player.Start(clipId, loop);
            }
        }

        private void registerElements(IObjectBuilder obj)
        {
            AnimationValueCreator.registerElement(obj, this);
        }

        /// <summary>
        /// Receive a message
        /// </summary>
        /// <param name="msg">The message to read</param>
        public void doHandleMessage(IDataReader msg)
        {
            XnaScrapId msgId = new XnaScrapId(msg);
            if (msgId == CREATE_ANIMATION_ID)
            {
                
            }
        }
    }
}
