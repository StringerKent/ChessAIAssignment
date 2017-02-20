using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Management;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chess
{
    public class Engine
    {
        public Engine() {
            //foreach (var item in new ManagementObjectSearcher("Select * from Win32_ComputerSystem").Get())
            //{
            //    Console.WriteLine("Number Of Physical Processors: {0} ", item["NumberOfProcessors"]);
            //}

            int coreCount = 0;
            foreach (var item in new ManagementObjectSearcher("Select * from Win32_Processor").Get()) {
                coreCount += int.Parse(item["NumberOfCores"].ToString());
            }
            CoreCount = coreCount;

            //Console.WriteLine("Number Of Cores: {0}", coreCount);
            //Console.WriteLine("Number Of Logical Processors: {0}", Environment.ProcessorCount);
            //foreach (var item in new ManagementObjectSearcher("Select * from Win32_ComputerSystem").Get())
            //{
            //    Console.WriteLine("Number Of Logical Processors: {0}", item["NumberOfLogicalProcessors"]);
            //}
        }

        public TimeSpan SearchFor { get; set; }

        public Color? ThinkingFor { get; set; }

        public async Task<Evaluation> AsyncBestMoveDeepeningSearch(Game game, TimeSpan time) {

            var t = Task.Run(() => BestMoveDeepeningSearch(game, time));
            try {
                await t;
            } catch (Exception) {
                throw;
            }
            PositionsDatabase.Instance.CleanUpOldPositions();
            return t.Result;
        }

        public void Abort() {
            Aborted = true;
        }

        internal Evaluation BestMoveDeepeningSearch(Game game, TimeSpan time) {
            Reset();
            //PositionsDatabase.Instance.Reset();
            var maxDepth = 200; //We will break long before due to timeout.
            Stopwatch = Stopwatch.StartNew();
            SearchFor = time;
            var playerColor = game.CurrentPlayer.Color;
            ThinkingFor = playerColor;
            var childMoves = game.GetLegalNextMoves().OrderFor(game.CurrentPlayer.Color).ToArray();
            var evaluatedMoves = childMoves.Select(x => new Evaluation { CmdsString = x.ToCommandString(), Move = x }).ToArray();
            var maximizing = playerColor == Color.Black;
            var bestEvaluation = maximizing ? evaluatedMoves.OrderBy(x => x.Value).Last() : evaluatedMoves.OrderBy(x => x.Value).First();

            for (var depth = 1; depth < maxDepth; depth++) {
                Debug.WriteLine($"\r\n==========\r\nStart Depth {depth}");
                var evaluation = BestCommandAtDepth(game, evaluatedMoves, depth);
                evaluatedMoves = playerColor == Color.White
                    ? evaluatedMoves.OrderBy(x => x.Value).ToArray()
                    : evaluatedMoves.OrderByDescending(x => x.Value).ToArray();

                if (evaluation == null) //canceled
                {
                    if (Aborted) {
                        ThinkingFor = null;
                        return null;
                    }
                    //Keep the last best moves found
                    break;
                }
                bestEvaluation = maximizing ? evaluatedMoves.OrderBy(x => x.Value).Last() : evaluatedMoves.OrderBy(x => x.Value).First();
                Debug.WriteLine($"End depth {depth} ({Stopwatch.Elapsed.TotalSeconds.ToString("F")})");
                Debug.WriteLine($"Best evaluation at depth{depth}:\r\n{bestEvaluation}");
                if (bestEvaluation.MateFound)
                    break;
                
            }
            Debug.WriteLine($"Selected evaluation:\r\n{bestEvaluation}");

            ThinkingFor = null;
            return bestEvaluation;
        }

        private int CoreCount { get; }
        private int LeafVisits { get; set; }
        private int NodeVisit { get; set; }
        private int BetaCutOffCount { get; set; }
        private int QuiteSearchNodes { get; set; }
        private int AlphaCutOffCount { get; set; }
        private bool Aborted { get; set; }
        private Stopwatch Stopwatch { get; set; } = new Stopwatch();
        private Evaluation BestCommandAtDepth(Game game, Evaluation[] evaluations, int depth) {
            var playerColor = game.CurrentPlayer.Color;
            var parallelism = CoreCount;

            var maximizing = playerColor == Color.Black;
            var alpha = -8000;
            var beta = 8000;
            var canceled = false;
            var mateFound = false;
            var localEvals = evaluations.Select(x => new Evaluation {CmdsString = x.CmdsString, Value = x.Value, Move = x.Move }).ToArray();                                    
            var commands = evaluations.Select(e => e.CmdsString);
            
            Parallel.ForEach(commands,
                new ParallelOptions { MaxDegreeOfParallelism = parallelism },
                (command) => {
                    if (!mateFound)
                    {
                        var gameCopy = game.Copy();
                        //Each thread needs its own Game and moves not to collide with other threads.
                        var copyMoves = gameCopy.GetLegalNextMoves().OrderFor(gameCopy.CurrentPlayer.Color);
                        var copyMove = copyMoves.Single(cm => cm.ToCommandString() == command);
                        gameCopy.PerformLegalMove(copyMove);
                        var v = AlphaBeta(gameCopy, copyMove, alpha, beta, !maximizing, depth, ref canceled, 1); //Switched Player!
                        gameCopy.UndoLastMove();

                        
                        var eval = localEvals.Single(x => x.Move.ToCommandString() == command);
                        eval.Value = v;
                        mateFound = (maximizing && v > 7900) || (!maximizing && v < -7900);
                        eval.BestLine = copyMove.BestLine();
                        eval.Move.ScoreInfo = copyMove.ScoreInfo;
                        eval.Move.IsCheck = copyMove.IsCheck;
                        eval.MateFound = mateFound;
                    }
                });

            
            if (canceled) {
                Debug.WriteLine($"\r\nDepth: {depth}\r\n Canceled");
                return null;
            }

            for (int i = 0; i < evaluations.Length; i++) {
                evaluations[i] = localEvals[i];
            }
            //Console.WriteLine($"\r\nDepth: {depth}\r\nCut off {CutOffCount} times.\r\nVisited {NodeVisit} nodes.\r\nLeaf Visits:{LeafVisits}\r\nBest Move {bestCommand.Move.ToString()} ({bestCommand.Value})\r\n{bestCommand.Move.BestLine()}");
            //Returning the actual instance. Moves were copied above.
            var bestEvaluation = maximizing ? localEvals.OrderBy(x => x.Value).Last() : localEvals.OrderBy(x => x.Value).First();
            bestEvaluation.Nodes = NodeVisit;
            bestEvaluation.QuiteSearchNodes = QuiteSearchNodes;
            bestEvaluation.LeafVisits = LeafVisits;
            bestEvaluation.AlphaCutoff = AlphaCutOffCount;
            bestEvaluation.BetaCutoff = BetaCutOffCount;
            bestEvaluation.DatabaseStats = PositionsDatabase.Instance.ToString();
            return evaluations.Single(x => x.CmdsString == bestEvaluation.CmdsString);
        }

        internal Evaluation BestAlphaBetaMove(Game game, int depth) {
            Reset();
            var playerColor = game.CurrentPlayer.Color;
            ThinkingFor = playerColor;
            var childMoves = game.GetLegalNextMoves().OrderFor(playerColor).ToArray();
            var evaluations = childMoves.Select(x => new Evaluation { CmdsString = x.ToCommandString(), Move = x }).ToArray();
            var bestEvaluatedMove = BestCommandAtDepth(game, evaluations, depth);
            ThinkingFor = null;
            return bestEvaluatedMove;// childMoves.Single(x => x.ToCommandString() == bestEvaluatedMove.CmdsString);
        }

        private int AlphaBeta(Game gameCopy, Move node, int alpha, int beta, bool maximizingPlayer, int depth, ref bool canceled, int recursion) {
            if ((SearchFor > TimeSpan.Zero && Stopwatch.Elapsed > SearchFor) || Aborted) {
                canceled = true;
                return 0;
            }

            NodeVisit++;
            int bestVal;// = maximizingPlayer ? alpha : beta;
            if (depth <= 0 || gameCopy.Ended) {
                if (node.Capture != null) {
                    bestVal = AlphaBetaQuite(gameCopy, node, alpha, beta, maximizingPlayer, 1, ref canceled, recursion + 1);
                } else { //No capture. No worries for horizon effect.
                    bestVal = node.ScoreAfterMove.Value + (maximizingPlayer ? 1 : -1);
                    //nodes must always have value. The reason is actually performance and move ordering and cutoffs. Theory and also my experience states this.
                    LeafVisits++;
                }
            } else if (maximizingPlayer) {
                bestVal = alpha;
                var childern = gameCopy.GetLegalNextMoves().OrderFor(Color.Black);
                if (!childern.Any()) {
                    bestVal = -NoChildrenEval(gameCopy, node, recursion);
                } else
                    foreach (var move in childern) {
                        gameCopy.PerformLegalMove(move);
                        var childValue = AlphaBeta(gameCopy, move, bestVal, beta, false, depth - 1, ref canceled, recursion + 1);
                        gameCopy.UndoLastMove();
                        if (childValue > bestVal || node.OpponentsBestAiMove == null)
                            node.OpponentsBestAiMove = move;
                        bestVal = Math.Max(bestVal, childValue);
                        if (beta <= bestVal) {
                            BetaCutOffCount++;
                            break;
                        }
                    }
            } else { //white player
                bestVal = beta;
                var childern = gameCopy.GetLegalNextMoves().OrderFor(Color.White);
                if (!childern.Any()) {
                    bestVal = NoChildrenEval(gameCopy, node, recursion);
                } else
                    foreach (var move in childern) {
                        gameCopy.PerformLegalMove(move);
                        var childValue = AlphaBeta(gameCopy, move, alpha, bestVal, true, depth - 1, ref canceled, recursion + 1);
                        gameCopy.UndoLastMove();
                        if (childValue < bestVal || node.OpponentsBestAiMove == null)
                            node.OpponentsBestAiMove = move;
                        bestVal = Math.Min(bestVal, childValue);
                        if (bestVal <= alpha) {
                            AlphaCutOffCount++;
                            break;
                        }
                    }
            }
            return bestVal;
        }

        private int AlphaBetaQuite(Game gameCopy, Move node, int alpha, int beta, bool maximizingPlayer, int depth, ref bool canceled, int recursion) {
            if ((SearchFor > TimeSpan.Zero && Stopwatch.Elapsed > SearchFor) || Aborted) {
                canceled = true;
                return 0;
            }

            QuiteSearchNodes++;
            int bestVal;
            if (depth == 0 || gameCopy.Ended) {
                bestVal = node.ScoreAfterMove.Value + (maximizingPlayer ? 1 : -1);
                LeafVisits++;
            } else if (maximizingPlayer) {
                bestVal = alpha;
                var childern = gameCopy.GetLegalNextMoves(justCaptures: true).OrderFor(Color.Black);
                if (!childern.Any()) {
                    bestVal = node.ScoreAfterMove.Value;
                } else
                    foreach (var move in childern) {
                        gameCopy.PerformLegalMove(move);
                        var childValue = AlphaBetaQuite(gameCopy, move, bestVal, beta, false, depth - 1, ref canceled, recursion + 1);
                        gameCopy.UndoLastMove();
                        if (childValue > bestVal || node.OpponentsBestAiMove == null)
                            node.OpponentsBestAiMove = move;
                        bestVal = Math.Max(bestVal, childValue);
                        if (beta <= bestVal) {
                            BetaCutOffCount++;
                            break;
                        }
                    }
            } else { //white player
                bestVal = beta;
                var childern = gameCopy.GetLegalNextMoves(justCaptures: true).OrderFor(Color.White);
                if (!childern.Any()) {
                    bestVal = node.ScoreAfterMove.Value;
                } else
                    foreach (var move in childern) {
                        gameCopy.PerformLegalMove(move);
                        var childValue = AlphaBetaQuite(gameCopy, move, alpha, bestVal, true, depth - 1, ref canceled, recursion + 1);
                        gameCopy.UndoLastMove();
                        if (childValue < bestVal || node.OpponentsBestAiMove == null)
                            node.OpponentsBestAiMove = move;
                        bestVal = Math.Min(bestVal, childValue);
                        if (bestVal <= alpha) {
                            AlphaCutOffCount++;
                            break;
                        }
                    }
            }
            return bestVal;
        }

        /// <summary>
        /// This evaluates the position when the opponent has no legal move. It is either mate or stale mate.
        /// </summary>
        /// <param name="gameCopy"></param>
        /// <param name="node"></param>
        /// <param name="maximizingPlayer"></param>
        /// <returns></returns>
        private int NoChildrenEval(Game gameCopy, Move node, int recursion) {
            if (gameCopy.CurrentPlayer.IsChecked) {
                node.ScoreInfo |= ScoreInfo.Mate;
                node.ScoreAfterMove = 8000 - recursion; //maximizing variable is used to negate value by calling function.
                gameCopy.Winner = gameCopy.OtherPlayer;
            } else {
                node.ScoreInfo |= ScoreInfo.StaleMate;
                node.ScoreAfterMove = 0;
            }
            gameCopy.Ended = true;
            PositionsDatabase.Instance.Store(gameCopy, node);
            return node.ScoreAfterMove.Value;
        }

        private void Reset() {
            Aborted = false;
            BetaCutOffCount = 0;
            AlphaCutOffCount = 0;
            NodeVisit = 0;
            LeafVisits = 0;
            QuiteSearchNodes = 0;
            PositionsDatabase.Instance.ResetMatches();            
        }

        //from http://will.thimbleby.net/algorithms/doku.php?id=minimax_search_with_alpha-beta_pruning
        //    function alphaBeta(node, alpha, beta, maximizingPlayer)
        //    {
        //        var bestValue;
        //        if (node.children.length === 0)
        //        {
        //            bestValue = node.data;
        //        }
        //        else if (maximizingPlayer)
        //        {
        //            bestValue = alpha;

        //            // Recurse for all children of node.
        //            for (var i = 0, c = node.children.length; i < c; i++)
        //            {
        //                var childValue = alphaBeta(node.children[i], bestValue, beta, false);
        //                bestValue = Math.max(bestValue, childValue);
        //                if (beta <= bestValue)
        //                {
        //                    break;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            bestValue = beta;

        //            // Recurse for all children of node.
        //            for (var i = 0, c = node.children.length; i < c; i++)
        //            {
        //                var childValue = alphaBeta(node.children[i], alpha, bestValue, true);
        //                bestValue = Math.min(bestValue, childValue);
        //                if (bestValue <= alpha)
        //                {
        //                    break;
        //                }
        //            }
        //        }
        //        return bestValue;
        //    }

    }

    static class Extension
    {
        public static IOrderedEnumerable<Move> OrderFor(this IEnumerable<Move> moves, Color color) {
            if (color == Color.White)
                return moves.OrderBy(x => x.ScoreAfterMove.Value);
            return moves.OrderByDescending(x => x.ScoreAfterMove.Value);
        }
    }
    
    /// <summary>
    /// This is like a wrapper for the best move found by engine. It also has a few nice data about the engines move.
    /// </summary>
    public class Evaluation
    {
        public Move Move { get; set; }
        public string CmdsString { get; set; }
        public int Value { get; set; }
        public int LeafVisits { get; set; }
        public int Nodes { get; set; }
        public string BestLine { get; set; }
        public int BetaCutoff { get; set; }
        public int AlphaCutoff { get; set; }
        public string DatabaseStats { get; set; }
        public int QuiteSearchNodes { get; set; }
        public bool MateFound { get; set; }
        

        public override string ToString() {
            return $"{Move} ({Value})\r\nBest line: {BestLine}\r\nNodes: {Nodes.KiloNumber()}\r\nLeafs: {LeafVisits.KiloNumber()}\r\nQuite search nodes: {QuiteSearchNodes.KiloNumber()}\r\nBetaCuts: {BetaCutoff.KiloNumber()}\r\nAlphaCuts: {AlphaCutoff.KiloNumber()}\r\n\r\nPosition DB:\r\n{DatabaseStats}";
        }
    }

    public static class NumberExtensions
    {
        public static string KiloNumber(this int number)
        {
            return number/1000 + "k";
        }
    }
}
