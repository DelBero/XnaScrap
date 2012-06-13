using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using XnaScrapCore.Core;
using SceneManagement.Service.Elements;
using SceneManagement.Service.Interfaces.Services;
using XnaScrapCore.Core.Interfaces.Elements;
using System.IO;
using XnaScrapCore.Core.Parameter;
using CollisionSystem.Service;
using XnaScrapCore.Core.Delegates;

namespace CollisionSystem.Elements.Collider
{

    public abstract class AbstractCollider : AbstractElement
    {
        #region member
        /// <summary>
        /// World position
        /// </summary>
        protected Vector3 m_position;
        private Vector3 m_relativePos;
        protected Microsoft.Xna.Framework.BoundingBox m_boundingBox;

        public Microsoft.Xna.Framework.BoundingBox BoundingBox
        {
            get { return m_boundingBox; }
        }

        /// <summary>
        /// World position
        /// </summary>
        public Vector3 Position
        {
            get { return m_position; }
            set { m_position = value; }
        }

        /// <summary>
        /// Position relative to the CollisionElement
        /// </summary>
        public Vector3 RelativePosition
        {
            get { return m_relativePos; }
        }

        protected Quaternion m_orientation;

        public Quaternion Orientation
        {
            get { return m_orientation; }
        }

        protected XnaScrapId m_collisionGroupId;
        protected List<XnaScrapId> m_collisionGroupIds = new List<XnaScrapId>();
        protected uint m_collidesWith = CollisionManager.CollideNone;
        protected uint m_collisionGroup = 1;

        public uint CollisionGroup
        {
            get { return m_collisionGroup; }
            set { m_collisionGroup = value; }
        }

        public uint CollidesWith
        {
            get { return m_collidesWith; }
            set { m_collidesWith = value; }
        }

        #endregion

        #region registration
        protected static void basicRegistration(ParameterSequenceBuilder sequenceBuilder)
        {
            sequenceBuilder.addParameter(new IdParameter("collisionGroup",CollisionManager.CollideAllId.ToString()));
            SequenceParameter collisionGroups = new SequenceParameter("collisionGroups", new IdParameter("collisionGroup", CollisionManager.CollideAllId.ToString()));
            sequenceBuilder.addParameter(collisionGroups);
            sequenceBuilder.addParameter(new FloatParameter("position","0.0,0.0,0.0"));
        }
        #endregion

        public AbstractCollider(IDataReader state)
            : base(state)
        {
            m_collisionGroupId = new XnaScrapId(state);
            int numCollisionGroups = state.ReadInt32();
            for (int i = 0; i < numCollisionGroups; ++i)
            {
                m_collisionGroupIds.Add( new XnaScrapId(state));
            }
            m_relativePos.X = state.ReadSingle();
            m_relativePos.Y = state.ReadSingle();
            m_relativePos.Z = state.ReadSingle();
        }

