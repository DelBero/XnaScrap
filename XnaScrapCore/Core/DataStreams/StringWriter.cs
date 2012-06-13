using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;

namespace XnaScrapCore.Core
{
    public class StringWriter: IDataWriter
    {
        private BinaryWriter m_writer;
        public StringWriter(Stream output)
        {
            m_writer = new BinaryWriter(output);
        }

        private void writeString(string s)
        {
            m_writer.Write(s);
        }

        public void Write(bool value)
        {
            string val = value ? "true" : "false";
            writeString(val);
        }
        //
        // Summary:
        //     Writes an unsigned byte to the current stream and advances the stream position
        //     by one byte.
        //
        // Parameters:
        //   value:
        //     The unsigned byte to write.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ObjectDisposedException:
        //     The stream is closed.
        public void Write(byte value)
        {
            m_writer.Write(new byte[] { value }, 0, 1);
        }
        //
        // Summary:
        //     Writes a byte array to the underlying stream.
        //
        // Parameters:
        //   buffer:
        //     A byte array containing the data to write.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   System.ArgumentNullException:
        //     buffer is null.
        public void Write(byte[] buffer)
        {
            m_writer.Write(buffer, 0, buffer.Length);
        }
        //
        // Summary:
        //     Writes a Unicode character to the current stream and advances the current
        //     position of the stream in accordance with the Encoding used and the specific
        //     characters being written to the stream.
        //
        // Parameters:
        //   ch:
        //     The non-surrogate, Unicode character to write.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   System.ArgumentException:
        //     ch is a single surrogate character.
        public void Write(char ch)
        {
            m_writer.Write(System.BitConverter.GetBytes(ch), 0, 1);
        }
        //
        // Summary:
        //     Writes a character array to the current stream and advances the current position
        //     of the stream in accordance with the Encoding used and the specific characters
        //     being written to the stream.
        //
        // Parameters:
        //   chars:
        //     A character array containing the data to write.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     chars is null.
        //
        //   System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
        public void Write(char[] chars)
        {
            writeString(new String(chars));
        }
        //
        // Summary:
        //     Writes a decimal value to the current stream and advances the stream position
        //     by sixteen bytes.
        //
        // Parameters:
        //   value:
        //     The decimal value to write.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ObjectDisposedException:
        //     The stream is closed.
        public void Write(decimal value)
        {
#if WINDOWS
            writeString(value.ToString(CultureInfo.GetCultureInfo("en-US").NumberFormat));
#else
            writeString(value.ToString());
#endif
        }
        //
        // Summary:
        //     Writes an eight-byte floating-point value to the current stream and advances
        //     the stream position by eight bytes.
        //
        // Parameters:
        //   value:
        //     The eight-byte floating-point value to write.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ObjectDisposedException:
        //     The stream is closed.
        public void Write(double value)
        {
#if WINDOWS
            writeString(value.ToString(CultureInfo.GetCultureInfo("en-US").NumberFormat));
#else
            writeString(value.ToString());
#endif
        }
        //
        // Summary:
        //     Writes a four-byte floating-point value to the current stream and advances
        //     the stream position by four bytes.
        //
        // Parameters:
        //   value:
        //     The four-byte floating-point value to write.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ObjectDisposedException:
        //     The stream is closed.
        public void Write(float value)
        {
#if WINDOWS
            writeString(value.ToString(CultureInfo.GetCultureInfo("en-US").NumberFormat));
#else
            writeString(value.ToString());
#endif
        }
        //
        // Summary:
        //     Writes a four-byte signed integer to the current stream and advances the
        //     stream position by four bytes.
        //
        // Parameters:
        //   value:
        //     The four-byte signed integer to write.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ObjectDisposedException:
        //     The stream is closed.
        public void Write(int value)
        {
#if WINDOWS
            writeString(value.ToString(CultureInfo.GetCultureInfo("en-US").NumberFormat));
#else
            writeString(value.ToString());
#endif
        }
        //
        // Summary:
        //     Writes an eight-byte signed integer to the current stream and advances the
        //     stream position by eight bytes.
        //
        // Parameters:
        //   value:
        //     The eight-byte signed integer to write.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ObjectDisposedException:
        //     The stream is closed.
        public void Write(long value)
        {
#if WINDOWS
            writeString(value.ToString(CultureInfo.GetCultureInfo("en-US").NumberFormat));
#else
            writeString(value.ToString());
#endif
        }
        //
        // Summary:
        //     Writes a signed byte to the current stream and advances the stream position
        //     by one byte.
        //
        // Parameters:
        //   value:
        //     The signed byte to write.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ObjectDisposedException:
        //     The stream is closed.
        public void Write(sbyte value)
        {
            writeString(value.ToString());
        }
        //
        // Summary:
        //     Writes a two-byte signed integer to the current stream and advances the stream
        //     position by two bytes.
        //
        // Parameters:
        //   value:
        //     The two-byte signed integer to write.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ObjectDisposedException:
        //     The stream is closed.
        public void Write(short value)
        {
#if WINDOWS
            writeString(value.ToString(CultureInfo.GetCultureInfo("en-US").NumberFormat));
#else
            writeString(value.ToString());
#endif
        }
        //
        // Summary:
        //     Writes a length-prefixed string to this stream in the current encoding of
        //     the System.IO.BinaryWriter, and advances the current position of the stream
        //     in accordance with the encoding used and the specific characters being written
        //     to the stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ArgumentNullException:
        //     value is null.
        //
        //   System.ObjectDisposedException:
        //     The stream is closed.
        public void Write(string value)
        {
            writeString(value);
        }
        //
        // Summary:
        //     Writes a four-byte unsigned integer to the current stream and advances the
        //     stream position by four bytes.
        //
        // Parameters:
        //   value:
        //     The four-byte unsigned integer to write.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ObjectDisposedException:
        //     The stream is closed.
        public void Write(uint value)
        {
#if WINDOWS
            writeString(value.ToString(CultureInfo.GetCultureInfo("en-US").NumberFormat));
#else
            writeString(value.ToString());
#endif
        }
        //
        // Summary:
        //     Writes an eight-byte unsigned integer to the current stream and advances
        //     the stream position by eight bytes.
        //
        // Parameters:
        //   value:
        //     The eight-byte unsigned integer to write.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ObjectDisposedException:
        //     The stream is closed.
        public void Write(ulong value)
        {
#if WINDOWS
            writeString(value.ToString(CultureInfo.GetCultureInfo("en-US").NumberFormat));
#else
            writeString(value.ToString());
#endif
        }
        //
        // Summary:
        //     Writes a two-byte unsigned integer to the current stream and advances the
        //     stream position by two bytes.
        //
        // Parameters:
        //   value:
        //     The two-byte unsigned integer to write.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ObjectDisposedException:
        //     The stream is closed.
        public void Write(ushort value)
        {
#if WINDOWS
            writeString(value.ToString(CultureInfo.GetCultureInfo("en-US").NumberFormat));
#else
            writeString(value.ToString());
#endif
        }
        ////
        //// Summary:
        ////     Writes a region of a byte array to the current stream.
        ////
        //// Parameters:
        ////   buffer:
        ////     A byte array containing the data to write.
        ////
        ////   index:
        ////     The starting point in buffer at which to begin writing.
        ////
        ////   count:
        ////     The number of bytes to write.
        ////
        //// Exceptions:
        ////   System.ArgumentException:
        ////     The buffer length minus index is less than count.
        ////
        ////   System.ArgumentNullException:
        ////     buffer is null.
        ////
        ////   System.ArgumentOutOfRangeException:
        ////     index or count is negative.
        ////
        ////   System.IO.IOException:
        ////     An I/O error occurs.
        ////
        ////   System.ObjectDisposedException:
        ////     The stream is closed.
        //public void Write(byte[] buffer, int index, int count)
        //{
        //    m_output.Write(buffer, index, count);
        //}
        ////
        //// Summary:
        ////     Writes a section of a character array to the current stream, and advances
        ////     the current position of the stream in accordance with the Encoding used and
        ////     perhaps the specific characters being written to the stream.
        ////
        //// Parameters:
        ////   chars:
        ////     A character array containing the data to write.
        ////
        ////   index:
        ////     The starting point in buffer from which to begin writing.
        ////
        ////   count:
        ////     The number of characters to write.
        ////
        //// Exceptions:
        ////   System.ArgumentException:
        ////     The buffer length minus index is less than count.
        ////
        ////   System.ArgumentNullException:
        ////     chars is null.
        ////
        ////   System.ArgumentOutOfRangeException:
        ////     index or count is negative.
        ////
        ////   System.IO.IOException:
        ////     An I/O error occurs.
        ////
        ////   System.ObjectDisposedException:
        ////     The stream is closed.
        //public void Write(char[] chars, int index, int count)
        //{
        //    m_output.Write(System.BitConverter.GetBytes(c), index, count);
        //}
    }
}
