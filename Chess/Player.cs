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
        public List<Move> Moves { get; set; } = new List<Move>();
        public bool IsChecked { get; set; }

        public bool Mated { get; set; }
        public bool HasCastledQueenSide { get; set; }
        public bool HasCastledKingSide { get; set; }
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
                HasCastledKingSide = HasCastledKingSide,
                HasCastledQueenSide = HasCastledQueenSide,
                IsChecked = IsChecked,
                Mated = Mated
            };
        }
    }
}
