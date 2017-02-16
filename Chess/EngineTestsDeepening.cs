using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chess
{
    [TestClass]
    public class EngineTestsDeepening
    {
        [TestInitialize]
        public void Setup()
        {
            Game = new Game();
            Game.New();
        }

        private Game Game { get; set; }
        
        [TestMethod]
        public void TestBestMoveStart()
        {
            var engine = new Engine();
            var move = engine.BestMoveDeepeningSearch(Game, TimeSpan.FromSeconds(30));
            Console.WriteLine(move);
            //Assert.AreEqual("Nc3", move.ToString());
        }
        
        [TestMethod]
        public void TestBestMoveQueenCaptureAlphaBeta()
        {
            var engine = new Engine();
            Assert.IsTrue(Game.TryPossibleMoveCommand(new MoveCommand(File.D, Rank._2, File.D, Rank._3)));
            Assert.IsTrue(Game.TryPossibleMoveCommand(new MoveCommand(File.E, Rank._7, File.E, Rank._6)));
            Assert.IsTrue(Game.TryPossibleMoveCommand(new MoveCommand(File.B, Rank._1, File.C, Rank._3)));
            Assert.IsTrue(Game.TryPossibleMoveCommand(new MoveCommand(File.D, Rank._8, File.G, Rank._5)));
            var evaluation = engine.BestMoveDeepeningSearch(Game, TimeSpan.FromSeconds(3));
            Console.WriteLine(evaluation);
            Assert.AreEqual("Bxg5", evaluation.Move.ToString());
        }

        [TestMethod]
        public void TestBestMoveBlackToPlay()
        {
            var engine = new Engine();
            Assert.IsTrue(Game.TryPossibleMoveCommand(new MoveCommand(File.B, Rank._1, File.C, Rank._3)));
            Assert.IsTrue(Game.TryPossibleMoveCommand(new MoveCommand(File.C, Rank._7, File.C, Rank._6)));
            Assert.IsTrue(Game.TryPossibleMoveCommand(new MoveCommand(File.C, Rank._3, File.D, Rank._5)));
            var evaluation = engine.BestMoveDeepeningSearch(Game, TimeSpan.FromSeconds(3));
            Console.WriteLine(evaluation);

            Assert.AreEqual("cxd5", evaluation.Move.ToString());
        }

        [TestMethod]
        public void TestAttackBlackQueen()
        {
            var engine = new Engine();
            Assert.IsTrue(Game.TryPossibleMoveCommand(new MoveCommand(File.E, Rank._2, File.E, Rank._4)));
            Assert.IsTrue(Game.TryPossibleMoveCommand(new MoveCommand(File.D, Rank._7, File.D, Rank._5)));
            Assert.IsTrue(Game.TryPossibleMoveCommand(new MoveCommand(File.E, Rank._4, File.D, Rank._5)));
            Assert.IsTrue(Game.TryPossibleMoveCommand(new MoveCommand(File.D, Rank._8, File.D, Rank._5)));
            Assert.IsTrue(Game.TryPossibleMoveCommand(new MoveCommand(File.B, Rank._1, File.C, Rank._3)));

            var evaluation = engine.BestMoveDeepeningSearch(Game, TimeSpan.FromSeconds(3));
            //not expecting a queen sacrifice
            Console.WriteLine(evaluation);

            Assert.AreNotEqual("Qxg2", evaluation.Move.ToString());
            Assert.AreNotEqual("Qxa2", evaluation.Move.ToString());
        }

        [TestMethod]
        public void TestMiniGameWhite()
        {
            Game.Reset();
            Game.AddPiece(File.H, Rank._1, new King(Color.White));
            Game.AddPiece(File.H, Rank._8, new King(Color.Black));
            Game.AddPiece(File.A, Rank._1, new Knight(Color.White));
            Game.AddPiece(File.B, Rank._3, new Pawn(Color.Black));
            Game.AddPiece(File.D, Rank._5, new Pawn(Color.Black));
            Game.SetInitials();
            PositionsDatabase.Instance.SetStartHash(Game);
            var ngine = new Engine();
            var evaluation = ngine.BestMoveDeepeningSearch(Game, TimeSpan.FromSeconds(3));
            Console.WriteLine(evaluation);

            Assert.AreEqual("Nxb3", evaluation.Move.ToString());
        }

        [TestMethod]
        public void TestMiniGameBlack()
        {
            Game.Reset();
            Game.AddPiece(File.H, Rank._1, new King(Color.White));
            Game.AddPiece(File.H, Rank._8, new King(Color.Black));
            Game.AddPiece(File.A, Rank._8, new Knight(Color.Black));
            Game.AddPiece(File.B, Rank._6, new Pawn(Color.White));
            Game.AddPiece(File.D, Rank._3, new Pawn(Color.White));
            Game.CurrentPlayer = Game.BlackPlayer;
            Game.SetInitials();
            PositionsDatabase.Instance.SetStartHash(Game);
            var ngine = new Engine();
            var evaluation = ngine.BestMoveDeepeningSearch(Game, TimeSpan.FromSeconds(3));
            Console.WriteLine(evaluation);

            Assert.AreEqual("Nxb6", evaluation.Move.ToString());
        }

        [TestMethod]
        public void TestBlackToPlay_WhiteMatesInOne()
        {
            LoadFile("TestGames\\mated.txt");

            var engine = new Engine();
            var evaluation = engine.BestMoveDeepeningSearch(Game, TimeSpan.FromSeconds(3));
            Console.WriteLine(evaluation);

            Assert.IsNotNull(evaluation);
        }

        [TestMethod]
        public void TestBlackToPlay_MatesInOne()
        {
            LoadFile("TestGames\\white_mated.txt");

            var engine = new Engine();
            var evaluation = engine.BestMoveDeepeningSearch(Game, TimeSpan.FromSeconds(3));
            Console.WriteLine(evaluation);

            Assert.IsNotNull(evaluation);
            Assert.AreEqual("Qh4#", evaluation.Move.ToString());
        }

        private void LoadFile(string fileName)
        {
            var gameFile = GameFile.Load(fileName);
            foreach (var moveCommand in gameFile.MoveCommands)
                Assert.IsTrue(Game.TryPossibleMoveCommand(moveCommand));

        }

        [TestMethod]
        public void WhiteToPlay_MatesInTwo_Deepening()
        {
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
            game.CurrentPlayer = game.WhitePlayer;
            game.SetInitials();
            var engine = new Engine();
            var move = engine.BestMoveDeepeningSearch(game, TimeSpan.FromSeconds(5));
            Console.WriteLine(move);
            Assert.AreEqual("Qxg6+", move.Move.ToString());
        }
        
        [TestMethod]
        public void BlackToPlay_MateInTwo_Deepening()
        {
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
            game.WhitePlayer.HasCastledKingSide = true;
            game.BlackPlayer.HasCastledKingSide = true;
            game.SetInitials();
            var engine = new Engine();
            var evaluation = engine.BestMoveDeepeningSearch(game, TimeSpan.FromSeconds(10));
            Console.WriteLine(evaluation);
            Assert.AreEqual("Rg1+", evaluation.Move.ToString());
        }

        [TestMethod]
        public void MateInThreeWhiteToPlay_Deepening()
        {
            var game = new Game();
            game.New();
            game.Reset();
            game
                .AddPiece("g8bK")
                .AddPiece("a7wR").AddPiece("b7wR").AddPiece("c7bP")
                .AddPiece("f6wK").AddPiece("g6wN")
                .AddPiece("f5wP")
                .AddPiece("b2bP").AddPiece("c3bR").AddPiece("e3bR").AddPiece("f3bN");
            
            game.WhitePlayer.HasCastledKingSide = true;
            game.BlackPlayer.HasCastledQueenSide = true;
            game.SetInitials();

            var engine = new Engine();
            var evaluation = engine.BestMoveDeepeningSearch(game, TimeSpan.FromSeconds(4));
            Console.WriteLine(evaluation.ToString());

            Assert.IsTrue(new [] {"Rb8+", "Ra8+"}.Contains(evaluation.Move.ToString()));
        }

        [TestMethod]
        public void MateInThree_BlackToPlay_Deepening()
        {
            var game = new Game();
            game.New();
            game.Reset();
            game
                .AddPiece("e1bQ")
                .AddPiece("h2wP").AddPiece("g2wQ").AddPiece("a2wR")
                .AddPiece("g3wR").AddPiece("f3wP").AddPiece("e3bB").AddPiece("b3wP")
                .AddPiece("g4wK").AddPiece("e4wP").AddPiece("c4wP").AddPiece("a4wP")
                .AddPiece("g5bP").AddPiece("e5bP").AddPiece("d5wB").AddPiece("a5bB")
                .AddPiece("g6bP").AddPiece("d6bP")
                .AddPiece("g7bK")
                .AddPiece("f8bR");
            //prevent generation of illegal moves.
            game.WhitePlayer.Pieces.ForEach(x => x.MoveCount = 2);
            game.BlackPlayer.Pieces.ForEach(x => x.MoveCount = 2);
            game.CurrentPlayer = game.BlackPlayer;
            game.SetInitials();
            var engine = new Engine();
            var move = engine.BestMoveDeepeningSearch(game, TimeSpan.FromSeconds(10));
            Console.WriteLine(move);
            Assert.AreEqual("Rf4+", move.Move.ToString());
        }

        //White mates in one black to play

        //Black mates in one white to play
    }
}