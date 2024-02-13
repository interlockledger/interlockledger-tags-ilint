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

using NUnit.Framework;

namespace InterlockLedger.Tags;

[TestFixture]
public class ILIntReaderTests
{
    [Test]
    public void Test0() => Assert.That(ILD([0]), Is.EqualTo(0ul), "ILIntDecodeFromByteArray 0");

    [Test]
    public void Test1() => Assert.That(ILD([1]), Is.EqualTo(1ul), "ILIntDecodeFromByteArray 1");

    [Test]
    public void Test1103823438329() => Assert.That(ILD([0xFD, 1, 1, 1, 1, 1, 1]), Is.EqualTo(1103823438329ul), "ILIntDecodeFromByteArray 1103823438329");

    [Test]
    public void Test128() => Assert.That(ILD([0x80]), Is.EqualTo(128ul), "ILIntDecodeFromByteArray 128");

    [Test]
    public void Test16843257() => Assert.That(ILD([0xFB, 1, 1, 1, 1]), Is.EqualTo(16843257ul), "ILIntDecodeFromByteArray 16843257");

    [Test]
    public void Test18446744073709551615() => Assert.That(ILD([0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 7]), Is.EqualTo(18446744073709551615ul), "ILIntDecodeFromByteArray 18446744073709551615");

    [Test]
    public void Test247() => Assert.That(ILD([0xF7]), Is.EqualTo(247ul), "ILIntDecodeFromByteArray 247");

    [Test]
    public void Test248() => Assert.That(ILD([0xF8, 0]), Is.EqualTo(248ul), "ILIntDecodeFromByteArray 248");

    [Test]
    public void Test249() => Assert.That(ILD([0xF8, 1]), Is.EqualTo(249ul), "ILIntDecodeFromByteArray 249");

    [Test]
    public void Test282578800148985() => Assert.That(ILD([0xFE, 1, 1, 1, 1, 1, 1, 1]), Is.EqualTo(282578800148985ul), "ILIntDecodeFromByteArray 282578800148985");

    [Test]
    public void Test4311810553() => Assert.That(ILD([0xFC, 1, 1, 1, 1, 1]), Is.EqualTo(4311810553ul), "ILIntDecodeFromByteArray 4311810553");

    [Test]
    public void Test503() => Assert.That(ILD([0xF8, 0xFF]), Is.EqualTo(503ul), "ILIntDecodeFromByteArray 503");

    [Test]
    public void Test505() => Assert.That(ILD([0xF9, 1, 1]), Is.EqualTo(505ul), "ILIntDecodeFromByteArray 505");

    [Test]
    public void Test66041() => Assert.That(ILD([0xFA, 1, 1, 1]), Is.EqualTo(66041ul), "ILIntDecodeFromByteArray 66041");

    [Test]
    public void Test72057594037928183() => Assert.That(ILD([0xFE, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF]), Is.EqualTo(72057594037928183ul), "ILIntDecodeFromByteArray 72057594037928183");

    [Test]
    public void Test72340172838076921() => Assert.That(ILD([0xFF, 1, 1, 1, 1, 1, 1, 1, 1]), Is.EqualTo(72340172838076921ul), "ILIntDecodeFromByteArray 72340172838076921");

    [Test]
    public void TestTooLargeBy1() => Assert.Throws<InvalidOperationException>(() => ILD([0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 8]), "ILIntDecodeFromByteArray too large by 1 => zero");

    [Test]
    public void TestTooLargeBy120() => Assert.Throws<InvalidOperationException>(() => ILD([0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x80]), "ILIntDecodeFromByteArray too large by 120 => zero");

    [Test]
    public void TestTooLargeByAll() => Assert.Throws<InvalidOperationException>(() => ILD([0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF]), "ILIntDecodeFromByteArray too large by all => zero");

    [Test]
    public void TryDecodeIEnum() {
        static (long consumed, bool ok, ulong? value) DoTry(params byte[] bytes) {
            var reader = new ILIntReader();
            Assert.That(reader.Ready, Is.False, "Should not be read before being supplied bytes");
            var ok = reader.TryDecode((IEnumerable<byte>)bytes, out var consumed);
            Assert.That(reader.Ready, Is.EqualTo(ok), "Should be ready when done consuming bytes");
            return (consumed, ok, ok ? reader.Value : (ulong?)null);
        }
        static void DoTryExpecting(bool okExpected, long consumedExpected, ulong? valueExpected, params byte[] bytes) {
            var (consumed, ok, value) = DoTry(bytes);
            Assert.Multiple(() => {
                Assert.That(ok, Is.EqualTo(okExpected));
                Assert.That(consumed, Is.EqualTo(consumedExpected));
                Assert.That(value, Is.EqualTo(valueExpected));
            });
        }
        DoTryExpecting(false, 0L, null);
        DoTryExpecting(true, 1L, 0UL, 0);
        DoTryExpecting(true, 1L, 0UL, 0, 1);
        DoTryExpecting(true, 2L, 249UL, 0xF8, 1);
        DoTryExpecting(true, 2L, 249UL, 0xF8, 1, 2, 3);
    }

