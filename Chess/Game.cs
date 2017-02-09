using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net.Configuration;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using Microsoft.Win32;

namespace Chess
{
    public class Game
    {
        public Board Board { get; private set; }
        public Player WhitePlayer { get; private set; }
        public Player BlackPlayer { get; private set; }
        public Player CurrentPlayer { get; set; }
        public Player OtherPlayer => CurrentPlayer == WhitePlayer ? BlackPlayer : WhitePlayer;
        public ulong Hash { get; set; }
        public string InitialPosition { get; private set; }

        public bool Ended { get; set; }
        public bool IsStaleMate { get; private set; }
        public Player Winner { get; set; }
        private readonly Stack<ulong> HashHistory = new Stack<ulong>();
        //50 moves rule
        private int MovesSinceLastCaptureOrPawnMove = 0;

        public void New() {
            PositionsDatabase.Instance.Reset();
            Board = new Board();
            WhitePlayer = new Player(Color.White);
            BlackPlayer = new Player(Color.Black);

            AddPiece(File.A, Rank._1, new Rook(Color.White));
            AddPiece(File.B, Rank._1, new Knight(Color.White));
            AddPiece(File.C, Rank._1, new Bishop(Color.White));
            AddPiece(File.D, Rank._1, new Queen(Color.White));
            AddPiece(File.E, Rank._1, new King(Color.White));
            AddPiece(File.F, Rank._1, new Bishop(Color.White));
            AddPiece(File.G, Rank._1, new Knight(Color.White));
            AddPiece(File.H, Rank._1, new Rook(Color.White));

            for (File i = 0; i <= File.H; i++)
                AddPiece(i, Rank._2, new Pawn(Color.White));

            AddPiece(File.A, Rank._8, new Rook(Color.Black));
            AddPiece(File.B, Rank._8, new Knight(Color.Black));
            AddPiece(File.C, Rank._8, new Bishop(Color.Black));
            AddPiece(File.D, Rank._8, new Queen(Color.Black));
            AddPiece(File.E, Rank._8, new King(Color.Black));
            AddPiece(File.F, Rank._8, new Bishop(Color.Black));
            AddPiece(File.G, Rank._8, new Knight(Color.Black));
            AddPiece(File.H, Rank._8, new Rook(Color.Black));

            for (File i = 0; i <= File.H; i++)
                AddPiece(i, Rank._7, new Pawn(Color.Black));

            CurrentPlayer = WhitePlayer;
            SetPieceFastAccess();
            Ended = false;
            IsStaleMate = false;
            Winner = null;
            BlackPlayer.Material = 0;
            WhitePlayer.Material = 0;
            SetInitials();
            HashHistory.Clear();
            PositionsDatabase.Instance.SetStartHash(this);
            HashHistory.Push(Hash);
        }

        public void Load(GameFile gameFile) {
            Reset();
            InitialPosition = gameFile.InitialPosition;
            var positionItem = gameFile.InitialPosition.Split(',').ToList();
            Debug.Assert(positionItem.First() == "Start");
            var i = 0;
            while (true) {
                i++;
                if (positionItem[i] == "White" || positionItem[i] == "Black")
                    break;
                this.AddPiece(positionItem[i]);
            }
            if (positionItem[i] == "White")
                CurrentPlayer = WhitePlayer;
            else {
                Debug.Assert(positionItem[i] == "Black");
                CurrentPlayer = WhitePlayer;
            }
            i++;
            for (int j = i; j < positionItem.Count; j++) {
                if (positionItem[j] == "WCK")
                    WhitePlayer.HasCastledKingSide = true;
                if (positionItem[j] == "WCQ")
                    WhitePlayer.HasCastledQueenSide = true;
                if (positionItem[j] == "BCK")
                    BlackPlayer.HasCastledQueenSide = true;
                if (positionItem[j] == "BCQ")
                    BlackPlayer.HasCastledQueenSide = true;
                SetPieceFastAccess();
                if (positionItem[j].StartsWith("ENP:")) {
                    var split = positionItem[j].Split(':');
                    EnPassantFile = (File)Enum.Parse(typeof(File), split[1]);
                }
            }

            PositionsDatabase.Instance.SetStartHash(this);
            InitialMaterial(WhitePlayer);
            InitialMaterial(BlackPlayer);
            foreach (var moveCommand in gameFile.MoveCommands) {
                if (!TryPossibleMoveCommand(moveCommand))
                    throw new ApplicationException("Invalid game file");
            }
        }

