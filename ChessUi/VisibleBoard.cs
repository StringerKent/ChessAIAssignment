using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;
using Chess;
using ChessUi.Properties;
using Color = System.Drawing.Color;

namespace ChessUi
{
    public class VisibleBoard
    {
        public VisibleBoard(Game game, Engine engine) {
            Game = game;
            Engine = engine;
        }

        private Game Game { get; }
        private Engine Engine { get; }

        public Dictionary<Square, RectangleF> Squares { get; set; } = new Dictionary<Square, RectangleF>();

        private float Left { get; set; }
        private float Top { get; set; }
        private float SquareSide { get; set; }
        private float Side { get; set; }
        public Square MouseDownSquare { get; set; }
        public Square MouseUpSquare { get; set; }
        public int MouseX { private get; set; }
        public int MouseY { private get; set; }
        private List<Square> HiLights { get; set; } = new List<Square>();

        public static Color SelectedColor = Color.FromArgb(255, 204, 232, 255);

        public void Paint(Graphics g) {
            if (Game == null)
                return;

            g.CompositingQuality = CompositingQuality.HighQuality;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.Clear(Color.Black);
            var width = g.VisibleClipBounds.Width;
            var height = g.VisibleClipBounds.Height;
            Side = height < width ? height : width;
            Side -= Side / 6;
            var center = new PointF(width / 2f, height / 2f);

            Left = center.X - Side / 2;
            Top = center.Y - Side / 2;
            SquareSide = Side / 8;

            DrawSquares(g);
            DrawLabels(g);
            DrawToPlay(g);
        }
        
        private void DrawToPlay(Graphics g) {
            var doing = " to play";
            if (Engine.ThinkingFor != null)
                doing = " thinking";
            var text = $"{Game.CurrentPlayer.Color} {doing}" + (Game.CurrentPlayer.IsChecked ? " (checked)" : "");
            var size = SquareSide / 4;
            size = size < 1 ? 1 : size;
            var labelFont = new Font(FontFamily.GenericSansSerif, size);

            var player = Game.CurrentPlayer;
            if (Flipped)
                player = Game.OtherPlayer;
            if (player == Game.WhitePlayer)
                g.DrawString(text, labelFont, Brushes.White, 10, g.ClipBounds.Height - SquareSide / 2);
            else
                g.DrawString(text, labelFont, Brushes.White, 10, 10);
        }

        private void DrawLabels(Graphics g) {
            var size = SquareSide / 4;
            size = size < 1 ? 1 : size;
            var labelFont = new Font(FontFamily.GenericSansSerif, size);
            for (int i = 0; i < 8; i++) {
                var x = Left - SquareSide / 3;
                var y = Flipped ? Top + Side - ((i + 1) * SquareSide) + SquareSide / 4 : Top + SquareSide * i + SquareSide / 4;
                g.DrawString((8 - i).ToString(), labelFont, Brushes.White, x, y);
            }

            var labels = "abcdefgh".ToCharArray();
            for (int i = 0; i < 8; i++) {
                var x = Flipped ? Left + (Side - (i + 1) * SquareSide) + SquareSide / 4 : Left + i * SquareSide + SquareSide / 4;
                var y = Top + SquareSide * 8;
                g.DrawString(labels[i].ToString(), labelFont, Brushes.White, x, y);
            }
        }

        private readonly SolidBrush _hiLightBrush = new SolidBrush(Color.FromArgb(150, SelectedColor));

