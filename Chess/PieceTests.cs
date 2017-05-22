using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chess
{
    [TestClass]
    public class PieceTests
    {
        private Game game;

        [TestInitialize]
        public void Init() {
            game = new Game();
            game.New();
            game.Reset();
        }

        [TestMethod]
        public void TestRookAttacks() {
            game.AddPiece("d4bR");
            var board = game.Board;
            var rook = board.Square(File.D, Rank._4).Piece;
            Assert.IsTrue(rook.Attacks(board.Square(File.D, Rank._6), board));
            Assert.IsTrue(rook.Attacks(board.Square(File.D, Rank._3), board));
            Assert.IsTrue(rook.Attacks(board.Square(File.A, Rank._4), board));
            Assert.IsTrue(rook.Attacks(board.Square(File.E, Rank._4), board));
            Assert.IsFalse(rook.Attacks(board.Square(File.E, Rank._5), board));
            Assert.IsTrue(rook.Attacks(board.Square(File.D, Rank._7), board));
            game.AddPiece("d6bP");
            //does not attack own piece
            Assert.IsFalse(rook.Attacks(board.Square(File.D, Rank._6), board));
            Assert.IsFalse(rook.Attacks(board.Square(File.D, Rank._8), board));
            game.AddPiece("b4wP");
            Assert.IsTrue(rook.Attacks(board.Square(File.B, Rank._4), board));
            Assert.IsFalse(rook.Attacks(board.Square(File.A, Rank._4), board));

        }

        [TestMethod]
        public void TestBishopAttacks() {
            game.AddPiece("d4bB");
            var board = game.Board;
            var bishop = board.Square(File.D, Rank._4).Piece;
            Assert.IsTrue(bishop.Attacks(board.Square(File.E, Rank._3), board));
            Assert.IsTrue(bishop.Attacks(board.Square(File.F, Rank._2), board));
            Assert.IsTrue(bishop.Attacks(board.Square(File.B, Rank._2), board));
            Assert.IsTrue(bishop.Attacks(board.Square(File.A, Rank._1), board));
            Assert.IsTrue(bishop.Attacks(board.Square(File.A, Rank._7), board));
            Assert.IsFalse(bishop.Attacks(board.Square(File.E, Rank._6), board));
            game.AddPiece("f6bP");
            //does not attack own piece
            Assert.IsFalse(bishop.Attacks(board.Square(File.F, Rank._6), board));
            Assert.IsFalse(bishop.Attacks(board.Square(File.G, Rank._7), board));

            game.AddPiece("f2wP");
            Assert.IsTrue(bishop.Attacks(board.Square(File.F, Rank._2), board));
            Assert.IsFalse(bishop.Attacks(board.Square(File.G, Rank._1), board));
        }

        [TestMethod]
        public void TestQueenAttacks() {
            game.AddPiece("b2wQ");
            var board = game.Board;
            var queen = board.Square(File.B, Rank._2).Piece;
            Assert.IsTrue(queen.Attacks(board.Square(File.A, Rank._1), board));
            Assert.IsTrue(queen.Attacks(board.Square(File.B, Rank._1), board));
            Assert.IsTrue(queen.Attacks(board.Square(File.A, Rank._2), board));
            Assert.IsTrue(queen.Attacks(board.Square(File.B, Rank._7), board));
            Assert.IsTrue(queen.Attacks(board.Square(File.G, Rank._7), board));
            Assert.IsFalse(queen.Attacks(board.Square(File.D, Rank._3), board));
            game.AddPiece("d4bN");
            Assert.IsTrue(queen.Attacks(board.Square(File.D, Rank._4), board));
            Assert.IsFalse(queen.Attacks(board.Square(File.E, Rank._5), board));
            game.AddPiece("g2wN");
            Assert.IsFalse(queen.Attacks(board.Square(File.G, Rank._2), board));
            Assert.IsFalse(queen.Attacks(board.Square(File.H, Rank._2), board));

        }

        [TestMethod]
        public void TestKnightAttacks() {
            game.AddPiece("d4wN");
            var board = game.Board;
            var knight = board.Square(File.D, Rank._4).Piece;
            Assert.IsTrue(knight.Attacks(board.Square(File.E, Rank._2), board));
            Assert.IsTrue(knight.Attacks(board.Square(File.C, Rank._2), board));
            Assert.IsTrue(knight.Attacks(board.Square(File.B, Rank._3), board));
            Assert.IsFalse(knight.Attacks(board.Square(File.H, Rank._3), board));

            game.AddPiece("e2wK");
            Assert.IsFalse(knight.Attacks(board.Square(File.E, Rank._2), board));

            game.AddPiece("c6bK");
            Assert.IsTrue(knight.Attacks(board.Square(File.C, Rank._6), board));
        }

        [TestMethod]
        public void TestWhitePawnAttacks() {
            game.AddPiece("d4wP");
            var board = game.Board;
            var pawn = board.Square(File.D, Rank._4).Piece;

            Assert.IsTrue(pawn.Attacks(board.Square(File.E, Rank._5), board));
            Assert.IsTrue(pawn.Attacks(board.Square(File.C, Rank._5), board));
            Assert.IsFalse(pawn.Attacks(board.Square(File.D, Rank._5), board));

            game.AddPiece("e5wP");
            Assert.IsFalse(pawn.Attacks(board.Square(File.E, Rank._5), board));

            game.AddPiece("c5bP");
            Assert.IsTrue(pawn.Attacks(board.Square(File.C, Rank._5), board));
        }

        [TestMethod]
        public void TestBlackPawnAttacks() {
            game.AddPiece("d6bP");
            var board = game.Board;
            var pawn = board.Square(File.D, Rank._6).Piece;

            Assert.IsTrue(pawn.Attacks(board.Square(File.E, Rank._5), board));
            Assert.IsTrue(pawn.Attacks(board.Square(File.C, Rank._5), board));
            Assert.IsFalse(pawn.Attacks(board.Square(File.D, Rank._5), board));

            game.AddPiece("e5bP");
            Assert.IsFalse(pawn.Attacks(board.Square(File.E, Rank._5), board));

            game.AddPiece("c5wP");
            Assert.IsTrue(pawn.Attacks(board.Square(File.C, Rank._5), board));
        }

        [TestMethod]
        public void TestKingAttacks() {
            game.AddPiece("d5bK");
            var board = game.Board;
            var king = board.Square(File.D, Rank._5).Piece;
            Assert.IsTrue(king.Attacks(board.Square(File.E, Rank._4), board));
            Assert.IsTrue(king.Attacks(board.Square(File.E, Rank._5), board));
            Assert.IsTrue(king.Attacks(board.Square(File.E, Rank._5), board));
            Assert.IsTrue(king.Attacks(board.Square(File.D, Rank._4), board));
            Assert.IsTrue(king.Attacks(board.Square(File.D, Rank._6), board));
            Assert.IsTrue(king.Attacks(board.Square(File.C, Rank._4), board));
            Assert.IsTrue(king.Attacks(board.Square(File.C, Rank._5), board));
            Assert.IsTrue(king.Attacks(board.Square(File.C, Rank._6), board));
            Assert.IsFalse(king.Attacks(board.Square(File.C, Rank._7), board));

            game.AddPiece("d4bP");
            Assert.IsFalse(king.Attacks(board.Square(File.D, Rank._4), board));
        }


    }
}