        public void Save(string fileName) {
            var gameFile = new GameFile(this);
            gameFile.Save(fileName);
        }

        public IEnumerable<Move> GetLegalUiMoves() {
            return Copy().GetLegalNextMoves(0);
        }

        public bool TryPossibleMoveCommand(MoveCommand moveCommand) {
            if (Ended)
                return false;

            var fromSquare = Board.Square(moveCommand.FromFile, moveCommand.FromRank);
            var toSquare = Board.Square(moveCommand.ToFile, moveCommand.ToRank);
            var possibleMoves = GetPossibleMoves();
            var piece = fromSquare?.Piece;
            var move = possibleMoves.SingleOrDefault(x => x.Piece == piece && x.ToSquare == toSquare);
            if (move == null)
                return false;
            TryPerform(move, 0);
            if (!move.IsLegal.Value)
                return false;
            PerformLegalMove(move);
            CommandCount++;
            if (CommandCount > 127) //There was not room for a bigger number
            {
                CommandCount = 0;
                PositionsDatabase.Instance.Reset();
            }
            HashHistory.Push(Hash);

            if (Ended)
                return true;

            var nextMoves = GetLegalNextMoves(0);
            if (!nextMoves.Any()) {
                Ended = true;
                if (CurrentPlayer.IsChecked) {
                    move.ScoreInfo |= ScoreInfo.Mate;
                    CurrentPlayer.Mated = true;
                    Winner = OtherPlayer;
                } else {
                    move.ScoreInfo |= ScoreInfo.StaleMate;
                    IsStaleMate = true;
                    Ended = true;
                }
            }
            return true;
        }

        public void UndoLastMove() {
            var move = OtherPlayer.Moves.LastOrDefault();
            if (move == null)
                return;
            WhitePlayer.Mated = false;
            BlackPlayer.Mated = false;
            Ended = false;
            IsStaleMate = false;
            Winner = null;
            Undo(move);
        }

        public void PerformLegalMove(Move move) {
            MoveCount++;
            move.Piece.MoveCount++;
            move.FromSquare.Piece = null; //use from square to remove piece
            var playColor = move.Piece.Color;

            var capture = move.Capture;
            if (capture != null) {
                OtherPlayer.Material -= capture.Value * 100;
                OtherPlayer.Pieces.Remove(capture);
            }

            move.ToSquare.SetPiece(move.Piece);

            if (move.IsPromotion) {
                move.Piece.Square = null;
                move.PromotedPawn = (Pawn)move.Piece;
                var queen = new Queen(playColor);
                AddPiece(move.ToSquare.File, move.ToSquare.Rank, queen);
                move.Piece = queen; //todo: test without it.
                CurrentPlayer.Material += 800; //add queen, remove pawn
                CurrentPlayer.Pieces.Remove(move.PromotedPawn);
            } else if (move.IsCastling) {
                Castle(move);
            } else if (move.IsEnpassant) {
                move.CapturedFrom.Piece = null;
                move.Capture.Square = null;
            }

            move.NumberInGame = (byte)(CurrentPlayer.Moves.Count + 1);

            CurrentPlayer.Moves.Add(move); //If it is found later that this is a illegal move it is removed in the undo - function

            move.WhiteWasChecked = WhitePlayer.IsChecked;
            move.BlackWasChecked = BlackPlayer.IsChecked;

            if (move.ScoreInfo.HasFlag(ScoreInfo.InsufficienMaterial))
                Ended = true;
            if (move.ScoreInfo.HasFlag(ScoreInfo.DrawByRepetion))
                Ended = true;

            move.PreviousEnPassant = EnPassantFile;
            EnPassantFile = null;
            if (move.Piece is Pawn && move.Piece.MoveCount == 1) {
                var dist = move.ToSquare.Rank - move.FromSquare.Rank;
                if (Math.Abs(dist) == 2)
                    EnPassantFile = move.FromSquare.File;
            }

            if (move.IsCheck.HasValue)
                OtherPlayer.IsChecked = move.IsCheck.Value;

            CurrentPlayer.IsChecked = false;
            SwitchPlayer();
            move.PreviousHash = Hash;
            PositionsDatabase.Instance.UpdateHash(this, move);
        }

