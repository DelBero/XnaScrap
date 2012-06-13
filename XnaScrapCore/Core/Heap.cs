using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XnaScrapCore.Core
{
    public class Heap
    {
        #region members
        private List<object> m_memory;
        private Stack<int> m_free = new Stack<int>();
        #endregion

        #region CDtors
        public Heap()
        {
            m_memory = new List<object>();
        }

        public Heap(int size)
        {
            m_memory = new List<object>(size);
            for (int i = size-1; i >= 0; --i)
            {
                m_free.Push(i);
            }
        }
        #endregion

        #region methods

        public String readString(int address)
        {
            return m_memory[address] as String;
        }

        public void writeString(String s, int address)
        {
            m_memory[address] = s;
        }

        public int allocString()
        {
            return allocString(String.Empty);
        }

        public int allocString(String s)
        {
            if (m_free.Count == 0)
            {
                m_memory.Add(s);
                return m_memory.Count - 1;
            }
            else
            {
                return m_free.Pop();
            }
        }

        public void free(int address)
        {
            m_free.Push(address);
        }

        public void garbageCollection() 
        {
            List<object> old = m_memory;
            m_memory = new List<object>();
            for (int address = 0; address < old.Count; ++address)
            {
                if (!m_free.Contains(address))
                {
                    m_memory.Add(old[address]);
                }
            }
            m_free.Clear();
        }
        #endregion
    }
}
