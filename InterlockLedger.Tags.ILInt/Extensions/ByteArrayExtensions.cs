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

namespace InterlockLedger;

public static class ByteArrayExtensions
{
    /// <summary>Decode ILInt from buffer bytes.</summary>
    /// <param name="buffer">The buffer.</param>
    /// <returns>Decoded ILInt value.</returns>
    public static ulong ILIntDecode(this byte[] buffer)
        => buffer.Required(nameof(buffer)).ILIntDecode(0, buffer.Length);

    /// <summary>Decode ILInt from a range of bytes from the buffer.</summary>
    /// <param name="buffer">The buffer.</param>
    /// <param name="index">The index of the first byte to use.</param>
    /// <param name="count">The maximum count of bytes that can be consumed.</param>
    /// <returns>Decoded ILInt value.</returns>
    public static ulong ILIntDecode(this byte[] buffer, int index, int count) {
        CheckBuffer(buffer, index, count);
        return new MemoryStream(buffer, index, count).ILIntDecode();
    }

    internal static void CheckBuffer(byte[] buffer, int offset, int count) {
        buffer.Required(nameof(buffer));
        if (offset < 0 || offset >= buffer.Length)
            throw new ArgumentOutOfRangeException(nameof(offset));
        if (count < 1 || offset + count > buffer.Length)
            throw new ArgumentOutOfRangeException(nameof(count));
    }
}