        public IEnumerable<Move> GetLegalNextMoves(int recursions, bool justCaptures = false) {
            var moves = justCaptures ? GetPossibleCaptureMoves() : GetPossibleMoves();
            foreach (var move in moves)
                TryPerform(move, recursions);

            return moves.Where(m => m.IsLegal.HasValue && m.IsLegal.Value);
        }

        public Game Copy() {
            var gameCopy = new Game { Board = new Board() };

            gameCopy.WhitePlayer = WhitePlayer.Copy();
            gameCopy.BlackPlayer = BlackPlayer.Copy();

            CopyPieces(WhitePlayer, gameCopy);
            CopyPieces(BlackPlayer, gameCopy);

            gameCopy.Ended = Ended;
            gameCopy.IsStaleMate = IsStaleMate;
            gameCopy.CurrentPlayer = CurrentPlayer.Color == Color.White ? gameCopy.WhitePlayer : gameCopy.BlackPlayer;
            if (Winner != null)
                gameCopy.Winner = Winner.Color == Color.White ? gameCopy.WhitePlayer : gameCopy.BlackPlayer;
            gameCopy.WhitePlayer.Material = WhitePlayer.Material;
            gameCopy.BlackPlayer.Material = BlackPlayer.Material;
            gameCopy.Hash = Hash;
            gameCopy.EnPassantFile = EnPassantFile;
            gameCopy.InitialPosition = InitialPosition;
            foreach (var hash in HashHistory)
                gameCopy.HashHistory.Push(hash);
            gameCopy.CommandCount = CommandCount;
            gameCopy.SetPieceFastAccess();
            return gameCopy;
        }

        internal byte CommandCount { get; private set; }

        internal bool TryStringMove(string command) {
            var cmd = MoveCommand.Parse(command);
            return TryPossibleMoveCommand(cmd);
        }

        internal void AddPiece(File file, Rank rank, Piece piece) {
            Board.Square(file, rank).SetPiece(piece);
            if (piece.Color == Color.Black)
                BlackPlayer.Pieces.Add(piece);
            else
                WhitePlayer.Pieces.Add(piece);
        }

        internal IList<Move> GetPossibleMoves() {
            var moves = new List<Move>();
            foreach (var piece in CurrentPlayer.Pieces)
                piece.AddPossibleMoves(this, moves);

            AddCastling(moves);
            return moves;
        }

        internal void Reset() {
            WhitePlayer.HasCastledKingSide = false;
            WhitePlayer.HasCastledQueenSide = false;
            BlackPlayer.HasCastledKingSide = false;
            BlackPlayer.HasCastledQueenSide = false;
            EnPassantFile = null;
            WhitePlayer.Material = 0;
            BlackPlayer.Material = 0;
            Ended = false;
            IsStaleMate = false;
            Winner = null;
            WhitePlayer.Pieces.Clear();
            BlackPlayer.Pieces.Clear();
            Board.ClearPieces();
            HashHistory.Clear();
            CommandCount = 0;
            PositionsDatabase.Instance.Reset();
        }

        internal bool MakeRandomMove(Random rnd) {
            var moves = GetLegalNextMoves(0).ToArray();
            if (!moves.Any())
                return false;
            Assert.IsTrue(moves.Any());
            var n = rnd.Next(moves.Length);
            PerformLegalMove(moves[n]);
            return true;
        }

        internal void SetInitials() {
            PositionsDatabase.Instance.SetStartHash(this);
            InitialPosition = GetPosition();
            SetPieceFastAccess();
            InitialMaterial(WhitePlayer);
            InitialMaterial(BlackPlayer);
        }

