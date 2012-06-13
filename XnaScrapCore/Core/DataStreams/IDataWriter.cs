using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XnaScrapCore.Core
{
    public interface IDataWriter
    {
        //
        // Summary:
        //     Writes a one-byte Boolean value to the current stream, with 0 representing
        //     false and 1 representing true.
        //
        // Parameters:
        //   value:
        //     The Boolean value to write (0 or 1).
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ObjectDisposedException:
        //     The stream is closed.
        void Write(bool value);
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
         void Write(byte value);
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
         void Write(byte[] buffer);
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
         void Write(char ch);
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
         void Write(char[] chars);
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
         //void Write(decimal value);
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
         void Write(double value);
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
         void Write(float value);
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
         void Write(int value);
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
         void Write(long value);
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
         void Write(sbyte value);
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
         void Write(short value);
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
         void Write(string value);
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
         void Write(uint value);
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
         void Write(ulong value);
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
         void Write(ushort value);
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
        // void Write(byte[] buffer, int index, int count);
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
        // void Write(char[] chars, int index, int count);
    }
}
