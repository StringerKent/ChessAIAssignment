using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chess
{
    public class GameFile
    {
        private GameFile()
        {
            
        }

        public GameFile(Game game)
        {
            var allMoves = new List<Move>();
            allMoves.AddRange(game.WhitePlayer.Moves);
            allMoves.AddRange(game.BlackPlayer.Moves);
            allMoves = allMoves.OrderBy(move => move.NumberInGame).ThenBy(move => move.Piece.Color).ToList();
            foreach (var move in allMoves)
                MoveCommands.Add(new MoveCommand(move));
            InitialPosition = game.InitialPosition;
        }

        public string InitialPosition { get; set; }

        public List<MoveCommand> MoveCommands { get; set; } = new List<MoveCommand>();

        public void Save(string fileName)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(InitialPosition);
            MoveCommands.ForEach(x => stringBuilder.AppendLine(x.ToString()));
            System.IO.File.WriteAllText(fileName, stringBuilder.ToString());
        }

        public static GameFile Load(string fileName)
        {
            var text = System.IO.File.ReadAllText(fileName);
            var gameFile = new GameFile();
            var fileRows = text.Split('\r').ToList();
            gameFile.InitialPosition = fileRows.First();
            fileRows.RemoveAt(0);
            foreach (var row in fileRows)
            {
                if (string.IsNullOrEmpty(row.Trim()))
                    continue;
                gameFile.MoveCommands.Add(MoveCommand.Parse(row.Trim()));
            }
            return gameFile;
        }
    }

    [TestClass]
    public class GameFileTest
    {
        [TestMethod]
        public void TestSaveLoad()
        {
            var game = new Game();
            game.New();
            Assert.IsTrue(game.TryStringMove("e2-e4"));
            Assert.IsTrue(game.TryStringMove("e7-e5"));
            var gameFile = new GameFile(game);
            var fileName = @"gamefile.txt";
            gameFile.Save(fileName);

            var loaded = GameFile.Load(fileName);
            for (int i = 0; i < gameFile.MoveCommands.Count; i++)
            {
                Assert.AreEqual(gameFile.MoveCommands[i].ToString(), loaded.MoveCommands[i].ToString());
            }
        }
        
    }
}
