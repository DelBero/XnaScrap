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
    public class HeightfieldCollider : AbstractCollider
    {
        #region registration
        #region Factory
        /// <summary>
        /// Factory class
        /// </summary>
        public class Constructor : IConstructor
        {
            public AbstractElement getInstance(IDataReader state)
            {
                return new HeightfieldCollider(state);
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
            objectBuilder.registerElement(new Constructor(), sequenceBuilder.CurrentSequence, typeof(HeightfieldCollider), "HeightfieldCollider");
        }
        #endregion

        public HeightfieldCollider(IDataReader state)
            : base(state)
        {
            addInterface(typeof(AbstractCollider));
        }

        #region AbstractCollider
        /// <summary>
        /// Will return true when the point is underneath the Heightfield
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
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
        //public override bool intersectsAxisAligned(PlaneCollider other, out Vector3 normal1, out Vector3 normal2)
        //{
        //    throw new NotImplementedException();
        //}
        #endregion
    }
}