        private void DrawSquares(Graphics g) {
            Squares.Clear();
            for (var r = Rank._1; r <= Rank._8; r++) {
                for (var f = File.A; f <= File.H; f++) {
                    var x = Flipped ? Left + (Side - ((int)f + 1) * SquareSide) : Left + (int)f * SquareSide;
                    var y = Flipped ? Top + ((int)r * SquareSide) : Top + Side - ((int)r + 1) * SquareSide;
                    var chessSquare = Game.Board.Square(f, r);
                    var brush = chessSquare.Color == Chess.Color.Black ? Brushes.Gray : Brushes.AntiqueWhite;

                    if (MouseDownSquare == chessSquare)
                        brush = new SolidBrush(ControlPaint.LightLight(((SolidBrush)brush).Color));

                    var rect = new RectangleF(x, y, SquareSide, SquareSide);
                    g.FillRectangle(brush, rect);
                    if (HiLights.Contains(chessSquare))
                        g.FillRectangle(_hiLightBrush, rect);

                    Squares.Add(chessSquare, rect);
                    if (chessSquare.Piece != null && MouseDownSquare != chessSquare) {
                        if (_animationOffset == null || chessSquare.Piece != _animationOffset.Item1)
                            DrawPiece(chessSquare, rect, g);
                    }

                    //g.DrawString(chessSquare.ToString(), new Font(FontFamily.GenericSansSerif, 12), Brushes.Red, x + SquareSide / 16,
                    //        y + SquareSide / 4);

                }
                if (MouseDownSquare?.Piece != null)
                {
                    var x = MouseX - SquareSide / 2;
                    var y = MouseY - SquareSide / 2;
                    var rect = new RectangleF(x, y, SquareSide, SquareSide);
                    DrawPiece(MouseDownSquare, rect, g);
                }


                if (_animationOffset != null && _animationOffset.Item1 != null)
                {
                    var rect = new RectangleF(_animationOffset.Item2.X, _animationOffset.Item2.Y, SquareSide * 1.1f, SquareSide * 1.1f);
                    DrawPiece(_animationOffset.Item1.Square, rect, g);
                }
            }
        }

        private void DrawPiece(Square square, RectangleF rect, Graphics g)
        {
            if (PieceImage == PieceImage.Regular)
            {
                var image = GetImage(square.Piece);
                var imageRect = new RectangleF(rect.Left, rect.Top, rect.Width, rect.Height);
                imageRect.Inflate(-2, -2);
                g.DrawImage(image, imageRect);
            }
            else
            {
                var size = SquareSide / 2;
                size = size < 1 ? 1 : size;
                rect.Offset(0, size / 3);
                var pieceFont = new Font(FontFamily.GenericSansSerif, size);
                g.DrawString(square.Piece.ImageChar.ToString(), pieceFont, Brushes.Black, rect);
            }
        }
    

        private Image GetImage(Piece piece)
        {
            if (piece.Color == Chess.Color.Black)
            {
                if (piece is Pawn)
                    return Resources.BlackPawn;
                if (piece is Bishop)
                    return Resources.BlackBishop;
                if (piece is Knight)
                    return Resources.BlackKnight;
                if (piece is Rook)
                    return Resources.BlackRook;
                if (piece is Queen)
                    return Resources.BlackQueen;
                if (piece is King)
                    return Resources.BlackKing;
            }
            else
            {
                if (piece is Pawn)
                    return Resources.WhitePawn;
                if (piece is Bishop)
                    return Resources.WhiteBishop;
                if (piece is Knight)
                    return Resources.WhiteKnight;
                if (piece is Rook)
                    return Resources.WhiteRook;
                if (piece is Queen)
                    return Resources.WhiteQueen;
                if (piece is King)
                    return Resources.WhiteKing;
            }

            throw new ApplicationException("Unknown piece type");
        }

        internal bool Flipped { get; set; }
        public PieceImage PieceImage { get; internal set; } = PieceImage.Regular;
        public bool EditMode { get; internal set; }

        public void MouseDown(int x, int y) {
            MouseDownSquare = GetSquare(x, y);
            if (MouseDownSquare == null)
                return;

            if (!EditMode)
            {
                var moves = Game.GetLegalUiMoves();
                var hiLights =
                    moves.Where(m => m.FromSquare.ToString() == MouseDownSquare.ToString())
                        .Select(m => m.ToSquare)
                        .ToList();
                HiLights =
                    Game.Board.Squares
                        .Where(s => hiLights.Select(h => h.ToString()).Contains(s.ToString()))
                        .Select(s => s)
                        .ToList();
            }
        }


        public void MouseUp(int x, int y) {
            HiLights.Clear();
            MouseUpSquare = GetSquare(x, y);
        }

        private Square GetSquare(int x, int y) {
            return Squares.SingleOrDefault(sqr => sqr.Value.Contains(x, y)).Key;            
        }

        public void OffsetAnimated(Piece piece, float x, float y) {
            _animationOffset = new Tuple<Piece, PointF>(piece, new PointF(x + SquareSide / 16, y + SquareSide / 4));
        }

        private Tuple<Piece, PointF> _animationOffset;

        internal void Drop(PieceType pieceType, int x, int y)
        {
            var square = GetSquare(x, y);
            Game.AddPiece(square, pieceType);
        }
    }

    public enum PieceImage
    {
        Newspaper,
        Regular
    }
}
