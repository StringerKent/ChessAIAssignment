using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Chess;
using Color = System.Drawing.Color;

namespace ChessApplication
{
    public class VisibleBoard
    {
        public VisibleBoard(Game game, Engine engine) {
            Game = game;
            Engine = engine;
        }

        public Game Game { get; }
        public Engine Engine { get; }

        public Dictionary<Square, RectangleF> Squares { get; set; } = new Dictionary<Square, RectangleF>();

        public float Left { get; set; }
        public float Top { get; set; }
        public float SquareSide { get; set; }
        public float Side { get; set; }
        public Square MouseDownSquare { get; set; }
        public Square MouseUpSquare { get; set; }
        public int MouseX { get; set; }
        public int MouseY { get; set; }
        public string LastMessage { get; set; }
        public List<Square> HiLights { get; set; } = new List<Square>();

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
            DrawLastMessage(g);
        }

        private void DrawLastMessage(Graphics g) {
            var text = LastMessage;
            var size = SquareSide / 4;
            size = size < 1 ? 1 : size;
            var labelFont = new Font(FontFamily.GenericSansSerif, size);

            g.DrawString(text, labelFont, Brushes.White, g.ClipBounds.Width / 2, 10);
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
                var x = Flipped ? Left + (Side - ((int)i + 1) * SquareSide) + SquareSide / 4 : Left + i * SquareSide + SquareSide / 4;
                var y = Top + SquareSide * 8;
                g.DrawString(labels[i].ToString(), labelFont, Brushes.White, x, y);
            }
        }

        private readonly SolidBrush _hiLightBrush = new SolidBrush(Color.FromArgb(196, SelectedColor));

        private void DrawSquares(Graphics g) {
            Squares.Clear();
            var size = SquareSide / 2;
            size = size < 1 ? 1 : size;
            var pieceFont = new Font(FontFamily.GenericSansSerif, size);
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
                        if (_offsetPiece == null || chessSquare.Piece != _offsetPiece.Item1)
                            g.DrawString(chessSquare.Piece.ImageChar.ToString(), pieceFont, Brushes.Black, x + SquareSide / 16, y + SquareSide / 4);
                    }

                    //g.DrawString(chessSquare.ToString(), new Font(FontFamily.GenericSansSerif, 12), Brushes.Red, x + SquareSide / 16,
                    //        y + SquareSide / 4);

                }
                if (MouseDownSquare?.Piece != null)
                    g.DrawString(MouseDownSquare.Piece.ImageChar.ToString(), pieceFont, Brushes.Black, MouseX - SquareSide / 2, MouseY - SquareSide / 2);
                if (_offsetPiece != null && _offsetPiece.Item1 != null)
                    g.DrawString(_offsetPiece.Item1.ImageChar.ToString(), pieceFont, Brushes.Black, _offsetPiece.Item2);

            }

        }

        internal bool Flipped { get; set; }

        public void MouseDown(MouseEventArgs e) {
            MouseDownSquare = GetSquare(e);
            if (MouseDownSquare == null)
                return;
            //var moves = Game.Copy().GetLegalNextMoves(0);
            var moves = Game.GetLegalUiMoves();
            var hiLights = moves.Where(x => x.FromSquare.ToString() == MouseDownSquare.ToString()).Select(x => x.ToSquare).ToList();
            HiLights =
                Game.Board.Squares
                    .Where(x => hiLights.Select(y => y.ToString()).Contains(x.ToString()))
                    .Select(x => x)
                    .ToList();
        }


        public void MouseUp(MouseEventArgs e) {
            HiLights.Clear();
            MouseUpSquare = GetSquare(e);
        }

        private Square GetSquare(MouseEventArgs e) {
            return Squares.SingleOrDefault(x => x.Value.Contains(e.X, e.Y)).Key;
            //var iFile = (int)(e.X - Left) / SquareSide;
            //if (iFile < 0 || iFile >= 8) return null;
            //var file = (File)iFile;

            //var side = SquareSide * 8;
            //var r = -((e.Y - Top - side) / SquareSide);
            //if (r < 0 || r >= 8) return null;

            //var rank = (Rank)(int)r;

            //return Squares.Single(x => x.Key.File == file && x.Key.Rank == rank).Key;
        }

        public void OffsetPiece(Piece piece, float x, float y) {
            _offsetPiece = new Tuple<Piece, PointF>(piece, new PointF(x + SquareSide / 16, y + SquareSide / 4));
        }

        private Tuple<Piece, PointF> _offsetPiece = null;
    }
}