        /// <summary>
        /// initializes the collider.
        /// </summary>
        /// <param name="position">The position of the collider within its containing element.</param>
        /// <param name="orientation">Orientation of the collider within its containing element</param>
        public void init(Vector3 position, Quaternion orientation)
        {
            m_relativePos = position;
            m_orientation = orientation;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void doInitialize()
        {
            // get collision group
            CollisionManager collisionManager = Owner.Game.Services.GetService(typeof(CollisionManager)) as CollisionManager;
            if (collisionManager != null)
            {
                m_collisionGroup = collisionManager.getCollisionGroup(m_collisionGroupId);
                foreach (XnaScrapId id in m_collisionGroupIds)
                {
                    m_collidesWith |= collisionManager.getCollisionGroup(id);
                }
            }
            //// get position
            //IPosition3D pos = Owner.getFirst(typeof(IPosition3D)) as IPosition3D;
            //if (pos != null)
            //{
            //    m_position = m_relativePos + pos.Position;
            //    pos.PositionChanged += new XnaScrapCore.Core.Delegates.Position3DChangedEventHandler(PositionChanged);
            //}

            //// get orientation
            //IOrientation3D orientation = Owner.getFirst(typeof(IOrientation3D)) as IOrientation3D;
            //if (orientation != null)
            //{
            //    m_orientation = orientation.Orientation;
            //    orientation.OrientationChanged += new XnaScrapCore.Core.Delegates.OrientationChangedEventHandler(OrientationChanged);
            //}

            base.doInitialize();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void doDestroy()
        {
            IPosition3D pos = Owner.getFirst(typeof(IPosition3D)) as IPosition3D;
            if (pos != null)
            {
                pos.PositionChanged -= new XnaScrapCore.Core.Delegates.Position3DChangedEventHandler(PositionChanged);
            }

            IOrientation3D orientation = Owner.getFirst(typeof(IOrientation3D)) as IOrientation3D;
            if (orientation != null)
            {
                orientation.OrientationChanged -= new XnaScrapCore.Core.Delegates.OrientationChangedEventHandler(OrientationChanged);
            }
            base.doDestroy();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        public override void doSerialize(IDataWriter state)
        {
            base.doSerialize(state);
            state.Write(m_collisionGroupId.ToString());
            state.Write(m_collisionGroupIds.Count);
            foreach (XnaScrapId collId in m_collisionGroupIds)
            {
                state.Write(collId.ToString());
            }
            state.Write(m_relativePos.X);
            state.Write(m_relativePos.Y);
            state.Write(m_relativePos.Z);
        }

        private void PositionChanged(object sender, Position3DChangedEventArgs e)
        {
            m_position = m_relativePos + e.Position;
            update(m_position);
        }

        private void OrientationChanged(object sender, EventArgs e)
        {
            IOrientation3D or = sender as IOrientation3D;
            if (or != null)
            {
                m_orientation = m_orientation * or.Orientation;
            }

        }

        public virtual void update(Vector3 position)
        {
            Position = m_relativePos + position;
        }

        /// <summary>
        /// Used to check intersections.
        /// </summary>
        /// <param name="other">The collider to check against.</param>
        /// <param name="normal1">the normal at the point of intersection (other collider).</param>
        /// <param name="normal2">the normal at the point of intersection (self).</param>
        /// <returns>True if a intersection happend.</returns>
        public bool intersects(AbstractCollider other, out Vector3 normal1, out Vector3 normal2)
        {
            normal1 = new Vector3();
            normal2 = new Vector3();
            if (other is BoxCollider)
            {
                return intersectsAxisAligned(other as BoxCollider, out normal1, out normal2);
            }
            else if (other is SphereCollider)
            {
                return intersectsAxisAligned(other as SphereCollider, out normal1, out normal2);
            }
            else if (other is OrientedBoxCollider)
            {
                return intersectsAxisAligned(other as OrientedBoxCollider, out normal1, out normal2);
            }
            //else if (other is CylinderCollider)
            //{
            //    return intersectsAxisAligned(other as CylinderCollider, out normal1, out normal2);
            //}
            //else if (other is CylinderCollider)
            //{
            //    return intersectsAxisAligned(other as CapsuleCollider, out normal1, out normal2);
            //}
            //else if (other is HeightfieldCollider)
            //{
            //    return intersectsAxisAligned(other as HeightfieldCollider, out normal1, out normal2);
            //}
            //else if (other is PlaneCollider)
            //{
            //    return intersectsAxisAligned(other as PlaneCollider, out normal1, out normal2);
            //}
            //else if (other is MeshCollider)
            //{
            //    return intersectsAxisAligned(other as MeshCollider, out normal1, out normal2);
            //}
            return false;
        }

        /// <summary>
        /// Checkes if the point lies in the collider. The point has to be in the local coordinate system of the collider's parent.
        /// </summary>
        /// <param name="point">Point to check.</param>
        /// <returns></returns>
        public abstract bool contains(Vector3 point);
        public abstract bool intersectsAxisAligned(BoxCollider other, out Vector3 normal1, out Vector3 normal2);
        public abstract bool intersectsAxisAligned(SphereCollider other, out Vector3 normal1, out Vector3 normal2);
        public abstract bool intersectsAxisAligned(OrientedBoxCollider other, out Vector3 normal1, out Vector3 normal2);
        //public abstract bool intersectsAxisAligned(PlaneCollider other, out Vector3 normal1, out Vector3 normal2);
        //public abstract bool intersectsAxisAligned(HeightfieldCollider other, out Vector3 normal1, out Vector3 normal2);
        //public abstract bool intersectsAxisAligned(CapsuleCollider other, out Vector3 normal1, out Vector3 normal2);
        //public abstract bool intersectsAxisAligned(CylinderCollider other, out Vector3 normal1, out Vector3 normal2);
        //public abstract bool intersectsAxisAligned(MeshCollider other, out Vector3 normal1, out Vector3 normal2);
        public abstract bool intersects(Ray ray);
    }
}
