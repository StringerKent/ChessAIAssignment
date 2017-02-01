using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace Chess
{
    public class Board
    {
        public Board()
        {
            Squares = new Square[64];
            var color = Color.Black;
            for (var f = File.A; f <= File.H; f++)
            {
                for (var r = Rank._1; r <= Rank._8; r++)
                {
                    Squares[(int)f + (int)r * 8] = new Square(f, r) { Color = color };
                    color = color == Color.Black ? Color.White : Color.Black;
                }
                color = color == Color.Black ? Color.White : Color.Black;
            }
        }

        public Square[] Squares { get; private set; }

        public Square Square(File file, Rank rank)
        {
            return Squares[(int)file + (int)rank * 8];
        }
        
        internal void ClearPieces()
        {
            foreach (var square in Squares)
            {
                if (square.Piece != null)
                    square.Piece.Square = null;
                square.Piece = null;
            }
        }
    }
}
