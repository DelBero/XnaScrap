using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;
using XnaScrapCore.Core.Interfaces.Services;
using XnaScrapCore.Core.Parameter;
using XnaScrapCore.Core;
using XnaScrapCore.Core.Elements;

namespace CollisionSystem.Elements.Collider
{
    /// <summary>
    /// Represents a cylinder. The reference point is a the bottom.
    /// </summary>
    public class CylinderCollider : AbstractCollider
    {
        #region members
        private float m_radius = 1.0f;

        public float Radius
        {
            get { return m_radius; }
            set { m_radius = value; }
        }
        private float m_height = 1.0f;

        public float Height
        {
            get { return m_height; }
            set { m_height = value; }
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
                return new CylinderCollider(state);
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
            sequenceBuilder.addParameter(new FloatParameter("radius","1.0"));
            sequenceBuilder.addParameter(new FloatParameter("height","1.0"));

            objectBuilder.registerElement(new Constructor(), sequenceBuilder.CurrentSequence, typeof(CylinderCollider), "CylinderCollider");
        }
        #endregion

        public CylinderCollider(IDataReader state)
            : base(state)
        {
            addInterface(typeof(AbstractCollider));

            m_radius = state.ReadSingle();
            m_height = state.ReadSingle();
        }

        #region IAbstractCollider
        public override bool contains(Vector3 point)
        {
            Vector3 _p = new Vector3(point.X - Position.X, point.Y - Position.Y, point.Z - Position.Z);
            //Vector3 _p2 = Vector3.Transform(_p, Matrix.CreateFromQuaternion(Orientation));

            if (_p.Y >= 0 && _p.Y <= m_height)
            {
                if (_p.LengthSquared() < (m_radius * m_radius))
                {
                    return true;
                }
            }

            return false;
        }

        public override bool intersects(Ray ray)
        {
            return false;
        }
        #endregion

        #region collision functions
        public override bool intersectsAxisAligned(BoxCollider other, out Vector3 normal1, out Vector3 normal2)
        {
            normal1 = new Vector3();
            normal2 = new Vector3();
            if (other.Bb.Min.Y + other.Position.Y > m_position.Y + m_height)
            {
                return false;
            }
            if (other.Bb.Max.Y + other.Position.Y < m_position.Y)
            {
                return false;
            }

            if (m_position.X + m_radius < other.Position.X - other.Bb.Min.X)
            {
                return false;
            }
            if (m_position.X - m_radius > other.Position.X + other.Bb.Max.X)
            {
                return false;
            }

            if (m_position.Z + m_radius < other.Position.Z - other.Bb.Min.Z)
            {
                return false;
            }
            if (m_position.Z - m_radius > other.Position.Z + other.Bb.Max.Z)
            {
                return false;
            }
            return true;
        }

        public override bool intersectsAxisAligned(SphereCollider other, out Vector3 normal1, out Vector3 normal2)
        {
            normal1 = new Vector3();
            normal2 = new Vector3();
            if (Position.Y + m_height < other.Position.Y - other.Radius)
            {
                return false;
            }
            if (Position.Y > other.Position.Y + other.Radius)
            {
                return false;
            }
            Vector3 _p = new Vector3(m_position.X, 0.0f,m_position.Z);
            Vector3 _p2 = new Vector3(other.Position.X, 0.0f, other.Position.Z);

            if (Vector3.DistanceSquared(_p,_p2) <= Math.Pow( m_radius + other.Radius,2)) 
            {
                return true;
            }

            return false;
        }

        //public override bool intersectsAxisAligned(CylinderCollider other, out Vector3 normal1, out Vector3 normal2)
        //{
        //    normal1 = new Vector3();
        //    normal2 = new Vector3();
        //    // get the higher clyinder
        //    CylinderCollider higher, lower;
        //    if (m_height < other.m_height)
        //    {
        //        higher = other;
        //        lower = this;
        //    }
        //    else
        //    {
        //        higher = this;
        //        lower = other;
        //    }

        //    if (higher.contains(lower.Position) || higher.contains(lower.Position + new Vector3(0.0f, other.m_height, 0.0f)))
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        //public override bool intersectsAxisAligned(CapsuleCollider other, out Vector3 normal1, out Vector3 normal2)
        //{
        //    normal1 = new Vector3();
        //    normal2 = new Vector3();
        //    XnaScrapCore.Core.MemoryStream ms = new XnaScrapCore.Core.MemoryStream();
        //    BinaryWriter bw = new BinaryWriter(ms);
        //    bw.Write(other.Radius);
        //    bw.Write(other.Height);
        //    ms.Position = 0;
        //    CylinderCollider o = new CylinderCollider(new BinaryReader(ms));
        //    if (intersects(o,out normal1, out normal2))
        //    {
        //        return true;
        //    }
        //    // check distance in xz plane
        //    Vector3 _p = new Vector3(m_position.X,0.0f,m_position.Z);
        //    Vector3 _p2 = new Vector3(other.Position.X, 0.0f, other.Position.Z);
        //    if (Vector3.DistanceSquared(_p, _p2) > Math.Pow(m_radius + other.Radius, 2))
        //    {
        //        return false;
        //    }
        //    if (m_position.Y > other.Position.Y + other.Height + other.Radius + other.Radius)
        //    {
        //        return false;
        //    }
        //    if (m_position.Y + m_height < other.Position.Y)
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        //public override bool intersectsAxisAligned(HeightfieldCollider other, out Vector3 normal1, out Vector3 normal2)
        //{
        //    return other.intersects(this, out normal1, out normal2);
        //}

        //public override bool intersectsAxisAligned(MeshCollider box, out Vector3 normal1, out Vector3 normal2)
        //{
        //    throw new NotImplementedException();
        //}

        //public override bool intersectsAxisAligned(PlaneCollider box, out Vector3 normal1, out Vector3 normal2)
        //{
        //    throw new NotImplementedException();
        //}
        #endregion
    }
}
