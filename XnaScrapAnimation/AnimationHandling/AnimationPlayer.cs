#region File Description
//-----------------------------------------------------------------------------
// AnimationPlayer.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using XnaScrapAnimation.Animations.SkinnedMeshAnimation;
using XnaScrapAnimation.Service;
using XnaScrapCore.Core;
using XnaScrapCore.Core.Delegates;
using XnaScrapCore.Core.Interfaces.Services;
#endregion

namespace XnaScrapAnimation.AnimationHandling
{
    /// <summary>
    /// The animation player is in charge of decoding bone position
    /// matrices from an animation clip.
    /// </summary>
    public class AnimationPlayer : AbstractAnimation
    {
        public class AnimationPlayerFactory : IAnimationFactory
        {
            #region member
            SkinningData m_data = null;
            private Dictionary<XnaScrapId, AbstractAnimationClip> m_clips = new Dictionary<XnaScrapId, AbstractAnimationClip>();
            #endregion
            public AnimationPlayerFactory(SkinningData data)
            {
                m_data = data;
                foreach (KeyValuePair<String, AnimationClip> keyValue in m_data.AnimationClips)
                {
                    XnaScrapId animId = new XnaScrapId(keyValue.Key);
                    keyValue.Value.ClipId = animId;
                    m_clips.Add(animId, keyValue.Value);
                }
            }
            #region IAnimationFactory Members

            public XnaScrapCore.Core.Interfaces.Services.IAbstractAnimation createAnimation()
            {
                return new AnimationPlayer(m_data, m_clips);
            }

            #endregion
        }
        #region helper
        class _CurrentClip
        {
            public AnimationClip clipValue;
            public TimeSpan timeValue;
            public int keyframe;
            public bool loop;
        };
        #endregion
        #region Fields
        // Information about the currently playing animation clip.
        List<_CurrentClip> m_currentClips = new List<_CurrentClip>();

        // Current animation transform matrices.
        Matrix[] boneTransforms;
        Matrix[] worldTransforms;
        Matrix[] skinTransforms;


        // Backlink to the bind pose and skeleton hierarchy data.
        SkinningData skinningDataValue;

        private Matrix m_root = Matrix.Identity;
        public Matrix Root
        {
            get
            {
                return m_root;
            }
            set
            {
                m_root = value;
            }
        }
        #endregion


        /// <summary>
        /// Constructs a new animation player.
        /// </summary>
        public AnimationPlayer(SkinningData skinningData) : base(new TimeSpan(), new XnaScrapId())
        {
            if (skinningData == null)
                throw new ArgumentNullException("skinningData");

            skinningDataValue = skinningData;

            boneTransforms = new Matrix[skinningData.BindPose.Count];
            worldTransforms = new Matrix[skinningData.BindPose.Count];
            skinTransforms = new Matrix[skinningData.BindPose.Count];
        }

        public AnimationPlayer(SkinningData skinningData, Dictionary<XnaScrapId, AbstractAnimationClip> clips)
            : base(new TimeSpan(), new XnaScrapId(), clips)
        {
            if (skinningData == null)
                throw new ArgumentNullException("skinningData");

            skinningDataValue = skinningData;

            boneTransforms = new Matrix[skinningData.BindPose.Count];
            worldTransforms = new Matrix[skinningData.BindPose.Count];
            skinTransforms = new Matrix[skinningData.BindPose.Count];
        }


        public override void Start(XnaScrapId animationId, bool loop)
        {
            if (skinningDataValue != null)
            {
                AbstractAnimationClip clip;
                if (m_clips.TryGetValue(animationId, out clip))
                {
                    foreach (_CurrentClip c in m_currentClips)
                    {
                        if (c.clipValue.ClipId == animationId)
                            return;
                    }
                    StartClip(clip as AnimationClip, loop);
                }
            }
        }

        public override void Start(AbstractAnimationClip clip, bool loop)
        {
            if (clip is AnimationClip)
                StartClip(clip as AnimationClip, loop);
        }

        /// <summary>
        /// Starts decoding the specified animation clip.
        /// </summary>
        public void StartClip(AnimationClip clip, bool loop)
        {
            if (clip == null)
                throw new ArgumentNullException("clip");

            _CurrentClip newClip = new _CurrentClip();
            newClip.clipValue = clip;
            newClip.timeValue = TimeSpan.Zero;
            newClip.keyframe = 0;
            newClip.loop = loop;

            m_currentClips.Add(newClip);

            Update(TimeSpan.Zero, true,Root,true);

            //currentClipValue = clip;
            //currentTimeValue = TimeSpan.Zero;
            //currentKeyframe = 0;

            // Initialize bone transforms to the bind pose.
            skinningDataValue.BindPose.CopyTo(boneTransforms, 0);
        }

        public override void update(AnimationInstance instance, TimeSpan time)
        {
            Update(time,true,Root, true);
            if (instance != null)
                instance.OnChanged(new AnimationSkinnedEventArgs(Time, AnimationId, this));
        }

        /// <summary>
        /// Advances the current animation position.
        /// </summary>
        public void Update(TimeSpan time, bool relativeToCurrentTime,
                           Matrix rootTransform, bool all)
        {
            UpdateBoneTransforms(time, relativeToCurrentTime);
            UpdateWorldTransforms(/*rootTransform*/ Matrix.Identity, all);
            UpdateSkinTransforms(all);
        }


