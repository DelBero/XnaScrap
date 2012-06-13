using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;
using XnaScrapCore.Core.Parameter;
using XnaScrapCore.Core.Interfaces.Services;
using XnaScrapCore.Core;
using XnaScrapCore.Core.Elements;

namespace CollisionSystem.Elements.Collider
{
    public class SphereCollider : AbstractCollider
    {
        #region members
        private Microsoft.Xna.Framework.BoundingSphere m_sphere;

        public float Radius
        {
            get { return m_sphere.Radius; }
            set { m_sphere.Radius = value; }
        }
        #endregion

        #region registration
        #region Factory
        /// <summary>
        /// Factory class
        /// </summary>
        public class Constructor : IConstructor
        {
            public AbstractElement getInstance(IDataReader state)
            {
                return new SphereCollider(state);
            }
        }
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectBuilder"></param>
        public static void registerElement(IObjectBuilder objectBuilder)
        {
            ParameterSequenceBuilder sequenceBuilder = new ParameterSequenceBuilder();
            sequenceBuilder.createSequence();
            basicRegistration(sequenceBuilder);
            sequenceBuilder.addParameter(new FloatParameter("radius", "0.5"));

            objectBuilder.registerElement(new Constructor(), sequenceBuilder.CurrentSequence, typeof(SphereCollider), "SphereCollider", null);
        }
        #endregion

        public SphereCollider(IDataReader state)
            : base(state)
        {
            addInterface(typeof(AbstractCollider));

            m_sphere.Radius = state.ReadSingle();
            m_boundingBox = BoundingBox.CreateFromSphere(m_sphere);
        }

        #region IAbstractCollider
        public override bool contains(Vector3 point)
        {
            return false;
        }

        public override bool intersects(Ray ray)
        {
            return false;
        }

        public override void update(Vector3 position)
        {
            base.update(position);
            m_sphere.Center = position;
        }
        #endregion

        #region collision functions
        public override bool intersectsAxisAligned(BoxCollider other, out Vector3 normal1, out Vector3 normal2)
        {
            Vector3 p = closestPointOnAabb(m_position, other);
            normal1 = new Vector3();
            normal2 = new Vector3();

            Vector3 v = p - m_position;
            if (v.LengthSquared() <= m_sphere.Radius * m_sphere.Radius)
            {
                normal1 = m_position - p;
                if (normal1 != Vector3.Zero)
                {
                    normal1.Normalize();
                }
                normal2 = Vector3.Negate(normal1);
            }
            return m_sphere.Intersects(other.BoundingBox);
        }
        public override bool intersectsAxisAligned(SphereCollider other, out Vector3 normal1, out Vector3 normal2)
        {
            normal1 = new Vector3();
            normal2 = new Vector3();
            return m_sphere.Intersects(other.m_sphere);
        }
        public override bool intersectsAxisAligned(OrientedBoxCollider other, out Vector3 normal1, out Vector3 normal2)
        {
            normal1 = normal2 = Vector3.Up;
            return false;
        }
        //public override bool intersectsAxisAligned(CapsuleCollider other, out Vector3 normal1, out Vector3 normal2)
        //{
        //    throw new NotImplementedException();
        //}
        //public override bool intersectsAxisAligned(CylinderCollider other, out Vector3 normal1, out Vector3 normal2)
        //{
        //    throw new NotImplementedException();
        //}
        //public override bool intersectsAxisAligned(HeightfieldCollider other, out Vector3 normal1, out Vector3 normal2)
        //{
        //    throw new NotImplementedException();
        //}
        //public override bool intersectsAxisAligned(MeshCollider other, out Vector3 normal1, out Vector3 normal2)
        //{
        //    throw new NotImplementedException();
        //}
        //public override bool intersectsAxisAligned(PlaneCollider other, out Vector3 normal1, out Vector3 normal2)
        //{
        //    float ret = Vector3.Dot(other.Normal, m_position);
        //    if (ret - other.D <= 0)
        //    {
        //        normal1 = other.Normal;
        //        normal2 = Vector3.Negate(normal1);
        //        return true;
        //    }
        //    normal1 = new Vector3();
        //    normal2 = new Vector3();
        //    return false;
        //}

        private Vector3 closestPointOnAabb(Vector3 point, BoxCollider other)
        {
            Vector3 min = other.BoundingBox.Min;
            Vector3 max = other.BoundingBox.Max;

            Vector3 q = Vector3.Zero;
            float v = 0;

            v = point.X;
            v = Math.Max(v, min.X);
            v = Math.Min(v, max.X);
            q.X = v;

            v = point.Y;
            v = Math.Max(v, min.Y);
            v = Math.Min(v, max.Y);
            q.Y = v;

            v = point.Z;
            v = Math.Max(v, min.Z);
            v = Math.Min(v, max.Z);
            q.Z = v;

            return q;

        }
        #endregion
    }
}
