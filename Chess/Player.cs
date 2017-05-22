using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    public class Player
    {
        public Player(Color color)
        {
            Color = color;
        }
        public List<Piece> Pieces { get; set; } = new List<Piece>();
        public Color Color { get; private set; }
        public Stack<Move> Moves { get; set; } = new Stack<Move>();
        public bool IsChecked { get; set; }
        public bool Mated { get; set; }
        public bool CanCastleQueenSide { get; set; }
        public bool CanCastleKingSide { get; set; }
        internal King King { get; set; }
        internal Pawn[] Pawns { get; set; }
        internal Piece[] KnightsBishops { get; set; }
        public Queen Queen { get; set; }
        internal int Material { get; set; }
        
        public override string ToString()
        {
            return $"{Color} player";
        }
        
        internal Player Copy()
        {
            return new Player(Color)
            {
                CanCastleKingSide = CanCastleKingSide,
                CanCastleQueenSide = CanCastleQueenSide,
                IsChecked = IsChecked,
                Mated = Mated
            };
        }

        internal string CastlingToFEN()
        {
            var rank = Rank._1;
            if (Color == Color.Black)
                rank = Rank._8;
            var sb = new StringBuilder();
            if (King.MoveCount == 0)
            {
                var kingRook = Pieces.OfType<Rook>()
                    .SingleOrDefault(p => p.Square?.Rank == rank && p.Square?.File == File.H);
                if (kingRook != null && kingRook.MoveCount == 0)
                    sb.Append(CanCastleKingSide ? "K" : "");
                var queenRook = Pieces.OfType<Rook>()
                    .SingleOrDefault(p => p.Square?.Rank == rank && p.Square?.File == File.A);
                if (queenRook != null && queenRook.MoveCount == 0)
                    sb.Append(CanCastleQueenSide ? "Q" : "");
            }

            var s = Color == Color.Black ? sb.ToString().ToLower() : sb.ToString();
            return s;
        }
    }
}
