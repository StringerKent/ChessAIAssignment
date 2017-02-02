﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    class PositionsDatabase
    {
        private PositionsDatabase() {
            InitZobrist();
        }

        private int Matches { get; set; }
        private int Collisions { get; set; }
        private Dictionary<ulong, int> Dictionary { get; set; } = new Dictionary<ulong, int>();
        private ulong[,] ZobristArray { get; } = new ulong[13, 64];
        private ulong[] Side { get; } = new ulong[2];

        private void InitZobrist() {
            var rnd = new Random(777);
            var buffer = new byte[8];
            for (var p = 0; p < 13; p++) //pieces
            {
                for (var s = 0; s < 64; s++) {
                    rnd.NextBytes(buffer);
                    var ui = BitConverter.ToUInt64(buffer, 0);
                    ZobristArray[p, s] = ui;
                }
            }
            rnd.NextBytes(buffer);
            Side[0] = BitConverter.ToUInt64(buffer, 0);
            rnd.NextBytes(buffer);
            Side[1] = BitConverter.ToUInt64(buffer, 0);
        }

        internal void SetStartHash(Game game) {
            game.Hash = 0;
            foreach (var piece in game.WhitePlayer.Pieces) {
                var p = piece.Type;
                var s = (int)piece.Square.File + (int)piece.Square.Rank * 8;
                game.Hash ^= ZobristArray[p, s];
            }
            foreach (var piece in game.BlackPlayer.Pieces) {
                var p = piece.Type;
                var s = (int)piece.Square.File + (int)piece.Square.Rank * 8;
                game.Hash ^= ZobristArray[p, s];
            }

            game.Hash ^= Side[(int)game.CurrentPlayer.Color];
        }

        internal void UpdateHash(Game game, Move move) {
            var fs = move.FromSquare;
            var ts = move.ToSquare;
            var fromSquare = (int)fs.File + (int)fs.Rank * 8;
            var toSquare = (int)ts.File + (int)ts.Rank * 8;

            var piece = (int)move.Piece.Type;

            if (!move.IsPromotion) {
                game.Hash ^= ZobristArray[piece, fromSquare]; //piece off
                game.Hash ^= ZobristArray[piece, toSquare]; //piece on new square
            }

            if (move.Capture != null) {
                var sqr = move.CapturedFrom;
                game.Hash ^= ZobristArray[move.Capture.Type, (int)sqr.File + (int)sqr.Rank * 8];
                //captured piece off, includes en passant
            }

            if (move.IsCastling) {
                var rook = move.CastleRook;
                var toRookSquare = (int)rook.Square.File + (int)rook.Square.Rank * 8;
                game.Hash ^= ZobristArray[move.CastleRook.Type, toRookSquare];
                //Todo Castling options
            }

            if (move.IsPromotion) {
                var pawnType = move.PromotedPawn.Type;
                game.Hash ^= ZobristArray[pawnType, fromSquare]; //pawn off                 
                var type = move.Piece.Color == Color.Black ? PieceType.BlackQueen : PieceType.WhiteQueen;
                game.Hash ^= ZobristArray[(int)type, toSquare]; //queen on new square                
            }

            game.Hash ^= Side[(int)move.Piece.Color];
        }

        internal void GetValue(Game game, Move move, int recursions) {
            int eval;
            if (Dictionary.TryGetValue(game.Hash, out eval)) {
                //It is nice not to have to evaluate illegal moves all the time.
                //But we have to handle occasional hash collisions, though they are very rare. Not even one during a game it seems.
                byte commandCount;
                bool legal;
                bool check;
                ScoreInfo scoreInfo;
                int scor;
                byte recurs;
                Unpack(eval, out commandCount, out legal, out check, out scoreInfo, out scor, out recurs);
                if (!legal) {
                    move.IsLegal = false;
                    return;
                }
                move.IsLegal = true;
                move.IsCheck = check;

                var score = scor;

                //Scores calculated at greater depth are prioritized. Is that good?
                //unless it is mate.
                if (scoreInfo.HasFlag(ScoreInfo.Mate) || recursions >= recurs) {
                    move.ScoreAfterMove = score;
                    move.ScoreInfo = scoreInfo;
                    Matches++;
                }
            }
        }

        private readonly object _lockObject = new object();
        internal void Store(Game game, Move move, int recursions) {
            Debug.Assert(move.IsLegal.HasValue);
            var score = move.ScoreAfterMove ?? 0;
            var chk = move.IsCheck ?? false;
            var eval = Pack(game.CommandCount, move.IsLegal.Value, chk, score, recursions, move.ScoreInfo);
            //eval.PositionString = game.GetPosition();

            lock (_lockObject) {
                if (Dictionary.ContainsKey(game.Hash)) {
                    //store values with recursion that are higher than existing
                    var dbEval = Dictionary[game.Hash];
                    byte commandCount;
                    bool legal;
                    bool check;
                    ScoreInfo scoreInfo;
                    int scor;
                    byte dbRecurs;
                    //todo: it should be faster just to unpack the recursions field.
                    Unpack(dbEval, out commandCount, out legal, out check, out scoreInfo, out scor, out dbRecurs);

                    //if (dbEval.PositionString != eval.PositionString) {
                    //    Collisions++;
                    //}

                    //Score found at greater depth replaces current score
                    if (dbRecurs <= recursions)
                        Dictionary[game.Hash] = eval;
                } else {
                    Dictionary.Add(game.Hash, eval);
                }
            }
        }

        public override string ToString() {
            return $"Entries: {Dictionary.Count.KiloNumber()}\r\nMatches: {Matches.KiloNumber()}";
        }

        internal static PositionsDatabase Instance { get; private set; } = new PositionsDatabase();

        internal void Reset() {
            Dictionary.Clear();
            Collisions = 0;
            Matches = 0;
            OldestCommands = 0;
        }

        internal void ResetMatches()
        {
            Matches = 0;
        }

        internal static void Unpack(int build, out byte oCommandNo, out bool oLegal, out bool check,
            out ScoreInfo oScoreInfo, out int oScore, out byte oRecursions) {
            oCommandNo = (byte)((build >> 25) & 0x7F); //7 bits
            oLegal = ((build >> 24) & 1) == 1;
            check = ((build >> 23) & 1) == 1;
            var negScore = ((build >> 22) & 1) == 1;
            oScore = (build >> 9) & 0x1FFF; //13bits
            oScore = negScore ? oScore * -1 : oScore;
            oRecursions = (byte)((build >> 4) & 0x1F); //5bits
            oScoreInfo = (ScoreInfo)(build & 0xF); //4 bits
        }

        /// <summary>
        /// Converts all the arguments to an int.
        /// </summary>
        /// <param name="commandNo">7 bit max 127</param>
        /// <param name="legal">1 bit</param>
        /// <param name="check">1 bit</param>
        /// <param name="score">1 bit for negative, 13 bits max 8191</param>
        /// <param name="recursions">5 bit, max 31 (Will the engine ever be this fast? ☺ )</param>
        /// <param name="scorInfo">4 bit</param>
        /// <returns></returns>
        internal static int Pack(byte commandNo, bool legal, bool check, int score, int recursions, ScoreInfo scorInfo) {
            var build = (int)commandNo;
            build <<= 1;
            build |= (legal ? 1 : 0);

            build <<= 1;
            build |= (check ? 1 : 0);

            build <<= 1;
            build |= score < 0 ? 1 : 0;

            build <<= 13;
            build |= Math.Abs(score);

            build <<= 5;
            build |= recursions;

            build <<= 4;
            build |= (byte)scorInfo;
            return build;
        }

        private int OldestCommands { get; set; }

        private readonly object _cleanLockObject = new object();

        internal void CleanUpOldPositions() {
            Debug.WriteLine("Before clean: " + Dictionary.Count);
            lock (_cleanLockObject) {
                while (Dictionary.Count > 2500000) {
                    //Removing all commands older than i.
                    //If the dictionary is still to large it decreases the boundary age of commands that should be removed.
                    Dictionary = Dictionary.Where(x => ((x.Value >> 25) & 0x7F) > OldestCommands)
                            .ToDictionary(x => x.Key, x => x.Value);
                    OldestCommands++;
                }
            }
            Debug.WriteLine("After clean: " + Dictionary.Count);
        }
    }
}
