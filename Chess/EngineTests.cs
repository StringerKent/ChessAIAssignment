using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chess
{
    [TestClass]
    public class EngineTests
    {
        [TestInitialize]
        public void Setup() {
            Game = new Game();
            Game.New();
            Game.SetInitials();
        }

        private Game Game { get; set; }

        [TestMethod]
        public void TestBestMoveStart() {
            var engine = new Engine();
            var move = engine.BestMoveAtDepth(Game, 4);
            Console.WriteLine(move);
        }

        [TestMethod]
        public void TestBestMoveQueenCaptureAlphaBeta() {
            var engine = new Engine();
            Assert.IsTrue(Game.TryPossibleMoveCommand(new MoveCommand(File.D, Rank._2, File.D, Rank._3)));
            Assert.IsTrue(Game.TryPossibleMoveCommand(new MoveCommand(File.E, Rank._7, File.E, Rank._6)));
            Assert.IsTrue(Game.TryPossibleMoveCommand(new MoveCommand(File.B, Rank._1, File.C, Rank._3)));
            Assert.IsTrue(Game.TryPossibleMoveCommand(new MoveCommand(File.D, Rank._8, File.G, Rank._5)));
            var move = engine.BestMoveAtDepth(Game, 2);
            Console.WriteLine(move);
            Assert.AreEqual("Bxg5", move.Move.ToString());
        }

        [TestMethod]
        public void TestBestMoveBlackToPlay() {
            var engine = new Engine();
            Assert.IsTrue(Game.TryPossibleMoveCommand(new MoveCommand(File.B, Rank._1, File.C, Rank._3)));
            Assert.IsTrue(Game.TryPossibleMoveCommand(new MoveCommand(File.C, Rank._7, File.C, Rank._6)));
            Assert.IsTrue(Game.TryPossibleMoveCommand(new MoveCommand(File.C, Rank._3, File.D, Rank._5)));
            var move = engine.BestMoveAtDepth(Game, 3);
            Console.WriteLine(move);
            Assert.AreEqual("cxd5", move.Move.ToString());
        }

        [TestMethod]
        public void TestAttackBlackQueen() {
            var engine = new Engine();
            Assert.IsTrue(Game.TryPossibleMoveCommand(new MoveCommand(File.E, Rank._2, File.E, Rank._4)));
            Assert.IsTrue(Game.TryPossibleMoveCommand(new MoveCommand(File.D, Rank._7, File.D, Rank._5)));
            Assert.IsTrue(Game.TryPossibleMoveCommand(new MoveCommand(File.E, Rank._4, File.D, Rank._5)));
            Assert.IsTrue(Game.TryPossibleMoveCommand(new MoveCommand(File.D, Rank._8, File.D, Rank._5)));
            Assert.IsTrue(Game.TryPossibleMoveCommand(new MoveCommand(File.B, Rank._1, File.C, Rank._3)));
            var evaluation = engine.BestMoveAtDepth(Game, 2);
            Console.WriteLine(evaluation);
            //not expecting a queen sacrifice
            Assert.AreNotEqual("Qxg2", evaluation.Move.ToString());
            Assert.AreNotEqual("Qxa2", evaluation.Move.ToString());
            //Assert.AreEqual("Qe5+", evaluation.Move.ToString());
        }

        [TestMethod]
        public void TestMiniGameWhite() {
            Game.Reset();
            Game.AddPiece(File.H, Rank._1, new King(Color.White));
            Game.AddPiece(File.H, Rank._8, new King(Color.Black));
            Game.AddPiece(File.A, Rank._1, new Knight(Color.White));
            Game.AddPiece(File.B, Rank._3, new Pawn(Color.Black));
            Game.AddPiece(File.D, Rank._5, new Pawn(Color.Black));
            Game.WhitePlayer.CanCastleKingSide = false;
            Game.BlackPlayer.CanCastleKingSide = false;
            Game.SetInitials();
            var ngine = new Engine();
            var evaluation = ngine.BestMoveAtDepth(Game, 3);
            Console.WriteLine(evaluation);
            Assert.AreEqual("Nxb3", evaluation.Move.ToString());
        }

        [TestMethod]
        public void TestMiniGameBlack() {
            Game.Reset();
            Game.AddPiece(File.H, Rank._1, new King(Color.White));
            Game.AddPiece(File.H, Rank._8, new King(Color.Black));
            Game.AddPiece(File.A, Rank._8, new Knight(Color.Black));
            Game.AddPiece(File.B, Rank._6, new Pawn(Color.White));
            Game.AddPiece(File.D, Rank._3, new Pawn(Color.White));
            Game.CurrentPlayer = Game.BlackPlayer;
            Game.WhitePlayer.CanCastleKingSide = false;
            Game.BlackPlayer.CanCastleKingSide = false;
            Game.SetInitials();
            var ngine = new Engine();
            var evaluation = ngine.BestMoveAtDepth(Game, 4);
            Console.WriteLine(evaluation);
            Assert.AreEqual("Nxb6", evaluation.Move.ToString());
        }

        [TestMethod]
        public void TestBlackToPlay_WhiteMatesInOne() {
            LoadFile("TestGames\\mated.txt");
            var engine = new Engine();
            var evaluation = engine.BestMoveAtDepth(Game, 3);
            Console.WriteLine(evaluation);
            Assert.IsNotNull(evaluation);
        }

        [TestMethod]
        public void TestBlackToPlay_MatesInOne() {
            LoadFile("TestGames\\white_mated.txt");

            var engine = new Engine();
            var bestMove = engine.BestMoveAtDepth(Game, 2);
            Assert.IsNotNull(bestMove);
            Assert.AreEqual("Qh4#", bestMove.Move.ToString());
        }

        private void LoadFile(string fileName) {
            var gameFile = GameFile.Load(fileName);
            foreach (var moveCommand in gameFile.MoveCommands)
                Assert.IsTrue(Game.TryPossibleMoveCommand(moveCommand));

        }

        [TestMethod]
        public void WhiteToPlay_MatesInTwo() {
            //see image mate in two white.png
            var game = new Game();
            game.New();
            game.Reset();
            game
            .AddPiece("a8bR").AddPiece("b8bN").AddPiece("c8bB").AddPiece("d8bQ").AddPiece("e8bK").AddPiece("f8bB").AddPiece("g8bN")
            .AddPiece("a7bP").AddPiece("b7bP").AddPiece("c7bP").AddPiece("d7bP").AddPiece("e7bP").AddPiece("h7bP")
            .AddPiece("g6pP")
            .AddPiece("g5bP").AddPiece("h5bR")
            .AddPiece("d4wP").AddPiece("e4wQ").AddPiece("f4bP")
            .AddPiece("d3wB").AddPiece("e3wP").AddPiece("g3wB")
            .AddPiece("a2wP").AddPiece("b2wP").AddPiece("c2wP").AddPiece("f2wP").AddPiece("g2wP").AddPiece("h2wP")
            .AddPiece("a1wR").AddPiece("b1wN").AddPiece("e1wK").AddPiece("g1wN").AddPiece("h1wR");
            game.WhitePlayer.CanCastleKingSide = false;
            game.BlackPlayer.CanCastleKingSide = false;
            game.CurrentPlayer = game.WhitePlayer;
            game.SetInitials();
            var engine = new Engine();
            var evaluation = engine.BestMoveAtDepth(game, 3);
            Console.WriteLine(evaluation);

            Assert.AreEqual("Qxg6+", evaluation.Move.ToString());

        }

        [TestMethod]
        public void BlackToPlay_MateInTwo() {
            //see image mate in two black.png
            var game = new Game();
            game.New();
            game.Reset();
            game
            .AddPiece("a8bR").AddPiece("e8bK").AddPiece("f8bB")
            .AddPiece("c7bP").AddPiece("d7bP").AddPiece("f7bP")
            .AddPiece("a6bP").AddPiece("g6bQ")
            .AddPiece("b5bP").AddPiece("e5wP")
            .AddPiece("h4wQ")
            .AddPiece("b3wP").AddPiece("e3wB").AddPiece("f3bB").AddPiece("h3wP")
            .AddPiece("b2wP").AddPiece("c2wP").AddPiece("e2wN").AddPiece("f2wP").AddPiece("g2bR")
            .AddPiece("a1wR").AddPiece("e1wR").AddPiece("f1wK");
            game.CurrentPlayer = game.BlackPlayer;
            DisableCastling(game);
            
            game.SetInitials();
            var engine = new Engine();
            var timer = Stopwatch.StartNew();
            var evaluation = engine.BestMoveAtDepth(game, 3);
            timer.Stop();
#if DEBUG
            var timeLimit = 12000;
#else
            var timeLimit = 12000;
#endif
            Console.WriteLine(evaluation.ToString());

            Assert.AreEqual("Rg1+", evaluation.Move.ToString());
            //Assert.AreEqual("Rg1+ Nxg1 Qxg2#", move.ToString());
            Assert.IsTrue(timer.ElapsedMilliseconds < timeLimit, "timer.ElapsedMilliseconds < timeLimit");
        }

        internal static void DisableCastling(Game game)
        {
            game.WhitePlayer.CanCastleKingSide = false;
            game.BlackPlayer.CanCastleKingSide = false;
            game.WhitePlayer.CanCastleQueenSide = false;
            game.BlackPlayer.CanCastleQueenSide = false;
        }

        [TestMethod]
        public void MateInThreeWhiteToPlay() {
            var game = new Game();
            game.New();
            game.Reset();
            game
                .AddPiece("g8bK")
                .AddPiece("a7wR").AddPiece("b7wR").AddPiece("c7bP")
                .AddPiece("f6wK").AddPiece("g6wN")
                .AddPiece("f5wP")
                .AddPiece("b2bP").AddPiece("c3bR").AddPiece("e3bR").AddPiece("f3bN");
            DisableCastling(game);
            game.SetInitials();
            var engine = new Engine();
            var move = engine.BestMoveAtDepth(game, 5);
            Console.WriteLine(move);
            var expects = new[] { "Rb8+", "Ra8+" };
            Assert.IsTrue(expects.Contains(move.Move.ToString()));
        }

        [TestMethod]
        public void MateInThreeBlackToPlay() {
            var game = new Game();
            game.New();
            game.Reset();
            game
                .AddPiece("b8bK")
                .AddPiece("a7bP").AddPiece("g7bP").AddPiece("h7bP")
                .AddPiece("b6bP").AddPiece("d6bQ").AddPiece("f6bP")
                .AddPiece("c5bP")
                .AddPiece("d4wP").AddPiece("f4bN").AddPiece("g4wP").AddPiece("h4wP")
                .AddPiece("a3wQ").AddPiece("c3wP").AddPiece("f3wP").AddPiece("g3wK")
                .AddPiece("a2wP").AddPiece("e2bR")
                .AddPiece("g1wR").AddPiece("h1wR");

            game.WhitePlayer.CanCastleKingSide = false;
            game.BlackPlayer.CanCastleQueenSide = false;
            game.CurrentPlayer = game.BlackPlayer;
            game.SetInitials();
            var engine = new Engine();
            var move = engine.BestMoveAtDepth(game, 5);
            Console.WriteLine(move.ToString());
            Assert.AreEqual("Nh5+", move.Move.ToString());
        }

        [TestMethod]
        public void TestGamePerformance() {
#if DEBUG
            Assert.Fail("Run this test in release configuration");
#endif
            //var expectedNodes = 330000;
            var gameFile = GameFile.Load("TestGames\\performance1.txt");
            Game.Load(gameFile);
            var engine = new Engine();
            var evaluation = engine.BestMoveDeepeningSearch(Game, TimeSpan.FromSeconds(10));
            Console.WriteLine(evaluation);
            //Assert.IsTrue(evaluation.Nodes > expectedNodes);
        }


        [TestMethod]
        public void RookKingEndgame()
        {
            Game.New();
            Game.Reset();
            Game.AddPiece("e5bK").AddPiece("c3bR").AddPiece("g5wK");
            Game.CurrentPlayer = Game.BlackPlayer;
            Game.SetInitials();
            Game.BlackPlayer.CanCastleKingSide = true;
            Game.WhitePlayer.CanCastleKingSide = true;
            var engine = new Engine();
            var move = engine.BestMoveAtDepth(Game, 3);
            Console.WriteLine(move.ToString());
            Assert.AreEqual("Rg3+", move.Move.ToString());
        }

        [TestMethod]
        public void Perft()
        {
            Game.New();
            var engine = new Engine();
            var nodes = engine.Perft(Game, 5);
            Console.WriteLine(nodes);
        }
    }
}