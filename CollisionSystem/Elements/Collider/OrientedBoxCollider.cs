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
    public class OrientedBoxCollider : AbstractCollider
    {
        #region member
        public Vector3 Center;
        public Vector3 HalfExtent;
        public Quaternion Orientation;
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
            sequenceBuilder.addParameter(new FloatParameter("center","0,0,0"));
            sequenceBuilder.addParameter(new FloatParameter("halfExtend", "0,0,0"));
            sequenceBuilder.addParameter(new FloatParameter("orientation", "0,0,0,1"));

            objectBuilder.registerElement(new Constructor(), sequenceBuilder.CurrentSequence, typeof(OrientedBoxCollider), "OrientedBoxCollider", null);
        }
        #endregion

        public OrientedBoxCollider(IDataReader state)
            : base(state)
        {
            addInterface(typeof(AbstractCollider));
            Center.X = state.ReadSingle();
            Center.Y = state.ReadSingle();
            Center.Z = state.ReadSingle();

            HalfExtent.X = state.ReadSingle();
            HalfExtent.Y = state.ReadSingle();
            HalfExtent.Z = state.ReadSingle();

            Orientation.X = state.ReadSingle();
            Orientation.Y = state.ReadSingle();
            Orientation.Z = state.ReadSingle();
            Orientation.W = state.ReadSingle();
        }

        public override void doSerialize(IDataWriter state)
        {
            base.doSerialize(state);
            state.Write(Center.X);
            state.Write(Center.Y);
            state.Write(Center.Z);

            state.Write(HalfExtent.X);
            state.Write(HalfExtent.Y);
            state.Write(HalfExtent.Z);

            state.Write(Orientation.X);
            state.Write(Orientation.Y);
            state.Write(Orientation.Z);
            state.Write(Orientation.W);
        }

        public Vector3 getCenter()
        {
            return Center;
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
            m_boundingBox.Min.X = position.X - x;
            m_boundingBox.Min.Y = position.Y - y;
            m_boundingBox.Min.Z = position.Z - z;
            m_boundingBox.Max.X = position.X + x;
            m_boundingBox.Max.Y = position.Y + y;
            m_boundingBox.Max.Z = position.Z + z;
        }
        #endregion

        #region collision functions
        public override bool intersectsAxisAligned(BoxCollider other, out Vector3 normal1, out Vector3 normal2)
        {
            normal1 = new Vector3();
            normal2 = new Vector3();
            return false;
            //return other.intersectsAxisAligned(this, out normal2, out normal1);
        }
        public override bool intersectsAxisAligned(SphereCollider other, out Vector3 normal1, out Vector3 normal2)
        {
            normal1 = normal2 = Vector3.Up;
            return false;
        }
        public override bool intersectsAxisAligned(OrientedBoxCollider other, out Vector3 normal1, out Vector3 normal2)
        {
            normal1 = normal2 = Vector3.Up;
            return Contains(ref other) != ContainmentType.Disjoint;
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

        #region helper
        /// <summary>
        /// Determine whether this box contains, intersects, or is disjoint from
        /// the given other box.
        /// </summary>
        public ContainmentType Contains(ref OrientedBoxCollider other)
        {
            // Build the 3x3 rotation matrix that defines the orientation of 'other' relative to this box
            Quaternion invOrient;
            Quaternion.Conjugate(ref Orientation, out invOrient);
            Quaternion relOrient;
            Quaternion.Multiply(ref invOrient, ref other.Orientation, out relOrient);

            Matrix relTransform = Matrix.CreateFromQuaternion(relOrient);
            relTransform.Translation = Vector3.Transform(other.Center - Center, invOrient);

            return ContainsRelativeBox(ref HalfExtent, ref other.HalfExtent, ref relTransform);
        }

        /// <summary>
        /// Determine whether the box described by half-extents hA, axis-aligned and centered at the origin, contains
        /// the box described by half-extents hB, whose position and orientation are given by the transform matrix mB.
        /// The matrix is assumed to contain only rigid motion; if it contains scaling or perpsective the result of
        /// this method will be incorrect.
        /// </summary>
        /// <param name="hA">Half-extents of first box</param>
        /// <param name="hB">Half-extents of second box</param>
        /// <param name="mB">Position and orientation of second box relative to first box</param>
        /// <returns>ContainmentType enum indicating whether the boxes are disjoin, intersecting, or
        /// whether box A contains box B.</returns>
        public static ContainmentType ContainsRelativeBox(ref Vector3 hA, ref Vector3 hB, ref Matrix mB)
        {
            Vector3 mB_T = mB.Translation;
            Vector3 mB_TA = new Vector3(Math.Abs(mB_T.X), Math.Abs(mB_T.Y), Math.Abs(mB_T.Z));

            // Transform the extents of B
            // TODO: check which coords Right/Up/Back refer to and access them directly. This looks dumb.
            Vector3 bX = mB.Right;      // x-axis of box B
            Vector3 bY = mB.Up;         // y-axis of box B
            Vector3 bZ = mB.Backward;   // z-axis of box B
            Vector3 hx_B = bX * hB.X;   // x extent of box B
            Vector3 hy_B = bY * hB.Y;   // y extent of box B
            Vector3 hz_B = bZ * hB.Z;   // z extent of box B

            // Check for containment first.
            float projx_B = Math.Abs(hx_B.X) + Math.Abs(hy_B.X) + Math.Abs(hz_B.X);
            float projy_B = Math.Abs(hx_B.Y) + Math.Abs(hy_B.Y) + Math.Abs(hz_B.Y);
            float projz_B = Math.Abs(hx_B.Z) + Math.Abs(hy_B.Z) + Math.Abs(hz_B.Z);
            if (mB_TA.X + projx_B <= hA.X && mB_TA.Y + projy_B <= hA.Y && mB_TA.Z + projz_B <= hA.Z)
                return ContainmentType.Contains;

            // Check for separation along the faces of the other box,
            // by projecting each local axis onto the other boxes' axes
            // http://www.cs.unc.edu/~geom/theses/gottschalk/main.pdf
            //
            // The general test form, given a choice of separating axis, is:
            //      sizeA = abs(dot(A.e1,axis)) + abs(dot(A.e2,axis)) + abs(dot(A.e3,axis))
            //      sizeB = abs(dot(B.e1,axis)) + abs(dot(B.e2,axis)) + abs(dot(B.e3,axis))
            //      distance = abs(dot(B.center - A.center),axis))
            //      if distance >= sizeA+sizeB, the boxes are disjoint
            //
            // We need to do this test on 15 axes:
            //      x, y, z axis of box A
            //      x, y, z axis of box B
            //      (v1 cross v2) for each v1 in A's axes, for each v2 in B's axes
            //
            // Since we're working in a space where A is axis-aligned and A.center=0, many
            // of the tests and products simplify away.

            // Check for separation along the axes of box A
            if (mB_TA.X >= hA.X + Math.Abs(hx_B.X) + Math.Abs(hy_B.X) + Math.Abs(hz_B.X))
                return ContainmentType.Disjoint;

            if (mB_TA.Y >= hA.Y + Math.Abs(hx_B.Y) + Math.Abs(hy_B.Y) + Math.Abs(hz_B.Y))
                return ContainmentType.Disjoint;

            if (mB_TA.Z >= hA.Z + Math.Abs(hx_B.Z) + Math.Abs(hy_B.Z) + Math.Abs(hz_B.Z))
                return ContainmentType.Disjoint;

            // Check for separation along the axes box B, hx_B/hy_B/hz_B
            if (Math.Abs(Vector3.Dot(mB_T, bX)) >= Math.Abs(hA.X * bX.X) + Math.Abs(hA.Y * bX.Y) + Math.Abs(hA.Z * bX.Z) + hB.X)
                return ContainmentType.Disjoint;

            if (Math.Abs(Vector3.Dot(mB_T, bY)) >= Math.Abs(hA.X * bY.X) + Math.Abs(hA.Y * bY.Y) + Math.Abs(hA.Z * bY.Z) + hB.Y)
                return ContainmentType.Disjoint;

            if (Math.Abs(Vector3.Dot(mB_T, bZ)) >= Math.Abs(hA.X * bZ.X) + Math.Abs(hA.Y * bZ.Y) + Math.Abs(hA.Z * bZ.Z) + hB.Z)
                return ContainmentType.Disjoint;

            // Check for separation in plane containing an axis of box A and and axis of box B
            //
            // We need to compute all 9 cross products to find them, but a lot of terms drop out
            // since we're working in A's local space. Also, since each such plane is parallel
            // to the defining axis in each box, we know those dot products will be 0 and can
            // omit them.
            Vector3 axis;

            // a.X ^ b.X = (1,0,0) ^ bX
            axis = new Vector3(0, -bX.Z, bX.Y);
            if (Math.Abs(Vector3.Dot(mB_T, axis)) >= Math.Abs(hA.Y * axis.Y) + Math.Abs(hA.Z * axis.Z) + Math.Abs(Vector3.Dot(axis, hy_B)) + Math.Abs(Vector3.Dot(axis, hz_B)))
                return ContainmentType.Disjoint;

            // a.X ^ b.Y = (1,0,0) ^ bY
            axis = new Vector3(0, -bY.Z, bY.Y);
            if (Math.Abs(Vector3.Dot(mB_T, axis)) >= Math.Abs(hA.Y * axis.Y) + Math.Abs(hA.Z * axis.Z) + Math.Abs(Vector3.Dot(axis, hz_B)) + Math.Abs(Vector3.Dot(axis, hx_B)))
                return ContainmentType.Disjoint;

            // a.X ^ b.Z = (1,0,0) ^ bZ
            axis = new Vector3(0, -bZ.Z, bZ.Y);
            if (Math.Abs(Vector3.Dot(mB_T, axis)) >= Math.Abs(hA.Y * axis.Y) + Math.Abs(hA.Z * axis.Z) + Math.Abs(Vector3.Dot(axis, hx_B)) + Math.Abs(Vector3.Dot(axis, hy_B)))
                return ContainmentType.Disjoint;

            // a.Y ^ b.X = (0,1,0) ^ bX
            axis = new Vector3(bX.Z, 0, -bX.X);
            if (Math.Abs(Vector3.Dot(mB_T, axis)) >= Math.Abs(hA.Z * axis.Z) + Math.Abs(hA.X * axis.X) + Math.Abs(Vector3.Dot(axis, hy_B)) + Math.Abs(Vector3.Dot(axis, hz_B)))
                return ContainmentType.Disjoint;

            // a.Y ^ b.Y = (0,1,0) ^ bY
            axis = new Vector3(bY.Z, 0, -bY.X);
            if (Math.Abs(Vector3.Dot(mB_T, axis)) >= Math.Abs(hA.Z * axis.Z) + Math.Abs(hA.X * axis.X) + Math.Abs(Vector3.Dot(axis, hz_B)) + Math.Abs(Vector3.Dot(axis, hx_B)))
                return ContainmentType.Disjoint;

            // a.Y ^ b.Z = (0,1,0) ^ bZ
            axis = new Vector3(bZ.Z, 0, -bZ.X);
            if (Math.Abs(Vector3.Dot(mB_T, axis)) >= Math.Abs(hA.Z * axis.Z) + Math.Abs(hA.X * axis.X) + Math.Abs(Vector3.Dot(axis, hx_B)) + Math.Abs(Vector3.Dot(axis, hy_B)))
                return ContainmentType.Disjoint;

            // a.Z ^ b.X = (0,0,1) ^ bX
            axis = new Vector3(-bX.Y, bX.X, 0);
            if (Math.Abs(Vector3.Dot(mB_T, axis)) >= Math.Abs(hA.X * axis.X) + Math.Abs(hA.Y * axis.Y) + Math.Abs(Vector3.Dot(axis, hy_B)) + Math.Abs(Vector3.Dot(axis, hz_B)))
                return ContainmentType.Disjoint;

            // a.Z ^ b.Y = (0,0,1) ^ bY
            axis = new Vector3(-bY.Y, bY.X, 0);
            if (Math.Abs(Vector3.Dot(mB_T, axis)) >= Math.Abs(hA.X * axis.X) + Math.Abs(hA.Y * axis.Y) + Math.Abs(Vector3.Dot(axis, hz_B)) + Math.Abs(Vector3.Dot(axis, hx_B)))
                return ContainmentType.Disjoint;

            // a.Z ^ b.Z = (0,0,1) ^ bZ
            axis = new Vector3(-bZ.Y, bZ.X, 0);
            if (Math.Abs(Vector3.Dot(mB_T, axis)) >= Math.Abs(hA.X * axis.X) + Math.Abs(hA.Y * axis.Y) + Math.Abs(Vector3.Dot(axis, hx_B)) + Math.Abs(Vector3.Dot(axis, hy_B)))
                return ContainmentType.Disjoint;

            return ContainmentType.Intersects;
        }
        #endregion
    }
}