    [Test]
    public void TryDecodeReadOnlySequence() {
        static (long consumed, bool ok, ulong? value) DoTry(int offset, int tail, params byte[] bytes) {
            var reader = new ILIntReader();
            Assert.That(reader.Ready, Is.False, "Should not be read before being supplied bytes");
            var newBytes = new byte[bytes.Length + offset + tail];
            Array.Copy(bytes, 0, newBytes, offset, bytes.Length);
            var sequence = new ReadOnlySequence<byte>(newBytes, offset, bytes.Length);
            var ok = reader.TryDecode(sequence, out var consumed);
            Assert.That(reader.Ready, Is.EqualTo(ok), "Should be ready when done consuming bytes");
            return (consumed, ok, ok ? reader.Value : (ulong?)null);
        }
        static void DoTryExpecting(bool okExpected, long consumedExpected, ulong? valueExpected, int offset, int tail, params byte[] bytes) {
            var (consumed, ok, value) = DoTry(offset, tail, bytes);
            Assert.Multiple(() => {
                Assert.That(ok, Is.EqualTo(okExpected));
                Assert.That(value, Is.EqualTo(valueExpected));
                Assert.That(consumed, Is.EqualTo(consumedExpected));
            });
        }
        for (int i = 0; i < 32; i++) {
            for (int j = 0; j < 32; j++) {
                DoTryExpecting(false, 0L, null, i, j);
                DoTryExpecting(true, 1L, 0UL, i, j, 0);
                DoTryExpecting(true, 1L, 0UL, i, j, 0, 1);
                DoTryExpecting(true, 2L, 249UL, i, j, 0xF8, 1);
                DoTryExpecting(true, 2L, 249UL, i, j, 0xF8, 1, 2, 3);
            }
        }
    }

    [Test]
    public void TryDecodeReadOnlySpan() {
        static (long consumed, bool ok, ulong? value) DoTry(int offset, int tail, params byte[] bytes) {
            var reader = new ILIntReader();
            Assert.That(reader.Ready, Is.False, "Should not be read before being supplied bytes");
            var newBytes = new byte[bytes.Length + offset + tail];
            Array.Copy(bytes, 0, newBytes, offset, bytes.Length);
            var span = new ReadOnlySpan<byte>(newBytes, offset, bytes.Length);
            var ok = reader.TryDecode(span, out var consumed);
            Assert.That(reader.Ready, Is.EqualTo(ok), "Should be ready when done consuming bytes");
            return (consumed, ok, ok ? reader.Value : (ulong?)null);
        }
        static void DoTryExpecting(bool okExpected, long consumedExpected, ulong? valueExpected, int offset, int tail, params byte[] bytes) {
            var (consumed, ok, value) = DoTry(offset, tail, bytes);
            Assert.Multiple(() => {
                Assert.That(ok, Is.EqualTo(okExpected));
                Assert.That(consumed, Is.EqualTo(consumedExpected));
                Assert.That(value, Is.EqualTo(valueExpected));
            });
        }
        for (int i = 0; i < 32; i++) {
            for (int j = 0; j < 32; j++) {
                DoTryExpecting(false, 0L, null, i, j);
                DoTryExpecting(true, 1L, 0UL, i, j, 0);
                DoTryExpecting(true, 1L, 0UL, i, j, 0, 1);
                DoTryExpecting(true, 2L, 249UL, i, j, 0xF8, 1);
                DoTryExpecting(true, 2L, 249UL, i, j, 0xF8, 1, 2, 3);
            }
        }
    }

    private static ulong ILD(byte[] bytes) {
        var reader = new ILIntReader();
        for (int k = 0; k < 2; k++) {
            if (k > 0) reader.Reset();
            Assert.That(reader.Ready, Is.False, "Should not be read before being supplied bytes");
            for (int i = 0; i < bytes.Length; i++) {
                var done = reader.Done(bytes[i]);
                if (i + 1 < bytes.Length && done)
                    Assert.Fail("Value decoded without all bytes");
                if (i + 1 == bytes.Length && !done)
                    Assert.Fail("Value not decoded with supplied bytes");
            }
            Assert.That(reader.Ready, "Should be ready when done consuming bytes");
        }
        return reader.Value;
    }
}