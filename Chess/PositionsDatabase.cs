using System;
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
        private ulong[] Castling { get; } = new ulong[64];


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

            for (int i = 0; i < 64; i++)
            {
                rnd.NextBytes(buffer);
                Castling[i] = BitConverter.ToUInt64(buffer, 0);
            }
        }

        internal void SetStartHash(Game game) {
            game.Hash = 0;
            foreach (var piece in game.WhitePlayer.Pieces) {
                game.Hash ^= ZobristArray[piece.Type, piece.Square.Index];
            }
            foreach (var piece in game.BlackPlayer.Pieces) {
                game.Hash ^= ZobristArray[piece.Type, piece.Square.Index];
            }

            game.Hash ^= Side[(int)game.CurrentPlayer.Color];
        }

        internal void UpdateHash(Move move) {
            var fs = move.FromSquare;
            var ts = move.ToSquare;
            ulong hash = 0;

            var pieceType = (int)move.Piece.Type;

            if (!move.IsPromotion) { 
                hash = ZobristArray[pieceType, ts.Index]; //piece on new square
                hash ^= ZobristArray[pieceType, fs.Index]; //piece off
            } else {
                var pawnType = move.PromotedPawn.Type;
                hash = ZobristArray[pawnType, fs.Index]; //pawn off                 
                var type = move.Piece.Color == Color.Black ? PieceType.BlackQueen : PieceType.WhiteQueen;
                hash ^= ZobristArray[(int)type, ts.Index]; //queen on new square 
            }

            if (move.Capture != null) {
                hash ^= ZobristArray[move.Capture.Type, move.CapturedFrom.Index];
                //captured piece off, includes en passant
            }

            if (move.IsCastling) {
                var rookSquareIndex = move.CastleRook.Square.Index;
                hash ^= ZobristArray[move.CastleRook.Type, move.CastleRook.Square.Index];
                hash ^= Castling[rookSquareIndex];
                //Todo Castling options
            }

            hash ^= Side[(int)move.Piece.Color];
            move.Hash = hash;
        }

        internal void GetValue(Game game, Move move) {
            int eval;
            if (Dictionary.TryGetValue(game.Hash, out eval)) {
                //It is nice not to have to evaluate illegal moves all the time.
                //But we have to handle occasional hash collisions, though they are very rare. Not even one during a game it seems.
                byte commandCount;
                bool legal;
                bool check;
                ScoreInfo scoreInfo;
                int score;
                int depth;
                Unpack(eval, out commandCount, out legal, out check, out scoreInfo, out score, out depth);
                if (!legal) {
                    move.IsLegal = false;
                    return;
                }
                move.IsLegal = true;
                move.IsCheck = check;
                move.ScoreAfterMove = score;
                move.ScoreInfo = scoreInfo;
                Matches++;
            }
        }

        private readonly object _lockObject = new object();
        internal void Store(Game game, Move move, int depth) {
            Debug.Assert(move.IsLegal.HasValue);
            var score = move.ScoreAfterMove ?? 0;
            var eval = Pack(game.CommandCount, move.IsLegal.Value, move.IsCheck, score, move.ScoreInfo, depth);

            lock (_lockObject) {
                if (Dictionary.ContainsKey(game.Hash)) {
                    if (eval != Dictionary[game.Hash])
                    {
                        var dbEval = Dictionary[game.Hash];
                        byte oCommandNo;
                        bool legal;
                        bool check;
                        ScoreInfo scoreInfo;
                        int oScore;
                        int oDepth;
                        Unpack(dbEval, out oCommandNo, out legal, out check, out scoreInfo, out oScore, out oDepth);
                        if (depth > oDepth)
                        {
                            Dictionary[game.Hash] = eval;
                        }
                        //Collisions++;
                        //Dictionary.Remove(game.Hash);
                    }
                } else {
                    Dictionary.Add(game.Hash, eval);
                }
            }
        }

        public override string ToString() {
            return $"Entries: {Dictionary.Count.KiloNumber()}\r\nMatches: {Matches.KiloNumber()}\r\nCollisions: {Collisions}";
        }

        internal static PositionsDatabase Instance { get; private set; } = new PositionsDatabase();

        internal void Reset() {
            Dictionary.Clear();
            Collisions = 0;
            Matches = 0;
            OldestCommands = 0;
        }

        internal void ResetCounters() {
            Matches = 0;
            Collisions = 0;
        }

        internal static void Unpack(int build, out byte oCommandNo, out bool oLegal, out bool check,
            out ScoreInfo oScoreInfo, out int oScore, out int oDepth) {
            oCommandNo = (byte)((build >> 25) & 0x7F); //7 bits
            oLegal = ((build >> 24) & 1) == 1;
            check = ((build >> 23) & 1) == 1;
            var negScore = ((build >> 22) & 1) == 1;
            oScore = (build >> 9) & 0x1FFF; //13bits
            oScore = negScore ? oScore * -1 : oScore;
            oDepth = (byte)((build >> 4) & 0x1F); //5bits
            oScoreInfo = (ScoreInfo)(build & 0xF); //4 bits
        }

        /// <summary>
        /// Converts all the arguments to an int.
        /// </summary>
        /// <param name="commandNo">7 bit max 127</param>
        /// <param name="legal">1 bit</param>
        /// <param name="check">1 bit</param>
        /// <param name="score">1 bit for negative, 13 bits max 8191</param>
        /// <param name="scorInfo">4 bit</param>
        /// <returns></returns>
        internal static int Pack(byte commandNo, bool legal, bool check, int score, ScoreInfo scorInfo, int depth) {
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
            build |= depth;

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
