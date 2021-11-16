// ******************************************************************************************************************************
//
// Copyright (c) 2018-2021 InterlockLedger Network
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met
//
// * Redistributions of source code must retain the above copyright notice, this
//   list of conditions and the following disclaimer.
//
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution.
//
// * Neither the name of the copyright holder nor the names of its
//   contributors may be used to endorse or promote products derived from
//   this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES, LOSS OF USE, DATA, OR PROFITS, OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
// ******************************************************************************************************************************

using static InterlockLedger.Tags.ILIntHelpers;

namespace InterlockLedger.Tags;

/// <summary>Extension and helper methods to deal with ILInt encoding/decoding.</summary>
public static class UlongExtensions
{
    /// <summary>
    ///   <para>Encodes value to an array of bytes using ILInt encoding.</para>
    ///   <para>
    ///     <em>It is basically an alias to one of the ILIntEncode overloads.</em>
    ///   </para>
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>ILInt encoded value as an array of bytes.</returns>
    public static byte[] AsILInt(this ulong value) => value.ILIntEncode();

    /*
     * To overcome this limitation, the ILInt standard defines a signed value transformation, called ILIntSignEnc and its reverse ILIntSignDec. The ideia behind this transformation is to encode the negative numbers in a way that minimize the number of msb set and thus use less space when encoded using ILInt.

ILIntSignEnc is defined as follows:

Convert the signed 64-bit value into an unsigned v value using two's complement;
If the sign bit (bit 63) of v is set, let e be not (v shl 1);
If the sign bit (bit 63) of v is not set, let e be (v shl 1);
Return e;
where shl is a bitwise shift lef operation and not is a bitwise not.

ILIntSignDec is defined as follows:

If bit 0 of e is set, let d be e shr 1;
If bit 1 of e is set, let d be not (e shr 1);
Let r be e converted to a 64-bit signed value using two's complement;
Return r;
where shr is an unsigned shift right operation and not is a bitwise not.

Using this encoding, the sign bit is moved to the least significant bit and the value is always converted to a small value if its absolute value is small.
     */

    public static long AsSignedILInt(this ulong value) {
        unchecked {
            var d = (long)(value >> 1);
            return (value & 1) == 0 ? d : ~d;
        }
    }

    public static ulong AsUnsignedILInt(this long value) {
        unchecked {
            var s = (ulong)(value << 1);
            return ((ulong)value & 0x8000000000000000ul) != 0 ? ~s : s;
        }
    }

    /// <summary>Encode the value as an ILInt in the provided buffer, in the specified range.</summary>
    /// <param name="value">The value.</param>
    /// <param name="buffer">The buffer.</param>
    /// <param name="offset">The offset to start storing bytes in the buffer.</param>
    /// <param name="count">The maximum count of bytes that can be stored, it may not be enough in which case it throws.</param>
    /// <returns>The provided buffer.</returns>
    /// <exception cref="TooFewBytesException"></exception>
    public static byte[] ILIntEncode(this ulong value, byte[] buffer, int offset, int count) {
        ByteArrayExtensions.CheckBuffer(buffer, offset, count);
        ILIntEncode(value, b => {
            if (--count < 0)
                throw new TooFewBytesException();
            buffer[offset++] = b;
        });
        return buffer;
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
        writeByte.Required(nameof(writeByte));
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
    public static int ILIntSize(this ulong value)
        => value < ILINT_BASE ? 1
        : value <= (0xFF + ILINT_BASE) ? 2
        : value <= (0xFFFF + ILINT_BASE) ? 3
        : value <= (0xFFFFFF + ILINT_BASE) ? 4
        : value <= (0xFFFFFFFFL + ILINT_BASE) ? 5
        : value <= (0xFF_FFFF_FFFF + ILINT_BASE) ? 6
        : value <= (0xFFFF_FFFF_FFFFL + ILINT_BASE) ? 7
        : value <= ((ulong)0xFF_FFFF_FFFF_FFFFL + ILINT_BASE) ? 8 : 9;
}
