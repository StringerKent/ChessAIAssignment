using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitChess
{
    [TestFixture]
    public class TestMovePatterns
    {
        [Test]
        public void TestKingMovePatters1()
        {
            var bitBoard = new BitBoard();
            var moveBits = bitBoard.KingMovePatternOfSquareIndex(4);
            
            var expected = 
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 1 1 1 0 0" +
                "0 0 0 1 0 1 0 0";

            var actualPattern = bitBoard.DebugPattern(moveBits);
            TestBitBoardSetup.AssertBoardPattern(expected, actualPattern);
        }

        [Test]
        public void TestKingMovePattersCenter()
        {
            var bitBoard = new BitBoard();
            var moveBits = bitBoard.KingMovePatternOfSquareIndex(36);

            var expected =
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 1 1 1 0 0" +
                "0 0 0 1 0 1 0 0" +
                "0 0 0 1 1 1 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0";

            var actualPattern = bitBoard.DebugPattern(moveBits);
            TestBitBoardSetup.AssertBoardPattern(expected, actualPattern);
        }

        [Test]
        public void TestKingMovePattersCorner()
        {
            var bitBoard = new BitBoard();
            var moveBits = bitBoard.KingMovePatternOfSquareIndex(56);
                        
            var expected =
                "0 1 0 0 0 0 0 0" +
                "1 1 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0";

            var actualPattern = bitBoard.DebugPattern(moveBits);
            TestBitBoardSetup.AssertBoardPattern(expected, actualPattern);
        }

        [Test]
        public void TestKnightMovePattern1()
        {
            var bitBoard = new BitBoard();
            var moveBits = bitBoard.KnightMovePatternOfSquareIndex(3);

            var expected =
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 1 0 1 0 0 0" +
                "0 1 0 0 0 1 0 0" +
                "0 0 0 0 0 0 0 0";

            var actualPattern = bitBoard.DebugPattern(moveBits);
            TestBitBoardSetup.AssertBoardPattern(expected, actualPattern);
        }

        [Test]
        public void TestKnightMovePatternCorner()
        {
            var bitBoard = new BitBoard();
            var moveBits = bitBoard.KnightMovePatternOfSquareIndex(63);

            var expected =
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 1 0 0" +
                "0 0 0 0 0 0 1 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0";

            var actualPattern = bitBoard.DebugPattern(moveBits);
            TestBitBoardSetup.AssertBoardPattern(expected, actualPattern);
        }

        [Test]
        public void TestKnightMovePatternCenter()
        {
            var bitBoard = new BitBoard();
            var moveBits = bitBoard.KnightMovePatternOfSquareIndex(28);

            var expected =
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 1 0 1 0 0" +
                "0 0 1 0 0 0 1 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 1 0 0 0 1 0" +
                "0 0 0 1 0 1 0 0" +
                "0 0 0 0 0 0 0 0";

            var actualPattern = bitBoard.DebugPattern(moveBits);
            TestBitBoardSetup.AssertBoardPattern(expected, actualPattern);
        }

        [Test]
        public void TestWhitePawnMovePattern1()
        {
            var bitBoard = new BitBoard();
            var moveBits = bitBoard.WhiteMovePatternOfSquareIndex(10);

            var expected =
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 1 0 0 0 0 0" +
                "0 0 1 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0";

            var actualPattern = bitBoard.DebugPattern(moveBits);
            TestBitBoardSetup.AssertBoardPattern(expected, actualPattern);
        }

        [Test]
        public void TestWhitePawnMovePattern2()
        {
            var bitBoard = new BitBoard();
            var moveBits = bitBoard.WhiteMovePatternOfSquareIndex(18);

            var expected =
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 1 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0";

            var actualPattern = bitBoard.DebugPattern(moveBits);
            TestBitBoardSetup.AssertBoardPattern(expected, actualPattern);
        }

        [Test]
        public void TestBlackPawnMovePattern1()
        {
            var bitBoard = new BitBoard();
            var moveBits = bitBoard.BlackMovePatternOfSquareIndex(54);

            var expected =
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 1 0" +
                "0 0 0 0 0 0 1 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0";

            var actualPattern = bitBoard.DebugPattern(moveBits);
            TestBitBoardSetup.AssertBoardPattern(expected, actualPattern);
        }

        [Test]
        public void TestBlackPawnMovePattern2()
        {
            var bitBoard = new BitBoard();
            var moveBits = bitBoard.BlackMovePatternOfSquareIndex(46);

            var expected =
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 1 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0" +
                "0 0 0 0 0 0 0 0";

            var actualPattern = bitBoard.DebugPattern(moveBits);
            TestBitBoardSetup.AssertBoardPattern(expected, actualPattern);
        }

        [Test]
        public void TestConvertSquareIndexToFileAndRank()
        {
            var squareIndex = 44;
            var expectedRank = 5;
            var expectedFile = 4;
            var actualRank = 0;
            var actualFile = 0;
            BitBoard.IndexToFileAndRank(squareIndex, out actualRank, out actualFile);

            Assert.AreEqual(expectedFile, actualFile);
            Assert.AreEqual(expectedRank, actualRank);
        }

        [Test]
        public void TestConvertFileAndRankToSquareIndex()
        {
            var expectedIndex = 44;
            var rank = 5;
            var file = 4;
            
            var actualSquareIndex = BitBoard.ToSquareIndex(rank, file);
            Assert.AreEqual(expectedIndex, actualSquareIndex);            
        }

        [Test]
        public void TestConvertInvalidFileAndRankToSquareIndex()
        {
            var expectedIndex = (int?)null;
            var rank = 9;
            var file = 5;

            var actualSquareIndex = BitBoard.ToSquareIndex(rank, file);
            Assert.AreEqual(expectedIndex, actualSquareIndex);
        }

        [Test]
        public void TestConvertFileAndRankToSquareIndexMin()
        {
            var expectedIndex = 0;
            var rank = 0;
            var file = 0;

            var actualSquareIndex = BitBoard.ToSquareIndex(rank, file);
            Assert.AreEqual(expectedIndex, actualSquareIndex);
        }

        [Test]
        public void TestConvertFileAndRankToSquareIndexMax()
        {
            var expectedIndex = 63;
            var rank = 7;
            var file = 7;
            var actualSquareIndex = BitBoard.ToSquareIndex(rank, file);
            Assert.AreEqual(expectedIndex, actualSquareIndex);
        }
    }
}