        private void InitialMaterial(Player player)
        {
            foreach (var piece in player.Pieces)
                player.Material += piece.Value*100;
        }

        private IList<Move> GetPossibleCaptureMoves() {

            var moves = new List<Move>();
            foreach (var piece in CurrentPlayer.Pieces)
                piece.AddCaptures(this, moves);

            return moves;
        }

        private void AddCastling(List<Move> moves) {
            var king = (King)CurrentPlayer.Pieces.Single(x => x is King);
            if (king.MoveCount > 0)
                return;

            if (CurrentPlayer.IsChecked)
                return;

            var rooks = CurrentPlayer.Pieces.Where(x => x is Rook).ToArray();
            if (rooks.Any()) {
                var firstRook = rooks.First();
                if (firstRook.MoveCount == 0 && !CurrentPlayer.HasCastledKingSide && !CurrentPlayer.HasCastledQueenSide) //has not moved
                {
                    var dir = 1;
                    if (firstRook.Square.File < king.Square.File)
                        dir = -1;
                    var toSquare = king.GetSquare(0, 2 * dir, this);
                    if (!CastlingBlocked(king, toSquare))
                        moves.Add(new Move(king, toSquare, isCastling: true, castleRook: firstRook));
                }

                if (rooks.Length > 1) {
                    var secondRook = rooks[1];
                    if (secondRook.MoveCount == 0 && !CurrentPlayer.HasCastledKingSide && !CurrentPlayer.HasCastledQueenSide) //has not moved
                    {
                        var dir = 1;
                        if (secondRook.Square.File < king.Square.File)
                            dir = -1;
                        var toSquare = king.GetSquare(0, 2 * dir, this);
                        if (!CastlingBlocked(king, toSquare))
                            moves.Add(new Move(king, toSquare, isCastling: true, castleRook: secondRook));
                    }
                }
            }
        }

        private bool CastlingBlocked(King king, Square toSquare) {
            var dir = 1;
            if (king.Square.File > toSquare.File)
                dir = -1;
            var sqr = king.Square;
            var file = 0;
            var list = new List<Square>(10);
            if (dir == -1) {
                var s = king.GetSquare(0, -3, this); //The b-file square.
                if (s.Piece != null)
                    return true;
                list.Add(s);
            }

            //Checks the two squares closest to king. As in king side castling.
            while (sqr != toSquare) {
                file += dir;
                sqr = king.GetSquare(0, file, this);
                if (sqr.Piece != null)
                    return true;
                list.Add(sqr);
            }

            foreach (var activePiece in OtherPlayer.Pieces)
                foreach (var square in list)
                    if (activePiece.Attacks(square, Board))
                        return true;

            return false;
        }

        private void SetScore(Move move) {
            if (move.ScoreAfterMove.HasValue)
                return;

            //It is only interesting to check for insufficient material if the material has decreased.
            if (move.Capture != null && InsufficientMaterial()) {
                move.ScoreInfo |= ScoreInfo.InsufficienMaterial;
                move.ScoreAfterMove = 0;
                return;
            }

            //todo, check for 50 move rule

            var black = BlackPlayer.Pieces.Select(x => x.PositionValue(this)).Sum() +
                DoublePawns(BlackPlayer);

            var white = WhitePlayer.Pieces.Select(x => x.PositionValue(this)).Sum() +
                DoublePawns(WhitePlayer);

            if (CommandCount > 20)
            {
                black += OpeningScore(BlackPlayer);
                white += OpeningScore(WhitePlayer);
            }

            if (WhitePlayer.Material < 1000)
                black += EndGameScore(BlackPlayer);

            if (BlackPlayer.Material < 1000)
                white += EndGameScore(WhitePlayer);

            var value = Material + black - white;
            move.ScoreAfterMove = value;
        }

