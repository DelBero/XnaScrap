#region File Description
//-----------------------------------------------------------------------------
// AnimationClip.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using XnaScrapAnimation.Service;
using XnaScrapCore.Core;
#endregion

namespace XnaScrapAnimation.AnimationHandling
{
    /// <summary>
    /// An animation clip is the runtime equivalent of the
    /// Microsoft.Xna.Framework.Content.Pipeline.Graphics.AnimationContent type.
    /// It holds all the keyframes needed to describe a single animation.
    /// </summary>
    public class AnimationClip : AbstractAnimationClip
    {
        /// <summary>
        /// Constructs a new animation clip object.
        /// </summary>
        public AnimationClip(TimeSpan duration, List<Keyframe> keyframes, int[] affectedBones, XnaScrapId clipId) : base(duration,clipId)
        {
            Duration = duration;
            Keyframes = keyframes;
            Affects = affectedBones;
        }

        /// <summary>
        /// Constructor used by the content pipeline
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="keyframes"></param>
        /// <param name="affectedBones"></param>
        public AnimationClip(TimeSpan duration, List<Keyframe> keyframes, int[] affectedBones)
            : base(duration, XnaScrapId.INVALID_ID)
        {
            Duration = duration;
            Keyframes = keyframes;
            Affects = affectedBones;
        }


        /// <summary>
        /// Private constructor for use by the XNB deserializer.
        /// </summary>
        private AnimationClip() : base(new TimeSpan(), XnaScrapId.INVALID_ID)
        {
        }


        /// <summary>
        /// Gets the total length of the animation.
        /// </summary>
        [ContentSerializer]
        public TimeSpan Duration { get; private set; }


        /// <summary>
        /// Gets a combined list containing all the keyframes for all bones,
        /// sorted by time.
        /// </summary>
        [ContentSerializer]
        public List<Keyframe> Keyframes { get; private set; }

        /// <summary>
        /// Gets a combined list containing all the keyframes for all bones,
        /// sorted by time.
        /// </summary>
        [ContentSerializer]
        public int[] Affects
        {
            get;
            private set;
        }
    }
}
