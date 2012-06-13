using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SceneManagement.Service.Interfaces.Services;
using Microsoft.Xna.Framework;
using SceneManagement.Service.Elements;
using SceneManagement.Service.Interfaces;
using XnaScrapCore.Core;

namespace SceneManagement.Service
{
    public interface INode
    {
        bool insertElement(SceneElement se);

        void updateElement(SceneElement se);

        void removeElement(SceneElement element);
    }

    class OctreeNode : INode
    {
        private static Vector3[] subVecs = new Vector3[] {
            new Vector3(0,0,0),
            new Vector3(0,0,1),
            new Vector3(1,0,1),
            new Vector3(1,0,0),
            new Vector3(0,1,0),
            new Vector3(0,1,1),
            new Vector3(1,1,1),
            new Vector3(1,1,0)
        };
        private static short m_maxLevel = 0;

        public static short MaxLevel
        {
            get { return OctreeNode.m_maxLevel; }
            set { OctreeNode.m_maxLevel = value; }
        }

        #region member
        private BoundingBox m_bb;
        private short m_level = 0;

        public BoundingBox BoundingBox
        {
            get { return m_bb; }
            set { m_bb = value; }
        }

        private OctreeNode[] m_children = null;
        private List<SceneElement> m_elements = new List<SceneElement>();
        private OctreeNode m_parent;
        #endregion

        #region CDtors

        public OctreeNode(short level) { m_level = level; }
        public OctreeNode(BoundingBox bb, short level, OctreeNode parent)
        {
            m_bb = bb;
            m_level = level;
            m_parent = parent;
        }
        #endregion