        private int EndGameScore(Player player)
        {
            //Distance from center
            var kingRank = player.King.Square.Rank;
            var kingFile = player.King.Square.File;
            var kingCloseBorder = kingRank == Rank._2 || kingRank == Rank._7 || kingFile == File.B || kingFile == File.G;
            var kingOnBorder = kingRank == Rank._1 || kingRank == Rank._8 || kingFile == File.A || kingFile == File.H;
            
            var oppSide = player.Color == Color.White ? Rank._8 : Rank._1;
            var pawnPromotionDist = player.Pawns.Where(x => x.Square != null).Sum(x => Math.Abs(oppSide - x.Square.Rank));
            return (kingCloseBorder ? -10 : 0) + (kingOnBorder ? -20 : 0) - pawnPromotionDist*2;
        }

        private int DoublePawns(Player player) {
            var score = 0;
            var pawns = player.Pawns;
            for (int i = 0; i < pawns.Length - 1; i++) {
                if (pawns[i].Square != null && pawns[i].Square.File == pawns[i + 1].Square?.File)
                    score -= 2;
            }
            return score;
        }

        private int OpeningScore(Player player) {
            
            //It is bad if queen moves in the opening.
            var queenScore = player.Queen?.MoveCount ?? 0 * -10;

            //Better if one knight or bishop has moved exactly one time during opening.
            var kbsMovedToMuch = player.KnightsBishops.Count(x => x.MoveCount > 1);
            var kbsMovedOnce = player.KnightsBishops.Count(x => x.MoveCount == 1);

            var kbs = kbsMovedOnce * 2 - kbsMovedToMuch * -2; //knights and bishops score

            return kbs + queenScore;
        }
        
        /// <summary>
        /// Evaluates all aspects of a move. Legality and score after move.
        /// This is used in move generation.
        /// </summary>
        /// <param name="move"></param>
        /// <param name="recursions"></param>
        /// <returns></returns>
        private void TryPerform(Move move, int recursions) {
            Debug.Assert(!move.IsLegal.HasValue);

            //Actually performs a possible move.
            PerformLegalMove(move);

            //Check in db if position is legal.
            PositionsDatabase.Instance.GetValue(this, move, recursions);

            if (!move.IsLegal.HasValue) //Not known, we have to spend time investigating.
            {
                if (KingChecked(OtherPlayer)) //Players are switched, so this is actually own king in check.
                {
                    move.IsLegal = false;
                    PositionsDatabase.Instance.Store(this, move, recursions); //Store it, so we don't have to check again.
                    UndoLastMove();
                    return;
                }
                move.IsCheck = KingChecked(CurrentPlayer);
            } else if (!move.IsLegal.Value) { //Position is already know not to be legal.
                UndoLastMove();
                return;
            }
            move.IsLegal = true;

            if (HashHistory.Count(x => x == Hash) >= 2) {
                move.ScoreInfo |= ScoreInfo.DrawByRepetion;
                move.ScoreAfterMove = 0;
                UndoLastMove();
                return;
            }

            if (!move.ScoreAfterMove.HasValue) { //Score can be null if we are on a deeper search, 
                SetScore(move);
                PositionsDatabase.Instance.Store(this, move, recursions);
            }
            UndoLastMove();
        }

        private bool InsufficientMaterial() {
            var count = WhitePlayer.Pieces.Count() + BlackPlayer.Pieces.Count();
            if (count < 3) //King king
                return true;
            return count <= 3 &&
                    (WhitePlayer.Pieces.Any(p => p.Value == 3) ||
                     BlackPlayer.Pieces.Any(p => p.Value == 3));

        }

        private bool KingChecked(Player checkedPlayer) {

            var kingSquare = checkedPlayer.King.Square;
            var otherPlayer = checkedPlayer == WhitePlayer ? BlackPlayer : WhitePlayer;
            foreach (var piece in otherPlayer.Pieces) {
                if (piece.Attacks(kingSquare, Board))
                    return true;
            }
            return false;
        }

        internal int MoveCount { get; private set; }

        internal File? EnPassantFile { get; private set; }
        public int Material => BlackPlayer.Material - WhitePlayer.Material;

        public bool EditMode { get; set; }

