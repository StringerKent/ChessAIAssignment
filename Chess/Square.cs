using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    public class Square
    {
        private Piece _piece;

        public Square(File file, Rank rank)
        {
            File = file;
            Rank = rank;
            var squareIndex =  (int)file + (int)rank*8;
            Bit = (ulong)1 << squareIndex;
            Index = squareIndex;
        }

        /// <summary>
        /// Bitwise operations can be used for faster calculations using this number.
        /// </summary>
        public ulong Bit { get; set; }

        public Color Color { get; set; }

        public Piece Piece
        {
            get { return _piece; }
            set
            {
                PieceType = value?.Type ?? (byte)Chess.PieceType.NoPiece;
                _piece = value; 
            }
        }

        public File File { get;}
        public Rank Rank { get;}

        public int Index { get; }

        private byte PieceType { get; set; }

        public void SetPiece(Piece piece)
        {
            if (Piece != null)
                Piece.Square = null;

            Piece = piece;
            if (piece != null)
                piece.Square = this;
        }

        public override string ToString()
        {
            return File.ToString().ToLower() + Rank.ToString().Replace("_", "");
        }
        
        internal Square[] NorthRayPatterns;

        internal Square[] NorthEastRayPatterns;
        
        internal Square[] EastRayPatterns;
        
        internal Square[] SouthEastRayPatterns;
        
        internal Square[] SouthRayPatterns;

        internal Square[] SouthWestPatterns;

        internal Square[] WestPatterns;

        internal Square[] NorthWestPatterns;
        
        internal Square[] KnightPatterns;

        internal Square[] KingPatterns;

        internal Square[] WhitePawnPatterns;

        internal Square[] BlackPawnPatterns;

        internal Square[] WhitePawnCapturePatterns;

        internal Square[] BlackPawnCapturePatterns;        
    }

    public enum File
    {
        A,B,C,D,E,F,G,H
    }

    public enum Rank
    {
        _1,_2,_3,_4,_5,_6,_7,_8
    }

    public enum Color
    {
        White,
        Black
    }
}
