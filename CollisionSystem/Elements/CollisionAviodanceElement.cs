using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CollisionSystem.Elements.Collider;
using XnaScrapCore.Core;
using System.IO;
using XnaScrapCore.Core.Elements;
using XnaScrapCore.Core.Parameter;
using XnaScrapCore.Core.Interfaces.Services;
using CollisionSystem.Service;
using Microsoft.Xna.Framework;
using XnaScrapCore.Core.Interfaces.Elements;
using CollisionSystem.Delegates;
using XnaScrapCore.Core.Delegates;
using SceneManagement.Service.Interfaces.Services;
using SceneManagement.Service.Elements;

namespace CollisionSystem.Elements
{
    

    public class CollisionAvoidanceElement : AbstractElement
    {
        #region registration
        #region Factory
        /// <summary>
        /// Factory class
        /// </summary>
        public class Constructor : IConstructor
        {
            private CollisionManager m_manager;
            public Constructor(CollisionManager manager)
            {
                m_manager = manager;
            }
            public AbstractElement getInstance(IDataReader state)
            {
                return new CollisionAvoidanceElement(state, m_manager);
            }
        }
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectBuilder"></param>
        public static void registerElement(IObjectBuilder objectBuilder, CollisionManager manager)
        {
            ParameterSequenceBuilder sequenceBuilder = new ParameterSequenceBuilder();
            sequenceBuilder.createSequence();

            objectBuilder.registerElement(new Constructor(manager), sequenceBuilder.CurrentSequence, typeof(CollisionAvoidanceElement), "CollisionAvoidanceElement", null);
        }
        #endregion

        #region member
        private List<AbstractCollider> m_colliders = new List<AbstractCollider>();

        public List<AbstractCollider> Colliders
        {
            get { return m_colliders; }
        }

        private CollisionManager m_manager;

        private SceneElement m_sceneElement;

        public Microsoft.Xna.Framework.BoundingBox BoundingBox
        {
            get { return m_sceneElement.BoundingBox; }
        }               

        private bool m_canMove = false;
        /// <summary>
        /// Returns true when the gameobject can be moved.
        /// </summary>
        public bool CanMove
        {
            get { return m_canMove; }
        }

        private Vector3 m_position;
        private Vector3 m_lastPosition;
        private Quaternion m_orientation;

        /// <summary>
        /// Returns the position befor the current collision detection iteration. This is assumed to be valid.
        /// </summary>
        public Vector3 LastFramePosition
        {
            get { return m_position; }
        }

        /// <summary>
        /// Returns the desired position. This comes from other components.
        /// </summary>
        public Vector3 Position
        {
            get { return m_lastPosition; }
        }


        //collision ending
        private bool m_isIntersecting = false;

        public bool IsIntersecting
        {
            get { return m_isIntersecting; }
            set { m_isIntersecting = value; }
        }
        private bool m_wasIntersecting = false;

        #endregion

        #region events
        public event CollisionEventHandler Collision;
        public event CollisionEndedEventHandler CollisionEnded;

        public void OnCollision(CollisionEventArgs e)
        {
            m_isIntersecting = true;
            if (Collision != null)
            {
                Collision(this, e);
            }
        }

        public void OnCollisionEnded(CollisionEventArgs e)
        {
            if (CollisionEnded != null)
            {
                CollisionEnded(this, e);
            }
        }
        #endregion

        public CollisionAvoidanceElement(IDataReader state, CollisionManager manager)
            : base(state)
        {
            m_manager = manager;
        }

        /// <summary>
        /// This is called by the collision manager
        /// </summary>
        public void beginUpdate()
        {
            m_isIntersecting = false;
            // bring all colliders in world position
            //foreach (AbstractCollider ac in Colliders)
            //{
                
            //}
        }


        /// <summary>
        /// This is called by the collision manager
        /// </summary>
        public void endUpdate()
        {
            if (!m_isIntersecting && m_wasIntersecting)
            {
                OnCollisionEnded(CollisionEventArgs.Empty);
            }
            m_isIntersecting = m_wasIntersecting;
        }

        #region updates
        private void update()
        {
            float x = (BoundingBox.Max.X - BoundingBox.Min.X) * 0.5f;
            float y = (BoundingBox.Max.Y - BoundingBox.Min.Y) * 0.5f;
            float z = (BoundingBox.Max.Z - BoundingBox.Min.Z) * 0.5f;
            BoundingBox bb = new BoundingBox();
            bb.Min.X = m_position.X - x;
            bb.Min.Y = m_position.Y - y;
            bb.Min.Z = m_position.Z - z;
            bb.Max.X = m_position.X + x;
            bb.Max.Y = m_position.Y + y;
            bb.Max.Z = m_position.Z + z;
            m_sceneElement.BoundingBox = bb;
            foreach (AbstractCollider ac in m_colliders)
            {
                ac.Position = m_position;
                ac.update(m_position + ac.RelativePosition);
            }
        }

        private void updateBB()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region eventhandler
        public void PositionChanged(object sender, Position3DChangedEventArgs e)
        {
            m_position = e.Position;
            update();
        }

        public void OrientationChanged(object sender, EventArgs e)
        {
            IOrientation3D or = sender as IOrientation3D;
            if (or != null)
            {
                m_orientation = or.Orientation;
            }
        }
        #endregion

        #region lifecycle
        /// <summary>
        /// Gathers all colliders in the gameobject.
        /// </summary>
        public override void  doInitialize()
        {
            //TODO gather colliders and add all to the bounding box 
            BoundingBox bb = new BoundingBox();
            List<AbstractElement> colliders = Owner.getAll(typeof(AbstractCollider));
            foreach (AbstractElement ae in colliders)
            {
                AbstractCollider ac = ae as AbstractCollider;
                m_colliders.Add(ac);
                bb = Microsoft.Xna.Framework.BoundingBox.CreateMerged(bb, ac.BoundingBox);
            }

            // add to scenemanagement
            ISceneManager sceneManager = Owner.Game.Services.GetService(typeof(ISceneManager)) as ISceneManager;
            if (sceneManager != null)
            {
                m_sceneElement = sceneManager.insertElementCollision(bb, Owner);
            }

            // add to collisionmanagement
            m_manager.addCollisionAvoider(this);

            // get position
            IPosition3D pos = Owner.getFirst(typeof(IPosition3D)) as IPosition3D;
            if (pos != null)
            {
                m_position = pos.Position;
                m_lastPosition = pos.Position;
                m_canMove = pos.Moves;
                pos.PositionChanged += new XnaScrapCore.Core.Delegates.Position3DChangedEventHandler(PositionChanged);
            }

            // get orientation
            IOrientation3D orientation = Owner.getFirst(typeof(IOrientation3D)) as IOrientation3D;
            if (orientation != null)
            {
                m_orientation = orientation.Orientation;
                orientation.OrientationChanged += new XnaScrapCore.Core.Delegates.OrientationChangedEventHandler(OrientationChanged);
            }
            

            base.doInitialize();
        }

        /// <summary>
        /// End Elements lifecycle
        /// </summary>
        public override void doDestroy()
        {
            m_manager.removeCollisionAvoider(this);

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

            // remove from scenemanagement
            ISceneManager sceneManager = Owner.Game.Services.GetService(typeof(ISceneManager)) as ISceneManager;
            if (sceneManager != null)
            {
                sceneManager.removeElementCollision(m_sceneElement);
            }

            // clean up event handler
            Collision = null;
            CollisionEnded = null;

            base.doDestroy();
        }
        #endregion
    }
}
