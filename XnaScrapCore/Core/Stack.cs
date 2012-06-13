using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XnaScrapCore.Core
{
    public class Stack
    {
        #region member
        MemoryStream m_data = new MemoryStream();

        public long Position
        {
            get
            {
                return m_data.Position;
            }
            set
            {
                m_data.Position = value;
            }
        }

        public MemoryStream DataStream
        {
            get
            {
                return m_data;
            }
            set
            {
                m_data = value;
            }
        }

        public byte[] Data
        {
            get
            {
                return m_data.Data;
            }
            set
            {
                m_data.Data = value;
            }
        }

        public bool Empty
        {
            get { return m_data.Position == 0; }
        }
        #endregion

        public Stack()
        {

        }

        public Stack(MemoryStream data)
        {
            m_data = data;
        }

        /// <summary>
        /// Pushes an integer on the stack an returns its 'address'.
        /// </summary>
        /// <param name="val"></param>
        /// <returns>Address of the pushed integer.</returns>
        public int push(int val)
        {
            int ret = (int)m_data.Position;
            m_data.Writer.Write(val);
            return ret;
        }
        /// <summary>
        /// Pushes a float on the stack an returns its 'address'.
        /// </summary>
        /// <param name="val"></param>
        /// <returns>Address of the pushed integer.</returns>
        public int push(float val)
        {
            int ret = (int)m_data.Position;
            m_data.Writer.Write(val);
            return ret;
        }
        /// <summary>
        /// Pushes a string on the stack an returns its 'address'.
        /// </summary>
        /// <param name="val"></param>
        /// <returns>Address of the pushed integer.</returns>
        public int push(String val)
        {
            int ret = (int)m_data.Position;
            m_data.Writer.Write(val);
            //m_data.Writer.Write(val.Length);
            push(val.Length);
            return ret;
        }

        /// <summary>
        /// Reads an integer from a specific address.
        /// </summary>
        /// <param name="addr">Address of the integer.</param>
        /// <returns></returns>
        public int readInt(int addr)
        {
            int pos = (int)m_data.Position;
            m_data.Position = addr;
            int ret = m_data.Reader.ReadInt32();
            m_data.Position = pos;
            return ret;
        }

        /// <summary>
        /// Writes ans integer to a address in the stack
        /// </summary>
        /// <param name="addr">absolute address</param>
        /// <param name="val">value to write</param>
        public void writeInt(int addr, int val)
        {
            int pos = (int)m_data.Position;
            m_data.Position = addr;
            m_data.Writer.Write(val);
            m_data.Position = pos;
        }

        /// <summary>
        /// Reads a float from a specific address.
        /// </summary>
        /// <param name="addr">Address of the float.</param>
        /// <returns></returns>
        public float readFlt(int addr)
        {
            int pos = (int)m_data.Position;
            m_data.Position = addr;
            float ret = m_data.Reader.ReadSingle();
            m_data.Position = pos;
            return ret;
        }

        /// <summary>
        /// Writes ans floating point value to a address in the stack
        /// </summary>
        /// <param name="addr">absolute address</param>
        /// <param name="val">value to write</param>
        public void writeFlt(int addr, float val)
        {
            int pos = (int)m_data.Position;
            m_data.Position = addr;
            m_data.Writer.Write(val);
            m_data.Position = pos;
        }

        /// <summary>
        /// Reads a string from a specific address.
        /// </summary>
        /// <param name="addr">Address of the string.</param>
        /// <returns></returns>
        public String readString(int addr)
        {
            int pos = (int)m_data.Position;
            m_data.Position = addr;
            String ret = m_data.Reader.ReadString();
            m_data.Position = pos;
            return ret;
        }

        /// <summary>
        /// Sets the stackpointer.
        /// </summary>
        /// <param name="addr">Targetaddress</param>
        public void set(int addr)
        {
            m_data.Position = addr;
        }

        public void reset(int addr)
        {
            m_data.Position -= addr;
        }

        /// <summary>
        /// Pops the last integer.
        /// </summary>
        /// <returns></returns>
        public int popInt()
        {
            m_data.Position -= sizeof(int);
            int ret = m_data.Reader.ReadInt32();
            m_data.Position -= sizeof(int);
            return ret;
        }

        public float popFloat()
        {
            m_data.Position -= sizeof(float);
            float ret = m_data.Reader.ReadSingle();
            m_data.Position -= sizeof(float);
            return ret;
        }

        public String popString()
        {
            int length = popInt();
            m_data.Position -= (length + 1);
            String ret = m_data.Reader.ReadString();
            m_data.Position -= (length + 1);
            return ret;
        }
    }
}
