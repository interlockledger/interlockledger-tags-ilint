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

using NUnit.Framework;

namespace InterlockLedger.Tags;

[TestFixture]
public class ByteArrayExtensionsTests
{
    [TestCase(new byte[] { 0 }, ExpectedResult = 0ul, TestName = "ILIntDecodeFromByteArray 0")]
    [TestCase(new byte[] { 1 }, ExpectedResult = 1ul, TestName = "ILIntDecodeFromByteArray 1")]
    [TestCase(new byte[] { 0x80 }, ExpectedResult = 128ul, TestName = "ILIntDecodeFromByteArray 128")]
    [TestCase(new byte[] { 0xF7 }, ExpectedResult = 247ul, TestName = "ILIntDecodeFromByteArray 247")]
    [TestCase(new byte[] { 0xF8, 0 }, ExpectedResult = 248ul, TestName = "ILIntDecodeFromByteArray 248")]
    [TestCase(new byte[] { 0xF8, 1 }, ExpectedResult = 249ul, TestName = "ILIntDecodeFromByteArray 249")]
    [TestCase(new byte[] { 0xF8, 0xFF }, ExpectedResult = 503ul, TestName = "ILIntDecodeFromByteArray 503")]
    [TestCase(new byte[] { 0xF9, 1, 1 }, ExpectedResult = 505ul, TestName = "ILIntDecodeFromByteArray 505")]
    [TestCase(new byte[] { 0xFA, 1, 1, 1 }, ExpectedResult = 66041ul, TestName = "ILIntDecodeFromByteArray 66041")]
    [TestCase(new byte[] { 0xFB, 1, 1, 1, 1 }, ExpectedResult = 16843257ul, TestName = "ILIntDecodeFromByteArray 16843257")]
    [TestCase(new byte[] { 0xFC, 1, 1, 1, 1, 1 }, ExpectedResult = 4311810553ul, TestName = "ILIntDecodeFromByteArray 4311810553")]
    [TestCase(new byte[] { 0xFD, 1, 1, 1, 1, 1, 1 }, ExpectedResult = 1103823438329ul, TestName = "ILIntDecodeFromByteArray 1103823438329")]
    [TestCase(new byte[] { 0xFE, 1, 1, 1, 1, 1, 1, 1 }, ExpectedResult = 282578800148985ul, TestName = "ILIntDecodeFromByteArray 282578800148985")]
    [TestCase(new byte[] { 0xFE, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, ExpectedResult = 72057594037928183ul, TestName = "ILIntDecodeFromByteArray 72057594037928183")]
    [TestCase(new byte[] { 0xFF, 1, 1, 1, 1, 1, 1, 1, 1 }, ExpectedResult = 72340172838076921ul, TestName = "ILIntDecodeFromByteArray 72340172838076921")]
    [TestCase(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 7 }, ExpectedResult = 18446744073709551615ul, TestName = "ILIntDecodeFromByteArray 18446744073709551615")]
    [TestCase(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 8 }, ExpectedResult = 0ul, TestName = "ILIntDecodeFromByteArray too large by 1 => zero")]
    [TestCase(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x80 }, ExpectedResult = 0ul, TestName = "ILIntDecodeFromByteArray too large by 120 => zero")]
    [TestCase(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, ExpectedResult = 0ul, TestName = "ILIntDecodeFromByteArray too large by all => zero")]
    public ulong ILIntDecodeFromByteArray(byte[] bytes) => bytes.ILIntDecode();

    [Test]
    public void ILIntDecodeFromTooShortByteArrayThrowsTooFewBytesException() {
        Assert.Throws<TooFewBytesException>(() => new byte[] { 0xF8 }.ILIntDecode());
        Assert.Throws<TooFewBytesException>(() => new byte[] { 0xF9, 0 }.ILIntDecode());
        Assert.Throws<TooFewBytesException>(() => new byte[] { 0xFA, 0, 0 }.ILIntDecode());
        Assert.Throws<TooFewBytesException>(() => new byte[] { 0xFB, 0, 0, 0 }.ILIntDecode());
        Assert.Throws<TooFewBytesException>(() => new byte[] { 0xFC, 0, 0, 0, 0 }.ILIntDecode());
        Assert.Throws<TooFewBytesException>(() => new byte[] { 0xFD, 0, 0, 0, 0, 0 }.ILIntDecode());
        Assert.Throws<TooFewBytesException>(() => new byte[] { 0xFE, 0, 0, 0, 0, 0, 0 }.ILIntDecode());
        Assert.Throws<TooFewBytesException>(() => new byte[] { 0xFF, 0, 0, 0, 0, 0, 0, 0 }.ILIntDecode());
    }
}