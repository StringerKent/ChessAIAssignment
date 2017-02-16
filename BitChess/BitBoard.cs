using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitChess
{
    /*
       "56 57 58 59 60 61 62 63" +
       "48 49 50 51 52 53 54 55" +
       "40 41 42 43 44 45 46 47" +
       "32 33 34 35 36 37 38 39" +
       "24 25 26 27 28 29 30 31" +
       "16 17 18 19 20 21 22 23" +
       " 8  9 10 11 12 13 14 15" +
       " 0  1  2  3  4  5  6  7"
    */
    
    public class BitBoard
    {
        public BitBoard()
        {
            for (int i = 0; i < 64; i++)
                SquareBits[i] = (ulong)1 << i;
        }

        public ulong[] SquareBits { get; set; } = new ulong[64];
        public ulong[] PieceBoards { get; set; } = new ulong[12];

        public void SetStartPos()
        {
            ulong whitePawns = 0xFF; //now on first rank
            whitePawns <<= 8; //shift them up on second
            PieceBoards[(int)PieceType.WhitePawn] = whitePawns;

            ulong blackPawns = 0xFF;
            blackPawns <<= 48; //7th rank
            PieceBoards[(int)PieceType.BlackPawn] = blackPawns;

            
            PieceBoards[(int)PieceType.WhiteKnight] = SquareBits[1] | SquareBits[6];
            PieceBoards[(int)PieceType.BlackKnight] = SquareBits[57] | SquareBits[62];

            PieceBoards[(int)PieceType.WhiteBishop] = SquareBits[2] | SquareBits[5];
            PieceBoards[(int)PieceType.BlackBishop] = SquareBits[58] | SquareBits[61];

            PieceBoards[(int)PieceType.WhiteRook] = SquareBits[0] | SquareBits[7];
            PieceBoards[(int)PieceType.BlackRook] = SquareBits[56] | SquareBits[63];

            PieceBoards[(int)PieceType.WhiteQueen] = SquareBits[3];
            PieceBoards[(int)PieceType.BlackQueen] = SquareBits[59];

            PieceBoards[(int)PieceType.WhiteKing] = SquareBits[4];
            PieceBoards[(int)PieceType.BlackKing] = SquareBits[60];
        }

        internal string DebugPattern(PieceType pieceType)
        {
            return Convert.ToString((long)PieceBoards[(int)pieceType], 2);
        }
    }

   
}