        /// <summary>
        /// Helper used by the Update method to refresh the BoneTransforms data.
        /// </summary>
        public void UpdateBoneTransforms(TimeSpan time, bool relativeToCurrentTime)
        {
            for (int i = 0; i < m_currentClips.Count; ++i)
            {
                TimeSpan animationTime = time;
                _CurrentClip curClip = m_currentClips[i];
                AnimationClip currentClipValue = curClip.clipValue;
                TimeSpan currentTimeValue = curClip.timeValue;
                int currentKeyframe = curClip.keyframe;

                if (currentClipValue == null)
                    throw new InvalidOperationException(
                                "AnimationPlayer.Update was called before StartClip");

                // Update the animation position.
                if (relativeToCurrentTime)
                {
                    animationTime += currentTimeValue;

                    // If we reached the end, loop back to the start if loop is set.
                    if (animationTime > currentClipValue.Duration)
                    {
                        if (curClip.loop)
                        {
                            while (animationTime >= currentClipValue.Duration)
                                animationTime -= currentClipValue.Duration;
                        }
                        else
                        {
                            finishClip(i);
                            --i;
                            continue;
                        }
                    }
                }

                if ((animationTime < TimeSpan.Zero) || (animationTime >= currentClipValue.Duration))
                    throw new ArgumentOutOfRangeException("time");

                // If the position moved backwards, reset the keyframe index.
                if (animationTime < currentTimeValue)
                {
                    currentKeyframe = 0;
                    skinningDataValue.BindPose.CopyTo(boneTransforms, 0);
                }

                currentTimeValue = animationTime;
                m_currentClips[i].timeValue = animationTime;

                // Read keyframe matrices.
                IList<Keyframe> keyframes = currentClipValue.Keyframes;

                while (currentKeyframe < keyframes.Count)
                {
                    Keyframe keyframe = keyframes[currentKeyframe];

                    // Stop when we've read up to the current time position.
                    if (keyframe.Time > currentTimeValue)
                        break;

                    // Use this keyframe.
                    boneTransforms[keyframe.Bone] = keyframe.Transform;

                    ++currentKeyframe;
                    m_currentClips[i].keyframe = currentKeyframe;
                }
            }
        }


        /// <summary>
        /// Helper used by the Update method to refresh the WorldTransforms data.
        /// </summary>
        public void UpdateWorldTransforms(Matrix rootTransform, bool all)
        {
            // Root bone.
            worldTransforms[0] = boneTransforms[0] * rootTransform;

            if (all)
            {
                // Child bones.
                for (int bone = 1; bone < worldTransforms.Length; bone++)
                {
                    int parentBone = skinningDataValue.SkeletonHierarchy[bone];

                    worldTransforms[bone] = boneTransforms[bone] *
                                                 worldTransforms[parentBone];
                }
            }
            else
            {
                foreach (_CurrentClip clip in m_currentClips)
                {
                    foreach (int boneindex in clip.clipValue.Affects)
                    {
                        int parentBone = skinningDataValue.SkeletonHierarchy[boneindex];

                        if (parentBone < 0)
                        {
                            worldTransforms[boneindex] = boneTransforms[boneindex];
                        }
                        else
                        {
                            worldTransforms[boneindex] = boneTransforms[boneindex] *
                                                     worldTransforms[parentBone];
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Helper used by the Update method to refresh the SkinTransforms data.
        /// </summary>
        public void UpdateSkinTransforms(bool all)
        {
            if (all)
            {
                for (int bone = 0; bone < skinTransforms.Length; bone++)
                {
                    skinTransforms[bone] = skinningDataValue.InverseBindPose[bone] *
                                                worldTransforms[bone];
                }
            }
            else
            {
                foreach (_CurrentClip clip in m_currentClips)
                {
                    foreach (int boneindex in clip.clipValue.Affects)
                    {
                        int parentBone = skinningDataValue.SkeletonHierarchy[boneindex];

                        skinTransforms[boneindex] = skinningDataValue.InverseBindPose[boneindex] *
                                                 worldTransforms[boneindex];
                    }
                }
            }
        }


        /// <summary>
        /// Gets the current bone transform matrices, relative to their parent bones.
        /// </summary>
        public Matrix[] GetBoneTransforms()
        {
            return boneTransforms;
        }


        /// <summary>
        /// Gets the current bone transform matrices, in absolute format.
        /// </summary>
        public Matrix[] GetWorldTransforms()
        {
            return worldTransforms;
        }


        /// <summary>
        /// Gets the current bone transform matrices,
        /// relative to the skinning bind pose.
        /// </summary>
        public Matrix[] GetSkinTransforms()
        {
            return skinTransforms;
        }

        private void finishClip(int index)
        {
            m_currentClips.RemoveAt(index);
        }

        ///// <summary>
        ///// Gets the clip currently being decoded.
        ///// </summary>
        //public AnimationClip CurrentClip
        //{
        //    get { return currentClipValue; }
        //}


        ///// <summary>
        ///// Gets the current play position.
        ///// </summary>
        //public TimeSpan CurrentTime
        //{
        //    get { return currentTimeValue; }
        //}
    }
}
