/******************************************************************************************************************************

Copyright (c) 2018-2019 InterlockLedger Network
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this
  list of conditions and the following disclaimer.

* Redistributions in binary form must reproduce the above copyright notice,
  this list of conditions and the following disclaimer in the documentation
  and/or other materials provided with the distribution.

* Neither the name of the copyright holder nor the names of its
  contributors may be used to endorse or promote products derived from
  this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

******************************************************************************************************************************/

using System;
using System.Buffers;
using System.IO;

namespace InterlockLedger.Tags
{
    /// <summary>Extension and helper methods to deal with ILInt encoding/decoding.</summary>
    public static class ILIntHelpers
    {
        public const int ILINT_BASE = 0xF8;
        public const ulong ILINT_MAX = ulong.MaxValue - ILINT_BASE;

        /// <summary>
        ///   <para>Encodes value to an array of bytes using ILInt encoding.</para>
        ///   <para>
        ///     <em>It is basically an alias to one of the ILIntEncode overloads.</em>
        ///   </para>
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>ILInt encoded value as an array of bytes.</returns>
        public static byte[] AsILInt(this ulong value) => value.ILIntEncode();

        /// <summary>Decode ILInt from buffer bytes.</summary>
        /// <param name="buffer">The buffer.</param>
        /// <returns>Decoded ILInt value.</returns>
        public static ulong ILIntDecode(this byte[] buffer) => ILIntDecode(buffer, 0, buffer.Length);

        /// <summary>Decode ILInt from a range of bytes from the buffer.</summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="index">The index of the first byte to use.</param>
        /// <param name="count">The maximum count of bytes that can be consumed.</param>
        /// <returns>Decoded ILInt value.</returns>
        public static ulong ILIntDecode(this byte[] buffer, int index, int count) => ILIntDecode(new MemoryStream(buffer, index, count));

        /// <summary>Decode an ILInt taking bytes from the stream.</summary>
        /// <param name="stream">The stream.</param>
        /// <returns>Decoded ILInt value.</returns>
        public static ulong ILIntDecode(this Stream stream) => ILIntDecode(() => stream.ReadSingleByte());

        /// <summary>Decodes an ILInt from bytes provided by a functor.</summary>
        /// <param name="readByte">The byte reading functor.</param>
        /// <returns>Decoded ILInt value.</returns>
        public static ulong ILIntDecode(Func<byte> readByte) {
            ulong value = 0;
            var nextByte = readByte();
            if (nextByte < ILINT_BASE)
                return nextByte;
            var size = nextByte - ILINT_BASE + 1;
            while (size-- > 0)
                value = (value << 8) + readByte();
            return value > ILINT_MAX ? 0 : value + ILINT_BASE;
        }

        /// <summary>Encode the value as an ILInt in the provided buffer, in the specified range.</summary>
        /// <param name="value">The value.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset to start storing bytes in the buffer.</param>
        /// <param name="count">The maximum count of bytes that can be stored, it may not be enough in which case it throws.</param>
        /// <returns>The provided buffer.</returns>
        /// <exception cref="TooFewBytesException"></exception>
        public static byte[] ILIntEncode(this ulong value, byte[] buffer, int offset, int count) {
            ILIntEncode(value, b => {
                if (--count < 0)
                    throw new TooFewBytesException();
                buffer[offset++] = b;
            });
            return buffer;
        }

        /// <summary>Encode the value as an ILInt, outputting the bytes to the stream.</summary>
        /// <param name="stream">The stream to receive encoded byte;</param>
        /// <param name="value">The value.</param>
        /// <returns>The provided stream to allow call chaining.</returns>
        public static Stream ILIntEncode(this Stream stream, ulong value) {
            value.ILIntEncode(stream.WriteByte);
            return stream;
        }

        /// <summary>Encode the value as an ILInt, output as an array of bytes.</summary>
        /// <param name="value">The value.</param>
        /// <returns>ILInt encoding of the value as an array of bytes.</returns>
        public static byte[] ILIntEncode(this ulong value) {
            var size = ILIntSize(value);
            return value.ILIntEncode(new byte[size], 0, size);
        }

        /// <summary>Encode a value as an ILInt, providing the byte to an action.</summary>
        /// <param name="value">The value to encode.</param>
        /// <param name="writeByte">  The action that will be called for each byte generated while encoding the value.</param>
        /// <exception cref="ArgumentNullException">writeByte</exception>
        public static void ILIntEncode(this ulong value, Action<byte> writeByte) {
            if (writeByte == null) {
                throw new ArgumentNullException(nameof(writeByte));
            }

            var size = ILIntSize(value);
            if (size == 1) {
                writeByte((byte)(value & 0xFF));
            } else {
                writeByte((byte)(ILINT_BASE + (size - 2)));
                value -= ILINT_BASE;
                for (var i = ((size - 2) * 8); i >= 0; i -= 8) {
                    writeByte((byte)((value >> i) & 0xFF));
                }
            }
        }

        /// <summary>Calculate the size in bytes the value will have when encoded as an ILInt.</summary>
        /// <param name="value">The value to measure.</param>
        /// <returns>Size in bytes the <strong>value</strong> will have when encoded as an ILInt.</returns>
        public static int ILIntSize(this ulong value) {
            if (value < ILINT_BASE)
                return 1;
            if (value <= (0xFF + ILINT_BASE))
                return 2;
            if (value <= (0xFFFF + ILINT_BASE))
                return 3;
            if (value <= (0xFFFFFF + ILINT_BASE))
                return 4;
            if (value <= (0xFFFFFFFFL + ILINT_BASE))
                return 5;
            if (value <= (0xFF_FFFF_FFFF + ILINT_BASE))
                return 6;
            if (value <= (0xFFFF_FFFF_FFFFL + ILINT_BASE))
                return 7;
            return value <= ((ulong)0xFF_FFFF_FFFF_FFFFL + ILINT_BASE) ? 8 : 9;
        }
    }
}
