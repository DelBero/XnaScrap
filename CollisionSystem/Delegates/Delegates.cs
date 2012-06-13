using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CollisionSystem.Elements;
using Microsoft.Xna.Framework;

namespace CollisionSystem.Delegates
{
    public delegate void CollisionEventHandler(object sender, CollisionEventArgs e);

    public delegate void CollisionEndedEventHandler(object sender, CollisionEventArgs e);

    public class CollisionEventArgs : EventArgs
    {
        public static CollisionEventArgs Empty;

        private CollisionDetectionElement m_otherCollider;

        public CollisionDetectionElement OtherCollider
        {
            get { return m_otherCollider; }
            set { m_otherCollider = value; }
        }

        private bool hasCollisionPoint = false;

        public bool HasCollisionPoint
        {
            get { return hasCollisionPoint; }
            set { hasCollisionPoint = value; }
        }

        private Vector3 m_collisionPosition;

        public Vector3 CollisionPosition
        {
            get { return m_collisionPosition; }
            set { m_collisionPosition = value; }
        }

        private bool hasCollisionNormal = false;

        public bool HasCollisionNormal
        {
            get { return hasCollisionNormal; }
            set { hasCollisionNormal = value; }
        }

        private Vector3 m_collisionNormal;

        public Vector3 CollisionNormal
        {
            get { return m_collisionNormal; }
            set { m_collisionNormal = value; }
        }

        public CollisionEventArgs(CollisionDetectionElement collider)
        {
            m_otherCollider = collider;
        }
    }
}
