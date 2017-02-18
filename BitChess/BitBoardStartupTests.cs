using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitChess
{
    [TestFixture]
    public class TestBitBoardSetup
    {
        internal static void AssertBoardPattern(string expected, string actual)
        {
            expected = expected.Replace(" ", "");
            actual = actual.PadLeft(64, '0');
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestWhitePawns()
        {
            var bitBoard = new BitBoard();
            bitBoard.SetStartPos();
            var expectedPattern =
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "1 1 1 1 1 1 1 1" +
                "0 0 0 0 0 0 0 0";

            AssertBoardPattern(expectedPattern, bitBoard.DebugPattern(PieceType.WhitePawn));
        }

        [Test]
        public void TestBlackPawns()
        {
            var bitBoard = new BitBoard();
            bitBoard.SetStartPos();
            var expectedPattern =
                "0 0 0 0 0 0 0 0" +
                "1 1 1 1 1 1 1 1" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0";

            AssertBoardPattern(expectedPattern, bitBoard.DebugPattern(PieceType.BlackPawn));
        }

        [Test]
        public void TestWhiteKnight()
        {
            var bitBoard = new BitBoard();
            bitBoard.SetStartPos();
            var expectedPattern =
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 1 0 0 0 0 1 0";

            AssertBoardPattern(expectedPattern, bitBoard.DebugPattern(PieceType.WhiteKnight));
        }

        [Test]
        public void TestBlackKnight()
        {
            var bitBoard = new BitBoard();
            bitBoard.SetStartPos();
            var expectedPattern =
                "0 1 0 0 0 0 1 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0";

            AssertBoardPattern(expectedPattern, bitBoard.DebugPattern(PieceType.BlackKnight));
        }

        [Test]
        public void TestWhiteBishops()
        {
            var bitBoard = new BitBoard();
            bitBoard.SetStartPos();
            var expectedPattern =
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 1 0 0 1 0 0";

            AssertBoardPattern(expectedPattern, bitBoard.DebugPattern(PieceType.WhiteBishop));
        }

        [Test]
        public void TestBlackBishops()
        {
            var bitBoard = new BitBoard();
            bitBoard.SetStartPos();
            var expectedPattern =
                "0 0 1 0 0 1 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0";

            AssertBoardPattern(expectedPattern, bitBoard.DebugPattern(PieceType.BlackBishop));
        }

        [Test]
        public void TestWhiteRook()
        {
            var bitBoard = new BitBoard();
            bitBoard.SetStartPos();
            var expectedPattern =
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "1 0 0 0 0 0 0 1";

            AssertBoardPattern(expectedPattern, bitBoard.DebugPattern(PieceType.WhiteRook));
        }

        [Test]
        public void TestBlackRook()
        {
            var bitBoard = new BitBoard();
            bitBoard.SetStartPos();
            var expectedPattern =
                "1 0 0 0 0 0 0 1" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0";

            AssertBoardPattern(expectedPattern, bitBoard.DebugPattern(PieceType.BlackRook));
        }

        [Test]
        public void TestWhiteQueen()
        {
            var bitBoard = new BitBoard();
            bitBoard.SetStartPos();
            var expectedPattern =
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 1 0 0 0 0";

            AssertBoardPattern(expectedPattern, bitBoard.DebugPattern(PieceType.WhiteQueen));
        }

        [Test]
        public void TestBlackQueen()
        {
            var bitBoard = new BitBoard();
            bitBoard.SetStartPos();
            var expectedPattern =
                "0 0 0 1 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0";

            AssertBoardPattern(expectedPattern, bitBoard.DebugPattern(PieceType.BlackQueen));
        }
    }
}
