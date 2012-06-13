using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaScrapCore.Core.Elements;
using XnaScrapCore.Core;
using System.IO;
using XnaScrapCore.Core.Interfaces.Services;
using XnaScrapCore.Core.Parameter;
using Microsoft.Xna.Framework;

namespace CollisionSystem.Elements.Collider
{
    /// <summary>
    /// Represents a cylinder. The reference point is a the bottom.
    /// </summary>
    public class PlaneCollider : AbstractCollider
    {
        #region members
        private Vector3 m_normal = new Vector3(0.0f, 1.0f, 0.0f);

        public Vector3 Normal
        {
            get { return m_normal; }
        }

        private float m_d = 0.0f;

        public float D
        {
            get { return m_d; }
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
                return new PlaneCollider(state);
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
            sequenceBuilder.addParameter(new FloatParameter("normal", "0.0,1.0,0.0"));
            sequenceBuilder.addParameter(new FloatParameter("d", "0.0"));
            sequenceBuilder.addParameter(new FloatParameter("bb_min", "-0.5,-0.5,-0.5"));
            sequenceBuilder.addParameter(new FloatParameter("bb_max", "0.5,0.5,0.5"));

            objectBuilder.registerElement(new Constructor(), sequenceBuilder.CurrentSequence, typeof(PlaneCollider), "PlaneCollider");
        }
        #endregion

        public PlaneCollider(IDataReader state)
            : base(state)
        {
            addInterface(typeof(AbstractCollider));

            m_normal.X = state.ReadSingle();
            m_normal.Y = state.ReadSingle();
            m_normal.Z = state.ReadSingle();
            m_d = state.ReadSingle();

            float min_x = state.ReadSingle();
            float min_y = state.ReadSingle();
            float min_z = state.ReadSingle();
            float max_x = state.ReadSingle();
            float max_y = state.ReadSingle();
            float max_z = state.ReadSingle();

            m_boundingBox = new Microsoft.Xna.Framework.BoundingBox(new Vector3(min_x, min_y, min_z), new Vector3(max_x, max_y, max_z));
        }

        private float getDistance(Vector3 point)
        {
            return Vector3.Dot(m_normal, point) + m_d;
        }

        #region IAbstractCollider
        /// <summary>
        /// Check a point against the plane.
        /// </summary>
        /// <param name="point">Point to check</param>
        /// <returns>True if the point is on or below the plane.</returns>
        public override bool contains(Vector3 point)
        {
            return getDistance(point) <= 0;
        }
        
        public override bool intersects(Ray ray)
        {
            return false;
        }
        #endregion

        #region collision functions
        public override bool intersectsAxisAligned(BoxCollider other, out Vector3 normal1, out Vector3 normal2)
        {
            normal1 = m_normal;
            normal2 = Vector3.Negate(normal1);
            Vector3 center = other.getCenter();
            Vector3 extend;
            Vector3 min = other.Bb.Min;
            Vector3 max = other.Bb.Max;
            Vector3.Subtract(ref max, ref min, out extend);
            float distance = getDistance(center);
            float maxDist = Vector3.Dot(m_normal, extend);
            if (distance > maxDist)
            {
                return false;
            }
            return true;
        }


        public override bool intersectsAxisAligned(SphereCollider other, out Vector3 normal1, out Vector3 normal2)
        {
            float ret = Vector3.Dot(Normal, other.Position);
            if (ret - D <= 0)
            {
                normal1 = Normal;
                normal2 = Vector3.Negate(normal1);
                return true;
            }
            normal1 = new Vector3();
            normal2 = new Vector3();
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
        //    throw new NotImplementedException();
        //}
        #endregion
    }
}
