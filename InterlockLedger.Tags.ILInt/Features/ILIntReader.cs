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

using System.Buffers;

namespace InterlockLedger.Tags;

/// <summary>
///   <para>Class to decode an ILInt from its bytes into a ulong value. </para>
///   <blockquote style="margin-right: 10px;" dir="ltr">
///     <para><strong>Stateful:</strong> can't be shared in multiple lines of execution.</para>
///     <para><em>One instance can be reused to decode many ILInt values, sequentially.</em></para>
///   </blockquote>
/// </summary>
public class ILIntReader
{
    /// <summary>Initializes a new instance of the <see cref="ILIntReader"/> class.</summary>
    public ILIntReader() => Reset();

    /// <summary>Gets if a completely decoded ILInt value is available. </summary>
    public bool Ready => _size == 0;

    /// <summary>Gets the decoded ILInt value.</summary>
    /// <value>The decoded value after processing all needed bytes.</value>
    /// <exception cref="InvalidOperationException">ILInt still not completely read</exception>
    public ulong Value => _size == 0 ? _value : throw new InvalidOperationException("ILInt still not completely read");

    /// <summary>Process the specified byte.</summary>
    /// <param name="nextByte">Next byte to be processed.</param>
    /// <returns>True when no more bytes need to be provided</returns>
    /// <exception cref="InvalidOperationException">Decoded ILInt value is too large</exception>
    public bool Done(byte nextByte) {
        if (_size < 0) {
            if (nextByte < ILIntHelpers.ILINT_BASE) {
                _value = nextByte;
                _size = 0;
                return true;
            }
            _size = nextByte - ILIntHelpers.ILINT_BASE + 1;
            return false;
        }
        _value = (_value << 8) + nextByte;
        if (_value > ILIntHelpers.ILINT_MAX)
            throw new InvalidOperationException("Decoded ILInt value is too large");
        if (--_size > 0)
            return false;
        _value += ILIntHelpers.ILINT_BASE;
        return true;
    }

    /// <summary>Resets this instance, to start decoding a new ILInt.</summary>
    public void Reset() {
        _size = -1;
        _value = 0;
    }

    public bool TryDecode(ReadOnlySpan<byte> bytes, out long consumed) {
        consumed = 0;
        foreach (var b in bytes) {
            consumed++;
            if (Done(b)) return true;
        }
        return false;
    }

    public bool TryDecode(IEnumerable<byte> bytes, out long consumed) {
        bytes.Required(nameof(bytes));
        consumed = 0;
        foreach (var b in bytes) {
            consumed++;
            if (Done(b)) return true;
        }
        return false;
    }

    public bool TryDecode(in ReadOnlySequence<byte> byteSequence, out long consumed) {
        consumed = 0;
        if (!byteSequence.IsEmpty) {
            foreach (var memory in byteSequence) {
                var result = TryDecode(memory.Span, out var consumedHere);
                consumed += consumedHere;
                if (result) return true;
            }
        }
        return false;
    }

    private int _size;
    private ulong _value;
}