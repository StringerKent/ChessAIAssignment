using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chess
{
    [TestClass]
    public class GameTests
    {
        [TestMethod]
        public void TestPositionOfPiecesWhenGameStarts()
        {
            var game = new Game();
            game.New();
            Assert.IsNotNull(game.Board);

            Assert.IsTrue(game.Board.Square(File.A, Rank._1).Piece is Rook);
            Assert.AreEqual(Color.White, game.Board.Square(File.A, Rank._1).Piece.Color);

            Assert.IsTrue(game.Board.Square(File.H, Rank._1).Piece is Rook);
            Assert.AreEqual(Color.White, game.Board.Square(File.H, Rank._1).Piece.Color);

            Assert.IsTrue(game.Board.Square(File.B, Rank._1).Piece is Knight);
            Assert.IsTrue(game.Board.Square(File.G, Rank._1).Piece is Knight);

            Assert.IsTrue(game.Board.Square(File.C, Rank._1).Piece is Bishop);
            Assert.IsTrue(game.Board.Square(File.F, Rank._1).Piece is Bishop);

            Assert.IsTrue(game.Board.Square(File.D, Rank._1).Piece is Queen);
            Assert.IsTrue(game.Board.Square(File.E, Rank._1).Piece is King);

            for (int i = 0; i < 8; i++)
                Assert.IsTrue(game.Board.Squares[i + 1 * 8].Piece is Pawn);

            Assert.IsTrue(game.Board.Square(File.A, Rank._8).Piece is Rook);
            Assert.AreEqual(Color.Black, game.Board.Square(File.A, Rank._8).Piece.Color);

            Assert.IsTrue(game.Board.Square(File.H, Rank._8).Piece is Rook);
            Assert.AreEqual(Color.Black, game.Board.Square(File.H, Rank._8).Piece.Color);

            Assert.IsTrue(game.Board.Square(File.B, Rank._8).Piece is Knight);
            Assert.IsTrue(game.Board.Square(File.G, Rank._8).Piece is Knight);

            Assert.IsTrue(game.Board.Square(File.C, Rank._8).Piece is Bishop);
            Assert.IsTrue(game.Board.Square(File.F, Rank._8).Piece is Bishop);

            Assert.IsTrue(game.Board.Square(File.D, Rank._8).Piece is Queen);
            Assert.IsTrue(game.Board.Square(File.E, Rank._8).Piece is King);

            for (int i = 0; i < 8; i++)
                Assert.IsTrue(game.Board.Squares[i + 6 * 8].Piece is Pawn);

        }

        [TestMethod]
        public void TestPlayersAtStartup()
        {
            var game = new Game();
            game.New();

            Assert.IsNotNull(game.WhitePlayer);
            Assert.IsNotNull(game.BlackPlayer);
            Assert.AreEqual(16, game.WhitePlayer.Pieces.Count);
            Assert.AreEqual(16, game.BlackPlayer.Pieces.Count);

            Assert.AreSame(game.WhitePlayer.Pieces.First().Square.Piece, game.WhitePlayer.Pieces.First());

            Assert.AreSame(game.CurrentPlayer, game.WhitePlayer);
            foreach (var piece in game.WhitePlayer.Pieces)
                Assert.IsTrue(piece.ImageChar != 0);
            foreach (var piece in game.BlackPlayer.Pieces)
                Assert.IsTrue(piece.ImageChar != 0);
        }

        [TestMethod]
        public void TestKingMoves()
        {
            var game = new Game();
            game.New();

            game.Reset();
            game.AddPiece(File.E, Rank._1, new King(Color.White));
            game.AddPiece(File.E, Rank._8, new King(Color.Black));

            game.AddPiece(File.E, Rank._2, new Pawn(Color.Black)); //Captures on E2
            game.AddPiece(File.F, Rank._2, new Pawn(Color.White)); //Blocks own King


            var moves = game.GetPseudoLegalMoves();
            var kingMoves = moves.Where(x => x.Piece is King).ToArray();
            Assert.AreEqual(4, kingMoves.Length);

            Assert.IsNotNull(kingMoves.Single(x => x.Piece.Color == Color.White &&
                                                   x.ToSquare.ToString() == "d1"));
            Assert.IsNotNull(kingMoves.Single(x => x.Piece.Color == Color.White &&
                                                   x.ToSquare.ToString() == "d2"));
            Assert.IsNotNull(kingMoves.Single(x => x.Piece.Color == Color.White &&
                                                   x.ToSquare.ToString() == "e2"));
            Assert.IsNotNull(kingMoves.Single(x => x.Piece.Color == Color.White &&
                                                   x.ToSquare.ToString() == "f1"));
        }

        [TestMethod]
        public void TestRookMoves()
        {
            var game = new Game();
            game.New();
            game.Reset();
            game.AddPiece(File.E, Rank._1, new King(Color.White));
            game.AddPiece(File.E, Rank._8, new King(Color.Black));
            game.AddPiece(File.E, Rank._6, new Pawn(Color.Black));

            game.AddPiece(File.E, Rank._2, new Rook(Color.White));
            game.SetInitials();
            var moves = game.GetPseudoLegalMoves();
            var rookMoves = moves.Where(x => x.Piece is Rook && x.Piece.Color == Color.White);

            var squares = rookMoves.Select(x => x.ToSquare.ToString()).ToArray();
            Assert.AreEqual(11, squares.Length);
            Assert.IsTrue(squares.Contains("e3"));
            Assert.IsTrue(squares.Contains("e4"));
            Assert.IsTrue(squares.Contains("e5"));
            Assert.IsTrue(squares.Contains("e6"));

            Assert.IsTrue(squares.Contains("a2"));
            Assert.IsTrue(squares.Contains("b2"));
            Assert.IsTrue(squares.Contains("c2"));
            Assert.IsTrue(squares.Contains("d2"));
            Assert.IsTrue(squares.Contains("f2"));
            Assert.IsTrue(squares.Contains("g2"));
            Assert.IsTrue(squares.Contains("h2"));

        }

        [TestMethod]
        public void TestBishopMoves()
        {
            var game = new Game();
            game.New();
            game.Reset();
            game.AddPiece(File.E, Rank._1, new King(Color.White));
            game.AddPiece(File.E, Rank._8, new King(Color.Black));

            game.AddPiece(File.E, Rank._4, new Bishop(Color.White));
            game.AddPiece(File.F, Rank._3, new Pawn(Color.White));
            game.AddPiece(File.D, Rank._3, new Pawn(Color.Black));

            var moves = game.GetPseudoLegalMoves();
            var bishopMoves = moves.Where(x => x.Piece is Bishop && x.Piece.Color == Color.White);
            var squares = bishopMoves.Select(x => x.ToSquare.ToString()).ToArray();
            Assert.AreEqual(8, squares.Length);
            Assert.IsTrue(squares.Contains("d3"));
            Assert.IsTrue(squares.Contains("d5"));
            Assert.IsTrue(squares.Contains("c6"));
            Assert.IsTrue(squares.Contains("b7"));
            Assert.IsTrue(squares.Contains("a8"));
            Assert.IsTrue(squares.Contains("f5"));
            Assert.IsTrue(squares.Contains("g6"));
            Assert.IsTrue(squares.Contains("h7"));
        }

        [TestMethod]
        public void TestKnightMoves()
        {
            var game = new Game();
            game.New();
            game.Reset();
            game.AddPiece(File.E, Rank._1, new King(Color.White));
            game.AddPiece(File.E, Rank._8, new King(Color.Black));

            game.AddPiece(File.E, Rank._4, new Knight(Color.White));

            game.AddPiece(File.D, Rank._6, new Pawn(Color.White));

            var moves = game.GetPseudoLegalMoves();
            var knightMoves = moves.Where(x => x.Piece is Knight && x.Piece.Color == Color.White);
            var squares = knightMoves.Select(x => x.ToSquare.ToString()).ToArray();
            Assert.AreEqual(7, squares.Length);
            Assert.IsTrue(squares.Contains("f6"));
            Assert.IsTrue(squares.Contains("g5"));
            Assert.IsTrue(squares.Contains("g3"));
            Assert.IsTrue(squares.Contains("f2"));
            Assert.IsTrue(squares.Contains("d2"));
            Assert.IsTrue(squares.Contains("c3"));
            Assert.IsTrue(squares.Contains("c5"));

        }

        [TestMethod]
        public void TestWhitePawnMoves()
        {
            var game = new Game();
            game.New();
            game.Reset();
            game.AddPiece(File.E, Rank._1, new King(Color.White));
            game.AddPiece(File.E, Rank._8, new King(Color.Black));
            game.AddPiece(File.E, Rank._2, new Pawn(Color.White));

            var moves = game.GetPseudoLegalMoves();
            var pawnMoves =
                moves.Where(x => x.Piece is Pawn && x.Piece.Color == Color.White)
                    .Select(x => x.ToSquare.ToString())
                    .ToArray();
            Assert.AreEqual(2, pawnMoves.Length);
            Assert.IsTrue(pawnMoves.Contains("e3"));
            Assert.IsTrue(pawnMoves.Contains("e4"));

            game.AddPiece(File.F, Rank._3, new Pawn(Color.Black));
            moves = game.GetPseudoLegalMoves();
            pawnMoves =
                moves.Where(x => x.Piece is Pawn && x.Piece.Color == Color.White)
                    .Select(x => x.ToSquare.ToString())
                    .ToArray();
            Assert.AreEqual(3, pawnMoves.Length);
            Assert.IsTrue(pawnMoves.Contains("f3"));

        }

        [TestMethod]
        public void TestBlackPawnMoves()
        {
            var game = new Game();
            game.New();
            game.Reset();
            game.AddPiece(File.E, Rank._1, new King(Color.White));
            game.AddPiece(File.E, Rank._8, new King(Color.Black));
            game.AddPiece(File.E, Rank._7, new Pawn(Color.Black));
            game.CurrentPlayer = game.BlackPlayer;

            var moves = game.GetPseudoLegalMoves();
            var pawnMoves =
                moves.Where(x => x.Piece is Pawn && x.Piece.Color == Color.Black)
                    .Select(x => x.ToSquare.ToString())
                    .ToArray();
            Assert.AreEqual(2, pawnMoves.Length);
            Assert.IsTrue(pawnMoves.Contains("e6"));
            Assert.IsTrue(pawnMoves.Contains("e5"));

            game.AddPiece(File.F, Rank._6, new Pawn(Color.White));
            moves = game.GetPseudoLegalMoves();
            pawnMoves =
                moves.Where(x => x.Piece is Pawn && x.Piece.Color == Color.Black)
                    .Select(x => x.ToSquare.ToString())
                    .ToArray();
            Assert.AreEqual(3, pawnMoves.Length);
            Assert.IsTrue(pawnMoves.Contains("f6"));

        }

        [TestMethod]
        public void TestPawnMoveTwoSquares()
        {
            var game = new Game();
            game.New();
            Assert.IsTrue(game.TryStringMove("b1-c3"));
            Assert.IsTrue(game.TryStringMove("b7-b6"));
            Assert.IsFalse(game.TryStringMove("c2-c4"));

        }

        [TestMethod]
        public void TestCastling()
        {
            var game = new Game();
            game.New();
            game.Reset();
            game.AddPiece(File.E, Rank._1, new King(Color.White));
            game.AddPiece(File.E, Rank._8, new King(Color.Black));
            game.AddPiece(File.A, Rank._1, new Rook(Color.White));
            game.AddPiece(File.H, Rank._1, new Rook(Color.White));
            game.SetInitials();
            var moves = game.GetPseudoLegalMoves();
            var kingMoves =
                moves.Where(x => x.Piece is King && x.Piece.Color == Color.White).Select(x => x.ToString()).ToArray();
            Assert.IsTrue(kingMoves.Contains("0-0"));
            Assert.IsTrue(kingMoves.Contains("0-0-0"));
            Assert.IsTrue(game.TryPossibleMoveCommand(new MoveCommand(File.E, Rank._1, File.G, Rank._1)));
            Assert.IsFalse(game.WhitePlayer.CanCastleKingSide);
            game.UndoLastMove();
            Assert.IsTrue(game.WhitePlayer.CanCastleKingSide);

            Assert.IsTrue(game.TryPossibleMoveCommand(new MoveCommand(File.E, Rank._1, File.C, Rank._1)));
            Assert.IsFalse(game.WhitePlayer.CanCastleQueenSide);
            game.UndoLastMove();
            Assert.IsTrue(game.WhitePlayer.CanCastleQueenSide);

        }

        [TestMethod]
        public void TestCastlingHasBeenChecked()
        {
            var game = new Game();
            game.New();
            game.Reset();
            game.AddPiece(File.E, Rank._1, new King(Color.White));
            game.AddPiece(File.E, Rank._8, new King(Color.Black));
            game.AddPiece(File.A, Rank._1, new Rook(Color.White));
            game.AddPiece(File.H, Rank._1, new Rook(Color.White));
            game.AddPiece(File.G, Rank._4, new Bishop(Color.White));
            game.AddPiece(File.A, Rank._3, new Rook(Color.Black));

            game.CurrentPlayer = game.BlackPlayer;
            game.SetInitials();

            Assert.IsTrue(game.TryPossibleMoveCommand(new MoveCommand(File.A, Rank._3, File.E, Rank._3)));
            Assert.IsTrue(game.WhitePlayer.IsChecked);
            Assert.IsTrue(game.TryPossibleMoveCommand(new MoveCommand(File.G, Rank._4, File.E, Rank._2)));
            Assert.IsFalse(game.WhitePlayer.IsChecked);
            Assert.IsTrue(game.TryPossibleMoveCommand(new MoveCommand(File.E, Rank._3, File.E, Rank._4)));
            Assert.AreSame(game.CurrentPlayer, game.WhitePlayer);
            var moves = game.GetPseudoLegalMoves();
            var kingMoves =
                moves.Where(x => x.Piece is King && x.Piece.Color == Color.White).Select(x => x.ToString()).ToArray();
            Assert.IsTrue(kingMoves.Contains("0-0"));
            Assert.IsTrue(game.TryPossibleMoveCommand(new MoveCommand(File.E, Rank._1, File.G, Rank._1)));
            Assert.IsFalse(game.WhitePlayer.CanCastleKingSide);
            game.UndoLastMove();
            Assert.IsTrue(game.WhitePlayer.CanCastleKingSide);

        }

        [TestMethod]
        public void TestBlockedCastling()
        {
            var game = new Game();
            game.New();
            game.Reset();
            game.AddPiece(File.E, Rank._1, new King(Color.White));
            game.AddPiece(File.E, Rank._8, new King(Color.Black));
            game.AddPiece(File.A, Rank._1, new Rook(Color.White));
            game.AddPiece(File.H, Rank._1, new Rook(Color.White));
            game.AddPiece(File.G, Rank._5, new Rook(Color.Black)); //should block king side
            game.AddPiece(File.D, Rank._1, new Knight(Color.White)); //should block queen side
            game.SetInitials();
            var moves = game.GetPseudoLegalMoves();
            var kingMoves =
                moves.Where(x => x.Piece is King && x.Piece.Color == Color.White).Select(x => x.ToString()).ToArray();
            Assert.IsFalse(kingMoves.Contains("0-0"));
            Assert.IsFalse(kingMoves.Contains("0-0-0"));
        }

        [TestMethod]
        public void TestMoveAndUndo()
        {
            var game = new Game();
            game.New();

            var result = game.TryPossibleMoveCommand(new MoveCommand(File.E, Rank._2, File.E, Rank._4));

            Assert.AreEqual(true, result);
            Assert.IsNull(game.Board.Square(File.E, Rank._2).Piece);
            Assert.IsTrue(game.Board.Square(File.E, Rank._4).Piece is Pawn);
            Assert.IsTrue(game.CurrentPlayer == game.BlackPlayer);
            Assert.AreEqual(1, game.WhitePlayer.Moves.Count);

            game.UndoLastMove();
            Assert.IsTrue(game.Board.Square(File.E, Rank._2).Piece is Pawn);
            Assert.IsNull(game.Board.Square(File.E, Rank._4).Piece);
            Assert.IsTrue(game.CurrentPlayer == game.WhitePlayer);
            Assert.AreEqual(0, game.WhitePlayer.Moves.Count);
        }


        [TestMethod]
        public void TestManyMovesAndUndo()
        {
            var game = new Game();
            game.New();

            Assert.AreEqual(20, game.GetPseudoLegalMoves().Count);

            Assert.IsTrue(game.TryPossibleMoveCommand(new MoveCommand(File.E, Rank._2, File.E, Rank._4))); //e4
            Assert.IsTrue(game.TryPossibleMoveCommand(new MoveCommand(File.E, Rank._7, File.E, Rank._5))); //e5

            Assert.IsTrue(game.TryPossibleMoveCommand(new MoveCommand(File.G, Rank._1, File.F, Rank._3))); //Nf3
            Assert.IsTrue(game.TryPossibleMoveCommand(new MoveCommand(File.G, Rank._8, File.F, Rank._6))); //Nf6

            Assert.IsTrue(game.TryPossibleMoveCommand(new MoveCommand(File.F, Rank._1, File.C, Rank._4))); //Bc4
            Assert.IsTrue(game.TryPossibleMoveCommand(new MoveCommand(File.F, Rank._8, File.C, Rank._5))); //Bc5

            Assert.IsTrue(game.TryPossibleMoveCommand(new MoveCommand(File.E, Rank._1, File.G, Rank._1))); //0-0
            Assert.IsTrue(game.TryPossibleMoveCommand(new MoveCommand(File.E, Rank._8, File.G, Rank._8))); //0-0

            Assert.IsTrue(game.TryPossibleMoveCommand(new MoveCommand(File.D, Rank._2, File.D, Rank._4))); //d4
            Assert.IsTrue(game.TryPossibleMoveCommand(new MoveCommand(File.C, Rank._5, File.D, Rank._4))); //Bxd4
            Assert.AreEqual(15, game.WhitePlayer.Pieces.Count());

            Assert.IsTrue(game.TryPossibleMoveCommand(new MoveCommand(File.F, Rank._3, File.D, Rank._4))); //Nxd4
            Assert.IsTrue(game.TryPossibleMoveCommand(new MoveCommand(File.E, Rank._5, File.D, Rank._4))); //Bxd4
            Assert.AreEqual(14, game.WhitePlayer.Pieces.Count());
            Assert.AreEqual(15, game.BlackPlayer.Pieces.Count());

            game.UndoLastMove();
            game.UndoLastMove();
            game.UndoLastMove();

            Assert.AreEqual(16, game.WhitePlayer.Pieces.Count());
            Assert.AreEqual(16, game.BlackPlayer.Pieces.Count());

            for (int i = 0; i < 9; i++) //from start
            {
                game.UndoLastMove();
            }

            Assert.AreEqual(20, game.GetPseudoLegalMoves().Count);

        }

        [TestMethod]
        public void TestUndoCapture()
        {
            var game = new Game();
            game.New();
            game.Reset();
            game.AddPiece(File.E, Rank._1, new King(Color.White));
            game.AddPiece(File.E, Rank._8, new King(Color.Black));

            game.AddPiece(File.E, Rank._2, new Pawn(Color.White));
            game.AddPiece(File.D, Rank._7, new Pawn(Color.Black));

            Assert.AreEqual(2, game.WhitePlayer.Pieces.Count());
            Assert.AreEqual(2, game.BlackPlayer.Pieces.Count());
            game.SetInitials();

            Assert.IsTrue(game.TryPossibleMoveCommand(new MoveCommand(File.E, Rank._2, File.E, Rank._4))); //e4
            Assert.IsTrue(game.TryPossibleMoveCommand(new MoveCommand(File.D, Rank._7, File.D, Rank._5))); //e5

            Assert.IsTrue(game.TryPossibleMoveCommand(new MoveCommand(File.E, Rank._4, File.D, Rank._5))); //exd5
            Assert.AreEqual(1, game.BlackPlayer.Pieces.Count());

            game.UndoLastMove();
            Assert.AreEqual(2, game.BlackPlayer.Pieces.Count());

            game.UndoLastMove();
            game.UndoLastMove(); //now from start

            //same moves again
            Assert.IsTrue(game.TryPossibleMoveCommand(new MoveCommand(File.E, Rank._2, File.E, Rank._4))); //e4
            Assert.IsTrue(game.TryPossibleMoveCommand(new MoveCommand(File.D, Rank._7, File.D, Rank._5))); //e5
            Assert.IsTrue(game.TryPossibleMoveCommand(new MoveCommand(File.E, Rank._4, File.D, Rank._5))); //exd5
            Assert.AreEqual(1, game.BlackPlayer.Pieces.Count());
        }

        [TestMethod]
        public void TestEnPassantWhiteRight()
        {
            var game = new Game();
            game.New();
            game.Reset();
            game.AddPiece(File.E, Rank._1, new King(Color.White));
            game.AddPiece(File.E, Rank._8, new King(Color.Black));

            game.AddPiece(File.D, Rank._4, new Pawn(Color.White));
            game.AddPiece(File.E, Rank._7, new Pawn(Color.Black));
            game.SetInitials();

            Assert.IsTrue(game.TryPossibleMoveCommand(new MoveCommand(File.D, Rank._4, File.D, Rank._5)));
            Assert.IsTrue(game.TryPossibleMoveCommand(new MoveCommand(File.E, Rank._7, File.E, Rank._5)));

            var moves = game.GetPseudoLegalMoves();
            var passant = moves.Single(x => x.IsEnpassant);

            Assert.IsTrue(passant.FromSquare.File == File.D && passant.FromSquare.Rank == Rank._5);
            Assert.IsTrue(passant.ToSquare.File == File.E && passant.ToSquare.Rank == Rank._6);
            Assert.IsTrue(passant.Capture is Pawn && passant.Capture.Color == Color.Black);
        }

        [TestMethod]
        public void TestEnPassantWhiteLeft()
        {
            var game = new Game();
            game.New();
            game.Reset();
            game.AddPiece(File.E, Rank._1, new King(Color.White));
            game.AddPiece(File.E, Rank._8, new King(Color.Black));

            game.AddPiece(File.D, Rank._4, new Pawn(Color.White));
            game.AddPiece(File.C, Rank._7, new Pawn(Color.Black));
            game.SetInitials();

            Assert.IsTrue(game.TryPossibleMoveCommand(new MoveCommand(File.D, Rank._4, File.D, Rank._5)));
            Assert.IsTrue(game.TryPossibleMoveCommand(new MoveCommand(File.C, Rank._7, File.C, Rank._5)));

            var moves = game.GetPseudoLegalMoves();
            var passant = moves.Single(x => x.IsEnpassant);

            Assert.IsTrue(passant.FromSquare.File == File.D && passant.FromSquare.Rank == Rank._5);
            Assert.IsTrue(passant.ToSquare.File == File.C && passant.ToSquare.Rank == Rank._6);
            Assert.IsTrue(passant.Capture is Pawn && passant.Capture.Color == Color.Black);
        }

        [TestMethod]
        public void TestEnPassantBlackRight()
        {
            var game = new Game();
            game.New();
            game.Reset();
            game.AddPiece(File.E, Rank._1, new King(Color.White));
            game.AddPiece(File.E, Rank._8, new King(Color.Black));

            game.AddPiece(File.E, Rank._2, new Pawn(Color.White));
            game.AddPiece(File.D, Rank._4, new Pawn(Color.Black));
            game.SetInitials();

            Assert.IsTrue(game.TryPossibleMoveCommand(new MoveCommand(File.E, Rank._2, File.E, Rank._4)));

            var moves = game.GetPseudoLegalMoves();
            var passant = moves.Single(x => x.IsEnpassant);

            Assert.IsTrue(passant.FromSquare.File == File.D && passant.FromSquare.Rank == Rank._4);
            Assert.IsTrue(passant.ToSquare.File == File.E && passant.ToSquare.Rank == Rank._3);
            Assert.IsTrue(passant.Capture is Pawn && passant.Capture.Color == Color.White);
        }

        [TestMethod]
        public void TestEnPassantBlackLeft()
        {
            var game = new Game();
            game.New();
            game.Reset();
            game.AddPiece(File.E, Rank._1, new King(Color.White));
            game.AddPiece(File.E, Rank._8, new King(Color.Black));

            game.AddPiece(File.C, Rank._2, new Pawn(Color.White));
            game.AddPiece(File.D, Rank._4, new Pawn(Color.Black));
            game.SetInitials();

            Assert.IsTrue(game.TryPossibleMoveCommand(new MoveCommand(File.C, Rank._2, File.C, Rank._4)));

            var moves = game.GetPseudoLegalMoves();
            var passant = moves.Single(x => x.IsEnpassant);

            Assert.IsTrue(passant.FromSquare.File == File.D && passant.FromSquare.Rank == Rank._4);
            Assert.IsTrue(passant.ToSquare.File == File.C && passant.ToSquare.Rank == Rank._3);
            Assert.IsTrue(passant.Capture is Pawn && passant.Capture.Color == Color.White);

            Assert.IsTrue(game.TryPossibleMoveCommand(new MoveCommand(File.D, Rank._4, File.C, Rank._3)));
            Assert.IsNull(passant.CapturedFrom.Piece);
        }

        [TestMethod]
        public void TestEnPassantBlackLeft_Negative()
        {
            var game = new Game();
            game.New();
            game.Reset();
            game.AddPiece(File.E, Rank._1, new King(Color.White));
            game.AddPiece(File.E, Rank._8, new King(Color.Black));

            game.AddPiece(File.C, Rank._2, new Pawn(Color.White));
            game.AddPiece(File.D, Rank._4, new Pawn(Color.Black));
            game.SetInitials();

            Assert.IsTrue(game.TryStringMove("c2-c4"));
            Assert.IsTrue(game.TryStringMove("e8-f8"));//Black King moves between
            Assert.IsTrue(game.TryStringMove("e1-f1"));//then white queen

            var moves = game.GetPseudoLegalMoves();
            Assert.IsFalse(moves.Any(x => x.IsEnpassant));

        }

        [TestMethod]
        public void TestInvalidMove()
        {
            var game = new Game();
            game.New();
            Assert.IsFalse(game.TryPossibleMoveCommand(new MoveCommand(File.A, Rank._1, File.F, Rank._1)));
        }

        [TestMethod]
        public void TestMoveToCheck()
        {
            var game = new Game();
            game.New();
            game.Reset();
            game.AddPiece(File.E, Rank._1, new King(Color.White));
            game.AddPiece(File.E, Rank._8, new King(Color.Black));
            game.AddPiece(File.D, Rank._1, new Queen(Color.Black));
            game.SetInitials();

            Assert.IsTrue(game.TryPossibleMoveCommand(new MoveCommand(File.E, Rank._1, File.F, Rank._2)));
            game.UndoLastMove();
            Assert.IsFalse(game.TryPossibleMoveCommand(new MoveCommand(File.E, Rank._1, File.F, Rank._1)));
            game.UndoLastMove();
            Assert.IsFalse(game.TryPossibleMoveCommand(new MoveCommand(File.E, Rank._1, File.E, Rank._2)));
            game.UndoLastMove();
            Assert.IsTrue(game.TryPossibleMoveCommand(new MoveCommand(File.E, Rank._1, File.D, Rank._1)));
        }

        [TestMethod]
        public void TestCheck()
        {
            var game = new Game();
            game.New();
            game.Reset();


            game.AddPiece(File.E, Rank._1, new King(Color.White));
            game.AddPiece(File.E, Rank._8, new King(Color.Black));
            game.AddPiece(File.F, Rank._2, new Rook(Color.White));
            game.SetInitials();

            Assert.IsTrue(game.TryPossibleMoveCommand(new MoveCommand(File.F, Rank._2, File.E, Rank._2)));//white
            Assert.IsTrue(game.BlackPlayer.IsChecked);

            Assert.IsTrue(game.TryPossibleMoveCommand(new MoveCommand(File.E, Rank._8, File.F, Rank._8)));//black
            Assert.IsFalse(game.BlackPlayer.IsChecked);

            game.UndoLastMove();
            Assert.IsTrue(game.BlackPlayer.IsChecked);

            game.UndoLastMove();
            Assert.IsFalse(game.BlackPlayer.IsChecked);
        }

        [TestMethod]
        public void TestCheckMate()
        {
            var game = new Game();
            game.New();

            Assert.IsTrue(game.TryPossibleMoveCommand(new MoveCommand(File.F, Rank._2, File.F, Rank._3)));
            Assert.IsTrue(game.TryPossibleMoveCommand(new MoveCommand(File.E, Rank._7, File.E, Rank._6)));
            Assert.IsTrue(game.TryPossibleMoveCommand(new MoveCommand(File.G, Rank._2, File.G, Rank._4)));
            Assert.IsTrue(game.TryPossibleMoveCommand(new MoveCommand(File.D, Rank._8, File.H, Rank._4)));
            Assert.IsTrue(game.WhitePlayer.Mated);
            Assert.IsTrue(game.Ended);
            Assert.AreSame(game.Winner, game.BlackPlayer);

            game.UndoLastMove();
            Assert.IsFalse(game.WhitePlayer.Mated);
            Assert.IsTrue(game.Winner == null);
        }

        [TestMethod]
        public void TestStaleMate()
        {
            var game = new Game();
            game.New();
            game.Reset();

            game.AddPiece(File.H, Rank._8, new King(Color.Black));
            game.AddPiece(File.A, Rank._1, new King(Color.White));
            game.AddPiece(File.G, Rank._2, new Queen(Color.White));
            game.SetInitials();

            Assert.IsTrue(game.TryPossibleMoveCommand(new MoveCommand(File.G, Rank._2, File.G, Rank._6)));
            Assert.IsTrue(game.Ended);
            Assert.IsTrue(game.IsStaleMate);
            game.UndoLastMove();

            Assert.IsFalse(game.Ended);
            Assert.IsFalse(game.IsStaleMate);
        }

        [TestMethod]
        public void TestNegativeStaleMate()
        {
            var game = new Game();
            game.New();
            game.Reset();

            game.AddPiece(File.H, Rank._8, new King(Color.Black));
            game.AddPiece(File.A, Rank._1, new King(Color.White));
            game.AddPiece(File.G, Rank._2, new Queen(Color.White));
            game.AddPiece(File.C, Rank._7, new Pawn(Color.Black)); //preventing stale
            game.SetInitials();

            Assert.IsTrue(game.TryPossibleMoveCommand(new MoveCommand(File.G, Rank._2, File.G, Rank._6)));
            Assert.IsFalse(game.Ended);
            Assert.IsFalse(game.IsStaleMate);
        }

        [TestMethod]
        public void TestPromotion()
        {
            var game = new Game();
            game.New();
            game.Reset();

            game.AddPiece(File.H, Rank._8, new King(Color.Black));
            game.AddPiece(File.A, Rank._1, new King(Color.White));
            game.AddPiece(File.B, Rank._6, new Pawn(Color.White));
            game.SetInitials();

            Assert.IsTrue(game.TryPossibleMoveCommand(new MoveCommand(File.B, Rank._6, File.B, Rank._7)));
            Assert.IsTrue(game.TryPossibleMoveCommand(new MoveCommand(File.H, Rank._8, File.H, Rank._7)));

            Assert.AreEqual(-100, game.Material);

            Assert.IsTrue(game.TryPossibleMoveCommand(new MoveCommand(File.B, Rank._7, File.B, Rank._8)));
            Assert.IsTrue(game.Board.Square(File.B, Rank._8).Piece is Queen);
            Assert.AreEqual(-900, game.Material);

            game.UndoLastMove();
            Assert.AreEqual(-100, game.Material);

            var square = game.Board.Square(File.B, Rank._7);
            var pawn = (Pawn)square.Piece;
            Assert.AreSame(pawn.Square, game.Board.Square(File.B, Rank._7));
            Assert.AreSame(square.Piece, pawn);

            game.UndoLastMove();
            game.UndoLastMove(); //undone all moves
            pawn = game.WhitePlayer.Pieces.OfType<Pawn>().Single();
            Assert.AreSame(pawn.Square.Piece, pawn);
        }

        [TestMethod]
        public void TestInsufficientMaterial()
        {
            var game = new Game();
            game.New();
            game.Reset();

            game.AddPiece(File.E, Rank._8, new King(Color.Black));
            game.AddPiece(File.E, Rank._1, new King(Color.White));

            game.AddPiece(File.D, Rank._4, new Queen(Color.Black));
            game.AddPiece(File.C, Rank._5, new Bishop(Color.White));
            game.SetInitials();

            Assert.IsTrue(game.TryPossibleMoveCommand(new MoveCommand(File.C, Rank._5, File.D, Rank._4)));
            Assert.IsTrue(game.Ended);
            Assert.AreEqual(0, game.WhitePlayer.Moves.First().ScoreAfterMove.Value);
        }

        [TestMethod]
        public void TestCopyGame()
        {
            var game = new Game();
            game.New();
            var gameFile = GameFile.Load("TestGames\\mated.txt");
            foreach (var command in gameFile.MoveCommands)
                Assert.IsTrue(game.TryPossibleMoveCommand(command));

            var copy = game.Copy();
            Assert.AreNotSame(game, copy);
        }

        [TestMethod]
        public void TestMaterial()
        {
            var game = new Game();
            game.New();
            Assert.AreEqual(0, game.Material);
            Assert.IsTrue(game.TryStringMove("e2-e4"));
            Assert.AreEqual(0, game.Material);
            Assert.IsTrue(game.TryStringMove("d7-d5"));
            Assert.AreEqual(0, game.Material);
            Assert.IsTrue(game.TryStringMove("e4-d5"));
            Assert.AreEqual(-100, game.Material);
            game.UndoLastMove();
            Assert.AreEqual(0, game.Material);


        }

        [TestMethod]
        public void TestDrawByRepetion()
        {
            var game = new Game();
            game.New();
            //start position
            Assert.IsTrue(game.TryStringMove("b1-c3"));
            Assert.IsTrue(game.TryStringMove("b8-c6"));
            Assert.IsTrue(game.TryStringMove("c3-b1"));
            Assert.IsTrue(game.TryStringMove("c6-b8")); //start position
            Assert.IsTrue(game.TryStringMove("b1-c3"));
            Assert.IsTrue(game.TryStringMove("b8-c6"));
            Assert.IsTrue(game.TryStringMove("c3-b1"));
            Assert.IsFalse(game.Ended);
            Assert.IsTrue(game.TryStringMove("c6-b8")); //start position, three times
            Assert.IsTrue(game.Ended);
            Assert.IsNull(game.Winner);

        }

        [TestMethod]
        public void TestGetFEN()
        {
            var game = new Game();
            game.New();
            Assert.AreEqual("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", game.GetFEN());
            Assert.IsTrue(game.TryStringMove("e2-e4"));
            Assert.AreEqual("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1", game.GetFEN());
            Assert.IsTrue(game.TryStringMove("c7-c5"));
            Assert.AreEqual("rnbqkbnr/pp1ppppp/8/2p5/4P3/8/PPPP1PPP/RNBQKBNR w KQkq c6 0 2", game.GetFEN());
            Assert.IsTrue(game.TryStringMove("g1-f3"));
            Assert.AreEqual("rnbqkbnr/pp1ppppp/8/2p5/4P3/5N2/PPPP1PPP/RNBQKB1R b KQkq - 1 2", game.GetFEN());
        }

        [TestMethod]
        public void TestParseFEN()
        {
            var game = new Game();
            game.New();
            var fen = "rnbqkbnr/pp1ppppp/8/2p5/4P3/5N2/PPPP1PPP/RNBQKB1R b KQkq - 1 2";
            game.LoadFEN(fen);
            Assert.AreEqual(fen, game.GetFEN());

            game = new Game();
            game.New();
            fen = "rnbqkbnr/pp1ppppp/8/2p5/4P3/8/PPPP1PPP/RNBQKBNR w KQkq c6 0 2";
            game.LoadFEN(fen);
            Assert.AreEqual(fen, game.GetFEN());
        }

        [TestMethod]
        public void TestDrawby50moverule()
        {
            var game = new Game();
            game.New();
            var gameFile = GameFile.Load("TestGames\\drawby50moverule.txt");
            game.Load(gameFile);
            Assert.IsTrue(game.Ended);
            game.UndoLastMove();
            Assert.IsFalse(game.Ended);
            Assert.IsTrue(game.TryStringMove("h8-g8"));
            Assert.IsTrue(game.Ended);
            
        }
    }
}