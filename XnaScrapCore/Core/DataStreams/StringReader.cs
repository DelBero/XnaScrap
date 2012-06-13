using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace XnaScrapCore.Core
{
    //public class StringReader : BinaryReader, IDataReader
    //{
    //    private BinaryReader m_reader;
    //    public StringReader(Stream input)
    //    {
    //        m_reader = new BinaryReader(input);
    //    }

    //    private string readString()
    //    {
    //        return m_reader.ReadString();
    //    }

    //    public void Write(bool value)
    //    {
    //        string val = value ? "1" : "0";
    //        readString(val);
    //    }
    //    //
    //    // Summary:
    //    //     Writes an unsigned byte to the current stream and advances the stream position
    //    //     by one byte.
    //    //
    //    // Parameters:
    //    //   value:
    //    //     The unsigned byte to write.
    //    //
    //    // Exceptions:
    //    //   System.IO.IOException:
    //    //     An I/O error occurs.
    //    //
    //    //   System.ObjectDisposedException:
    //    //     The stream is closed.
    //    public void Write(byte value)
    //    {
    //        m_reader.Write(new byte[] { value }, 0, 1);
    //    }
    //    //
    //    // Summary:
    //    //     Writes a byte array to the underlying stream.
    //    //
    //    // Parameters:
    //    //   buffer:
    //    //     A byte array containing the data to write.
    //    //
    //    // Exceptions:
    //    //   System.IO.IOException:
    //    //     An I/O error occurs.
    //    //
    //    //   System.ObjectDisposedException:
    //    //     The stream is closed.
    //    //
    //    //   System.ArgumentNullException:
    //    //     buffer is null.
    //    public void Write(byte[] buffer)
    //    {
    //        m_reader.Write(buffer, 0, buffer.Length);
    //    }
    //    //
    //    // Summary:
    //    //     Writes a Unicode character to the current stream and advances the current
    //    //     position of the stream in accordance with the Encoding used and the specific
    //    //     characters being written to the stream.
    //    //
    //    // Parameters:
    //    //   ch:
    //    //     The non-surrogate, Unicode character to write.
    //    //
    //    // Exceptions:
    //    //   System.IO.IOException:
    //    //     An I/O error occurs.
    //    //
    //    //   System.ObjectDisposedException:
    //    //     The stream is closed.
    //    //
    //    //   System.ArgumentException:
    //    //     ch is a single surrogate character.
    //    public void Write(char ch)
    //    {
    //        m_reader.Write(System.BitConverter.GetBytes(ch), 0, 1);
    //    }
    //    //
    //    // Summary:
    //    //     Writes a character array to the current stream and advances the current position
    //    //     of the stream in accordance with the Encoding used and the specific characters
    //    //     being written to the stream.
    //    //
    //    // Parameters:
    //    //   chars:
    //    //     A character array containing the data to write.
    //    //
    //    // Exceptions:
    //    //   System.ArgumentNullException:
    //    //     chars is null.
    //    //
    //    //   System.ObjectDisposedException:
    //    //     The stream is closed.
    //    //
    //    //   System.IO.IOException:
    //    //     An I/O error occurs.
    //    public void Write(char[] chars)
    //    {
    //        readString(new String(chars));
    //    }
    //    //
    //    // Summary:
    //    //     Writes a decimal value to the current stream and advances the stream position
    //    //     by sixteen bytes.
    //    //
    //    // Parameters:
    //    //   value:
    //    //     The decimal value to write.
    //    //
    //    // Exceptions:
    //    //   System.IO.IOException:
    //    //     An I/O error occurs.
    //    //
    //    //   System.ObjectDisposedException:
    //    //     The stream is closed.
    //    public void Write(decimal value)
    //    {
    //        readString(value.ToString());
    //    }
    //    //
    //    // Summary:
    //    //     Writes an eight-byte floating-point value to the current stream and advances
    //    //     the stream position by eight bytes.
    //    //
    //    // Parameters:
    //    //   value:
    //    //     The eight-byte floating-point value to write.
    //    //
    //    // Exceptions:
    //    //   System.IO.IOException:
    //    //     An I/O error occurs.
    //    //
    //    //   System.ObjectDisposedException:
    //    //     The stream is closed.
    //    public void Write(double value)
    //    {
    //        readString(value.ToString());
    //    }
    //    //
    //    // Summary:
    //    //     Writes a four-byte floating-point value to the current stream and advances
    //    //     the stream position by four bytes.
    //    //
    //    // Parameters:
    //    //   value:
    //    //     The four-byte floating-point value to write.
    //    //
    //    // Exceptions:
    //    //   System.IO.IOException:
    //    //     An I/O error occurs.
    //    //
    //    //   System.ObjectDisposedException:
    //    //     The stream is closed.
    //    public void Write(float value)
    //    {
    //        readString(value.ToString());
    //    }
    //    //
    //    // Summary:
    //    //     Writes a four-byte signed integer to the current stream and advances the
    //    //     stream position by four bytes.
    //    //
    //    // Parameters:
    //    //   value:
    //    //     The four-byte signed integer to write.
    //    //
    //    // Exceptions:
    //    //   System.IO.IOException:
    //    //     An I/O error occurs.
    //    //
    //    //   System.ObjectDisposedException:
    //    //     The stream is closed.
    //    public void Write(int value)
    //    {
    //        readString(value.ToString());
    //    }
    //    //
    //    // Summary:
    //    //     Writes an eight-byte signed integer to the current stream and advances the
    //    //     stream position by eight bytes.
    //    //
    //    // Parameters:
    //    //   value:
    //    //     The eight-byte signed integer to write.
    //    //
    //    // Exceptions:
    //    //   System.IO.IOException:
    //    //     An I/O error occurs.
    //    //
    //    //   System.ObjectDisposedException:
    //    //     The stream is closed.
    //    public void Write(long value)
    //    {
    //        readString(value.ToString());
    //    }
    //    //
    //    // Summary:
    //    //     Writes a signed byte to the current stream and advances the stream position
    //    //     by one byte.
    //    //
    //    // Parameters:
    //    //   value:
    //    //     The signed byte to write.
    //    //
    //    // Exceptions:
    //    //   System.IO.IOException:
    //    //     An I/O error occurs.
    //    //
    //    //   System.ObjectDisposedException:
    //    //     The stream is closed.
    //    public void Write(sbyte value)
    //    {
    //        readString(value.ToString());
    //    }
    //    //
    //    // Summary:
    //    //     Writes a two-byte signed integer to the current stream and advances the stream
    //    //     position by two bytes.
    //    //
    //    // Parameters:
    //    //   value:
    //    //     The two-byte signed integer to write.
    //    //
    //    // Exceptions:
    //    //   System.IO.IOException:
    //    //     An I/O error occurs.
    //    //
    //    //   System.ObjectDisposedException:
    //    //     The stream is closed.
    //    public void Write(short value)
    //    {
    //        readString(value.ToString());
    //    }
    //    //
    //    // Summary:
    //    //     Writes a length-prefixed string to this stream in the current encoding of
    //    //     the System.IO.BinaryWriter, and advances the current position of the stream
    //    //     in accordance with the encoding used and the specific characters being written
    //    //     to the stream.
    //    //
    //    // Parameters:
    //    //   value:
    //    //     The value to write.
    //    //
    //    // Exceptions:
    //    //   System.IO.IOException:
    //    //     An I/O error occurs.
    //    //
    //    //   System.ArgumentNullException:
    //    //     value is null.
    //    //
    //    //   System.ObjectDisposedException:
    //    //     The stream is closed.
    //    public void Write(string value)
    //    {
    //        readString(value);
    //    }
    //    //
    //    // Summary:
    //    //     Writes a four-byte unsigned integer to the current stream and advances the
    //    //     stream position by four bytes.
    //    //
    //    // Parameters:
    //    //   value:
    //    //     The four-byte unsigned integer to write.
    //    //
    //    // Exceptions:
    //    //   System.IO.IOException:
    //    //     An I/O error occurs.
    //    //
    //    //   System.ObjectDisposedException:
    //    //     The stream is closed.
    //    public void Write(uint value)
    //    {
    //        readString(value.ToString());
    //    }
    //    //
    //    // Summary:
    //    //     Writes an eight-byte unsigned integer to the current stream and advances
    //    //     the stream position by eight bytes.
    //    //
    //    // Parameters:
    //    //   value:
    //    //     The eight-byte unsigned integer to write.
    //    //
    //    // Exceptions:
    //    //   System.IO.IOException:
    //    //     An I/O error occurs.
    //    //
    //    //   System.ObjectDisposedException:
    //    //     The stream is closed.
    //    public void Write(ulong value)
    //    {
    //        readString(value.ToString());
    //    }
    //    //
    //    // Summary:
    //    //     Writes a two-byte unsigned integer to the current stream and advances the
    //    //     stream position by two bytes.
    //    //
    //    // Parameters:
    //    //   value:
    //    //     The two-byte unsigned integer to write.
    //    //
    //    // Exceptions:
    //    //   System.IO.IOException:
    //    //     An I/O error occurs.
    //    //
    //    //   System.ObjectDisposedException:
    //    //     The stream is closed.
    //    public void Write(ushort value)
    //    {
    //        readString(value.ToString());
    //    }
    //}
}
