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
using XnaScrapCore.Core.Interfaces.Elements;
using XnaScrapCore.Core.Delegates;

namespace CollisionSystem.Elements.Collider
{
    public class BoxCollider : AbstractCollider
    {
        #region member
        private Vector3 m_scale = new Vector3(1.0f, 1.0f, 1.0f);

        public Vector3 Scale
        {
            get { return m_scale; }
        }

        public Microsoft.Xna.Framework.BoundingBox Bb
        {
            get { return m_boundingBox; }
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
                return new BoxCollider(state);
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
            sequenceBuilder.addParameter(new FloatParameter("scale","1,1,1"));

            objectBuilder.registerElement(new Constructor(), sequenceBuilder.CurrentSequence, typeof(BoxCollider), "BoxCollider", null);
        }
        #endregion

        public BoxCollider(IDataReader state)
            : base(state)
        {
            addInterface(typeof(AbstractCollider));

            m_scale.X = state.ReadSingle();
            m_scale.Y = state.ReadSingle();
            m_scale.Z = state.ReadSingle();
            m_boundingBox = new Microsoft.Xna.Framework.BoundingBox(new Vector3(-0.5f * m_scale.X, -0.5f * m_scale.Y, -0.5f * m_scale.Z), new Vector3(0.5f * m_scale.X, 0.5f * m_scale.Y, 0.5f * m_scale.Z));
        }

        public override void doSerialize(IDataWriter state)
        {
            base.doSerialize(state);
            state.Write(m_scale.X);
            state.Write(m_scale.Y);
            state.Write(m_scale.Z);
        }

        public Vector3 getCenter()
        {
            return new Vector3((Bb.Min.X + Bb.Max.X) * 0.5f, (Bb.Min.Y + Bb.Max.Y) * 0.5f, (Bb.Min.Z + Bb.Max.Z) * 0.5f);
        }

        #region IAbstractCollider
        public override bool contains(Vector3 point)
        {
            ContainmentType contain = m_boundingBox.Contains(point);
            return contain == ContainmentType.Contains;
        }

        public override bool intersects(Ray ray)
        {
            float? result;
            m_boundingBox.Intersects(ref ray, out result);
            return result.HasValue;
        }

        public override void update(Vector3 position)
        {
            base.update(position);
            float x = (BoundingBox.Max.X - BoundingBox.Min.X) * 0.5f;
            float y = (BoundingBox.Max.Y - BoundingBox.Min.Y) * 0.5f;
            float z = (BoundingBox.Max.Z - BoundingBox.Min.Z) * 0.5f;
            m_boundingBox.Min.X = Position.X - x;
            m_boundingBox.Min.Y = Position.Y - y;
            m_boundingBox.Min.Z = Position.Z - z;
            m_boundingBox.Max.X = Position.X + x;
            m_boundingBox.Max.Y = Position.Y + y;
            m_boundingBox.Max.Z = Position.Z + z;
        }
        #endregion

        #region collision functions
        public override bool intersectsAxisAligned(BoxCollider other, out Vector3 normal1, out Vector3 normal2)
        {
            normal1 = new Vector3();
            normal2 = new Vector3();
            return m_boundingBox.Intersects(other.m_boundingBox);
            //return other.intersectsAxisAligned(this, out normal2, out normal1);
        }
        public override bool intersectsAxisAligned(SphereCollider other, out Vector3 normal1, out Vector3 normal2)
        {
            return other.intersectsAxisAligned(this, out normal2, out normal1);
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
        //    throw new NotImplementedException();
        //}
        #endregion
    }
}
