using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace XnaScrapCore.Core
{
    public class XnaScrapId : IComparable<XnaScrapId>, IEquatable<XnaScrapId>
    {
        public static readonly XnaScrapId INVALID_ID = new XnaScrapId("B883DA68-2A4E-4D57-ADAC-AE615E2E841A");

        private static int m_autoId;

        public static XnaScrapId CreateId() 
        {
            String id = m_autoId.ToString();
            ++m_autoId;
            return new XnaScrapId(id);
        }

        private String m_value;

        public XnaScrapId()
        {
            m_value = INVALID_ID.m_value;
        }

        public XnaScrapId(String id)
        {
            if (id.Length == 0)
            {
                m_value = INVALID_ID.m_value;
            }
            else
            {
                m_value = id;
            }
        }

        public XnaScrapId(IDataReader reader)
        {
            deserialize(reader);
        }

        public void deserialize(IDataReader reader)
        {
            m_value = reader.ReadString();
        }

        public void serialize(IDataWriter writer)
        {
            writer.Write(m_value);
        }
 
        public override string ToString()
        {
            return m_value;
        }

        public bool Equals(XnaScrapId rhs)
        {
            return m_value.Equals(rhs.m_value);
        }

        public override bool Equals(object obj)
        {
            if (obj is XnaScrapId)
                return Equals(obj as XnaScrapId);
            else
                return m_value.Equals(obj);
        }

        public override int GetHashCode()
        {
            return m_value.GetHashCode();
        }

        public static bool operator==(XnaScrapId lhs, XnaScrapId rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(XnaScrapId lhs, XnaScrapId rhs)
        {
            return !lhs.Equals(rhs);
        }

        public int CompareTo(XnaScrapId other)
        {
            return m_value.CompareTo(other.m_value);
        }
    }
}
