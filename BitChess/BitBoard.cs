using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            var firstBit = (ulong)1 << 63;
            for (int i = 0; i < 64; i++)
                SquareBits[i] = firstBit >> i;

            MovePatterns[(int)MovePattern.King] = new ulong[64];
            MovePatterns[(int)MovePattern.Knight] = new ulong[64];
            MovePatterns[(int)MovePattern.WhitePawn] = new ulong[64];
            MovePatterns[(int)MovePattern.BlackPawn] = new ulong[64];
            MovePatterns[(int)MovePattern.WhitePawnAttacks] = new ulong[64];
            MovePatterns[(int)MovePattern.BlackPawnAttacks] = new ulong[64];
            
            for (int i = 0; i < 64; i++)
            {
                MovePatterns[(int)MovePattern.King][i] = CalcKingMoves(i);
                MovePatterns[(int)MovePattern.Knight][i] = CalcKnightMoves(i);
                MovePatterns[(int)MovePattern.WhitePawn][i] = CalcWhitePawnMoves(i);
                MovePatterns[(int)MovePattern.BlackPawn][i] = CalcBlackPawnMoves(i);
                MovePatterns[(int)MovePattern.WhitePawnAttacks][i] = CalcWhitePawnAttacs(i);
                MovePatterns[(int)MovePattern.BlackPawnAttacks][i] = CalcBlackPawnAttacs(i);
            }            
        }

        private ulong CalcWhitePawnAttacs(int squareIndex)
        {
            var rank = squareIndex / 8;
            var file = squareIndex % 8;

            var list = new List<int?>();

            list.Add(ToSquareIndex(rank + 1, file - 1));
            list.Add(ToSquareIndex(rank + 1, file + 1));

            ulong moves = 0;
            foreach (var item in list.Where(x => x.HasValue))
                moves |= SquareBits[item.Value];

            return moves;
        }

        private ulong CalcBlackPawnAttacs(int squareIndex)
        {
            var rank = squareIndex / 8;
            var file = squareIndex % 8;

            var list = new List<int?>();

            list.Add(ToSquareIndex(rank - 1, file - 1));
            list.Add(ToSquareIndex(rank - 1, file + 1));

            ulong moves = 0;
            foreach (var item in list.Where(x => x.HasValue))
                moves |= SquareBits[item.Value];

            return moves;
        }

        private ulong[] SquareBits { get; set; } = new ulong[64];
        private ulong[] PieceBoards { get; set; } = new ulong[12];

        private ulong[][] MovePatterns { get; set; } = new ulong[12][];

        public void SetStartPos()
        {
            ulong whitePawns = 0;
            for (int i = 8; i < 16; i++)
                whitePawns |= SquareBits[i];
            PieceBoards[(int)PieceType.WhitePawn] = whitePawns;

            ulong blackPawns = 0;
            for (int i = 48; i < 56; i++)
                blackPawns |= SquareBits[i];
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



        internal static int? ToSquareIndex(int rank, int file)
        {
            if (file < 0 || file > 7)
                return null;
            if (rank < 0 || rank > 7)
                return null;
            return rank * 8 + file;
        }

        internal static void IndexToFileAndRank(int squareIndex, out int actualRank, out int actualFile)
        {
            actualFile = (squareIndex % 8);
            actualRank = (squareIndex / 8);
        }

        internal string DebugPattern(PieceType pieceType)
        {
            return DebugPattern(PieceBoards[(int)pieceType]);
        }
        internal string DebugPattern(ulong board)
        {
            var stringBuider = new StringBuilder();
            var temp = Convert.ToString((long)board, 2).PadLeft(64, '0');
            stringBuider.Append(temp.Substring(56, 8));
            stringBuider.Append(temp.Substring(48, 8));
            stringBuider.Append(temp.Substring(40, 8));
            stringBuider.Append(temp.Substring(32, 8));
            stringBuider.Append(temp.Substring(24, 8));
            stringBuider.Append(temp.Substring(16, 8));
            stringBuider.Append(temp.Substring(8, 8));
            stringBuider.Append(temp.Substring(0, 8));
            return stringBuider.ToString();
        }

        private ulong CalcKingMoves(int squareIndex)
        {

            var rank = squareIndex / 8;
            var file = squareIndex % 8;

            var list = new List<int?>();

            list.Add(ToSquareIndex(rank - 1, file - 1));
            list.Add(ToSquareIndex(rank - 1, file - 0));
            list.Add(ToSquareIndex(rank - 1, file + 1));
            list.Add(ToSquareIndex(rank, file - 1));
            list.Add(ToSquareIndex(rank, file + 1));
            list.Add(ToSquareIndex(rank + 1, file - 1));
            list.Add(ToSquareIndex(rank + 1, file));
            list.Add(ToSquareIndex(rank + 1, file + 1));

            ulong kingMoves = 0;
            foreach (var item in list.Where(x => x.HasValue))
                kingMoves |= SquareBits[item.Value];

            return kingMoves;
        }        

        private ulong CalcKnightMoves(int squareIndex)
        {
            var rank = squareIndex / 8;
            var file = squareIndex % 8;

            var list = new List<int?>();

            list.Add(ToSquareIndex(rank - 2, file - 1));
            list.Add(ToSquareIndex(rank - 2, file + 1));
            list.Add(ToSquareIndex(rank - 1, file + 2));
            list.Add(ToSquareIndex(rank - 1, file - 2));
            list.Add(ToSquareIndex(rank + 1, file - 2));
            list.Add(ToSquareIndex(rank + 1, file + 2));
            list.Add(ToSquareIndex(rank + 2, file - 1));
            list.Add(ToSquareIndex(rank + 2, file + 1));

            ulong moves = 0;
            foreach (var item in list.Where(x => x.HasValue))
                moves |= SquareBits[item.Value];

            return moves;
        }
        
        private ulong CalcWhitePawnMoves(int squareIndex)
        {
            var rank = squareIndex / 8;
            var file = squareIndex % 8;

            var list = new List<int?>();
            list.Add(ToSquareIndex(rank + 1, file));
            if (rank == 1)
                list.Add(ToSquareIndex(rank + 2, file));

            ulong moves = 0;
            foreach (var item in list.Where(x => x.HasValue))
                moves |= SquareBits[item.Value];

            return moves;
        }
               

        private ulong CalcBlackPawnMoves(int squareIndex)
        {
            var rank = squareIndex / 8;
            var file = squareIndex % 8;

            var list = new List<int?>();
            list.Add(ToSquareIndex(rank - 1, file));
            if (rank == 6)
                list.Add(ToSquareIndex(rank - 2, file));

            ulong moves = 0;
            foreach (var item in list.Where(x => x.HasValue))
                moves |= SquareBits[item.Value];

            return moves;
        }
               

        internal ulong KingMovePatternOfSquareIndex(int squareIndex)
        {
            return MovePatterns[(int)MovePattern.King][squareIndex];
        }

        internal ulong KnightMovePatternOfSquareIndex(int squareIndex)
        {
            return MovePatterns[(int)MovePattern.Knight][squareIndex];
        }

        internal ulong WhiteMovePatternOfSquareIndex(int squareIndex)
        {
            return MovePatterns[(int)MovePattern.WhitePawn][squareIndex];
        }

        internal ulong BlackMovePatternOfSquareIndex(int squareIndex)
        {
            return MovePatterns[(int)MovePattern.BlackPawn][squareIndex];
        }
        internal ulong WhiteAttacsMovePatternOfSquareIndex(int squareIndex)
        {
            return MovePatterns[(int)MovePattern.WhitePawnAttacks][squareIndex];
        }

        internal ulong BlackAttacsMovePatternOfSquareIndex(int squareIndex)
        {
            return MovePatterns[(int)MovePattern.BlackPawnAttacks][squareIndex];
        }

    }
}
