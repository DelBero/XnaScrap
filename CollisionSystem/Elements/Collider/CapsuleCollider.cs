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
    /// Represents a Capsule. The reference point is at the bottom of the bottom hemisphere.
    /// </summary>
    public class CapsuleCollider : AbstractCollider
    {
        #region member
        private float m_height;

        public float Height
        {
            get { return m_height; }
            set { m_height = value; }
        }
        private float m_radius;

        public float Radius
        {
            get { return m_radius; }
            set { m_radius = value; }
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
                return new CapsuleCollider(state);
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
            sequenceBuilder.addParameter(new FloatParameter("radius", "1.0"));
            sequenceBuilder.addParameter(new FloatParameter("height", "1.0"));

            objectBuilder.registerElement(new Constructor(), sequenceBuilder.CurrentSequence, typeof(CapsuleCollider), "CapsuleCollider");
        }
        #endregion

        public CapsuleCollider(IDataReader state)
            : base(state)
        {
            addInterface(typeof(AbstractCollider));
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
        #endregion

        #region collision functions
        public override bool intersectsAxisAligned(BoxCollider other, out Vector3 normal1, out Vector3 normal2)
        {
            throw new NotImplementedException();
        }
        public override bool intersectsAxisAligned(SphereCollider other, out Vector3 normal1, out Vector3 normal2)
        {
            throw new NotImplementedException();
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
        //public override bool intersectsAxisAligned(PlaneCollider box, out Vector3 normal1, out Vector3 normal2)
        //{
        //    throw new NotImplementedException();
        //}
        #endregion
    }
}
