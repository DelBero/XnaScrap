using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CollisionSystem.Elements;
using Microsoft.Xna.Framework;

namespace CollisionSystem.Service
{
    public struct Collision
    {
        private CollisionDetectionElement m_1;

        public CollisionDetectionElement Collider1
        {
            get { return m_1; }
            set { m_1 = value; }
        }
        private CollisionDetectionElement m_2;

        public CollisionDetectionElement Collider2
        {
            get { return m_2; }
            set { m_2 = value; }
        }

        private Vector3 m_normal1;
        private Vector3 m_normal2;

        

        /// <summary>
        /// The normal of Collider1 at the intersection point.
        /// </summary>
        public Vector3 Normal1
        {
            get { return m_normal1; }
            set { m_normal1 = value; }
        }

        /// <summary>
        /// The normal of Collider2 at the intersection point.
        /// </summary>
        public Vector3 Normal2
        {
            get { return m_normal2; }
            set { m_normal2 = value; }
        }

        public Collision(CollisionDetectionElement collider1, CollisionDetectionElement collider2, Vector3 normal1, Vector3 normal2)
        {
            m_1 = collider1;
            m_2 = collider2;
            m_normal1 = normal1;
            m_normal2 = normal2;
        }
    }
}
