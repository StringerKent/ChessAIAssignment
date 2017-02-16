using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    public abstract class Piece
    {
        public Color Color { get; set; }
        public abstract char ImageChar { get; }
        public abstract char Char { get; }
        public Square Square { get; set; }
        public abstract int Value { get; }
        public byte Type { get; protected set; }
        public int MoveCount { get; set; }

        public abstract void AddPossibleMoves(Game game, List<Move> moves);
        public abstract void AddCaptures(Game game, List<Move> moves);

        public abstract int PositionValue(Game game);

        public abstract bool Attacks(Square piece, Board board);

        internal Square GetSquare(int rankDiff, int fileDiff, Game game) {
            return game.Board.Squares[((int)Square.File + fileDiff) + ((int)Square.Rank + rankDiff) * 8];
        }

        /// <summary>
        /// Gets a square relative to this piece. If it is outside the board it returns null.
        /// </summary>
        /// <param name="rankDiff"></param>
        /// <param name="fileDiff"></param>
        /// <param name="game"></param>
        /// <returns></returns>
        internal Square GetSquareSafe(int rankDiff, int fileDiff, Game game) {
            var file = Square.File + fileDiff;
            if (file < 0 || file > File.H)
                return null;
            var rank = Square.Rank + rankDiff;
            if (rank < 0 || rank > Rank._8)
                return null;

            return game.Board.Squares[(int)file + (int)rank * 8];
        }

        protected void AddMoves(int dirR, int dirF, Game game, List<Move> list) {
            var r = 0;
            var f = 0;

            while (true) {
                r += dirR;
                f += dirF;
                var toSqr = GetSquareSafe(r, f, game);
                if (toSqr == null)
                    break;

                if (toSqr.Piece != null) {
                    if (toSqr.Piece.Color != Color)
                        list.Add(new Move(this, toSqr));
                    break;
                }
                list.Add(new Move(this, toSqr));
            }
        }

        protected void AddCaptures(int dirR, int dirF, Game game, List<Move> list) {
            var r = 0;
            var f = 0;

            while (true) {
                r += dirR;
                f += dirF;
                var toSqr = GetSquareSafe(r, f, game);
                if (toSqr == null)
                    break;

                if (toSqr.Piece != null) {
                    if (toSqr.Piece.Color != Color) {
                        list.Add(new Move(this, toSqr));
                    }
                    break;
                }
            }
        }

        public override string ToString() {
            return $"{Color} {Char}";
        }

        public abstract Piece Copy(Square[] squares);

        protected Piece CopySquare(Square[] squares, Piece p) {
            if (Square != null) {
                p.Square = squares.Single(x => x.File == Square.File && x.Rank== Square.Rank);
                p.Square.Piece = p;
            }

            return p;
        }

        public string ToPositionString() {
            var c = Color == Color.White ? "w" : "b";
            return Square + c + Char;
        }
    }

    public class King : Piece
    {
        public King(Color color) {
            Color = color;
            Type = color == Color.White ? (byte)PieceType.WhiteKing : (byte)PieceType.BlackKing;
        }
        public override char ImageChar => Color == Color.White ? '♔' : '♚';
        public override char Char => 'K';
        public override int Value => 0;

        public override void AddPossibleMoves(Game game, List<Move> moves) {
            var possibilities = new[]
            {
                GetSquareSafe(-1, -1, game),
                GetSquareSafe(-1, 0, game),
                GetSquareSafe(-1, 1, game),
                GetSquareSafe(0, -1, game),
                GetSquareSafe(0, 1, game),
                GetSquareSafe(1, -1, game),
                GetSquareSafe(1, 0, game),
                GetSquareSafe(1, 1, game)
            };

            foreach (var toSqr in possibilities.Where(x => x != null)) {
                if (toSqr.Piece == null || toSqr.Piece.Color != Color)
                    moves.Add(new Move(this, toSqr));
            }
        }

        public override void AddCaptures(Game game, List<Move> moves) {
            var possibilities = new[]
            {
                GetSquareSafe(-1, -1, game),
                GetSquareSafe(-1, 0, game),
                GetSquareSafe(-1, 1, game),
                GetSquareSafe(0, -1, game),
                GetSquareSafe(0, 1, game),
                GetSquareSafe(1, -1, game),
                GetSquareSafe(1, 0, game),
                GetSquareSafe(1, 1, game)
            };

            foreach (var toSqr in possibilities.Where(x => x != null)) {
                if (toSqr.Piece != null && toSqr.Piece.Color != Color)
                    moves.Add(new Move(this, toSqr));
            }
        }

        public override int PositionValue(Game game) {
            if (HasCastled)
                return 5;
            return 0;
        }

        public override bool Attacks(Square square, Board board) {
            if (square.Piece != null && square.Piece.Color == Color)
                return false;
            var difR = Square.Rank - square.Rank;
            var difF = Square.File - square.File;
            if (Math.Abs(difR) > 1 || Math.Abs(difF) > 1)
                return false;
            return true;
        }

        public override Piece Copy(Square[] squares) {
            var p = new King(Color) {
                MoveCount = MoveCount,
                HasCastled = HasCastled
            };

            return CopySquare(squares, p);
        }

        public bool HasCastled { get; set; }
    }


    public class Queen : Piece
    {
        public Queen(Color color) {
            Color = color;
            Type = color == Color.White ? (byte)PieceType.WhiteQueen : (byte)PieceType.BlackQueen;
        }
        public override char ImageChar => Color == Color.White ? '♕' : '♛';
        public override char Char => 'Q';
        public override int Value => 900;

        public override void AddPossibleMoves(Game game, List<Move> moves) {

            AddMoves(-1, 1, game, moves);
            AddMoves(1, 1, game, moves);
            AddMoves(1, -1, game, moves);
            AddMoves(-1, -1, game, moves);

            AddMoves(1, 0, game, moves);
            AddMoves(-1, 0, game, moves);
            AddMoves(0, -1, game, moves);
            AddMoves(0, 1, game, moves);
        }

        public override void AddCaptures(Game game, List<Move> moves) {
            AddCaptures(-1, 1, game, moves);
            AddCaptures(1, 1, game, moves);
            AddCaptures(1, -1, game, moves);
            AddCaptures(-1, -1, game, moves);

            AddCaptures(1, 0, game, moves);
            AddCaptures(-1, 0, game, moves);
            AddCaptures(0, -1, game, moves);
            AddCaptures(0, 1, game, moves);
        }

        public override int PositionValue(Game game) {
            return 0;
        }

        public override bool Attacks(Square square, Board board) {
            return Rook.Attacks(this, square, board) || Bishop.Attacks(this, square, board);
        }

        public override Piece Copy(Square[] squares) {
            var p = new Queen(Color) {
                MoveCount = MoveCount
            };
            return CopySquare(squares, p);

        }
    }
    public class Rook : Piece
    {
        public Rook(Color color) {
            Color = color;
            Type = color == Color.White ? (byte)PieceType.WhiteRook : (byte)PieceType.BlackRook;
        }
        public override char ImageChar => Color == Color.White ? '♖' : '♜';
        public override char Char => 'R';

        public override int Value => 500;

        public override void AddPossibleMoves(Game game, List<Move> moves) {
            AddMoves(1, 0, game, moves);
            AddMoves(-1, 0, game, moves);
            AddMoves(0, -1, game, moves);
            AddMoves(0, 1, game, moves);
        }

        public override void AddCaptures(Game game, List<Move> moves) {
            AddCaptures(1, 0, game, moves);
            AddCaptures(-1, 0, game, moves);
            AddCaptures(0, -1, game, moves);
            AddCaptures(0, 1, game, moves);
        }

        public override int PositionValue(Game game) {
            var pawnCount = 0;
            var squares = game.Board.Squares;
            for (int i = (int)Square.File; i < 64; i += 8) {
                if (squares[i].Piece is Pawn)
                    pawnCount++;
            }
            if (pawnCount == 0) //open file
                return 4;
            if (pawnCount == 1) //semi open file
                return 2;
            return 0;
        }

        public override bool Attacks(Square square, Board board) {
            return Attacks(this, square, board);
        }

        internal static bool Attacks(Piece attacker, Square square, Board board) {
            if (square.Piece != null && square.Piece.Color == attacker.Color)
                return false;

            if (attacker.Square.Rank != square.Rank && attacker.Square.File != square.File)
                return false;

            //check pieces between
            var deltaF = square.File - attacker.Square.File;
            var deltaR = square.Rank - attacker.Square.Rank;
            var dirF = deltaF == 0 ? 0 : deltaF / Math.Abs(deltaF); //-1 or +1
            var dirR = deltaR == 0 ? 0 : deltaR / Math.Abs(deltaR); //-1 or +1

            var f = attacker.Square.File;
            var r = attacker.Square.Rank;
            while (!(f == square.File && r == square.Rank)) {
                f += dirF;
                r += dirR;
                var piece = board.Squares[(int)f + (int)r * 8].Piece;
                if (piece != null)
                    return piece.Square == square;
            }
            return true;
        }

        public override Piece Copy(Square[] squares) {
            var p = new Rook(Color) {
                MoveCount = MoveCount
            };
            return CopySquare(squares, p);

        }
    }

    public class Bishop : Piece
    {
        public Bishop(Color color) {
            Color = color;
            Type = color == Color.White ? (byte)PieceType.WhiteBishop : (byte)PieceType.BlackBishop;

        }

        public override char ImageChar => Color == Color.White ? '♗' : '♝';
        public override char Char => 'B';

        public override int Value => 300;

        public override void AddPossibleMoves(Game game, List<Move> moves) {
            AddMoves(1, 1, game, moves);
            AddMoves(-1, 1, game, moves);
            AddMoves(1, -1, game, moves);
            AddMoves(-1, -1, game, moves);
        }

        public override void AddCaptures(Game game, List<Move> moves) {
            AddCaptures(1, 1, game, moves);
            AddCaptures(-1, 1, game, moves);
            AddCaptures(1, -1, game, moves);
            AddCaptures(-1, -1, game, moves);
        }

        public override int PositionValue(Game game)
        {
            return 0;
            //var pawns = game.GetPlayer(this).Pawns;
            //Bad bishops.
            //Two minus points for every pawn on same square color as the bishops color.
            //return pawns.Count(p => p.Square?.Color == Color) * -2;
        }

        public override bool Attacks(Square square, Board board) {
            return Attacks(this, square, board);
        }

        internal static bool Attacks(Piece attacker, Square square, Board board) {
            if (square.Piece != null && square.Piece.Color == attacker.Color)
                return false;

            //check pieces between
            var deltaF = square.File - attacker.Square.File;
            var deltaR = square.Rank - attacker.Square.Rank;

            if (Math.Abs(deltaF) != Math.Abs(deltaR))
                return false;

            var dirF = deltaF / Math.Abs(deltaF); //-1 or +1
            var dirR = deltaR / Math.Abs(deltaR); //-1 or +1

            var f = attacker.Square.File;
            var r = attacker.Square.Rank;
            while (!(f == square.File && r == square.Rank)) {
                f += dirF;
                r += dirR;
                var piece = board.Squares[(int)f + (int)r * 8].Piece;
                if (piece != null)
                    return piece.Square == square;
            }
            return true;
        }

        public override Piece Copy(Square[] squares) {
            var p = new Bishop(Color) {
                MoveCount = MoveCount
            };
            return CopySquare(squares, p);
        }
    }

    public class Knight : Piece
    {
        public Knight(Color color) {
            Color = color;
            Type = color == Color.White ? (byte)PieceType.WhiteNight : (byte)PieceType.BlackKnight;

        }

        public override char ImageChar => Color == Color.White ? '♘' : '♞';
        public override char Char => 'N';

        public override int Value => 300;

        public override void AddPossibleMoves(Game game, List<Move> moves) {
            var possibilities = new[]
            {
                GetSquareSafe(-2, -1, game),
                GetSquareSafe(2, -1, game),
                GetSquareSafe(-2, 1, game),
                GetSquareSafe(2, 1, game),
                GetSquareSafe(-1, -2, game),
                GetSquareSafe(1, -2, game),
                GetSquareSafe(-1, 2, game),
                GetSquareSafe(1, 2, game)
            };

            foreach (var toSqr in possibilities.Where(x => x != null)) {
                if (toSqr.Piece == null || toSqr.Piece.Color != Color)
                    moves.Add(new Move(this, toSqr));
            }
        }

        public override void AddCaptures(Game game, List<Move> moves) {
            var possibilities = new[]
            {
                GetSquareSafe(-2, -1, game),
                GetSquareSafe(2, -1, game),
                GetSquareSafe(-2, 1, game),
                GetSquareSafe(2, 1, game),
                GetSquareSafe(-1, -2, game),
                GetSquareSafe(1, -2, game),
                GetSquareSafe(-1, 2, game),
                GetSquareSafe(1, 2, game)
            };

            foreach (var toSqr in possibilities.Where(x => x != null)) {
                if (toSqr.Piece != null && toSqr.Piece.Color != Color)
                    moves.Add(new Move(this, toSqr));
            }
        }

        public override int PositionValue(Game game) {
            var r = Square.Rank;
            var f = Square.File;
            var score = 0;
            if (r == 0 || r == Rank._8)
                score -= 2; //knight on the rim
            else if (r == Rank._2 || r == Rank._7)
                score -= 1;

            if (f == 0 || f == File.H)
                score -= 2;
            else if (f == File.G || f == File.B)
                score -= 1;

            return score;
        }

        public override bool Attacks(Square square, Board board) {
            if (square.Piece != null && square.Piece.Color == Color)
                return false;

            //Check pieces between
            var deltaF = Square.File - square.File;
            var deltaR = Square.Rank - square.Rank;
            var absF = Math.Abs(deltaF);
            var absR = Math.Abs(deltaR);
            if (!((absF == 1 && absR == 2) || (absF == 2 && absR == 1)))
                return false;

            return true;
        }

        public override Piece Copy(Square[] squares) {
            var p = new Knight(Color) {
                MoveCount = MoveCount
            };

            return CopySquare(squares, p);
        }
    }

    public class Pawn : Piece
    {
        public Pawn(Color color) {
            Color = color;
            Type = color == Color.White ? (byte)PieceType.WhitePawn : (byte)PieceType.BlackPawn;

        }
        public override char ImageChar => Color == Color.White ? '♙' : '♟';
        public override char Char => 'P';

        public override int Value => 100;

        public override void AddPossibleMoves(Game game, List<Move> moves) {
            var pawnMoves = new List<Move>();
            var rankDiff = 1;
            if (Color == Color.Black)
                rankDiff = -1;

            var toSquare = GetSquare(rankDiff, 0, game);
            if (toSquare.Piece == null)
                pawnMoves.Add(new Move(this, toSquare));

            if ((Color == Color.White && Square.Rank == Rank._2) ||
                (Color == Color.Black && Square.Rank == Rank._7)) {
                var nextSquare = GetSquare(rankDiff * 2, 0, game);
                if (nextSquare.Piece == null && toSquare.Piece == null)
                    pawnMoves.Add(new Move(this, nextSquare));
            }

            //captures
            if (Square.File != File.H) {
                var toSqr = GetSquare(rankDiff, 1, game);
                if (toSqr.Piece != null && toSqr.Piece.Color != Color)
                    pawnMoves.Add(new Move(this, toSqr));
            }

            if (Square.File != File.A) {
                var toSqr = GetSquare(rankDiff, -1, game);
                if (toSqr.Piece != null && toSqr.Piece.Color != Color)
                    pawnMoves.Add(new Move(this, toSqr));
            }

            GetPassants(game, pawnMoves);
            SetPromotions(pawnMoves);
            moves.AddRange(pawnMoves);
        }

        public override void AddCaptures(Game game, List<Move> moves) {
            var pawnMoves = new List<Move>();
            var rankDiff = 1;
            if (Color == Color.Black)
                rankDiff = -1;

            if (Square.File != File.H) {
                var toSqr = GetSquare(rankDiff, 1, game);
                if (toSqr.Piece != null && toSqr.Piece.Color != Color)
                    pawnMoves.Add(new Move(this, toSqr));
            }

            if (Square.File != File.A) {
                var toSqr = GetSquare(rankDiff, -1, game);
                if (toSqr.Piece != null && toSqr.Piece.Color != Color)
                    pawnMoves.Add(new Move(this, toSqr));
            }

            GetPassants(game, pawnMoves);
            SetPromotions(pawnMoves);
            moves.AddRange(pawnMoves);
        }

        public override int PositionValue(Game game) {
            if (Color == Color.White) {
                return (Square.File == File.D && Square.Rank == Rank._4) || (Square.File == File.E && Square.Rank == Rank._4) ? 11 : 0;
            } else {
                return (Square.File == File.D && Square.Rank == Rank._5) || (Square.File == File.E && Square.Rank == Rank._5) ? 11 : 0;
            }
        }

        public override bool Attacks(Square square, Board board) {
            if (square.Piece != null && square.Piece.Color == Color)
                return false;

            var rDiff = Square.Piece.Color == Color.White ? -1 : 1;
            if (Square.Rank - square.Rank != rDiff)
                return false;

            var fDiff = Math.Abs(Square.File - square.File);
            if (fDiff != 1)
                return false;

            return true;
        }

        public override Piece Copy(Square[] squares) {
            var p = new Pawn(Color) {
                MoveCount = MoveCount
            };
            return CopySquare(squares, p);
        }

        private void SetPromotions(IEnumerable<Move> moves) {
            var rank = Rank._7;
            if (Color == Color.Black)
                rank = Rank._2;

            foreach (var move in moves.Where(x => x.FromSquare.Rank == rank))
                move.IsPromotion = true;
        }

        private void GetPassants(Game game, List<Move> moves) {

            if (!game.EnPassantFile.HasValue)
                return;
            //var epFile = game.EnPassantFile.Value;
            //var fileDiff = Square.File - epFile;
            //if (Math.Abs(fileDiff) != 1)
            //    return;

            //var rank = Color == Color.White ? Rank._5 : Rank._4;
            //var rankDiff = Color == Color.White ? 1 : -1;
            //if (Square.Rank != rank)
            //    return;
            ////we now know the capturing pawn is in the correct place
            //var capture = (Pawn)game.Board.Square(epFile, Square.Rank).Piece;
            //var squareBehindPawn = capture.GetSquare(rankDiff, 0, game);

            //moves.Add(new Move(this, squareBehindPawn, isEnPassant: true, capturedEnPassant: capture));

            var oppLastMovedPiece = game.OtherPlayer.Moves.FirstOrDefault()?.Piece;
            if (!(oppLastMovedPiece is Pawn))
                return;

            var rank = Color == Color.White ? Rank._5 : Rank._4;
            if (Square.Rank != rank)
                return;

            if (Color == Color.White) {
                if (Square.Rank == Rank._5) {
                    var toBeCapR = GetSquareSafe(0, 1, game)?.Piece;
                    if (oppLastMovedPiece == toBeCapR)
                        if (toBeCapR is Pawn && toBeCapR.MoveCount == 1)
                            moves.Add(new Move(this, GetSquare(1, 1, game), false, true, toBeCapR));

                    var toBeCapL = GetSquareSafe(0, -1, game)?.Piece;
                    if (oppLastMovedPiece == toBeCapL)
                        if (toBeCapL is Pawn && toBeCapL.MoveCount == 1)
                            moves.Add(new Move(this, GetSquare(1, -1, game), false, true, toBeCapL));
                }
            } else {
                if (Square.Rank == Rank._4) {
                    var toBeCapR = GetSquareSafe(0, 1, game)?.Piece;
                    if (oppLastMovedPiece == toBeCapR)
                        if (toBeCapR is Pawn && toBeCapR.MoveCount == 1)
                            moves.Add(new Move(this, GetSquare(-1, 1, game), false, true, toBeCapR));

                    var toBeCapL = GetSquareSafe(0, -1, game)?.Piece;
                    if (oppLastMovedPiece == toBeCapL)
                        if (toBeCapL is Pawn && toBeCapL.MoveCount == 1)
                            moves.Add(new Move(this, GetSquare(-1, -1, game), false, true, toBeCapL));
                }
            }
        }
    }

    public enum PieceType : byte
    {
        NoPiece = 0,
        WhiteKing = 1,
        WhiteQueen = 2,
        WhiteRook = 3,
        WhiteBishop = 4,
        WhiteNight = 5,
        WhitePawn = 6,
        BlackKing = 7,
        BlackQueen = 8,
        BlackRook = 9,
        BlackBishop = 10,
        BlackKnight = 11,
        BlackPawn = 12
    }
}
