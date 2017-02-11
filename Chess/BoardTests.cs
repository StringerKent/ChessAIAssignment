using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chess
{
    [TestClass]
    public class BoardTests
    {
        [TestMethod]
        public void BoardHasSquaresTest()
        {
            var board = new Board();
            Assert.AreEqual(64, board.Squares.Length);
        }
        
        [TestInitialize]
        public void Setup()
        {
            Game = new Game();
            Game.New();
        }

        private Game Game { get; set; }

        [TestMethod]
        public void TestMaterial()
        {
            Assert.AreEqual(0, Game.Material);
            Assert.IsTrue(Game.TryPossibleMoveCommand(new MoveCommand(File.D, Rank._2, File.D, Rank._4))); //d4
            Assert.IsTrue(Game.TryPossibleMoveCommand(new MoveCommand(File.E, Rank._7, File.E, Rank._5))); // - e5
            Assert.IsTrue(Game.TryPossibleMoveCommand(new MoveCommand(File.D, Rank._4, File.E, Rank._5))); //dxe5
            Assert.AreEqual(-100, Game.Material);
            Assert.IsTrue(Game.TryPossibleMoveCommand(new MoveCommand(File.F, Rank._7, File.F, Rank._6))); // - f6
            Assert.IsTrue(Game.TryPossibleMoveCommand(new MoveCommand(File.A, Rank._2, File.A, Rank._3))); //a3
            Assert.IsTrue(Game.TryPossibleMoveCommand(new MoveCommand(File.F, Rank._6, File.E, Rank._5))); // - fxe5
            Assert.AreEqual(0, Game.Material);
            Game.UndoLastMove(); // - fxe5
            Assert.AreEqual(-100, Game.Material);
            Game.UndoLastMove(); // a3
            Game.UndoLastMove(); // - f6
            Game.UndoLastMove(); //dxe5
            Assert.AreEqual(0, Game.Material);
        }

        [TestMethod]
        public void TestPawnPositionValue()
        {
            Assert.IsTrue(Game.TryPossibleMoveCommand(new MoveCommand(File.D, Rank._2, File.D, Rank._4)));
            Assert.IsTrue(Game.TryPossibleMoveCommand(new MoveCommand(File.D, Rank._7, File.D, Rank._5)));

            Assert.AreEqual(11, Game.Board.Square(File.D, Rank._4).Piece.PositionValue(Game));
            Assert.AreEqual(11, Game.Board.Square(File.D, Rank._5).Piece.PositionValue(Game));
        }

        [TestMethod]
        public void TestNightPositionValue()
        {
            Assert.IsTrue(Game.TryPossibleMoveCommand(new MoveCommand(File.G, Rank._1, File.F, Rank._3)));

            Assert.AreEqual(0, Game.Board.Square(File.F, Rank._3).Piece.PositionValue(Game));
            Assert.AreEqual(-3, Game.Board.Square(File.B, Rank._1).Piece.PositionValue(Game));
        }

        [TestMethod]
        public void TestBishopPositionValue()
        {
            Game.Reset();
            Game.AddPiece(File.E, Rank._1, new King(Color.White));
            Game.AddPiece(File.E, Rank._8, new King(Color.Black));

            Game.AddPiece(File.E, Rank._2, new Pawn(Color.White));//white square
            Game.AddPiece(File.F, Rank._3, new Pawn(Color.White));//white square
            Game.AddPiece(File.F, Rank._1, new Bishop(Color.White));//white square
            Game.SetInitials();
            var bishop = (Bishop)Game.Board.Square(File.F, (int)Rank._1).Piece;
            Assert.AreEqual(0, bishop.PositionValue(Game));
        }

        [TestMethod]
        public void TestRookAndKingPositionValue()
        {
            Game.Reset();
            Game.AddPiece(File.E, Rank._1, new King(Color.White));
            Game.AddPiece(File.E, Rank._8, new King(Color.Black));

            Game.AddPiece(File.A, Rank._1, new Rook(Color.White));
            Game.AddPiece(File.H, Rank._1, new Rook(Color.White));//white square
            Game.AddPiece(File.A, Rank._2, new Pawn(Color.White));//white square
            Game.SetInitials();
            var rookAsemi = (Rook)Game.Board.Square(File.A, (int)Rank._1).Piece;
            var rookHopen = (Rook)Game.Board.Square(File.H, (int)Rank._1).Piece;
            Assert.AreEqual(2, rookAsemi.PositionValue(Game));
            Assert.AreEqual(4, rookHopen.PositionValue(Game));

            var king = (King)Game.Board.Square(File.E,(int) Rank._1).Piece;
            Assert.AreEqual(0, king.PositionValue(Game));

            Assert.IsTrue(Game.TryPossibleMoveCommand(new MoveCommand(File.E, Rank._1, File.G, Rank._1))); //0-0
            Assert.AreEqual(5, king.PositionValue(Game));
        }

        [TestMethod]
        public void TestPerformance()
        {
            var watch = Stopwatch.StartNew();
            var loadCount = 2000000;
            var gameCount = 0;
            var rnd = new Random(40);
            var startHash = Game.Hash;
            while (Game.MoveCount < loadCount)
            {
                var moves = Game.MakeRandomMove(rnd);
                if (!moves || Game.WhitePlayer.Moves.Count > 40)
                {
                    while (Game.BlackPlayer.Moves.Count > 0 || Game.WhitePlayer.Moves.Count > 0)
                        Game.UndoLastMove();

                    gameCount++;
                    Assert.IsTrue(Game.WhitePlayer.Pieces.All(p => p.Square.Piece == p));
                    Assert.IsTrue(Game.BlackPlayer.Pieces.All(p => p.Square.Piece == p));
                    Assert.AreEqual(0, Game.Material);
                    Assert.AreEqual(Game.Hash, startHash);
                }
            }
            watch.Stop();
            Console.WriteLine(PositionsDatabase.Instance.ToString());

            var moveSpeed = Game.MoveCount / (double)watch.ElapsedMilliseconds * 1000;
            
            Console.WriteLine($"{loadCount} moves\r\n{gameCount} games\r\v{(int)moveSpeed} moves/sec");
#if DEBUG
            //var expectedSpeed = 150000;
#else
            var expectedSpeed = 300000;
#endif
//            Assert.IsTrue(moveSpeed > expectedSpeed);
            
        }


    }
}