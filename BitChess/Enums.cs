using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitChess
{
    public enum PieceType
    {
        WhitePawn=0,
        WhiteKnight=1,
        WhiteBishop=2,
        WhiteRook,
        WhiteQueen,
        WhiteKing,
        BlackPawn,
        BlackKnight,
        BlackBishop,
        BlackRook,
        BlackQueen,
        BlackKing
    }

    public enum MovePattern
    {
        King,
        Knight,
        WhitePawn,
        WhitePawnAttacks,
        BlackPawn,
        BlackPawnAttacks,
        Rook,
        Bishop,
        Queen
    }
}
