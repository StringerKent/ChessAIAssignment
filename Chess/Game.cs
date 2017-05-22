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
        
        private int MovesSinceLastCaptureOrPawnMove = 0;

        public void New()
        {
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
            WhitePlayer.CanCastleKingSide = true;
            WhitePlayer.CanCastleQueenSide = true;
            BlackPlayer.CanCastleKingSide = true;
            BlackPlayer.CanCastleQueenSide = true;

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

        public void Load(GameFile gameFile)
        {
            Reset();
            InitialPosition = gameFile.InitialPosition;
            var positionItem = gameFile.InitialPosition.Split(',').ToList();
            Debug.Assert(positionItem.First() == "Start");
            var i = 0;
            while (true)
            {
                i++;
                if (positionItem[i] == "White" || positionItem[i] == "Black")
                    break;
                this.AddPiece(positionItem[i]);
            }
            if (positionItem[i] == "White")
                CurrentPlayer = WhitePlayer;
            else
            {
                Debug.Assert(positionItem[i] == "Black");
                CurrentPlayer = BlackPlayer;
            }
            i++;
            for (int j = i; j < positionItem.Count; j++)
            {
                if (positionItem[j] == "WCK")
                    WhitePlayer.CanCastleKingSide = false;
                if (positionItem[j] == "WCQ")
                    WhitePlayer.CanCastleQueenSide = false;
                if (positionItem[j] == "BCK")
                    BlackPlayer.CanCastleKingSide = false;
                if (positionItem[j] == "BCQ")
                    BlackPlayer.CanCastleQueenSide = false;
                if (positionItem[j].StartsWith("ENP:"))
                {
                    var split = positionItem[j].Split(':');
                    EnPassantFile = (File)Enum.Parse(typeof(File), split[1]);
                }
            }
            SetPieceFastAccess();

            PositionsDatabase.Instance.SetStartHash(this);
            InitialMaterial(WhitePlayer);
            InitialMaterial(BlackPlayer);
            foreach (var moveCommand in gameFile.MoveCommands)
            {
                if (!TryPossibleMoveCommand(moveCommand))
                    throw new ApplicationException("Invalid game file");
            }
        }

        public void Save(string fileName)
        {
            var gameFile = new GameFile(this);
            gameFile.Save(fileName);
        }

        public IEnumerable<Move> GetLegalUiMoves()
        {
            return Copy().GetLegalNextMoves(CurrentPlayer.Color);
        }

        public bool TryPossibleMoveCommand(MoveCommand moveCommand)
        {
            if (Ended)
                return false;

            var fromSquare = Board.Square(moveCommand.FromFile, moveCommand.FromRank);
            var toSquare = Board.Square(moveCommand.ToFile, moveCommand.ToRank);
            var possibleMoves = GetPseudoLegalMoves();
            var piece = fromSquare?.Piece;
            var move = possibleMoves.SingleOrDefault(x => x.Piece == piece && x.ToSquare == toSquare);
            if (move == null)
                return false;
            TryPerform(move);
            if (!move.IsLegal.Value)
                return false;

            if (CurrentPlayer.Color == Color.Black)
                move.NumberInGame = BlackPlayer.Moves.Count + 1;
            else
                move.NumberInGame = (BlackPlayer.Moves.FirstOrDefault()?.NumberInGame ?? 0) + 1;

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

            var nextMoves = GetLegalNextMoves(CurrentPlayer.Color);
            if (!nextMoves.Any())
            {
                Ended = true;
                if (CurrentPlayer.IsChecked)
                {
                    move.ScoreInfo |= ScoreInfo.Mate;
                    CurrentPlayer.Mated = true;
                    Winner = OtherPlayer;
                }
                else
                {
                    move.ScoreInfo |= ScoreInfo.StaleMate;
                    IsStaleMate = true;
                    Ended = true;
                }
            }
            return true;
        }

        public void UndoLastMove()
        {
            var mv = OtherPlayer.Moves.FirstOrDefault();
            if (mv == null)
                return;
            Undo(mv);
        }

        public void PerformLegalMove(Move move)
        {
            move.PreviousMovesSinceLastCaptureOrPawnMove = MovesSinceLastCaptureOrPawnMove;
            MovesSinceLastCaptureOrPawnMove++;
            var fromSquare = move.FromSquare;
            MoveCount++;
            var piece = move.Piece;
            piece.MoveCount++;
            fromSquare.Piece = null; //use from square to remove piece
            var playColor = piece.Color;

            var capture = move.Capture;
            if (capture != null)
            {
                OtherPlayer.Material -= capture.Value;
                OtherPlayer.Pieces.Remove(capture);
                MovesSinceLastCaptureOrPawnMove = 0;
            }

            move.ToSquare.SetPiece(piece);

            if (move.IsPromotion)
            {
                piece.Square = null;
                move.PromotedPawn = (Pawn)piece;
                var queen = new Queen(playColor);
                AddPiece(move.ToSquare.File, move.ToSquare.Rank, queen);
                move.Piece = queen; //todo: test without it.
                CurrentPlayer.Material += 800; //add queen, remove pawn
                CurrentPlayer.Pieces.Remove(move.PromotedPawn);
            }
            else if (move.IsCastling)
            {
                Castle(move);
            }
            else if (move.IsEnpassant)
            {
                move.CapturedFrom.Piece = null;
                move.Capture.Square = null;
            }
            CurrentPlayer.Moves.Push(move); //If it is found later that this is an illegal move it is removed in the undo - function

            move.WhiteWasChecked = WhitePlayer.IsChecked;
            move.BlackWasChecked = BlackPlayer.IsChecked;

            if (move.ScoreInfo.HasFlag(ScoreInfo.InsufficientMaterial))
                Ended = true;
            if (move.ScoreInfo.HasFlag(ScoreInfo.DrawByRepetion))
                Ended = true;
            if (move.PreviousMovesSinceLastCaptureOrPawnMove > 50)
                Ended = true;

            move.PreviousEnPassant = EnPassantFile;
            EnPassantFile = null;
            if (piece is Pawn)
            {
                MovesSinceLastCaptureOrPawnMove = 0;
                if (piece.MoveCount == 1)
                {
                    var dist = move.ToSquare.Rank - fromSquare.Rank;
                    if (Math.Abs(dist) == 2)
                        EnPassantFile = fromSquare.File;
                }
            }

            OtherPlayer.IsChecked = move.IsCheck;

            CurrentPlayer.IsChecked = false;
            SwitchPlayer();
            move.PreviousHash = Hash;
            PositionsDatabase.Instance.UpdateHash(move);
            
            Hash ^= move.Hash;
        }

        public Move[] GetLegalNextMoves(Color color)
        {
            var moves = GetPseudoLegalMoves();
            if (!moves.Any())
                return new Move[0];
            foreach (var move in moves)
                TryPerform(move);

            if (color == Color.White)
                return moves.Where(m => m.IsLegal.Value).OrderBy(x => x.ScoreAfterMove.Value).ToArray();
            return moves.Where(m => m.IsLegal.Value).OrderByDescending(x => x.ScoreAfterMove.Value).ToArray();

        }

        public Move[] GetLegalCaptureMoves(Color color)
        {
            var moves = GetPseudoLegalCaptureMoves();
            if (!moves.Any())
                return new Move[0];
            foreach (var move in moves)
                TryPerform(move, true);


            if (color == Color.White)
                return moves.Where(m => m.IsLegal.Value).OrderBy(x => x.ScoreAfterMove.Value).ToArray();
            return moves.Where(m => m.IsLegal.Value).OrderByDescending(x => x.ScoreAfterMove.Value).ToArray();
        }

        public Game Copy()
        {
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
            gameCopy.MovesSinceLastCaptureOrPawnMove = MovesSinceLastCaptureOrPawnMove;
            gameCopy.SetPieceFastAccess();
            return gameCopy;
        }

        public void EditClearPieces()
        {
            WhitePlayer.Pieces.Clear();
            BlackPlayer.Pieces.Clear();
            Board.ClearPieces();
            AddPiece(File.E, Rank._1, new King(Color.White));
            WhitePlayer.King = (King)WhitePlayer.Pieces.Single(x => x is King);
            AddPiece(File.E, Rank._8, new King(Color.Black));
            BlackPlayer.King = (King)BlackPlayer.Pieces.Single(x => x is King);


        }

        internal byte CommandCount { get; private set; }

        internal bool TryStringMove(string command)
        {
            var cmd = MoveCommand.Parse(command);
            return TryPossibleMoveCommand(cmd);
        }

        internal void AddPiece(File file, Rank rank, Piece piece)
        {
            Board.Square(file, rank).SetPiece(piece);
            if (piece.Color == Color.Black)
                BlackPlayer.Pieces.Add(piece);
            else
                WhitePlayer.Pieces.Add(piece);
        }

        internal List<Move> GetPseudoLegalMoves()
        {
            var moves = new List<Move>();
            foreach (var piece in CurrentPlayer.Pieces)
                piece.AddPseudoLegalMoves(this, moves);

            AddCastling(moves);
            return moves;
        }

        internal void Reset()
        {
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
            BlackPlayer.Moves.Clear();
            WhitePlayer.Moves.Clear();
            MovesSinceLastCaptureOrPawnMove = 0;
            PositionsDatabase.Instance.Reset();
        }

        internal bool MakeRandomMove(Random rnd)
        {
            var moves = GetLegalNextMoves(CurrentPlayer.Color).ToArray();
            if (!moves.Any())
                return false;
            Assert.IsTrue(moves.Any());
            var n = rnd.Next(moves.Length);
            PerformLegalMove(moves[n]);
            return true;
        }

        public void SetInitials()
        {
            CommandCount = 0;
            PositionsDatabase.Instance.SetStartHash(this);
            InitialPosition = GetPosition();
            SetPieceFastAccess();
            InitialMaterial(WhitePlayer);
            InitialMaterial(BlackPlayer);
        }

        private void InitialMaterial(Player player)
        {
            player.Material = 0;
            foreach (var piece in player.Pieces)
                player.Material += piece.Value;
        }

        private List<Move> GetPseudoLegalCaptureMoves()
        {

            var moves = new List<Move>();
            foreach (var piece in CurrentPlayer.Pieces)
                piece.AddCaptures(this, moves);

            return moves;
        }

        private void AddCastling(List<Move> moves)
        {
            var king = CurrentPlayer.King;
            if (king.MoveCount > 0)
                return;

            if (CurrentPlayer.IsChecked)
                return;

            var rooks = CurrentPlayer.Pieces.OfType<Rook>().Where(x => x.MoveCount == 0).ToArray();
            var kingRook = rooks.SingleOrDefault(x => x.Square.File == File.H);
            if (kingRook != null && CurrentPlayer.CanCastleKingSide)
            {
                var toSquare = king.GetSquare(0, 2, this);
                if (!CastlingBlocked(king, toSquare))
                    moves.Add(new Move(king, toSquare, isCastling: true, castleRook: kingRook));
            }

            var queenRook = rooks.SingleOrDefault(x => x.Square.File == File.A);
            if (queenRook != null && CurrentPlayer.CanCastleQueenSide)
            {
                var toSquare = king.GetSquare(0, -2, this);
                if (!CastlingBlocked(king, toSquare))
                    moves.Add(new Move(king, toSquare, isCastling: true, castleRook: queenRook));
            }
        }

        private bool CastlingBlocked(King king, Square toSquare)
        {
            var dir = 1;
            if (king.Square.File > toSquare.File)
                dir = -1;
            var sqr = king.Square;
            var file = 0;
            var list = new List<Square>(10);
            if (dir == -1)
            {
                var s = king.GetSquare(0, -3, this); //The b-file square.
                if (s.Piece != null)
                    return true;
                list.Add(s);
            }

            //Checks the two squares closest to king. As in king side castling.
            while (sqr != toSquare)
            {
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
        private void SetScore(Move move)
        {
            //It is only interesting to check for insufficient material if the material has decreased.
            if (move.Capture != null && InsufficientMaterial())
            {
                move.ScoreInfo |= ScoreInfo.InsufficientMaterial;
                move.ScoreAfterMove = 0;
                return;
            }

            if (MovesSinceLastCaptureOrPawnMove > 50)
            {
                //move.ScoreInfo |= ScoreInfo.FiftyMoveRule; There is no room in DB to store the reason.
                move.ScoreAfterMove = 0;
                return;
            }

            var black = BlackPlayer.Pieces.Select(x => x.PositionValue(this)).Sum() +
                DoublePawns(BlackPlayer);

            var white = WhitePlayer.Pieces.Select(x => x.PositionValue(this)).Sum() +
                DoublePawns(WhitePlayer);

            if (CommandCount < 20)
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
            return (kingCloseBorder ? -10 : 0) + (kingOnBorder ? -20 : 0) - pawnPromotionDist * 2;
        }

        private int DoublePawns(Player player)
        {
            var score = 0;
            var pawns = player.Pawns;
            for (int i = 0; i < pawns.Length - 1; i++)
            {
                if (pawns[i].Square != null && pawns[i].Square.File == pawns[i + 1].Square?.File)
                    score -= 2;
            }
            return score;
        }

        private int OpeningScore(Player player)
        {

            //It is bad if queen moves in the opening.
            var queenScore = (player.Queen?.MoveCount ?? 0) * -6;

            //Better if one knight or bishop has moved exactly one time during opening.
            var kbsMovedToMuch = player.KnightsBishops.Count(x => x.MoveCount > 1);
            var kbsMovedOnce = player.KnightsBishops.Count(x => x.MoveCount == 1);

            var kbs = kbsMovedOnce * 3 - kbsMovedToMuch * -2; //knights and bishops score

            return kbs + queenScore;
        }

        /// <summary>
        /// Evaluates all aspects of a move. Legality and score after move.
        /// This is used in move generation.
        /// </summary>
        /// <param name="move"></param>
        /// <param name="recursions"></param>
        /// <returns></returns>
        private void TryPerform(Move move, bool quiteSearch = false)
        {
            Debug.Assert(!move.IsLegal.HasValue);

            //Actually performs a psuedo legal move.
            PerformLegalMove(move);

            //Check in db if position is legal.
            PositionsDatabase.Instance.GetValue(this, move);

            if (!move.IsLegal.HasValue) //Not known, we have to spend time investigating.
            {
                if (KingChecked(OtherPlayer)) //Players are switched, so this is actually own king in check.
                {
                    move.IsLegal = false;
                    PositionsDatabase.Instance.Store(this, move, 100); //Store it, so we don't have to check again.
                    Undo(move);
                    return;
                }
                move.IsCheck = KingChecked(CurrentPlayer);
            }
            else if (!move.IsLegal.Value)
            { //Position is already know not to be legal.
                Undo(move);
                return;
            }
            move.IsLegal = true;

            if (HashHistory.Count(x => x == Hash) >= 2)
            {
                move.ScoreInfo |= ScoreInfo.DrawByRepetion;
                move.ScoreAfterMove = 0;
                Undo(move);
                return;
            }

            if (!move.ScoreAfterMove.HasValue)
            { //Score can be null if we are on a deeper search, 
                if (quiteSearch)
                    move.ScoreAfterMove = Material;
                else
                {
                    SetScore(move);
                    PositionsDatabase.Instance.Store(this, move, 0);
                }
            }
            Undo(move);
        }

        private bool InsufficientMaterial()
        {
            var count = WhitePlayer.Pieces.Count() + BlackPlayer.Pieces.Count();
            if (count == 2) //King and King
                return true;
            //At least one player has more pieces than just one knight or bishop
            return count <= 3 &&
                    (WhitePlayer.Pieces.Any(p => p.Value == 300) ||
                     BlackPlayer.Pieces.Any(p => p.Value == 300));

        }

        private bool KingChecked(Player checkedPlayer)
        {

            var kingSquare = checkedPlayer.King.Square;
            var otherPlayer = checkedPlayer == WhitePlayer ? BlackPlayer : WhitePlayer;
            foreach (var piece in otherPlayer.Pieces)
            {
                if (piece.Attacks(kingSquare, Board))
                    return true;
            }
            return false;
        }

        internal int MoveCount { get; private set; }

        internal File? EnPassantFile { get; private set; }
        public int Material => BlackPlayer.Material - WhitePlayer.Material;

        public bool EditMode { get; set; }

        private void Castle(Move move)
        {
            var king = (King)move.Piece;
            Square fromRookSquare = null, toRookSquare = null;
            if (king.Square.File == File.G)
            {
                fromRookSquare = Board.Square(File.H, king.Square.Rank);
                toRookSquare = Board.Square(File.F, king.Square.Rank);
                CurrentPlayer.CanCastleKingSide = false;
            }
            if (king.Square.File == File.C)
            {
                fromRookSquare = Board.Square(0, king.Square.Rank);
                toRookSquare = Board.Square(File.D, king.Square.Rank);
                CurrentPlayer.CanCastleQueenSide = false;
            }
            Debug.Assert(fromRookSquare != null && toRookSquare != null);
            var rook = (Rook)fromRookSquare.Piece;
            fromRookSquare.Piece = null;
            toRookSquare.SetPiece(rook);
            king.HasCastled = true;
        }

        internal void Undo(Move move)
        {
            Hash ^= move.Hash;
            WhitePlayer.Mated = false;
            BlackPlayer.Mated = false;
            Ended = false;
            IsStaleMate = false;
            Winner = null;

            Debug.Assert(move.PreviousHash == Hash, "Previous hash differs from hash after undo");
            SwitchPlayer();
            move.Piece.MoveCount--;
            move.FromSquare.Piece = move.Piece;
            move.Piece.Square = move.FromSquare;
            move.ToSquare.Piece = null;

            var capture = move.Capture;
            if (capture != null)
            {
                move.CapturedFrom.Piece = capture;
                capture.Square = move.CapturedFrom;
                OtherPlayer.Material += capture.Value;
                OtherPlayer.Pieces.Add(capture);
            }

            if (move.IsEnpassant)
                move.ToSquare.Piece = null;

            if (move.IsCastling)
                UnCastle(move);

            if (move.IsPromotion)
            {
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
            MovesSinceLastCaptureOrPawnMove = move.PreviousMovesSinceLastCaptureOrPawnMove;
            CurrentPlayer.Moves.Pop();
        }

        private void UnCastle(Move move)
        {
            //The king is moved back.
            //Placing the rook on the corner square.
            var king = (King)move.Piece;
            Square fromRookSquare = null, toRookSquare = null;
            if (move.ToSquare.File == File.G)
            {
                fromRookSquare = Board.Square(File.H, king.Square.Rank);
                toRookSquare = Board.Square(File.F, king.Square.Rank);
                CurrentPlayer.CanCastleKingSide = true;
            }
            if (move.ToSquare.File == File.C)
            {
                fromRookSquare = Board.Square(0, king.Square.Rank);
                toRookSquare = Board.Square(File.D, king.Square.Rank);
                CurrentPlayer.CanCastleQueenSide = true;
            }
            Debug.Assert(fromRookSquare != null && toRookSquare != null);
            var rook = (Rook)toRookSquare.Piece;
            toRookSquare.Piece = null;
            fromRookSquare.SetPiece(rook);
            king.HasCastled = false;
        }

        private void SwitchPlayer()
        {
            CurrentPlayer = CurrentPlayer == WhitePlayer ? BlackPlayer : WhitePlayer;
        }

        private void CopyPieces(Player player, Game gameCopy)
        {
            foreach (var piece in player.Pieces)
            {
                var pieceCopy = piece.Copy(gameCopy.Board.Squares);
                if (pieceCopy.Square != null)
                    gameCopy.AddPiece(piece.Square.File, piece.Square.Rank, pieceCopy);
                //else: The piece was captured and connected to the game through the move.
                //But since CopyPosition does not copy moves we forget about the piece
            }
        }

        private string GetPosition()
        {
            var stringBuildder = new StringBuilder();
            stringBuildder.Append("Start,");
            foreach (var square in Board.Squares)
            {
                if (square.Piece != null)
                    stringBuildder.Append(square.Piece.ToPositionString() + ",");
            }
            stringBuildder.Append(CurrentPlayer.Color + ",");
            if (!WhitePlayer.CanCastleKingSide)
                stringBuildder.Append("WCK,");
            if (!WhitePlayer.CanCastleQueenSide)
                stringBuildder.Append("WCQ,");
            if (!BlackPlayer.CanCastleKingSide)
                stringBuildder.Append("BCK,");
            if (!BlackPlayer.CanCastleQueenSide)
                stringBuildder.Append("BCQ,");
            if (EnPassantFile != null)
                stringBuildder.Append("ENP:" + EnPassantFile.Value);
            return stringBuildder.ToString();
        }

        private void SetPieceFastAccess()
        {
            WhitePlayer.King = WhitePlayer.Pieces.OfType<King>().Single();
            BlackPlayer.King = BlackPlayer.Pieces.OfType<King>().Single();

            WhitePlayer.Queen = WhitePlayer.Pieces.OfType<Queen>().FirstOrDefault();
            BlackPlayer.Queen = BlackPlayer.Pieces.OfType<Queen>().FirstOrDefault();

            WhitePlayer.Pawns = WhitePlayer.Pieces.OfType<Pawn>().ToArray();
            BlackPlayer.Pawns = BlackPlayer.Pieces.OfType<Pawn>().ToArray();

            WhitePlayer.KnightsBishops = WhitePlayer.Pieces.Where(x => x.Value == 300).ToArray();
            BlackPlayer.KnightsBishops = BlackPlayer.Pieces.Where(x => x.Value == 300).ToArray();
        }

        public void MakeEditMove(Square fromSquare, Square toSquare)
        {
            var piece = fromSquare.Piece;
            if (piece == null)
                return;

            if (toSquare?.Piece != null)
                return;

            piece.Square = null;
            fromSquare.Piece = null;

            if (toSquare != null)
            {
                toSquare.Piece = piece;
                piece.Square = toSquare;
            }
            else
            {
                if (piece.Color == Color.White)
                    WhitePlayer.Pieces.Remove(piece);
                else
                    BlackPlayer.Pieces.Remove(piece);
            }
        }

        public void EnterEditMode()
        {
            EditMode = true;
            WhitePlayer.Moves.Clear();
            BlackPlayer.Moves.Clear();
            HashHistory.Clear();
            CommandCount = 0;
            PositionsDatabase.Instance.Reset();
        }

        public bool OtherKingAttacked()
        {
            return KingChecked(OtherPlayer);
        }

        public string GetFEN()
        {
            var sb = new StringBuilder();
            for (Rank rank = Rank._8; rank >= 0; rank--)
            {
                var emptyCount = 0;
                for (File file = File.A; file <= File.H; file++)
                {
                    var square = Board.Squares[(int)file + (int)rank * 8];
                    if (square.Piece == null)
                        emptyCount++;
                    else
                    {
                        if (emptyCount > 0)
                        {
                            sb.Append(emptyCount);
                            emptyCount = 0;
                        }
                        sb.Append(square.Piece.FenChar);
                    }

                }
                if (emptyCount > 0)
                {
                    sb.Append(emptyCount);
                    emptyCount = 0;
                }
                if (rank > Rank._1)
                    sb.Append("/");

            }

            sb.Append(" ");
            sb.Append(CurrentPlayer == WhitePlayer ? "w" : "b");
            sb.Append(" ");
            //todo: also check king and rook placement and movecount
            var castlWhite = WhitePlayer.CastlingToFEN();
            var castlBlack = BlackPlayer.CastlingToFEN();
            sb.Append(castlWhite + castlBlack);
            if (castlWhite + castlBlack == "")
                sb.Append("-");
            sb.Append(" ");

            var enpRank = CurrentPlayer == WhitePlayer ? "6" : "3";
            sb.Append(EnPassantFile.HasValue ? EnPassantFile.Value.ToString().ToLower() + enpRank : "-");
            sb.Append(" ");
            sb.Append(MovesSinceLastCaptureOrPawnMove);
            sb.Append(" ");
            sb.Append(CommandCount / 2 + 1);

            return sb.ToString();
        }

        public void LoadFEN(string fenString)
        {
            Reset();
            //rnbqkbnr/pp1ppppp/8/2p5/4P3/5N2/PPPP1PPP/RNBQKB1R b KQkq - 1 2
            var split = fenString.Split(' ');
            var files = split[0].Split('/');
            for (int i = 0; i < 8; i++)
            {
                var rank = Rank._8 - i;
                var fileString = files[i];
                var file = File.A;
                foreach (var pieceChar in fileString.ToCharArray())
                {
                    if (char.IsDigit(pieceChar))
                        file += int.Parse(pieceChar.ToString());
                    else
                    {
                        var color = char.IsLower(pieceChar) ? Color.Black : Color.White;
                        var piece = GameExtensions.PieceFromString(color, pieceChar.ToString().ToString().ToUpper());
                        AddPiece(file, rank, piece);
                        file++;
                    }
                }
            }
            CurrentPlayer = split[1] == "b" ? BlackPlayer : WhitePlayer;
            if (split[3] != "-")
                EnPassantFile = (File)Enum.Parse(typeof(File), split[3].Substring(0, 1).ToUpper());

            var castlingString = split[2];
            WhitePlayer.CanCastleKingSide = castlingString.Contains("K");
            WhitePlayer.CanCastleQueenSide = castlingString.Contains("Q");
            BlackPlayer.CanCastleKingSide = castlingString.Contains("k");
            BlackPlayer.CanCastleQueenSide = castlingString.Contains("q");

            MovesSinceLastCaptureOrPawnMove = int.Parse(split[4]);
            CommandCount = (byte)(int.Parse(split[5]) * 2 - 1);
            PositionsDatabase.Instance.SetOldestCommand(CommandCount);

            SetPieceFastAccess();
            PositionsDatabase.Instance.SetStartHash(this);
            InitialMaterial(WhitePlayer);
            InitialMaterial(BlackPlayer);
        }
    }

    public static class GameExtensions
    {
        /// <summary>
        /// string format. E.g. e1bR (black Rook)
        /// </summary>
        /// <param name="game"></param>
        /// <param name="pieceString"></param>
        public static Game AddPiece(this Game game, string pieceString)
        {
            var file = (File)Enum.Parse(typeof(File), pieceString.Substring(0, 1).ToUpper());
            var rank = (Rank)Enum.Parse(typeof(Rank), "_" + pieceString.Substring(1, 1));
            var colorChar = pieceString.Substring(2, 1);
            var color = colorChar == "w" ? Color.White : Color.Black;
            var pieceTypeChar = pieceString.Substring(3, 1);
            Piece piece = PieceFromString(color, pieceTypeChar);
            game.AddPiece(file, rank, piece);
            return game;
        }

        internal static Piece PieceFromString(Color color, string pieceTypeChar)
        {
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
                throw new ApplicationException("Invalid format of add piece string");
            return piece;
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
            }
            game.AddPiece(square.File, square.Rank, piece);
        }
    }
}