        private void Castle(Move move) {
            var king = (King)move.Piece;
            Square fromRookSquare = null, toRookSquare = null;
            if (king.Square.File == File.G) {
                fromRookSquare = Board.Square(File.H, king.Square.Rank);
                toRookSquare = Board.Square(File.F, king.Square.Rank);
                CurrentPlayer.HasCastledKingSide = true;
            }
            if (king.Square.File == File.C) {
                fromRookSquare = Board.Square(0, king.Square.Rank);
                toRookSquare = Board.Square(File.D, king.Square.Rank);
                CurrentPlayer.HasCastledQueenSide = true;
            }
            Debug.Assert(fromRookSquare != null && toRookSquare != null);
            var rook = (Rook)fromRookSquare.Piece;
            fromRookSquare.Piece = null;
            toRookSquare.SetPiece(rook);
            king.HasCastled = true;
        }

        private void Undo(Move move) {
            PositionsDatabase.Instance.UpdateHash(this, move); //xoring back to previous hash 
            Debug.Assert(move.PreviousHash == Hash, "Previous hash differs from hash after undo");
            SwitchPlayer();
            move.Piece.MoveCount--;
            move.FromSquare.Piece = move.Piece;
            move.Piece.Square = move.FromSquare;
            move.ToSquare.Piece = null;

            var capture = move.Capture;
            if (capture != null) {
                move.CapturedFrom.Piece = capture;
                capture.Square = move.CapturedFrom;
                OtherPlayer.Material += capture.Value * 100;
                OtherPlayer.Pieces.Add(capture);
            }

            if (move.IsEnpassant)
                move.ToSquare.Piece = null;

            if (move.IsCastling)
                UnCastle(move);

            if (move.IsPromotion) {
                var queen = (Queen)move.FromSquare.Piece;
                CurrentPlayer.Pieces.Remove(queen);
                queen.Square.Piece = null;
                queen.Square = null;

                var pawn = move.PromotedPawn;
                pawn.Square = move.FromSquare;
                move.FromSquare.Piece = pawn;
                move.Piece = pawn;
                CurrentPlayer.Material -= 800; //remove queen, add pawn
                CurrentPlayer.Pieces.Add(pawn);
            }
            BlackPlayer.IsChecked = move.BlackWasChecked;
            WhitePlayer.IsChecked = move.WhiteWasChecked;
            EnPassantFile = move.PreviousEnPassant;
            CurrentPlayer.Moves.Remove(move);
        }

        private void UnCastle(Move move) {
            //The king is moved back.
            //Placing the rook on the corner square.
            var king = (King)move.Piece;
            Square fromRookSquare = null, toRookSquare = null;
            if (move.ToSquare.File == File.G) {
                fromRookSquare = Board.Square(File.H, king.Square.Rank);
                toRookSquare = Board.Square(File.F, king.Square.Rank);
                CurrentPlayer.HasCastledKingSide = false;
            }
            if (move.ToSquare.File == File.C) {
                fromRookSquare = Board.Square(0, king.Square.Rank);
                toRookSquare = Board.Square(File.D, king.Square.Rank);
                CurrentPlayer.HasCastledQueenSide = false;
            }
            Debug.Assert(fromRookSquare != null && toRookSquare != null);
            var rook = (Rook)toRookSquare.Piece;
            toRookSquare.Piece = null;
            fromRookSquare.SetPiece(rook);
            king.HasCastled = false;
        }

        private void SwitchPlayer() {
            CurrentPlayer = CurrentPlayer == WhitePlayer ? BlackPlayer : WhitePlayer;
        }

        private void CopyPieces(Player player, Game gameCopy) {
            foreach (var piece in player.Pieces) {
                var pieceCopy = piece.Copy(gameCopy.Board.Squares);
                if (pieceCopy.Square != null)
                    gameCopy.AddPiece(piece.Square.File, piece.Square.Rank, pieceCopy);
                //else: The piece was captured and is connected to the game through the move.
                //But since CopyPosition does not copy moves we forget about the piece
            }
        }

