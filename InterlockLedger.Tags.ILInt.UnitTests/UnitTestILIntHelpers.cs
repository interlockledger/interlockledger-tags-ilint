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

using NUnit.Framework;

namespace InterlockLedger.Tags
{
    [TestFixture]
    public class UnitTestILIntHelpers
    {
        [TestCase((ulong)1)]
        [TestCase((ulong)128)]
        [TestCase((ulong)247)]
        [TestCase((ulong)248)]
        [TestCase((ulong)249)]
        [TestCase((ulong)503)]
        [TestCase((ulong)505)]
        [TestCase((ulong)66041)]
        [TestCase((ulong)16843257)]
        [TestCase((ulong)4311810553)]
        [TestCase((ulong)1103823438329)]
        [TestCase((ulong)282578800148985)]
        [TestCase((ulong)72057594037928183)]
        [TestCase((ulong)72340172838076921)]
        [TestCase(18446744073709551615)]
        public void AsILIntILIntDecode(ulong value) => Assert.AreEqual(value, value.AsILInt().ILIntDecode());

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

        [TestCase(new byte[] { 0 }, ExpectedResult = 0, TestName = "ILIntDecodeFromByteArray 0")]
        [TestCase(new byte[] { 1 }, ExpectedResult = 1, TestName = "ILIntDecodeFromByteArray 1")]
        [TestCase(new byte[] { 0x80 }, ExpectedResult = 128, TestName = "ILIntDecodeFromByteArray 128")]
        [TestCase(new byte[] { 0xF7 }, ExpectedResult = 247, TestName = "ILIntDecodeFromByteArray 247")]
        [TestCase(new byte[] { 0xF8, 0 }, ExpectedResult = 248, TestName = "ILIntDecodeFromByteArray 248")]
        [TestCase(new byte[] { 0xF8, 1 }, ExpectedResult = 249, TestName = "ILIntDecodeFromByteArray 249")]
        [TestCase(new byte[] { 0xF8, 0xFF }, ExpectedResult = 503, TestName = "ILIntDecodeFromByteArray 503")]
        [TestCase(new byte[] { 0xF9, 1, 1 }, ExpectedResult = 505, TestName = "ILIntDecodeFromByteArray 505")]
        [TestCase(new byte[] { 0xFA, 1, 1, 1 }, ExpectedResult = 66041, TestName = "ILIntDecodeFromByteArray 66041")]
        [TestCase(new byte[] { 0xFB, 1, 1, 1, 1 }, ExpectedResult = 16843257, TestName = "ILIntDecodeFromByteArray 16843257")]
        [TestCase(new byte[] { 0xFC, 1, 1, 1, 1, 1 }, ExpectedResult = 4311810553, TestName = "ILIntDecodeFromByteArray 4311810553")]
        [TestCase(new byte[] { 0xFD, 1, 1, 1, 1, 1, 1 }, ExpectedResult = 1103823438329, TestName = "ILIntDecodeFromByteArray 1103823438329")]
        [TestCase(new byte[] { 0xFE, 1, 1, 1, 1, 1, 1, 1 }, ExpectedResult = 282578800148985, TestName = "ILIntDecodeFromByteArray 282578800148985")]
        [TestCase(new byte[] { 0xFE, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, ExpectedResult = 72057594037928183, TestName = "ILIntDecodeFromByteArray 72057594037928183")]
        [TestCase(new byte[] { 0xFF, 1, 1, 1, 1, 1, 1, 1, 1 }, ExpectedResult = 72340172838076921, TestName = "ILIntDecodeFromByteArray 72340172838076921")]
        [TestCase(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 7 }, ExpectedResult = 18446744073709551615, TestName = "ILIntDecodeFromByteArray 18446744073709551615")]
        [TestCase(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 8 }, ExpectedResult = 0, TestName = "ILIntDecodeFromByteArray too large by 1 => zero")]
        [TestCase(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x80 }, ExpectedResult = 0, TestName = "ILIntDecodeFromByteArray too large by 120 => zero")]
        [TestCase(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, ExpectedResult = 0, TestName = "ILIntDecodeFromByteArray too large by all => zero")]
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
}