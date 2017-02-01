using System;

namespace Chess
{
    public class MoveCommand
    {
        public MoveCommand(File fromFile, Rank fromRank, File toFile, Rank toRank)
        {
            FromFile = fromFile;
            FromRank = fromRank;
            ToFile = toFile;
            ToRank = toRank;
        }

        public MoveCommand(Move move)
        {
            FromFile = move.FromSquare.File;
            FromRank = move.FromSquare.Rank;
            ToFile = move.ToSquare.File;
            ToRank = move.ToSquare.Rank;
        }

        public Rank ToRank { get; private set; }

        public File ToFile { get; private set; }

        public Rank FromRank { get; private set; }

        public File FromFile { get; private set; }

        public static MoveCommand Parse(string command)
        {
            var split = command.Split('-');
            var fromFile = (File)Enum.Parse(typeof(File), split[0].Substring(0, 1).ToUpper());
            var fromRank = (Rank) Enum.Parse(typeof(Rank), "_" + split[0].Substring(1, 1));
            var toFile = (File)Enum.Parse(typeof(File), split[1].Substring(0, 1).ToUpper());
            var toRank = (Rank) Enum.Parse(typeof(Rank), "_" + split[1].Substring(1, 1).ToUpper());
            return new MoveCommand(fromFile, fromRank, toFile,toRank);
        }

        public override string ToString()
        {
            return $"{FromFile.ToString().ToLower()}{FromRank.ToString().Replace("_", "")}"+
                   $"-{ToFile.ToString().ToLower()}{ToRank.ToString().Replace("_", "")}";
        }
    }
}