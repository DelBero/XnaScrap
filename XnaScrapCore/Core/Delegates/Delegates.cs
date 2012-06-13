using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XnaScrapCore.Core.Delegates
{
    #region Pos3D
    public delegate void Position3DChangedEventHandler(object sender, Position3DChangedEventArgs e);

    public class Position3DChangedEventArgs : EventArgs
    {
        private Vector3 m_position;

        public Vector3 Position
        {
            get { return m_position; }
        }

        public Position3DChangedEventArgs(Vector3 position)
        {
            m_position = position;
        }
    }
    #endregion

    #region Pos2D
    public delegate void Position2DChangedEventHandler(object sender, Position2DChangedEventArgs e);

    public class Position2DChangedEventArgs : EventArgs
    {
        private Vector2 m_position;

        public Vector2 Position
        {
            get { return m_position; }
        }

        public Position2DChangedEventArgs(Vector2 position)
        {
            m_position = position;
        }
    }
    #endregion

    #region Scale3D
    public delegate void Scale3DChangedEventHandler(object sender, Scale3DChangedEventArgs e);

    public class Scale3DChangedEventArgs : EventArgs
    {
        private Vector3 m_scale;

        public Vector3 Scale
        {
            get { return m_scale; }
        }

        public Scale3DChangedEventArgs(Vector3 scale)
        {
            m_scale = scale;
        }
    }
    #endregion

    #region Scale2D
    public delegate void Scale2DChangedEventHandler(object sender, Scale2DChangedEventArgs e);

    public class Scale2DChangedEventArgs : EventArgs
    {
        private Vector2 m_scale;

        public Vector2 Scale
        {
            get { return m_scale; }
        }

        public Scale2DChangedEventArgs(Vector2 scale)
        {
            m_scale = scale;
        }
    }
    
    #endregion

    #region orientation3d
    public delegate void OrientationChangedEventHandler(object sender, Orientation3DChangedEventArgs e);

    public class Orientation3DChangedEventArgs : EventArgs
    {
        private Quaternion m_orientation;

        public Quaternion Orientation
        {
            get { return m_orientation; }
        }

        public Orientation3DChangedEventArgs(Quaternion orientation)
        {
            m_orientation = orientation;
        }
    }

    #endregion

    public class AnimationEventArgs : EventArgs
    {
        public static AnimationEventArgs EmptyEvent;

        private TimeSpan m_time;
        private XnaScrapId m_animationId;

        public XnaScrapId AnimationId
        {
            get { return m_animationId; }
        }

        public TimeSpan Time
        {
            get { return m_time; }
        }

        public AnimationEventArgs(TimeSpan time, XnaScrapId animationId) 
        {
            m_time = time;
            m_animationId = animationId;
        }
    }

    public class AnimationValueEventArgs : AnimationEventArgs
    {
        private float m_value;

        public float Value
        {
          get { return m_value; }
        }

        public AnimationValueEventArgs(TimeSpan time, XnaScrapId animationId, float value) : base(time, animationId)
        {
            m_value = value;
        }
    }

    public class AnimationSkinnedEventArgs : AnimationEventArgs
    {
        private object m_source;

        public object Source
        {
          get { return m_source; }
        }

        public AnimationSkinnedEventArgs(TimeSpan time, XnaScrapId animationId, object source)
            : base(time, animationId)
        {
            m_source = source;
        }
    }

    public delegate void AnimationChangedEventHandler(object sender, AnimationEventArgs e);
}
