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

using InterlockLedger.Tags;

namespace InterlockLedger;

public static class StreamExtensions
{
    /// <summary>Decode an ILInt taking bytes from the stream.</summary>
    /// <param name="stream">The stream.</param>
    /// <returns>Decoded ILInt value.</returns>
    public static ulong ILIntDecode(this Stream stream) {
        stream.Required(nameof(stream));
        return ILIntHelpers.ILIntDecode(() => stream.ReadSingleByte());
    }

    /// <summary>Encode the value as an ILInt, outputting the bytes to the stream.</summary>
    /// <param name="stream">The stream to receive encoded bytes</param>
    /// <param name="value">The value.</param>
    /// <returns>The provided stream to allow call chaining.</returns>
    public static Stream ILIntEncode(this Stream stream, ulong value) {
        value.ILIntEncode(stream.Required(nameof(stream)).WriteByte);
        return stream;
    }

    /// <summary>Reads a single byte from the streams trying up to 3 times.</summary>
    /// <param name="s">The stream to read from.</param>
    /// <returns>Read byte.</returns>
    /// <exception cref="TooFewBytesException">After 3 tries no byte could be read.</exception>
    public static byte ReadSingleByte(this Stream s) {
        s.Required(nameof(s));
        var bytes = new byte[1];
        var retries = 3;
        while (retries-- > 0) {
            if (s.Read(bytes, 0, 1) == 1)
                return bytes[0];
            Thread.Sleep(100);
        }
        throw new TooFewBytesException();
    }
}