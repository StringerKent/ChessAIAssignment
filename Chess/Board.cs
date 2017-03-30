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
            SetPatterns();
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


        private void SetPatterns()
        {
            SetKnightPatterns();
            SetKingPatterns();
            SetWhitePawnPatterns();
            SetBlackPawnPatterns();
            SetWhitePawnCapturePatterns();
            SetBlackPawnCapturePatterns();
            
            NorthRayPatterns = SetRayPatterns(1, 0);
            NorthEastRayPatterns = SetRayPatterns(1, 1);
            EastRayPatterns = SetRayPatterns(0, 1);
            SouthEastRayPatterns = SetRayPatterns(-1, 1);
            SouthRayPatterns = SetRayPatterns(-1, 0);
            SouthWestPatterns = SetRayPatterns(-1, -1);
            WestPatterns = SetRayPatterns(0, -1);
            NorthWestPatterns = SetRayPatterns(1, -1);            
        }

        internal Square[][] KnightPatterns;
        private void SetKnightPatterns()
        {
            KnightPatterns = new Square[64][];
            for (int i = 0; i < 64; i++)
            {
                var pattern = Knight.GetPattern(i);
                KnightPatterns[i] = new Square[pattern.Length];
                for (int j = 0; j < pattern.Length; j++)
                    KnightPatterns[i][j] = Squares[pattern[j]];
            }
        }

        internal Square[][] KingPatterns;
        private void SetKingPatterns()
        {
            KingPatterns = new Square[64][];
            for (int i = 0; i < 64; i++)
            {
                var pattern = King.GetPattern(i);
                KingPatterns[i] = new Square[pattern.Length];
                for (int j = 0; j < pattern.Length; j++)
                    KingPatterns[i][j] = Squares[pattern[j]];
            }
        }

        internal Square[][] WhitePawnPatterns;
        private void SetWhitePawnPatterns()
        {
            WhitePawnPatterns = new Square[64][];
            for (int i = 0; i < 64; i++)
            {
                var pattern = Pawn.GetWhitePattern(i);
                WhitePawnPatterns[i] = new Square[pattern.Length];
                for (int j = 0; j < pattern.Length; j++)
                    WhitePawnPatterns[i][j] = Squares[pattern[j]];
            }
        }

        internal Square[][] BlackPawnPatterns;
        private void SetBlackPawnPatterns()
        {
            BlackPawnPatterns = new Square[64][];
            for (int i = 0; i < 64; i++)
            {
                var pattern = Pawn.GetBlackPattern(i);
                BlackPawnPatterns[i] = new Square[pattern.Length];
                for (int j = 0; j < pattern.Length; j++)
                    BlackPawnPatterns[i][j] = Squares[pattern[j]];
            }
        }

        internal Square[][] WhitePawnCapturePatterns;
        private void SetWhitePawnCapturePatterns()
        {
            WhitePawnCapturePatterns = new Square[64][];
            for (int i = 0; i < 64; i++)
            {
                var pattern = Pawn.GetWhiteCapturePattern(i);
                WhitePawnCapturePatterns[i] = new Square[pattern.Length];
                for (int j = 0; j < pattern.Length; j++)
                    WhitePawnCapturePatterns[i][j] = Squares[pattern[j]];
            }
        }

        internal Square[][] BlackPawnCapturePatterns;
        private void SetBlackPawnCapturePatterns()
        {
            BlackPawnCapturePatterns = new Square[64][];
            for (int i = 0; i < 64; i++)
            {
                var pattern = Pawn.GetBlackCapturePattern(i);
                BlackPawnCapturePatterns[i] = new Square[pattern.Length];
                for (int j = 0; j < pattern.Length; j++)
                    BlackPawnCapturePatterns[i][j] = Squares[pattern[j]];
            }
        }

        internal Square[][] NorthRayPatterns;

        internal Square[][] NorthEastRayPatterns;
        
        internal Square[][] EastRayPatterns;
        
        internal Square[][] SouthEastRayPatterns;
        
        internal Square[][] SouthRayPatterns;

        internal Square[][] SouthWestPatterns;

        internal Square[][] WestPatterns;

        internal Square[][] NorthWestPatterns;

        
        private Square[][] SetRayPatterns(int rankDiff, int fileDiff)
        {
            var squares = new Square[64][];
            for (int i = 0; i < 64; i++)
            {
                var pattern = Piece.GetSquareRayIndexes(i, rankDiff, fileDiff);
                squares[i] = new Square[pattern.Length];
                for (int j = 0; j < pattern.Length; j++)
                    squares[i][j] = Squares[pattern[j]];
            }
            return squares;
        }
    }
}