        private void insertHere(SceneElement se)
        {
            if (!m_elements.Contains(se))
            {
                m_elements.Add(se);
                se.Node = this;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        public bool insertElement(SceneElement element)
        {
            bool inserted = false;
            //if (element.BoundingBox.Intersects(BoundingBox))
            {
                if (m_children != null)
                {
                    // we only want to insert the object in one node.
                    // if more then one child contains the node we insert it in the current node
                    short nodeCount = 0;
                    OctreeNode target = null;
                    foreach (OctreeNode node in m_children)
                    {
                        if (element.BoundingBox.Intersects(node.BoundingBox))
                        {
                            ++nodeCount;
                            target = node;
                        }
                    }
                    if (nodeCount == 1)
                    {
                        inserted = target.insertElement(element);
                    }
                    else
                    {
                        insertHere(element);
                        inserted = true;
                    }
                }
                else
                {
                    if (m_level < MaxLevel)
                    {
                        split();
                        foreach (OctreeNode node in m_children)
                        {
                            inserted = inserted || node.insertElement(element);
                        }
                    }
                    else
                    {
                        insertHere(element);
                        inserted = true;
                    }
                }
            }
            return inserted;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        public void removeElement(SceneElement element)
        {
            if (m_elements.Contains(element))
            {
                m_elements.Remove(element);
            }
            if (m_children != null)
            {
                foreach (OctreeNode node in m_children)
                {
                    node.removeElement(element);
                }
            }
        }

        public void updateElement(SceneElement se)
        {
            // remove the element from it's current node
            if (se.Node != null)
            {
                removeElement(se);
                se.Node = null;
            }
            // is the element still inside this node
            if (se.BoundingBox.Intersects(BoundingBox))
            {
                insertElement(se);
            }
            else
            {
                // no? give it to parent
                if (m_parent != null)
                {
                    m_parent.updateElement(se);
                }
                else // top node
                {
                    insertElement(se);
                }
            }
        }

        /// <summary>
        /// Splits the node and inserts the elements into the children
        /// </summary>
        public void split()
        {
            if (m_children != null)
            {
                return;
            }
            Vector3 center = new Vector3();
            center.X = (m_bb.Min.X + m_bb.Max.X) * 0.5f;
            center.Y = (m_bb.Min.Y + m_bb.Max.Y) * 0.5f;
            center.Z = (m_bb.Min.Z + m_bb.Max.Z) * 0.5f;

            m_children = new OctreeNode[8];
            for (short i = 0; i < 8; ++i)
            {
                Vector3 vec = subVecs[i];
                Vector3 min = new Vector3(), max = new Vector3();
                min.X = center.X + (m_bb.Min.X * (1.0f - vec.X));
                min.Y = center.Y + (m_bb.Min.Y * (1.0f - vec.Y));
                min.Z = center.Z + (m_bb.Min.Z * (1.0f - vec.Z));

                max.X = center.X + (m_bb.Max.X * vec.X);
                max.Y = center.Y + (m_bb.Max.Y * vec.Y);
                max.Z = center.Z + (m_bb.Max.Z * vec.Z);

                m_children[i] = new OctreeNode( new BoundingBox(min, max),(short)(m_level + 1), this);
            }
        }

        public void getElements(BoundingBox bb, ISelectionExecutor executor)
        {
            
            ContainmentType t = bb.Contains(BoundingBox);
            if (t == ContainmentType.Contains)
            {
                foreach (SceneElement e in m_elements)
                {
                    executor.execute(e);
                    if (m_children != null)
                    {
                        foreach (OctreeNode node in m_children)
                        {
                            node.getElements(bb, executor);
                        }
                    }
                }
            }
            else if (t == ContainmentType.Intersects)
            {
                foreach (SceneElement se in m_elements)
                {
                    t = bb.Contains(se.BoundingBox);
                    if (t == ContainmentType.Contains || t == ContainmentType.Intersects)
                    {
                        executor.execute(se);
                    }
                }
                if (m_children != null)
                {
                    foreach (OctreeNode node in m_children)
                    {
                        node.getElements(bb, executor);
                    }
                }
            }
            return;
        }

        public void getElements(Ray ray, ISelectionExecutor executor)
        {
            float? intersects = ray.Intersects(BoundingBox);
            if (intersects.HasValue)
            {
                foreach (SceneElement e in m_elements)
                {
                    if (ray.Intersects(e.BoundingBox).HasValue)
                    {
                        executor.execute(e);
                        if (m_children != null)
                        {
                            foreach (OctreeNode node in m_children)
                            {
                                node.getElements(ray, executor);
                            }
                        }
                    }
                }
            }
        }

        public List<SceneElement> getElements(BoundingFrustum bf)
        {
            List<SceneElement> elements = new List<SceneElement>();
            if (BoundingBox.Intersects(bf))
            {
                elements.AddRange(m_elements);
            }

            foreach (OctreeNode node in m_children)
            {
                node.getElements(bf);
            }

            return elements;
        }

        public List<SceneElement> getElements(Ray ray)
        {
            List<SceneElement> elements = new List<SceneElement>();
            float? i = BoundingBox.Intersects(ray);
            if (i.HasValue)
            {
                elements.AddRange(m_elements);
            }

            foreach (OctreeNode node in m_children)
            {
                node.getElements(ray);
            }
            return elements;
        }
    }

    /// <summary>
    /// A classic Octree
    /// </summary>
    public class Octree : IElementContainer
    {
        private OctreeNode m_root = new OctreeNode(0);
        private Dictionary<GameObject, SceneElement> m_sceneElements = new Dictionary<GameObject, SceneElement>();

        public Octree()
        {
            m_root.BoundingBox = new BoundingBox(new Vector3(-1000, -1000, -1000), new Vector3(1000, 1000, 1000));
        }

        public SceneElement insertElement(BoundingBox bb, GameObject gameObject)
        {
            ContainmentType t = m_root.BoundingBox.Contains(bb);
            if (t == ContainmentType.Contains) {
                SceneElement se;
                if (!m_sceneElements.TryGetValue(gameObject, out se))
                {
                    se = new SceneElement(bb, gameObject);
                    m_root.insertElement(se);
                }
                return se;
            }
            return null;
        }

        public void removeElement(SceneElement se)
        {
            m_root.removeElement(se);
        }

        public void getElements(BoundingBox bb, ISelectionExecutor executor)
        {
            m_root.getElements(bb, executor);
        }

        public void getElements(Ray ray, ISelectionExecutor executor)
        {
            m_root.getElements(ray, executor);
        }
    }
}
