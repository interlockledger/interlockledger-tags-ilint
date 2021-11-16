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
public class UlongExtensionsTests
{
    [TestCase((ulong)0, ExpectedResult = new byte[] { 0 })]
    [TestCase((ulong)1, ExpectedResult = new byte[] { 1 })]
    [TestCase((ulong)128, ExpectedResult = new byte[] { 0x80 })]
    [TestCase((ulong)247, ExpectedResult = new byte[] { 0xF7 })]
    [TestCase((ulong)248, ExpectedResult = new byte[] { 0xF8, 0 })]
    [TestCase((ulong)249, ExpectedResult = new byte[] { 0xF8, 1 })]
    [TestCase((ulong)503, ExpectedResult = new byte[] { 0xF8, 0xFF })]
    [TestCase((ulong)505, ExpectedResult = new byte[] { 0xF9, 1, 1 })]
    [TestCase((ulong)66041, ExpectedResult = new byte[] { 0xFA, 1, 1, 1 })]
    [TestCase((ulong)16843257, ExpectedResult = new byte[] { 0xFB, 1, 1, 1, 1 })]
    [TestCase((ulong)4311810553, ExpectedResult = new byte[] { 0xFC, 1, 1, 1, 1, 1 })]
    [TestCase((ulong)1103823438329, ExpectedResult = new byte[] { 0xFD, 1, 1, 1, 1, 1, 1 })]
    [TestCase((ulong)282578800148985, ExpectedResult = new byte[] { 0xFE, 1, 1, 1, 1, 1, 1, 1 })]
    [TestCase((ulong)72057594037928183, ExpectedResult = new byte[] { 0xFE, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF })]
    [TestCase((ulong)72340172838076921, ExpectedResult = new byte[] { 0xFF, 1, 1, 1, 1, 1, 1, 1, 1 })]
    [TestCase(18446744073709551615, ExpectedResult = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 7 })]
    public byte[] AsILIntToByteArray(ulong value) => value.AsILInt();

    [TestCase(0UL, ExpectedResult = 0L)]
    [TestCase(2UL, ExpectedResult = 1L)]
    [TestCase(0xFEUL, ExpectedResult = 127L)]
    [TestCase(1UL, ExpectedResult = -1L)]
    [TestCase(3UL, ExpectedResult = -2L)]
    [TestCase(0xFFUL, ExpectedResult = -128L)]
    public long AsSignedILInt(ulong value) => value.AsSignedILInt();

    [TestCase(0L, ExpectedResult = 0UL)]
    [TestCase(1L, ExpectedResult = 2UL)]
    [TestCase(127L, ExpectedResult = 0xFEUL)]
    [TestCase(-1L, ExpectedResult = 1UL)]
    [TestCase(-2L, ExpectedResult = 3UL)]
    [TestCase(-128L, ExpectedResult = 0xFFUL)]
    public ulong AsUnsignedILInt(long value) => value.AsUnsignedILInt();

    [TestCase((ulong)0, ExpectedResult = 1)]
    [TestCase((ulong)ILIntHelpers.ILINT_BASE - 1, ExpectedResult = 1)]
    [TestCase((ulong)ILIntHelpers.ILINT_BASE, ExpectedResult = 2)]
    [TestCase((ulong)ILIntHelpers.ILINT_BASE + 0xFE, ExpectedResult = 2)]
    [TestCase((ulong)ILIntHelpers.ILINT_BASE + 0xFF, ExpectedResult = 2)]
    [TestCase((ulong)ILIntHelpers.ILINT_BASE + 0xFF + 1, ExpectedResult = 3)]
    [TestCase((ulong)ILIntHelpers.ILINT_BASE + 0xFFFE, ExpectedResult = 3)]
    [TestCase((ulong)ILIntHelpers.ILINT_BASE + 0xFFFF, ExpectedResult = 3)]
    [TestCase((ulong)ILIntHelpers.ILINT_BASE + 0xFFFF + 1, ExpectedResult = 4)]
    [TestCase((ulong)ILIntHelpers.ILINT_BASE + 0xFFFFFE, ExpectedResult = 4)]
    [TestCase((ulong)ILIntHelpers.ILINT_BASE + 0xFFFFFF, ExpectedResult = 4)]
    [TestCase((ulong)ILIntHelpers.ILINT_BASE + 0xFFFFFF + 1, ExpectedResult = 5)]
    [TestCase((ulong)ILIntHelpers.ILINT_BASE + 0xFFFFFFFEL, ExpectedResult = 5)]
    [TestCase((ulong)ILIntHelpers.ILINT_BASE + 0xFFFFFFFFL, ExpectedResult = 5)]
    [TestCase((ulong)ILIntHelpers.ILINT_BASE + 0xFFFFFFFFL + 1, ExpectedResult = 6)]
    [TestCase((ulong)ILIntHelpers.ILINT_BASE + 0xFFFFFFFFFE, ExpectedResult = 6)]
    [TestCase((ulong)ILIntHelpers.ILINT_BASE + 0xFFFFFFFFFF, ExpectedResult = 6)]
    [TestCase((ulong)ILIntHelpers.ILINT_BASE + 0xFFFFFFFFFF + 1, ExpectedResult = 7)]
    [TestCase((ulong)ILIntHelpers.ILINT_BASE + 0xFFFFFFFFFFFE, ExpectedResult = 7)]
    [TestCase((ulong)ILIntHelpers.ILINT_BASE + 0xFFFFFFFFFFFF, ExpectedResult = 7)]
    [TestCase((ulong)ILIntHelpers.ILINT_BASE + 0xFFFFFFFFFFFF + 1, ExpectedResult = 8)]
    [TestCase((ulong)ILIntHelpers.ILINT_BASE + 0xFFFFFFFFFFFFFE, ExpectedResult = 8)]
    [TestCase((ulong)ILIntHelpers.ILINT_BASE + 0xFFFFFFFFFFFFFF, ExpectedResult = 8)]
    [TestCase((ulong)ILIntHelpers.ILINT_BASE + 0xFFFFFFFFFFFFFF + 1, ExpectedResult = 9)]
    public int ILIntSize(ulong value) => value.ILIntSize();
}
