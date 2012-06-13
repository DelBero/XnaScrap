using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using System.Runtime;
using System.IO;

namespace XnaScrapCore.Core
{
    public interface IDataReader
    {
        //
        // Summary:
        //     Reads a Boolean value from the current stream and advances the current position
        //     of the stream by one byte.
        //
        // Returns:
        //     true if the byte is nonzero; otherwise, false.
        //
        // Exceptions:
        //   System.IO.EndOfStreamException:
        //     The end of the stream is reached.
        //
        //   System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
         bool ReadBoolean();
        //
        // Summary:
        //     Reads the next byte from the current stream and advances the current position
        //     of the stream by one byte.
        //
        // Returns:
        //     The next byte read from the current stream.
        //
        // Exceptions:
        //   System.IO.EndOfStreamException:
        //     The end of the stream is reached.
        //
        //   System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
         byte ReadByte();
        //
        // Summary:
        //     Reads the specified number of bytes from the current stream into a byte array
        //     and advances the current position by that number of bytes.
        //
        // Parameters:
        //   count:
        //     The number of bytes to read.
        //
        // Returns:
        //     A byte array containing data read from the underlying stream. This might
        //     be less than the number of bytes requested if the end of the stream is reached.
        //
        // Exceptions:
        //   System.ArgumentException:
        //     The number of decoded characters to read is greater than count. This can
        //     happen if a Unicode decoder returns fallback characters or a surrogate pair.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   System.ArgumentOutOfRangeException:
        //     count is negative.
         byte[] ReadBytes(int count);
        //
        // Summary:
        //     Reads the next character from the current stream and advances the current
        //     position of the stream in accordance with the Encoding used and the specific
        //     character being read from the stream.
        //
        // Returns:
        //     A character read from the current stream.
        //
        // Exceptions:
        //   System.IO.EndOfStreamException:
        //     The end of the stream is reached.
        //
        //   System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ArgumentException:
        //     A surrogate character was read.
         char ReadChar();
        //
        // Summary:
        //     Reads the specified number of characters from the current stream, returns
        //     the data in a character array, and advances the current position in accordance
        //     with the Encoding used and the specific character being read from the stream.
        //
        // Parameters:
        //   count:
        //     The number of characters to read.
        //
        // Returns:
        //     A character array containing data read from the underlying stream. This might
        //     be less than the number of characters requested if the end of the stream
        //     is reached.
        //
        // Exceptions:
        //   System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ArgumentOutOfRangeException:
        //     count is negative.
         char[] ReadChars(int count);
        //
        // Summary:
        //     Reads a decimal value from the current stream and advances the current position
        //     of the stream by sixteen bytes.
        //
        // Returns:
        //     A decimal value read from the current stream.
        //
        // Exceptions:
        //   System.IO.EndOfStreamException:
        //     The end of the stream is reached.
        //
        //   System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
         //decimal ReadDecimal();
        //
        // Summary:
        //     Reads an 8-byte floating point value from the current stream and advances
        //     the current position of the stream by eight bytes.
        //
        // Returns:
        //     An 8-byte floating point value read from the current stream.
        //
        // Exceptions:
        //   System.IO.EndOfStreamException:
        //     The end of the stream is reached.
        //
        //   System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
         double ReadDouble();
        //
        // Summary:
        //     Reads a 2-byte signed integer from the current stream and advances the current
        //     position of the stream by two bytes.
        //
        // Returns:
        //     A 2-byte signed integer read from the current stream.
        //
        // Exceptions:
        //   System.IO.EndOfStreamException:
        //     The end of the stream is reached.
        //
        //   System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
         short ReadInt16();
        //
        // Summary:
        //     Reads a 4-byte signed integer from the current stream and advances the current
        //     position of the stream by four bytes.
        //
        // Returns:
        //     A 4-byte signed integer read from the current stream.
        //
        // Exceptions:
        //   System.IO.EndOfStreamException:
        //     The end of the stream is reached.
        //
        //   System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
         int ReadInt32();
        //
        // Summary:
        //     Reads an 8-byte signed integer from the current stream and advances the current
        //     position of the stream by eight bytes.
        //
        // Returns:
        //     An 8-byte signed integer read from the current stream.
        //
        // Exceptions:
        //   System.IO.EndOfStreamException:
        //     The end of the stream is reached.
        //
        //   System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
         long ReadInt64();
        //
        // Summary:
        //     Reads a signed byte from this stream and advances the current position of
        //     the stream by one byte.
        //
        // Returns:
        //     A signed byte read from the current stream.
        //
        // Exceptions:
        //   System.IO.EndOfStreamException:
        //     The end of the stream is reached.
        //
        //   System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
         sbyte ReadSByte();
        //
        // Summary:
        //     Reads a 4-byte floating point value from the current stream and advances
        //     the current position of the stream by four bytes.
        //
        // Returns:
        //     A 4-byte floating point value read from the current stream.
        //
        // Exceptions:
        //   System.IO.EndOfStreamException:
        //     The end of the stream is reached.
        //
        //   System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
         float ReadSingle();
        //
        // Summary:
        //     Reads a string from the current stream. The string is prefixed with the length,
        //     encoded as an integer seven bits at a time.
        //
        // Returns:
        //     The string being read.
        //
        // Exceptions:
        //   System.IO.EndOfStreamException:
        //     The end of the stream is reached.
        //
        //   System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
         string ReadString();
        //
        // Summary:
        //     Reads a 2-byte unsigned integer from the current stream using little-endian
        //     encoding and advances the position of the stream by two bytes.
        //
        // Returns:
        //     A 2-byte unsigned integer read from this stream.
        //
        // Exceptions:
        //   System.IO.EndOfStreamException:
        //     The end of the stream is reached.
        //
        //   System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
         ushort ReadUInt16();
        //
        // Summary:
        //     Reads a 4-byte unsigned integer from the current stream and advances the
        //     position of the stream by four bytes.
        //
        // Returns:
        //     A 4-byte unsigned integer read from this stream.
        //
        // Exceptions:
        //   System.IO.EndOfStreamException:
        //     The end of the stream is reached.
        //
        //   System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
         uint ReadUInt32();
        //
        // Summary:
        //     Reads an 8-byte unsigned integer from the current stream and advances the
        //     position of the stream by eight bytes.
        //
        // Returns:
        //     An 8-byte unsigned integer read from this stream.
        //
        // Exceptions:
        //   System.IO.EndOfStreamException:
        //     The end of the stream is reached.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ObjectDisposedException:
        //     The stream is closed.
         ulong ReadUInt64();

         Stream BaseStream { get; }
    }
}
