using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace Chess
{
    public class Board
    {
        public Board() {
            Squares = new Square[64];
            var color = Color.Black;
            for (var f = File.A; f <= File.H; f++) {
                for (var r = Rank._1; r <= Rank._8; r++) {
                    Squares[(int)f + (int)r * 8] = new Square(f, r) { Color = color };
                    color = color == Color.Black ? Color.White : Color.Black;
                }
                color = color == Color.Black ? Color.White : Color.Black;
            }
            SetPatterns();
        }

        public Square[] Squares { get; private set; }

        public Square Square(File file, Rank rank) {
            return Squares[(int)file + (int)rank * 8];
        }

        internal void ClearPieces() {
            foreach (var square in Squares) {
                if (square.Piece != null)
                    square.Piece.Square = null;
                square.Piece = null;
            }
        }


        private void SetPatterns() {
            SetKnightPatterns();
            SetKingPatterns();
            SetWhitePawnPatterns();
            SetBlackPawnPatterns();
            SetWhitePawnCapturePatterns();
            SetBlackPawnCapturePatterns();

            foreach (var square in Squares) {
                square.NorthRayPatterns = SetRayPatterns(square, 1, 0);
                square.NorthEastRayPatterns = SetRayPatterns(square, 1, 1);
                square.EastRayPatterns = SetRayPatterns(square, 0, 1);
                square.SouthEastRayPatterns = SetRayPatterns(square, -1, 1);
                square.SouthRayPatterns = SetRayPatterns(square, -1, 0);
                square.SouthWestPatterns = SetRayPatterns(square, -1, -1);
                square.WestPatterns = SetRayPatterns(square, 0, -1);
                square.NorthWestPatterns = SetRayPatterns(square, 1, -1);
                square.KnightsPositionScore = SetKnightPositionScore(square);
            }
        }

        private int SetKnightPositionScore(Square square)
        {
            var r = square.Rank;
            var f = square.File;
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

        private void SetKnightPatterns() {
            for (int i = 0; i < 64; i++) {
                var pattern = Knight.GetPattern(i);
                Squares[i].KnightPatterns = new Square[pattern.Length];
                for (int j = 0; j < pattern.Length; j++)
                    Squares[i].KnightPatterns[j] = Squares[pattern[j]];
            }
        }

        private void SetKingPatterns() {
            for (int i = 0; i < 64; i++) {
                var pattern = King.GetPattern(i);
                Squares[i].KingPatterns = new Square[pattern.Length];
                for (int j = 0; j < pattern.Length; j++)
                    Squares[i].KingPatterns[j] = Squares[pattern[j]];
            }
        }

        private void SetWhitePawnPatterns() {
            for (int i = 0; i < 64; i++) {
                var pattern = Pawn.GetWhitePattern(i);
                Squares[i].WhitePawnPatterns = new Square[pattern.Length];
                for (int j = 0; j < pattern.Length; j++)
                    Squares[i].WhitePawnPatterns[j] = Squares[pattern[j]];
            }
        }

        private void SetBlackPawnPatterns() {
            for (int i = 0; i < 64; i++) {
                var pattern = Pawn.GetBlackPattern(i);
                Squares[i].BlackPawnPatterns = new Square[pattern.Length];
                for (int j = 0; j < pattern.Length; j++)
                    Squares[i].BlackPawnPatterns[j] = Squares[pattern[j]];
            }
        }

        private void SetWhitePawnCapturePatterns() {
            for (int i = 0; i < 64; i++) {
                var pattern = Pawn.GetWhiteCapturePattern(i);
                Squares[i].WhitePawnCapturePatterns = new Square[pattern.Length];
                for (int j = 0; j < pattern.Length; j++)
                    Squares[i].WhitePawnCapturePatterns[j] = Squares[pattern[j]];
            }
        }

        private void SetBlackPawnCapturePatterns() {
            for (int i = 0; i < 64; i++) {
                var pattern = Pawn.GetBlackCapturePattern(i);
                Squares[i].BlackPawnCapturePatterns = new Square[pattern.Length];
                for (int j = 0; j < pattern.Length; j++)
                    Squares[i].BlackPawnCapturePatterns[j] = Squares[pattern[j]];
            }
        }


        private Square[] SetRayPatterns(Square square, int rankDiff, int fileDiff) {

            var pattern = Piece.GetSquareRayIndexes(square.Index, rankDiff, fileDiff);
            var squares = new Square[pattern.Length];
            for (int j = 0; j < pattern.Length; j++)
                squares[j] = Squares[pattern[j]];
            return squares;
        }
    }
}