        private string GetPosition() {
            var stringBuildder = new StringBuilder();
            stringBuildder.Append("Start,");
            foreach (var square in Board.Squares) {
                if (square.Piece != null)
                    stringBuildder.Append(square.Piece.ToPositionString() + ",");
            }
            stringBuildder.Append(CurrentPlayer.Color + ",");
            if (WhitePlayer.HasCastledKingSide)
                stringBuildder.Append("WCK,");
            if (WhitePlayer.HasCastledQueenSide)
                stringBuildder.Append("WCQ,");
            if (BlackPlayer.HasCastledKingSide)
                stringBuildder.Append("BCK,");
            if (BlackPlayer.HasCastledQueenSide)
                stringBuildder.Append("BCQ,");
            if (EnPassantFile != null)
                stringBuildder.Append("ENP:" + EnPassantFile.Value);
            return stringBuildder.ToString();
        }

        private void SetPieceFastAccess() {
            WhitePlayer.King = WhitePlayer.Pieces.OfType<King>().Single();
            BlackPlayer.King = BlackPlayer.Pieces.OfType<King>().Single();

            WhitePlayer.Queen = WhitePlayer.Pieces.OfType<Queen>().FirstOrDefault();
            BlackPlayer.Queen = BlackPlayer.Pieces.OfType<Queen>().FirstOrDefault();

            WhitePlayer.Pawns = WhitePlayer.Pieces.OfType<Pawn>().ToArray();
            BlackPlayer.Pawns = BlackPlayer.Pieces.OfType<Pawn>().ToArray();

            WhitePlayer.KnightsBishops = WhitePlayer.Pieces.Where(x => x.Value == 3).ToArray();
            BlackPlayer.KnightsBishops = BlackPlayer.Pieces.Where(x => x.Value == 3).ToArray();

        }
    }

    public static class GameExtensions
    {
        /// <summary>
        /// string format. E.g. e1bR (black Rook)
        /// </summary>
        /// <param name="game"></param>
        /// <param name="pieceString"></param>
        public static Game AddPiece(this Game game, string pieceString) {
            var file = (File)Enum.Parse(typeof(File), pieceString.Substring(0, 1).ToUpper());
            var rank = (Rank)Enum.Parse(typeof(Rank), "_" + pieceString.Substring(1, 1));
            var colorChar = pieceString.Substring(2, 1);
            var color = colorChar == "w" ? Color.White : Color.Black;
            var pieceTypeChar = pieceString.Substring(3, 1);
            Piece piece = null;
            if (pieceTypeChar == "K")
                piece = new King(color);
            else if (pieceTypeChar == "Q")
                piece = new Queen(color);
            else if (pieceTypeChar == "R")
                piece = new Rook(color);
            else if (pieceTypeChar == "N")
                piece = new Knight(color);
            else if (pieceTypeChar == "B")
                piece = new Bishop(color);
            else if (pieceTypeChar == "P")
                piece = new Pawn(color);
            if (piece == null)
                throw new ApplicationException("Invalid format of add piece string" + pieceString);
            game.AddPiece(file, rank, piece);
            return game;
        }

        public static void AddPiece(this Game game, Square square, PieceType type)
        {
            Piece piece = null;
            switch (type)
            {
                case PieceType.NoPiece:
                    break;
                case PieceType.WhiteKing:
                    piece = new King(Color.White);
                    break;
                case PieceType.WhiteQueen:
                    piece = new Queen(Color.White);
                    break;
                case PieceType.WhiteRook:
                    piece = new Rook(Color.White);
                    break;
                case PieceType.WhiteBishop:
                    piece = new Bishop(Color.White);
                    break;
                case PieceType.WhiteNight:
                    piece = new Knight(Color.White);
                    break;
                case PieceType.WhitePawn:
                    piece = new Pawn(Color.White);
                    break;
                case PieceType.BlackKing:
                    piece = new King(Color.Black);
                    break;
                case PieceType.BlackQueen:
                    piece = new Queen(Color.Black);
                    break;
                case PieceType.BlackRook:
                    piece = new Rook(Color.Black);
                    break;
                case PieceType.BlackBishop:
                    piece = new Bishop(Color.Black);
                    break;
                case PieceType.BlackKnight:
                    piece = new Knight(Color.Black);
                    break;
                case PieceType.BlackPawn:
                    piece = new Pawn(Color.Black);
                    break;
                default:
                    throw new NotImplementedException();
                    break;
            }
            game.AddPiece(square.File, square.Rank, piece);
        }
    }
}