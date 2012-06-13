using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace XnaScrapCore.Core
{

    // TODO implement ReadByte / WriteByte
    public class MemoryStream : Stream
    {
        public const long m_minSize = 32;
        public const int m_growfactor = 2;
        private byte[] m_data = new byte[m_minSize];

        public byte[] Data
        {
            get { return m_data; }
            set { m_data = value; }
        }
        private int m_position = 0;

        public IDataReader Reader
        {
            get { return new DataReader(this); }
        }
        public IDataWriter Writer
        {
            get { return new DataWriter(this); }
        }

        public override bool CanRead
        {
            get { return true; }
        }
        public override bool CanWrite
        {
            get { return true; }
        }
        public override bool CanSeek
        {
            get { return false; }
        }
        
        public MemoryStream()
        {
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            // resize
            if (m_position + count > m_data.Length)
            {
                long newSize = m_data.Length * m_growfactor;
                // resize until it fits
                while (newSize < m_position + count) { newSize *= m_growfactor; }
                // copy old content
                byte[] temp = new byte[newSize];
                m_data.CopyTo(temp,0);
                m_data = temp;
            }
            //buffer.CopyTo(m_data,m_position);
            Array.Copy(buffer,0,m_data,m_position,count);
            m_position += count;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int actualRead = count;
            if (m_data.Length - m_position < count) 
            {
                actualRead = (int)(m_data.Length - m_position);
            }
            //m_data.CopyTo(buffer,m_position);
            Array.Copy(m_data, m_position, buffer, 0, actualRead);
            m_position += actualRead;
            return actualRead;
        }

        public override void WriteByte(byte value)
        {
            if (m_position < m_data.Length)
            {
                m_data[m_position++] = value;
            }
            else
            {
                long newSize = m_data.Length * m_growfactor;
                // copy old content
                byte[] temp = new byte[newSize];
                m_data.CopyTo(temp, 0);
                m_data = temp;

                m_data[m_position++] = value;
            }
        }

        public override int ReadByte()
        {
            if (m_position < m_data.Length)
            {
                return m_data[m_position++];
            }
            return -1;
        }

        public override void Flush()
        {
            
        }

        public override long Length
        {
            get { return m_data.Length; }
        }

        public override long Position
        {
            get
            {
                return m_position;
            }
            set
            {
                if (value >= 0 && value < Length)
                {
                    m_position = (int)value;
                }
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    {
                        m_position = (int)offset;
                        break;
                    }
                case SeekOrigin.Current:
                    {
                        m_position += (int)offset;
                        if (m_position > m_data.Length)
                        {
                            m_position = m_data.Length;
                        }
                        break;
                    }
                case SeekOrigin.End:
                    {
                        m_position = m_data.Length - (int)offset;
                        break;
                    }
            }
            return m_position;
        }

        public override void SetLength(long value)
        {
        }

        public void reset()
        {
            m_data = new byte[m_minSize];
            m_position = 0;
        }
    }
}
